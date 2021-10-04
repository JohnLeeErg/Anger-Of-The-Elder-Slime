using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Building : MonoBehaviour
{
    [SerializeField] Sprite[] towerImages;
    [SerializeField] GameObject[] towers;
    [SerializeField] TMP_Text[] towerCosts;
    [SerializeField] Button[] towerButtons;
    [SerializeField] int startingMoney = 800;
    [SerializeField] Canvas parentCanvas;

    [SerializeField] int[] costs =
    {
        200, //shoot
        400, //beam
        250, //aoe
        75, //wall
        300, //mine
        25
    };

    int houses = 0;
    [SerializeField] int money = 0;
    [SerializeField] TMP_Text moneyText, housesText, towerText;
    [SerializeField] Image towerTextBG;
    
    [SerializeField] AudioSource PickUp, Error, Place;

    [SerializeField] GameObject ghostBuilding, cantBuild;
    [SerializeField] GameObject deathText;

    Transform ghostTransform;
    SpriteRenderer ghostSpriteRenderer, cantBuildSR;

    Transform towerTextTransform;

    Vector3 mouseOffset = new Vector3(2, -2, 0);

    float transparency = 0.5f;

    int selectedTower;
    int maxHouses;
    bool selected = false;
    private Camera cam;
    private bool hoverEnabled = true;

    private string[] towerDescriptions =
    {
        "This tower shoots boba pearls rightwards towards the slimes.",
        "This tower fires a constant beam at a nearby slime.",
        "This tower stomps, damaging all the nearby slimes.",
        "This wall building blocks slimes, protecting your towers and homes.",
        "This building generates sugar cubes, but is quite weak to slimes.",
        "This will reveal the rage level on a tile without triggering it.",
        "EMPTY SPACE: You shouldn't be seeing this...",
        "This is the wave counter, once you beat all 5 waves you'll continue on.",
        "This is how many homes you have left, lose them all and lose the game!",
        "Little sugar cubes will appear around the map, click them quick for sugar!",
    };

    private string[] toolTips =
    {
        "Clicking this icon will show you a new tip every time you hover over it!",
        "If you find the hover text annoying, press 'H' to toggle it.",
        "The bar above the big slime on the right tracks the amount of rage it has built up.",
        "The number of fire icons on a tile is how much the rage bar will fill when you build.",
        "When the bar fills, the big slime will destroy everything within a section.",
        "The bar will go down over time, but placing new towers will stall this.",
        "The red-ish dividers on the grid denote the three different sections of the map.",
        "When the big slime rages, it will always target the section most recently built in.",
        "Since the big slime destroys slimes, you may be able to use rage strategically.",
        "Make sure you try not to miss any of the sugar cubes around the map!",
        "Building mines early will generate more value, but may leave you open to attack.",
        "Bushes and trees will block bullets, so try to give your boba towers long sightlines.",
        "Walls will not block the bullets from your boba towers.",
        "Slimes die when they damage a building, use your walls accordingly :3.",
        "Struggling with the higher health slimes? Trying saving for a beam tower.",
        "A well placed cupcake tower at the front makes short work of low health slimes.",
        "If the rage meter is nearly full, a magnifying glass might be a smart buy.",
        "Clustering all your towers in one section is a recipe for disaster.",
        "There are grid locations that slimes will rarely go to, use these to your benefit.",
        "You've read all the hints! You should now be a master of this game!",
    };

    private int tipCounter = 0;

    public void PickupSugar(int sugarValue)
    {
        PickUp.Play();
        AddMoney(sugarValue);
    }

    public void SlimeDeath(int sugarValue, Vector3 slimePos, Quaternion slimeRot)
    {
        AddMoney(sugarValue);
        Instantiate(deathText, slimePos, slimeRot);
    }

    public void HoverButton(int button)
    {
        if(hoverEnabled)
        {
            towerText.enabled = true;
            towerTextBG.enabled = true;
            if (button == 99)
            {
                towerText.text = toolTips[tipCounter];
            }
            else
            {
                towerText.text = towerDescriptions[button];
                //Cursor.visible = false;
            }
        }
    }

    public void HoverButtonOff(int button)
    {
        towerText.text = "";
        towerText.enabled = false;
        towerTextBG.enabled = false;
        Cursor.visible = true;

    }

    // Start is called before the first frame update
    void Start()
    {
        AddMoney(startingMoney);
        moneyText.text = ""+money;

        cam = Camera.main;
        ghostTransform = Instantiate(ghostBuilding, Vector3.zero, Quaternion.identity).transform;
        ghostSpriteRenderer = ghostTransform.GetComponent<SpriteRenderer>();
        ghostSpriteRenderer.color = new Color(ghostSpriteRenderer.color.r, ghostSpriteRenderer.color.g, ghostSpriteRenderer.color.b, transparency);
        ghostSpriteRenderer.enabled = false;

        cantBuildSR = Instantiate(cantBuild, ghostTransform).GetComponent<SpriteRenderer>();
        cantBuildSR.transform.position = ghostTransform.position;

        for (int x = 0; x < towerCosts.Length; x++)
        {
            towerCosts[x].text = ""+costs[x];
        }

        towerTextTransform = towerTextBG.transform;

        towerText.enabled = false;
        towerTextBG.enabled = false;

        //UI following mouse code
        Vector2 pos;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentCanvas.transform as RectTransform, Input.mousePosition,
            parentCanvas.worldCamera,
            out pos);
    }

    // Update is called once per frame
    void Update()
    {
        if (selected)
        {
            MoveSnapMouse();

            if (Input.GetMouseButtonDown(0))
            {
                Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
                mousePos = new Vector3(Mathf.Round(mousePos.x), Mathf.Round(mousePos.y), 0);

                if (selectedTower == 5)
                {
                    if (SlimeAngerManager.instance.IsTileBuildable(mousePos))
                    {
                        SpendMoney(costs[selectedTower]);
                        SlimeAngerManager.instance.RevealTile(mousePos);
                        Place.Play();
                        Deselect();
                    }
                    else
                    {
                        Error.Play();
                    }
                }
                else
                {
                    if (SlimeAngerManager.instance.IsTileBuildable(mousePos))
                    {
                        SlimeAngerManager.instance.BuiltOnTile(mousePos);
                        Instantiate(towers[selectedTower], mousePos, Quaternion.identity);
                        SpendMoney(costs[selectedTower]);
                        Place.Play();
                        Deselect();
                    }
                    else
                    {
                        Error.Play();
                    }
                }
            }
            else if (Input.GetMouseButtonDown(1))
            {
                Deselect();
            }
        }

        if(Input.GetKeyDown(KeyCode.H))
        {
            hoverEnabled = !hoverEnabled;
        }

        ButtonCosts();
        FollowMouseUI();
    }

    void FollowMouseUI()
    {
        Vector2 movePos;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentCanvas.transform as RectTransform,
            Input.mousePosition, parentCanvas.worldCamera,
            out movePos);

        towerTextTransform.position = parentCanvas.transform.TransformPoint(movePos) + mouseOffset;

    }

    void ButtonCosts()
    {
        for (int x = 0; x < towerButtons.Length; x++)
        {
            if(money - costs[x] < 0)
            {
                towerButtons[x].interactable = false;
            }
            else
            {
                towerButtons[x].interactable = true;
            }
        }
    }

    public void NewHint()
    {
        tipCounter++;
        if(tipCounter >= toolTips.Length)
        {
            tipCounter = 1;
        }

        towerText.text = toolTips[tipCounter];
    }

    public void AddHome()
    {
        houses++;
        maxHouses++;
        housesText.text = "" + houses + "/" + maxHouses; 
    }

    public void RemoveHome()
    {
        houses--;
        housesText.text = "" + houses + "/" + maxHouses;

        if (houses <= 0)
        {
            Lose();
        }
    }

    void Lose()
    {
        sceneManager.instance.EndGameBad();
    }

    void AddMoney(int newMoney)
    {
        money += newMoney;
        moneyText.text = "" + money;
    }

    void SpendMoney(int spentMoney)
    {
        money -= spentMoney;
        moneyText.text = "" + money;
    }

    void Deselect()
    {
        selected = false;
        ghostSpriteRenderer.enabled = false;
        cantBuildSR.enabled = false;
    }

    void MoveSnapMouse()
    {
        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        mousePos = new Vector3(Mathf.Round(mousePos.x), Mathf.Round(mousePos.y), 0);
        ghostTransform.position = mousePos;

        if(SlimeAngerManager.instance.IsTileBuildable(mousePos))
        {
            cantBuildSR.enabled = false;
        }
        else
        {
            cantBuildSR.enabled = true;
        }
    }

    public void TowerSelect(int buttonNum)
    {
        selectedTower = buttonNum;
        selected = true;
        ghostSpriteRenderer.sprite = towerImages[buttonNum];
        ghostSpriteRenderer.enabled = true;


        if (buttonNum == 0) //Shooting
        {

        }
        else if (buttonNum == 1) //Beam
        {

        }
        else if (buttonNum == 2) //AOE
        {

        }
        else if (buttonNum == 3) //Wall
        {

        }
        else if (buttonNum == 4) // Mine
        {

        }
    }

}
