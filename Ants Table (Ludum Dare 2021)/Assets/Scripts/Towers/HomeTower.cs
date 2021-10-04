using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeTower : Tower
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject.FindGameObjectWithTag("Builder")?.GetComponent<Building>().AddHome();
        health = maxHealth;
    }
    public override void Death()
    {
        GameObject.FindGameObjectWithTag("Builder")?.GetComponent<Building>().RemoveHome();
        Destroy(gameObject);
    }
}
