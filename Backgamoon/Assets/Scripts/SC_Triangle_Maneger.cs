using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_Trianslge_ : MonoBehaviour
{
    Dictionary<string, GameObject> Triangles;
    private static int TRIANGLES_AMOUNT = 24;
    bool turn;
    SC_Board board;
    int[] curr_dice;
    string source_triangle;


    #region MonoBehaviour
    void Awake()
    {
        Triangles = new Dictionary<string, GameObject>();
        board = GameObject.Find("Board").GetComponent<SC_Board>();
        curr_dice = new int[2];
    }
    void Start()
    {
        init_triangles_dict();
        source_triangle = null;
        curr_dice[0] = 0;
        curr_dice[1] = 0;
    }

    private void OnEnable()
    {
        SC_Board.Turn += Turn;
        SC_Triangle.pressed_triangle += pressed_triangle;
        SC_DiceManeger.Roll_Dice += Roll_Dice;

    }

    private void OnDisable()
    {
        SC_Board.Turn -= Turn;
        SC_Triangle.pressed_triangle -= pressed_triangle;
        SC_DiceManeger.Roll_Dice -= Roll_Dice;

    }

    #endregion

    void Turn(bool t)
    {
        turn = t;
        Debug.Log("TManeger Change turn " + turn);
    }

    void pressed_triangle(string name)
    {
        Debug.Log("TManeger pressed_triangle " + name);
        Debug.Log("flags= " + board.flags["turn_stage"]);
        Debug.Log("TManeger turn " + turn);

        switch (board.flags["turn_stage"])
        {
            case 1:
                handle_press_after_throw(name);
                break;
            case 2:
                break;
            default:
                break;


        }
    }

    private void handle_press_after_throw(string name)
    {
        if (is_valid_press(name))
        {
            //take piece from triangle
            get_stack_script(name).pop_piece();

            //turn on relevant triangle (pressed,pressed+dice1, pressed+dice2
            get_triangle_script("Triangle" + (get_triangle_number(name) + curr_dice[0])).change_sprite_stat();
            get_triangle_script("Triangle" + (get_triangle_number(name) + curr_dice[1])).change_sprite_stat();

            //raise flag in SC_Board
            board.flags["turn_stage"] = 2;
        }
    }

    private bool is_valid_press(string name)
    {
        // check if the triangle was pressed to move a piece matches the turn (if orange pieces triangle when turn=True)
        char pressed_pieces_color= get_triangle_script(name).get_stack_color();
        Debug.Log("is_valid_press with color "+ pressed_pieces_color);
        if (((pressed_pieces_color == 'O') && turn) || (pressed_pieces_color == 'G') && !turn)
            return true;
        return false;
    }

    private void init_triangles_dict()
    {
        string currname;
        for (int i = 0; i < TRIANGLES_AMOUNT; i++)
        {
            currname = "Triangle" + i;
            Triangles.Add(currname, GameObject.Find(currname));
            get_triangle_script(currname).change_sprite_stat();
        }
    }

    SC_TrianglePiecesStack get_stack_script(string name)
    {
        return Triangles[name].transform.Find("TrianglePiecesStack").gameObject.GetComponent<SC_TrianglePiecesStack>();
    }

    SC_Triangle get_triangle_script(string name)
    {
        return Triangles[name].GetComponent<SC_Triangle>();
    }
    
    int get_triangle_number(string name)
    {
        int index = 0;
        foreach (var key in Triangles.Keys)
        {
            if (key == name)
                return index;
            index++;
        }
        return -1;
    }
    private void Roll_Dice(int left, int right = 0)
    {
        curr_dice[0] = left;
        curr_dice[1] = right;
    }
}
