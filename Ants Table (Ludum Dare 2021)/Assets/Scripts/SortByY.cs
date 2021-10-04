using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortByY : MonoBehaviour
{
    Renderer rendComp;
    // Start is called before the first frame update
    void Start()
    {
        rendComp = GetComponent<Renderer>();   
    }

    // Update is called once per frame
    void Update()
    {
        rendComp.sortingOrder =-(int) (transform.position.y*100);
    }
}
