using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SlimeAngerManager : MonoBehaviour
{
    [SerializeField] Tilemap placeablesTilemap;
    [SerializeField] Transform Slime;
    [SerializeField] int rageOmeter=0;
    [SerializeField] int maxRage;
    [SerializeField] int minRagePerTile, maxRagePerTile;
    [SerializeField] float decayRate; // every this seconds, rage goes down by 1.
    float decayTimer;
    public int sliceHeight;
    public int sliceWidth;
    public int numberOfSlices = 3;
    int indexOfDestroyedSlice = 0; //also useful for positioning the slime
    public Vector3Int TopLeftOfGrid;
    public Vector3Int BottomRightOfGrid;
    public Dictionary<Vector3, RageObject> rageSpots = new Dictionary<Vector3, RageObject>();
    public static SlimeAngerManager instance;
    List<Vector3> CurrentSliceToDestroy = new List<Vector3>();

    [Header("inhale effect stuff")]
    [SerializeField] float inhaleSpeed;
    [SerializeField] float wiggleAmp, wiggleFreq,sleepAmp,sleepFreq;
    List<GameObject> inhaledObjects = new List<GameObject>();

    [SerializeField] float moveSpeed, angryFaceDelay, sinkHeight, sinkDelay;
    Vector3 startSlimePos;
    bool slimeIsAwake = false;
    [SerializeField] ShakeMe cameraShake;
    [SerializeField] SpriteRenderer redBar;
    [SerializeField] GameObject wholeBar;
    [SerializeField] GameObject textParticlePrefab;
    [SerializeField] Transform textParent;
    [SerializeField] float snoreFrequency;// how often in seconds to emit a new snore
    [SerializeField] ParticleSystem suckSystem;
    [SerializeField] Sprite sleepy, angry, sucky, satisfied;
    [SerializeField] GameObject sugarPrefab;
    SpriteRenderer spriteComp;
    ShakeMe selfShake;
    float sleepySinewave = 0;
    float snoreTimer = 0;
    GameObject previousSnore;
    Color barOrange;
    // Start is called before the first frame update
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            print("duplicate rage managers!!");
            
        }
    }
    void Start()
    {
        TopLeftOfGrid = Vector3Int.RoundToInt( placeablesTilemap.GetCellCenterWorld(new Vector3Int(placeablesTilemap.origin.x, placeablesTilemap.origin.y + placeablesTilemap.size.y,0)));
        BottomRightOfGrid = Vector3Int.RoundToInt(placeablesTilemap.GetCellCenterWorld(new Vector3Int(placeablesTilemap.origin.x + placeablesTilemap.size.x, placeablesTilemap.origin.y,0)));
        print("topLeft: " +TopLeftOfGrid);
        print("bottomRight: " + BottomRightOfGrid);
        sliceHeight = placeablesTilemap.size.y;

        sliceWidth = placeablesTilemap.size.x / numberOfSlices;

        startSlimePos = Slime.position;
        //print(sliceWidth);
        //print(sliceHeight);
        selfShake = Slime.GetComponentInChildren<ShakeMe>();
        spriteComp = Slime.GetComponentInChildren<SpriteRenderer>();
        barOrange=redBar.color;
    }
    public void AddTile(RageObject newBuildableTile)
    {
        rageSpots.Add(GridManagement.RoundVectorToInts(newBuildableTile.transform.position), newBuildableTile);
        newBuildableTile.rageAmount = Random.Range(minRagePerTile, maxRagePerTile + 1);
        SpriteRenderer fireBar = newBuildableTile.rageBar.GetComponent<SpriteRenderer>();
        fireBar.size = new Vector2(Mathf.Round(3f * ((float)newBuildableTile.rageAmount / (float)maxRagePerTile)), fireBar.size.y);
        fireBar.size =new Vector2( Mathf.Clamp(fireBar.size.x, 0, 3f),fireBar.size.y);
    }
    public bool IsTileBuildable(Vector3 tilePosition)
    {
        if (!slimeIsAwake)
        {
            if (rageSpots.ContainsKey(GridManagement.RoundVectorToInts(tilePosition)))
            {
                if (!GridManagement.getObjectFromPosition(tilePosition))
                {
                    return true;
                }

            }
        }
            return false;
        
    }
    public void RevealTile(Vector3 tilePosition)
    {
        rageSpots[tilePosition]?.rageBar.parent.gameObject.SetActive(true);
    }
    public void BuiltOnTile(Vector3 tilePosition)
    {
        rageSpots[tilePosition]?.rageBar.parent.gameObject.SetActive(true);
        rageOmeter += rageSpots[tilePosition].rageAmount;
        //it would do the visual react with the ? or ! or whatever here
        //if it's over the threshhold do the big ol thang
        snoreTimer = 0;
        decayTimer = 0;
        Destroy(previousSnore);
        if (rageOmeter >= maxRage)
        {
            
            GetRowOfTile(tilePosition);
            StartCoroutine(SlimeWakeAndMove());

            previousSnore = Instantiate(textParticlePrefab, textParent);
            LetterParticle newLetter = previousSnore.GetComponent<LetterParticle>();
            newLetter.Initialize("!!!", Color.red, new Vector2(-2,2), 2, .1f, .001f, 3f);
            newLetter.transform.localScale *= 2;
        }
        else
        {
            if (textParent && textParticlePrefab)
            {
                previousSnore = Instantiate(textParticlePrefab, textParent);
                LetterParticle newLetter = previousSnore.GetComponent<LetterParticle>();
                float roundedRage = Mathf.Round(3*rageSpots[tilePosition].rageAmount/maxRagePerTile);
                if (roundedRage <= 1)
                {
                    newLetter.Initialize("...", Color.yellow, new Vector2(-2,2), 2, .1f, .001f, 3f);
                }else if (roundedRage <= 2)
                {

                    newLetter.Initialize("..?", new Color(255, 128, 0), new Vector2(-2,2), 2, .1f, .001f, 3f);
                }
                else
                {

                    newLetter.Initialize("..!", Color.red, new Vector2(-2,2), 2, .1f, .001f, 3f);
                }
            }
        }
    }
    /// <summary>
    /// populates a list of the tile co-ordinates you have to destroy based on the last one that angered you
    /// </summary>
    /// <param name="tilePosition"></param>
    void GetRowOfTile(Vector3 tilePosition)
    {
        for(int i=0; i < numberOfSlices; i ++)
        {
            if((int) tilePosition.x >= TopLeftOfGrid.x + i *sliceWidth && (int)tilePosition.x < TopLeftOfGrid.x + i *sliceWidth+sliceWidth)
            {
                print("index of " + i);
                indexOfDestroyedSlice = i;
                break;
            }
        }
        
    }
    
    void DislodgeBuildingsAndSlimes()
    {
        CurrentSliceToDestroy.Clear();
        for (int x = TopLeftOfGrid.x + indexOfDestroyedSlice * sliceWidth; x < TopLeftOfGrid.x + indexOfDestroyedSlice * sliceWidth + sliceWidth; x++)
        {
            for (int y = TopLeftOfGrid.y; y >= TopLeftOfGrid.y - sliceHeight; y--)
            {
                CurrentSliceToDestroy.Add(new Vector3(x, y));

                if (GridManagement.getObjectFromPosition(new Vector3(x, y)))
                {
                    print(GridManagement.getObjectFromPosition(new Vector3(x, y)) + "at " + new Vector3(x, y));
                }
            }
        }
        print(CurrentSliceToDestroy.Count);
        Debug.DrawLine(CurrentSliceToDestroy[0], CurrentSliceToDestroy[CurrentSliceToDestroy.Count - 1], Color.red, 1000);


        foreach (Vector3 eachPos in CurrentSliceToDestroy)
        {
            GameObject objInPos=GridManagement.getObjectFromPosition(eachPos);
            if (objInPos)
            {
                if (objInPos.tag == "Slime")
                {
                    SlimePathfinding slimeScript = objInPos.GetComponent<SlimePathfinding>();
                    if (slimeScript)
                    {
                        slimeScript.enabled = false;
                        slimeScript.StopAllCoroutines();
                    }
                    GridManagement.ClearPosition(eachPos, true, false);
                    inhaledObjects.Add(objInPos);
                }
                else if (objInPos.tag == "Tower")
                {
                    Tower towerScript =objInPos.GetComponent<Tower>();
                    if (towerScript)
                    {
                        towerScript.enabled = false;
                    }
                    //remove it from the grid without deleting it
                    GridManagement.ClearPosition(eachPos, true, false);
                    inhaledObjects.Add(objInPos);
                    print("collected tower: " + objInPos);
                    Instantiate(sugarPrefab, eachPos,Quaternion.identity);
                }
            }

        }
    }
    IEnumerator SlimeWakeAndMove()
    {
        //set animation to awake n angry
        spriteComp.sprite = angry;
        slimeIsAwake = true;
        GameObject.FindGameObjectWithTag("Music").GetComponent<MusicManager>().RageHappens();
        yield return new WaitForSeconds(angryFaceDelay);
        wholeBar?.SetActive(false);
        selfShake?.ShakeObjectIndefinitely(.15f);
        while (Slime.transform.position.y > sinkHeight)
        {
            Slime.transform.position += Vector3.down * moveSpeed * Time.deltaTime;
            yield return null;
        }
        Slime.transform.position = new Vector3(TopLeftOfGrid.x+indexOfDestroyedSlice*sliceWidth + (sliceWidth/2), sinkHeight);
        selfShake?.StopShaking();
        yield return new WaitForSeconds(sinkDelay);

        selfShake?.ShakeObjectIndefinitely(.15f);
        while (Slime.transform.position.y < startSlimePos.y)
        {
            Slime.transform.position += Vector3.up * moveSpeed * Time.deltaTime;
            yield return null;
        }
        Slime.transform.position = new Vector3(Slime.transform.position.x, startSlimePos.y);
        StartCoroutine(InhaleObjects());
    }
    IEnumerator SlimeReturnToSlumber()
    {
        
        selfShake?.ShakeObjectIndefinitely(.15f);
        while (Slime.transform.position.y > sinkHeight)
        {
            Slime.transform.position += Vector3.down * moveSpeed * Time.deltaTime;
            yield return null;
        }
        Slime.transform.position = new Vector3(startSlimePos.x, sinkHeight);
        selfShake?.StopShaking();
        yield return new WaitForSeconds(sinkDelay);
        selfShake?.ShakeObjectIndefinitely(.15f);
        while (Slime.transform.position.y < startSlimePos.y)
        {
            Slime.transform.position += Vector3.up * moveSpeed * Time.deltaTime;
            yield return null;
        }
        Slime.transform.position = startSlimePos;
        //set animation back to sleepy

        slimeIsAwake = false;
        selfShake?.StopShaking();
        rageOmeter = 0;
        yield return new WaitForSeconds(angryFaceDelay);
        if (redBar)
        {
            redBar.size = new Vector2(Mathf.Round(maxRage * ((float)rageOmeter / (float)maxRage)), redBar.size.y);
            redBar.size = new Vector2(Mathf.Clamp(redBar.size.x, 0, maxRage), redBar.size.y);
        }
        wholeBar?.SetActive(true);
        spriteComp.sprite = sleepy;
    }
    IEnumerator InhaleObjects()
    {
        //decide what to suck
        DislodgeBuildingsAndSlimes();

        cameraShake?.ShakeObjectIndefinitely(.15f);

        //set animation to inhale
        spriteComp.sprite = sucky;
        suckSystem.Play();
        float t = 0;
        int deletedGuys = 0;
        print(inhaledObjects.Count);
        while (deletedGuys!=inhaledObjects.Count)
        {
            foreach(GameObject each in inhaledObjects)
            {
                if (each)
                {
                    each.transform.position = Vector3.MoveTowards(each.transform.position, Slime.position, inhaleSpeed * Time.deltaTime);
                    each.transform.position = new Vector3( each.transform.position.x + Mathf.Sin(t * wiggleFreq)*wiggleAmp/2,each.transform.position.y);
                    t = +Time.deltaTime;
                    if (each.transform.position.y >= Slime.position.y)
                    {
                        //create like an explosion effect or something
                        deletedGuys++;
                        Destroy(each);
                    }
                }
            }
            
            yield return null;
        }
        inhaledObjects.Clear();
        cameraShake?.StopShaking();
        selfShake?.StopShaking();
        StartCoroutine(SlimeReturnToSlumber());

        //set animation back to angry
        spriteComp.sprite = satisfied;
        suckSystem.Stop();
    }
    // Update is called once per frame
    void Update()
    {
        if (redBar)
        {
            redBar.size = new Vector2(Mathf.Round(maxRage * ((float)rageOmeter / (float)maxRage)), redBar.size.y);
            redBar.size = new Vector2(Mathf.Clamp(redBar.size.x, 0, maxRage), redBar.size.y);
        }
        if (!slimeIsAwake)
        {
            sleepySinewave += Time.deltaTime;
            Slime.position = new Vector3(Slime.position.x,startSlimePos.y+  Mathf.Sin( sleepySinewave* sleepFreq) * sleepAmp/2);

            snoreTimer += Time.deltaTime;

            if (snoreTimer >= snoreFrequency)
            {
                snoreTimer = 0;
                if (textParent && textParticlePrefab)
                {
                    previousSnore = Instantiate(textParticlePrefab, textParent);
                    LetterParticle newLetter = previousSnore.GetComponent<LetterParticle>();
                    newLetter.Initialize("ZZZ", Color.white, new Vector2(-2, 2), 2, .1f, .001f, 3f);
                }
            }
            decayTimer += Time.deltaTime;
            redBar.color = Color.Lerp(barOrange, Color.white, decayTimer / decayRate);
            if (decayTimer >= decayRate)
            {
                if (rageOmeter > 0)
                {
                    decayTimer = 0;
                    rageOmeter--;
                }
                
            }
        }
    }
}
