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
    bool turn; // true= orange turn false= green turn
    int[] curr_dice;
    string[] last_triangle;
    int Triangle_Calc_Sign;
    SC_Piece Piece_Script;

    void Awake()
    {
        Debug.Log("Awake " + name);
        DiceRoller = new GameObject[2];
        DiceRoller[0] = GameObject.Find("Sprite_LeftRollDice");
        DiceRoller[1] = GameObject.Find("Sprite_RightRollDice");
        TrianglesContainers = new Dictionary<string, GameObject>();
        curr_dice=new int[2];
        last_triangle = new string[3];
    }

    void Start()
    {
        Debug.Log("Start " + name);
        assign_values_to_TrianglesContainers();

        /*opposite logic: for orange to start: turn= false, Triangle_Calc_Sign = -1.    for left to start do the oppiste */
        turn = false;
        Triangle_Calc_Sign = -1;
        /********************************************************************************/

        curr_dice[0] =0;
        curr_dice[1] = 0;
        last_triangle[0] = null;
        last_triangle[1] = null;
        last_triangle[2] = null;
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

    private void assign_values_to_TrianglesContainers()
    {
        /* FUNCTION ALSO SETACTIVE FALSE TO ALL TRIANGLES */
        string currname;
        Transform curr_triangle_transform;
        for (int i = 0; i < TRIANGLES_AMOUNT; i++)
        {
            currname = "Triangle" + i;
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
        SetUnActiveTriangles();
        string[] dist_triangle = new string[3];
        dist_triangle[0]= "Triangle" + (t_num + (curr_dice[0]*Triangle_Calc_Sign));
        dist_triangle[1]= "Triangle" + (t_num + (curr_dice[1] * Triangle_Calc_Sign));
        dist_triangle[2] = "Triangle" + (t_num + ((curr_dice[0]+curr_dice[1]) * Triangle_Calc_Sign));

        for (int i = 0; i < 3; i++)
        {
            if (i ==0 && dist_triangle[i] == dist_triangle[i + 1]) // if dice is double
                continue;
            if (TrianglesContainers.ContainsKey(dist_triangle[i]))
            {
                Change_TriangleState(TrianglesContainers[dist_triangle[i]].transform.parent.Find("Sprite_Triangle").gameObject);
                last_triangle[i] = dist_triangle[i];
            }
            else last_triangle[i] = null;
        }


    }

    private void SetUnActiveTriangles()
    {
        for (int i = 0; i < 3; i++)
        {
            if (last_triangle[i] != null)
            {
                Change_TriangleState(TrianglesContainers[last_triangle[i]].transform.parent.Find("Sprite_Triangle").gameObject);
                last_triangle[i] = null;
            }
        }
    }

    public void ChangeTurn()
    {
        SetUnActiveTriangles();
        Triangle_Calc_Sign *= -1;
        turn = !turn;

        Debug.Log("Changing turns to: " + turn);
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
