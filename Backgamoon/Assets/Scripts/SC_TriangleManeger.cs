using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_TriangleManeger : MonoBehaviour
{
    public bool turn;

    void Awake()
    {
        Debug.Log("Awake" + name);
    }
    void OnEnable()
    {
        //Debug.Log("<color=yellow> SC_Sprite_Triangle OnEnable</Color>");
        SC_Board.Turn += Turn;

    }

    void OnDisable()
    {
        SC_Board.Turn -= Turn;
    }

    void Turn(int t)
    {
        Debug.Log("turn"+name);
        turn = (t == 1);
    }
}
