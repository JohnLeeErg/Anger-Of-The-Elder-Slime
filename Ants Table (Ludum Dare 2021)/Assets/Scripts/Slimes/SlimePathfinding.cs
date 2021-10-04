using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimePathfinding : MonoBehaviour
{
    [SerializeField] float PATH_AROUND_TOWERS_PERCENTAGE = .5f;

    [SerializeField] string SLIME_TAG = "Slime";
    [SerializeField] string TOWER_TAG = "Tower";
    float speed = 1f;

    //non-inclusive
    public int MIN_ROW = -5;
    public int MAX_ROW = 2;

    [SerializeField] Vector3 currentPosition = new Vector3(0, 0, 0);
    [SerializeField] bool attackMode = false;
    [SerializeField] int nextRow = 0;

    [SerializeField] int killX = -15;
    [SerializeField] float pauseLength = .4f;

    Vector3 NULL_VECTOR = new Vector3(-1000000, -1000000, -1000000);
    Vector3 GRID_NULL_VECTOR = Vector3.one / 2;

    public Vector3 startingPos = new Vector3(0,0,0);

    bool movedForward = false;
    bool upFirst = true;

    Slime slime; 

    // Start is called before the first frame update
    void Start()
    {
        slime = gameObject.GetComponent<Slime>();
        speed = slime.speed;
        Move();
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.A)) {
        //    Move();
        //}
        if(currentPosition==new Vector3(.5f, .5f, .5f))
        {
            slime.Death();
        }
    }

    void Move() {
        currentPosition = GridManagement.GetPositionOfObject(gameObject);

        if (currentPosition == GRID_NULL_VECTOR)
        {
            gameObject.transform.position = new Vector3(startingPos.x + 3, startingPos.y, startingPos.z);
            GridManagement.SetObjectToPosition(gameObject, new Vector3(startingPos.x+3, startingPos.y, startingPos.z), true);
            return;
        }

        if (currentPosition.x == killX) {
            slime.dropSugar = false;
            slime.Damage(200);
        }


        nextRow = (int)currentPosition.x - 1;

        //prevents going back on itself when travelling multiple tiles vertically
        if (movedForward) {
            upFirst = Random.Range(0f, 1f) > .5f;
        }

        Vector3 nextPosition = NULL_VECTOR;


        //default movement script

        if (NextTileHasHouse()) {
            attackMode = true;
            nextPosition = ForwardTile();
        }
        else if (upFirst && IsHouseNorth())
        {
            print("found house north");
            if (!IsTileHouse(UpTile()))
            {
                nextPosition = UpTile();
            }
        }
        else if (IsHouseSouth())
        {
            print("found house south");
            if (!IsTileHouse(DownTile()))
            {
                nextPosition = DownTile();
            }
        }
        
        else if (NextTileFree())
        {
            nextPosition = ForwardTile();
        }

        else if (!NextTileHasTowers())
        {
            if (upFirst)
            {
                if (IsFreeTileNorth())
                {
                    nextPosition = UpTile();
                }
                else if (IsFreeTileSouth())
                {
                    nextPosition = DownTile();
                }
            }
            else
            {
                if (IsFreeTileSouth())
                {
                    nextPosition = DownTile();
                }
                else if (IsFreeTileNorth())
                {
                    nextPosition = UpTile();
                }
            }
        }

        //desides if 
        else if (NextTileHasTowers())
        {
            attackMode = Random.Range(0f, 1f) > PATH_AROUND_TOWERS_PERCENTAGE;

            if (attackMode)
            {
                nextPosition = ForwardTile();
            }
            else
            {
                if (upFirst)
                {
                    if (IsFreeTileNorth())
                    {
                        nextPosition = UpTile();
                    }
                    else if (IsFreeTileSouth())
                    {
                        nextPosition = DownTile();
                    }
                }
                else
                {
                    if (IsFreeTileSouth())
                    {
                        nextPosition = DownTile();
                    }
                    else if (IsFreeTileNorth())
                    {
                        nextPosition = UpTile();
                    }
                }
            }
        }

        //you are not in attack mode but you cant move forward cause all ways are blocked
        if (nextPosition == NULL_VECTOR) {
            attackMode = true;
            Vector3 nearestWall = GetNearestWall();

            if (nearestWall != NULL_VECTOR) {
                //if you are right beside a wall blocking your route
                if (((nearestWall - currentPosition).magnitude) == 1 && ((nearestWall - currentPosition) != BackTile())) {
                    nextPosition = nearestWall;
                }
                else
                {
                    nextPosition = new Vector3(currentPosition.x, currentPosition.y + Mathf.Sign(nearestWall.y - currentPosition.y), currentPosition.z);
                }
            }
        }

        if (nextPosition != NULL_VECTOR)
        {

            movedForward = ForwardTile() == nextPosition;
            if (attackMode)
            {
                Attack(nextPosition);
                MoveToTile(nextPosition);
            }
            else
            {

                MoveToTile(nextPosition);
            }
        }
        else {
            StartCoroutine(DelayCheck());
        }
    }

    bool NextTileFree()
    {
        return IsTileFree(ForwardTile());
    }

    Vector3 GetNearestWall() {
        Vector3 returnVector = NULL_VECTOR;
        if (NextTileHasTowers()) {
            returnVector = ForwardTile();
        }
        else {
            for (int i = 1; i < MAX_ROW - MIN_ROW; i++) {
                Vector3 upTile = new Vector3(currentPosition.x, currentPosition.y + i, currentPosition.z);
                Vector3 downTile = new Vector3(currentPosition.x, currentPosition.y - i, currentPosition.z);

                if (TileHasTowers(upTile))
                {
                    returnVector = upTile;
                    break;
                }
                else if (TileHasTowers(downTile)) {
                    returnVector = downTile;
                    break;
                }

                upTile = new Vector3(upTile.x - 1, upTile.y, upTile.z);
                downTile = new Vector3(downTile.x - 1, downTile.y, downTile.z);

                if (TileHasTowers(upTile))
                {
                    returnVector = upTile;
                    break;
                }
                else if (TileHasTowers(downTile))
                {
                    returnVector = downTile;
                    break;
                }
            }
        }

        return returnVector;
    }

    bool TileHasTowers(Vector3 pos) {
        bool tileFree = false;

        GameObject gridObject = GridManagement.getObjectFromPosition(pos);

        if (gridObject)
        {
            if (gridObject.CompareTag(TOWER_TAG))
            {
                tileFree = true;
            }
        }

        return tileFree;
    }

    bool NextTileHasHouse()
    {
        bool tileFree = false;

        GameObject gridObject = GridManagement.getObjectFromPosition(ForwardTile());

        if (gridObject)
        {
            if (gridObject.GetComponent<HomeTower>())
            {
                tileFree = true;
            }
        }

        return tileFree;
    }
    bool IsTileHouse(Vector3 pos)
    {
        bool tileHasHouse = false;

        GameObject gridObject = GridManagement.getObjectFromPosition(pos);

        if (gridObject)
        {
            if (gridObject.GetComponent<HomeTower>())
            {
                print("found a house");
                tileHasHouse = true;
            }
        }

        return tileHasHouse;
    }
    //this includes towers 
    bool IsTileFree(Vector3 pos)
    {
        bool tileFree = true;

        GameObject gridObject = GridManagement.getObjectFromPosition(pos);

        if (gridObject)
        {
            tileFree = false;
        }

        return tileFree;

    }

    bool NextTileHasTowers() {
        bool tileFree = false;

        GameObject gridObject = GridManagement.getObjectFromPosition(ForwardTile());

        if (gridObject)
        {
            if (gridObject.CompareTag(TOWER_TAG))
            {
                tileFree = true;
            }
        }

        return tileFree;
    }

    bool IsFreeTileSouth() {
        //checks south then north
        bool path = false;
        for (float i = currentPosition.y - 1; i >= MIN_ROW; i--)
        {
            Vector3 goal = new Vector3(currentPosition.x, i, currentPosition.z);
            if (!IsTileFree(goal))
            {
                path = false;
                break;
            }
            else {
                goal = new Vector3(nextRow, i, currentPosition.z);
                if (IsTileFree(goal))
                {
                    path = true;
                    break;
                }
            }
        }
        return path;
    }

    bool IsFreeTileNorth()
    {
        //checks south then north
        bool path = false;
        for (float i = currentPosition.y + 1; i < MAX_ROW; i++)
        {
            Vector3 goal = new Vector3(currentPosition.x, i, currentPosition.z);
            if (!IsTileFree(goal)) {
                path = false;
                break;
            }
            else {
                goal = new Vector3(nextRow, i, currentPosition.z);
                if (IsTileFree(goal))
                {
                    path = true;
                    break;
                }
            }
        }
        return path;
    }
    bool IsHouseSouth()
    {
        //checks south then north
        bool path = false;
        for (float i = currentPosition.y - 1; i >= MIN_ROW; i--)
        {
            Vector3 goal = new Vector3(currentPosition.x, i, currentPosition.z);
            if (!IsTileHouse(goal) && !IsTileFree(goal))
            {
                path = false;
                break;
            }
            else
            {
                goal = new Vector3(nextRow, i, currentPosition.z);
                if (IsTileHouse(goal))
                {
                    path = true;
                    break;
                }
            }
        }
        return path;
    }
    bool IsHouseNorth()
    {
        //checks north for house
        bool path = false;
        for (float i = currentPosition.y + 1; i < MAX_ROW; i++)
        {
            Vector3 goal = new Vector3(currentPosition.x, i, currentPosition.z);
            if (!IsTileHouse(goal) && !IsTileFree(goal))
            {
                path = false;
                break;
            }
            else
            {
                goal = new Vector3(nextRow, i, currentPosition.z);
                if (IsTileHouse(goal))
                {
                    path = true;
                    break;
                }
            }
        }
        return path;
    }


    void MoveToTile(Vector3 newPos) {
        GridManagement.SetObjectToPosition(gameObject, newPos, false); //if theres something there dont move

        StartCoroutine(PositionLerp(gameObject, newPos));

    }

    IEnumerator PositionLerp(GameObject thingToMove, Vector3 positionToMoveTo)
    {
        float currentTime = 0;
        Vector3 originalPosish = thingToMove.transform.position;
        float step = .001f;
        float realPauseLength = .2f * speed;


        if (positionToMoveTo == ForwardTile())
        {
            yield return new WaitForSeconds(realPauseLength);
            currentTime += realPauseLength;
            //pre-jump
            while (true)
            {
                currentTime += Time.deltaTime;

                if (currentTime / speed <= 1f)
                {
                    float yMod;
                    float percent = (currentTime- realPauseLength) /( speed- realPauseLength); 

                    if (percent < .5f)
                    {
                        yMod = (percent)/ 1.5f;
                    }
                    else {
                        yMod = (1 - percent)/1.5f;
                    }
                    Mathf.Clamp(yMod,0,.5f);

                    Vector3 move = Vector3.Lerp(originalPosish, positionToMoveTo, percent);
                    move.y += yMod;

                    thingToMove.transform.position = move;
                    yield return new WaitForEndOfFrame();
                }
                else
                {
                    break;
                }
            }


            //post-jump

        }
        else { 
            
            while (true)
            {
                currentTime += Time.deltaTime;
                thingToMove.transform.position = Vector3.Lerp(originalPosish, positionToMoveTo, currentTime / speed);
                if (currentTime / speed <= 1f)
                {
                    yield return new WaitForEndOfFrame();
                }
                else
                {
                    break;
                }
            }
        }

        //snap it at the end to it's grid pos
        thingToMove.transform.position = GridManagement.GetPositionOfObject(thingToMove);

        Move();
    }

    IEnumerator DelayCheck()
    {
        yield return new WaitForSeconds(speed);
        Move();
    }

    void Attack(Vector3 enemyPos) {
        GameObject gridObject = GridManagement.getObjectFromPosition(enemyPos);
        if (gridObject) {
            if (gridObject.CompareTag(TOWER_TAG)) {
                gridObject.GetComponent<Tower>().Damage(slime.GetDamage());
                print(slime.GetDamage());
                slime.Death();
            }
        }
    }
    

    Vector3 ForwardTile() {
        return new Vector3(nextRow, currentPosition.y, currentPosition.z);
    }
    Vector3 DownTile()
    {
        return new Vector3(currentPosition.x, currentPosition.y-1, currentPosition.z);
    }
    Vector3 UpTile()
    {
        return new Vector3(currentPosition.x, currentPosition.y + 1, currentPosition.z);
    }
    Vector3 BackTile()
    {
        return new Vector3(currentPosition.x+1, currentPosition.y, currentPosition.z);
    }
}
