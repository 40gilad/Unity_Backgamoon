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

    #region MonoBehaviour
    void Awake()
    {
        Triangles = new Dictionary<string, GameObject>();
        board = GameObject.Find("Board").GetComponent<SC_Board>();
    }
    void Start()
    {
        init_triangles_dict();
    }

    private void OnEnable()
    {
        SC_Board.Turn += Turn;
        SC_Triangle.pressed_triangle += pressed_triangle;
    }

    private void OnDisable()
    {
        SC_Board.Turn -= Turn;
        SC_Triangle.pressed_triangle -= pressed_triangle;
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
            Debug.Log("Handle press after dice roll!");
        }
    }

    private bool is_valid_press(string name)
    {
        // check if the triangle was pressed to move a piece matches the turn (if orange pieces triangle when turn=True)
        
        char pressed_pieces_color= Triangles[name].GetComponent<SC_Triangle>().get_stack_color();
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
            Triangles[currname].GetComponent<SC_Triangle>().change_sprite_stat();
        }
    }
}
