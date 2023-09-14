using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_Sprit_Triangle : MonoBehaviour
{
    bool turn;
    GameObject[] Pieces; //0= orange piece, 1= green piece

    void Awake()
    {
        //Debug.Log("Awake" + name);
        Pieces= new GameObject[2];
    }
    void Start()
    {
        turn = false;
        Pieces[0] = GameObject.Find("OrangePiece");
        Pieces[1] = GameObject.Find("GreenPiece");

    }

    void OnEnable()
    {
        Debug.Log("SC_Sprit_Triangle OnEnable turn= " + turn);
        turn = GameObject.Find("SC_TriangleManeger").GetComponent<SC_TriangleManeger>().turn;
    }

    private void OnMouseDown()
    {
        Debug.Log("Pressed on " + transform.parent.name);
        Debug.Log("SC_Sprit_Triangle.OnMouseDown() with turn: " + turn);
        if (turn)//orange turn
            Move_Piece(Instantiate(Pieces[0]));
        else if (!turn)
            Move_Piece(Instantiate(Pieces[1]));
    }

    void Move_Piece(GameObject piece)
    {
        GameObject Triangle_Stack = transform.parent.transform.Find("TrianglePiecesStack").gameObject;
        int pieces_amout = Triangle_Stack.GetComponent<SC_TrianglePiecesStack>().pieces_amount;
        piece.transform.parent = Triangle_Stack.transform;
        piece.GetComponent<Transform>().localPosition = new Vector3(0, (-0.7f * pieces_amout), 0);
        piece.GetComponent<Transform>().localScale = new Vector3(1, 1, 1);
        Triangle_Stack.GetComponent<SC_TrianglePiecesStack>().pieces_amount++;
    }
}
