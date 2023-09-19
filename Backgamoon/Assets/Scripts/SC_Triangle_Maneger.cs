using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class SC_Triangle_Maneger : MonoBehaviour
{
    public delegate void Finish_Move_Handler();
    public static Finish_Move_Handler finish_turn;
    Dictionary<string, GameObject> Triangles;
    private static int TRIANGLES_AMOUNT = 24;
    bool turn;
    int turn_moves;
    int direction_accelerator = 0;
    SC_Board board;
    int[] curr_dice;
    int source_triangle;
    int[] dest_triangles;
    public GameObject orange_piece;
    public GameObject green_piece;



    #region MonoBehaviour
    void Awake()
    {
        Triangles = new Dictionary<string, GameObject>();
        board = GameObject.Find("Board").GetComponent<SC_Board>();
        curr_dice = new int[2];
        dest_triangles = new int[2];
    }
    void Start()
    {
        init_triangles_dict();
        init_vars();
        direction_accelerator = 1;//needs to be set to 0
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

    #region Delegates
    void Turn(bool t)
    {
        turn = t;
        if (turn)
            direction_accelerator = 1;
        else if (!turn)
            direction_accelerator = -1;
        else
            direction_accelerator = 0;

    }
    void pressed_triangle(string name)
    {
        Debug.Log("TManeger pressed_triangle " + name);
        Debug.Log("flags= " + board.flags["turn_stage"]);
        Debug.Log("TManeger turn " + turn);


        if(board.flags["turn_stage"]==1)//pressing on source triangle
            handle_press_after_throw(name);
        if (transform.Find("Sprite_Triangle"))// need to check if sprite was pressed means that sprite is on
            handle_press_as_new_location(name);

    }
    private void Roll_Dice(int left, int right = 0)
    {
        curr_dice[0] = left;
        curr_dice[1] = right;
    }
    #endregion

    #region Mouse Click Handlers
    private void handle_press_after_throw(string name)
    {
        if (is_valid_press(name))
        {
            Debug.Log("is_valid_press dire_acc= " + direction_accelerator);
            source_triangle = get_triangle_number(name);
            dest_triangles[0] = source_triangle + (curr_dice[0] * direction_accelerator);
            dest_triangles[1] = source_triangle + (curr_dice[1] * direction_accelerator);
            //take piece from triangle
            get_triangle_script(name).pop_piece();

            //turn on relevant triangle (pressed,pressed+dice1, pressed+dice2
            if (curr_dice[0] != 0 && is_valid_destination(dest_triangles[0]))
                get_triangle_script("Triangle" + (dest_triangles[0])).change_sprite_stat();
            if (curr_dice[1] != 0 && is_valid_destination(dest_triangles[1]))
                get_triangle_script("Triangle" + (dest_triangles[1])).change_sprite_stat();

            //raise flag in SC_Board?
        }
    }

    private void handle_press_as_new_location(string name)
    {
        Debug.Log("handle_press_as_move " + name);

        int triangle_number = get_triangle_number(name);
        turn_moves++;
        SC_Triangle sc_triangle = get_triangle_script(name);

        if (sc_triangle.is_vunarable(turn))
        {
            sc_triangle.pop_piece();
            Debug.Log("<color=red>write capture logic</color>");
        }

        push_piece(name);
        if (is_valid_destination(triangle_number))
        {
            update_dice(triangle_number - source_triangle);
            end_move(triangle_number);
        }
    }

    #endregion

    #region Support functions
    private bool is_valid_press(string name)
    {
        // check if the triangle was pressed to move a piece matches the turn (if orange pieces triangle when turn=True)
        char pressed_pieces_color = get_triangle_script(name).get_stack_color();
        Debug.Log("is_valid_press with color " + pressed_pieces_color);
        if (((pressed_pieces_color == 'O') && turn) || (pressed_pieces_color == 'G') && !turn)
            return true;
        return false;
    }

    private bool is_valid_destination(int dest)
    {
        char dest_color = get_triangle_script("Triangle" + dest).get_stack_color();
        if (turn && dest > source_triangle && (dest_color == 'O' || dest_color == 'N')
            || (!turn && dest < source_triangle && (dest_color == 'G' || dest_color == 'N')))
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

    void update_dice(int n)
    {
        if (n == curr_dice[0])
            curr_dice[0] = 0;
        else if (n == curr_dice[1])
            curr_dice[1] = 0;
    }

    void push_piece(string name)
    {
        SC_Triangle sc_triangle = get_triangle_script(name);
        if (turn)
            sc_triangle.push_piece(Instantiate(orange_piece), 'O');
        else if (!turn)
            sc_triangle.push_piece(Instantiate(green_piece), 'G');
        sc_triangle.change_sprite_stat();
    }

    void end_move(int triangle_number)
    {
        if (turn_moves == 2 && board.flags["double"] == 0)
        {
            init_vars();
            finish_turn();
            return;
        }
        board.flags["turn_stage"] = 1;
        if (dest_triangles[0] == triangle_number)
        {
            dest_triangles[0] = -1;
            triangle_number = dest_triangles[1];
        }
        else if (dest_triangles[1] == triangle_number)
        {
            dest_triangles[1] = -1;
            triangle_number = dest_triangles[0];
        }
        //Triangles["Triangle" + triangle_number].GetComponent<SC_Triangle>().change_sprite_stat();
    }

    private void init_vars()
    {
        source_triangle = -1;
        turn_moves = 0;
        curr_dice[0] = 0;
        curr_dice[1] = 0;
        dest_triangles[0] = -1;
        dest_triangles[1] = -1;
    }
    #endregion
}
