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
    int[] curr_dice;

    void Awake()
    {
        Debug.Log("Awake " + name);
        DiceRoller = new GameObject[2];
        DiceRoller[0] = GameObject.Find("Sprite_LeftRollDice");
        DiceRoller[1] = GameObject.Find("Sprite_RightRollDice");
        TrianglesContainers = new Dictionary<string, GameObject>();
        curr_dice=new int[2];
    }

    void Start()
    {
        Debug.Log("Start " + name);
        assign_values_to_TrianglesContainers();
        DiceRoller[0].SetActive(false);
        turn = false;
        curr_dice[0] =0;
        curr_dice[1] = 0;
        ChangeTurn();

    }
    void OnEnable()
    {
        SC_DiceManeger.Roll_Dice += Roll_Dice;
        SC_Piece.Piece_Press += Piece_press;
    }

    private void OnDisable()
    {
        SC_Piece.Piece_Press -= Piece_press;
        SC_DiceManeger.Roll_Dice -= Roll_Dice;
    }

    void Update()
    {
        
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

    private void Piece_press(int t_num)
    {
        //SetUnActiveTriangles();
        string[] dist_triangle = new string[3];
        dist_triangle[0]= "Triangle" + (t_num + curr_dice[0]);
        dist_triangle[1]= "Triangle" + (t_num + curr_dice[1]);
        dist_triangle[2] = "Triangle" + (t_num + (curr_dice[0]+curr_dice[1]));

        for (int i = 0; i < 3; i++)
        {

            if (TrianglesContainers.ContainsKey(dist_triangle[i]))
                Change_TriangleState(TrianglesContainers[dist_triangle[i]].transform.parent.Find("Sprite_Triangle").gameObject);
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

    private void Roll_Dice(int left, int right = 0)
    {
        curr_dice[0] = left;
        curr_dice[1] = right;
    }
}
