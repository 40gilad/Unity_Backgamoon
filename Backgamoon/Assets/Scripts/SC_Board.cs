using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
public class SC_Board : MonoBehaviour
{
    public delegate void Turn_Handler(int t);
    public static Turn_Handler Turn;
    GameObject[] DiceRoller=null;
    private static int TRIANGLES_AMOUNT = 24;
    private float PieceStackDist = 0.7f;
    Dictionary<string, GameObject> TrianglesContainers;
    bool turn;

    void Awake()
    {
        DiceRoller = new GameObject[2];
        DiceRoller[0] = GameObject.Find("Sprite_LeftRollDice");
        DiceRoller[1] = GameObject.Find("Sprite_RightRollDice");
        DiceRoller[0].SetActive(false);
        TrianglesContainers = new Dictionary<string, GameObject>();
        assign_values_to_TrianglesContainers();
        turn=false;
    }

    void OnEnable()
    {
        SC_Piece.Piece_Press += Piece_press;
    }

    private void OnDisable()
    {
        SC_Piece.Piece_Press -= Piece_press;

    }

    void Update()
    {
        
    }

    void Start()
    {
        Debug.Log("start SC_Board");
        ChangeTurn(); 
    }

    private void assign_values_to_TrianglesContainers()
    {
        /* FUNCTION ALSO SETACTIVE FALSE TO ALL TRIANGLES */
        string currname;
        Transform curr_triangle_transform;
        for (int i = 0; i < TRIANGLES_AMOUNT; i++)
        {
            currname = "Triangle" + i;
            //Debug.Log("Awake" + currname);
            curr_triangle_transform = GameObject.Find(currname).transform;
            Change_TriangleState(curr_triangle_transform.Find("Sprite_Triangle").gameObject);
            TrianglesContainers.Add(currname, curr_triangle_transform.Find("TrianglePiecesStack").gameObject);
        }
    }
    void Change_TriangleState(GameObject t)
    {
        t.SetActive(!t.activeSelf);
    }

    private void Piece_press(string t_name)
    {
        SetUnActiveTriangles();
        if (TrianglesContainers.ContainsKey(t_name))
        {
            Debug.Log("Piece_press " + t_name);
            Change_TriangleState(TrianglesContainers[t_name].transform.parent.Find("Sprite_Triangle").gameObject);
        }

    }

    private void SetUnActiveTriangles()
    {
        
    }

    void ChangeTurn()
    {
        turn = !turn;
        if (turn)
        {
            DiceRoller[0].SetActive(false);
            DiceRoller[1].SetActive(true);
            Turn(1);
        }
        else
        {
            DiceRoller[0].SetActive(true);
            DiceRoller[1].SetActive(false);
            Turn(0);
        }
    }
}
