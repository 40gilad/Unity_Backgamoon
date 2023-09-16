using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_Trianslge_ : MonoBehaviour
{
    Dictionary<string, GameObject> TrianglesContainers;
    private static int TRIANGLES_AMOUNT = 24;

    void Awake()
    {
        TrianglesContainers = new Dictionary<string, GameObject>();

    }
    void Start()
    {
        init_triangles_dict();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void init_triangles_dict()
    {
        string currname;
        for (int i = 0; i < TRIANGLES_AMOUNT; i++)
        {
            currname = "Triangle" + i;
            TrianglesContainers.Add(currname, GameObject.Find(currname));
            TrianglesContainers[currname].GetComponent<SC_Triangle>().change_sprite_stat();
        }
    }
}
