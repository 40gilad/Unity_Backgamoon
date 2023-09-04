using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_Piece : MonoBehaviour
{
    public delegate void Piece_Press_Handler(int n);
    public static Piece_Press_Handler Piece_Press;
    bool turn;
    void Start()
    {
        turn = false;
    }

    void OnEnable()
    {
        SC_Board.Turn += Turn;
    }

    void OnDisable()
    {
        SC_Board.Turn -= Turn;
    }
    // Update is called once per frame

    private void OnMouseDown()
    {
        string piece_color = name.Substring(0, 5);
        if (turn && piece_color=="Orang")
        {
            string triangle_name = transform.parent.transform.parent.name;
            Debug.Log("calling with : " + triangle_name);
            Piece_Press(int.Parse(triangle_name.Substring(8)));
        }
        else if(!turn && piece_color == "Green")
        {
            string triangle_name = transform.parent.transform.parent.name;
            Debug.Log("calling with : " + triangle_name);
            Piece_Press(int.Parse(triangle_name.Substring(8)));
        }
    }
    private void Turn(int t)
    {
        turn = (t == 1);
    }
}
