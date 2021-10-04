using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// object representing each tile that you can build on, has no collision with pathfinding stuff on its own
/// </summary>
public class RageObject : MonoBehaviour
{
    bool debug = true;
    public int rageAmount;
    public Transform rageBar;
    // Start is called before the first frame update
    void Start()
    {
        SlimeAngerManager.instance.AddTile(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //just for debug, activate rage when clicked
    private void OnMouseDown()
    {
        if (debug)
        {
            if(SlimeAngerManager.instance.IsTileBuildable(GridManagement.RoundVectorToInts(transform.position)))
            SlimeAngerManager.instance.BuiltOnTile(GridManagement.RoundVectorToInts(transform.position));
        }
    }
}
