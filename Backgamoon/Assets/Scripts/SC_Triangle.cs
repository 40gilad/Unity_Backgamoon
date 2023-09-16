using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_TriangleManeger : MonoBehaviour
{
    GameObject Sprite_Triangle;
    void Awake()
    {
        Debug.Log("Awake" + name);
    }

    private void Start()
    {
        Sprite_Triangle = transform.Find("Sprite_Triangle").gameObject;
    }

    public void change_sprite_stat()
    {
        Sprite_Triangle.SetActive(!Sprite_Triangle.activeSelf);
    }
}
