using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Unity.Loading;
using UnityEngine;
using UnityEngine.Apple;

public class SC_Triangle_Maneger : MonoBehaviour
{
    public delegate void Finish_Move_Handler();
    public static Finish_Move_Handler finish_turn;
    Dictionary<string, GameObject> Triangles;
    private const int TRIANGLES_AMOUNT = 25; //triangle 24- green captured, triangle -1 - orange captured
    private const int LAST_TRIANGLE = 23;

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
        Debug.Log("Awake");
        Triangles = new Dictionary<string, GameObject>();
        board = GameObject.Find("Board").GetComponent<SC_Board>();
        curr_dice = new int[2];
        dest_triangles = new int[2];
    }

    void Start()
    {
        Debug.Log("Start");
        init_triangles_dict();
        init_vars();
        direction_accelerator = 1;//needs to be set to 0
    }

    private void OnEnable()
    {
        Debug.Log("OnEnable");
        SC_Board.Turn += Turn;
        SC_Triangle.pressed_triangle += pressed_triangle;
        SC_DiceManeger.Roll_Dice += Roll_Dice;
    }

    private void OnDisable()
    {
        Debug.Log("OnDisable");
        SC_Board.Turn -= Turn;
        SC_Triangle.pressed_triangle -= pressed_triangle;
        SC_DiceManeger.Roll_Dice -= Roll_Dice;
    }
    #endregion

    #region Delegates
    void Turn(bool t)
    {
        Debug.Log("Turn " + t);
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
        if (get_triangle_number(name) == source_triangle)//repressed, cancele the press
        {

        }
        Debug.Log("pressed_triangle " + name);
        if ((turn && board.flags["captures"] == 1) || (!turn && board.flags["captures"] == 2))
            Debug.Log("breakpoint");
        turn_off_dest_triangles();
        if (get_triangle_number(name) == dest_triangles[0] || get_triangle_number(name) == dest_triangles[1])
            handle_press_as_new_location(name);
        else if (board.flags["turn_stage"] == 1)//pressing on source triangle
            handle_press_after_throw(name);
    }

    private void Roll_Dice(int left, int right = 0)
    {
        Debug.Log("Roll_Dice " + left + ", " + right);
        curr_dice[0] = left;
        curr_dice[1] = right;
        if (!check_available_moves())
            board.ChangeTurn();
    }
    #endregion

    #region Mouse Click Handlers
    private void handle_press_after_throw(string name)
    {
        Debug.Log("handle_press_after_throw " + name);
        if (name == "Triangle-1")
            Debug.Log("breakpoing- line 95");
        source_triangle = get_triangle_number(name);
        Debug.Log("<color=blue> handle_press_after_throw triangle " + name + "</color>");
        if (is_valid_press(name))
        {
            if (source_triangle == 100 || source_triangle == 200)
                Debug.Log("<color=cayan> check is valid press on captured</color>");
            dest_triangles[0] = source_triangle + (curr_dice[0] * direction_accelerator);
            dest_triangles[1] = source_triangle + (curr_dice[1] * direction_accelerator);

            //turn on relevant triangle (pressed,pressed+dice1, pressed+dice2
            if (board.flags["double"] == 1)
            {
                if (curr_dice[0] != 0 && is_valid_destination(dest_triangles[0]))
                    get_triangle_script("Triangle" + (dest_triangles[0])).change_sprite_stat();
            }
            else
            {
                if (curr_dice[0] != 0 && is_valid_destination(dest_triangles[0]))
                    get_triangle_script("Triangle" + (dest_triangles[0])).change_sprite_stat();
                if (curr_dice[1] != 0 && is_valid_destination(dest_triangles[1]))
                    get_triangle_script("Triangle" + (dest_triangles[1])).change_sprite_stat();
            }
        }
    }

    private void handle_press_as_new_location(string name)
    {
        Debug.Log("handle_press_as_new_location " + name);
        int triangle_number = get_triangle_number(name);
        turn_moves++;
        SC_Triangle sc_triangle = get_triangle_script(name);
        get_triangle_script("Triangle" + source_triangle).pop_piece();
        if (sc_triangle.is_vunarable(turn))
            captured(name);
        push_piece(name);
        if (board.flags["double"] != 1)
        {
            if (turn)
                update_dice(triangle_number - source_triangle);
            else if (!turn)
                update_dice(source_triangle - triangle_number);
        }
        end_move(triangle_number);
    }
    #endregion

    #region Support functions
    private bool is_valid_press(string name)
    {
        Debug.Log("is_valid_press " + name);
        // check if the triangle was pressed to move a piece matches the turn (if orange pieces triangle when turn=True)
        char pressed_pieces_color = get_triangle_script(name).get_stack_color();
        if (((pressed_pieces_color == 'O') && turn) || (pressed_pieces_color == 'G') && !turn)
            return true;
        return false;
    }

    private bool is_valid_destination(int dest)
    {
        if (dest > 23 || dest < 0)
            return false;
        Debug.Log("is_valid_destination " + dest);
        SC_Triangle Tdest = get_triangle_script("Triangle" + dest);
        char dest_color = Tdest.get_stack_color();
        if (turn && dest > source_triangle && (dest_color == 'O' || dest_color == 'N' || (dest_color == 'G' && Tdest.is_vunarable(turn)))
            || (!turn && dest < source_triangle && (dest_color == 'G' || dest_color == 'N' || (dest_color == 'O' && Tdest.is_vunarable(turn)))))
            return true;
        return false;
    }

    private void init_triangles_dict()
    {
        Debug.Log("init_triangles_dict");
        string currname;
        for (int i = -1; i < TRIANGLES_AMOUNT; i++)
        {
            currname = "Triangle" + i;
            Triangles.Add(currname, GameObject.Find(currname));
            get_triangle_script(currname).change_sprite_stat();
        }
    }

    SC_Triangle get_triangle_script(string name)
    {
        Debug.Log("get_triangle_script " + name);
        return Triangles[name].GetComponent<SC_Triangle>();
    }

    int get_triangle_number(string name)
    {
        Debug.Log("get_triangle_number " + name);
        int index = -1;
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
        Debug.Log("update_dice " + n);
        if (n == curr_dice[0])
            curr_dice[0] = 0;
        else if (n == curr_dice[1])
            curr_dice[1] = 0;
    }

    void push_piece(string name)
    {
        Debug.Log("push_piece " + name);
        SC_Triangle sc_triangle = get_triangle_script(name);
        if (turn)
            sc_triangle.push_piece(Instantiate(orange_piece), 'O');
        else if (!turn)
            sc_triangle.push_piece(Instantiate(green_piece), 'G');
    }

    void end_move(int triangle_number)
    {
        Debug.Log("end_move " + triangle_number);
        if (board.flags["double"] == 1 && turn_moves == 4)
        {
            init_vars();
            finish_turn();
            return;
        }
        else if (turn_moves == 2 && board.flags["double"] == 0)
        {
            init_vars();
            finish_turn();
            return;
        }
        board.flags["turn_stage"] = 1;
        if (board.flags["double"] == 1)
            dest_triangles[0] = dest_triangles[1] = -2;
        else if (dest_triangles[0] == triangle_number)
        {
            dest_triangles[0] = -2;
            triangle_number = dest_triangles[1];
        }
        else if (dest_triangles[1] == triangle_number)
        {
            dest_triangles[1] = -2;
            triangle_number = dest_triangles[0];
        }
    }

    void turn_off_dest_triangles()
    {
        Debug.Log("turn_off_dest_triangles");
        for (int i = 0; i < 2; i++)
        {
            if (dest_triangles[i] != -2)
            {
                SC_Triangle curr = get_triangle_script("Triangle" + dest_triangles[i]);
                if (curr.is_sprite_active())
                    curr.change_sprite_stat();
            }
        }
    }

    private void init_vars()
    {
        Debug.Log("init_vars");
        source_triangle = -1;
        turn_moves = 0;
        curr_dice[0] = 0;
        curr_dice[1] = 0;
        turn_off_dest_triangles();
        dest_triangles[0] = -2;
        dest_triangles[1] = -2;
    }

    private void captured(string name)
    {
        Debug.Log("captured " + name);
        turn = !turn;
        SC_Triangle sc_triangle = get_triangle_script(name);
        sc_triangle.pop_piece();
        if (turn)
        {
            push_piece("Triangle-1");
            board.flags["captures"] = 1;
        }
        else if (!turn)
        {
            push_piece("Triangle24");
            board.flags["captures"] = 2;
        }
        turn = !turn;
    }

    private bool check_available_moves()
    {
        int[] green_stacks = new int[24];
        int[] orange_stacks = new int[24];
        green_stacks = get_stacks('G');
        orange_stacks = get_stacks('O');
        if (turn)//check available moves for orange
        {
            if (board.flags["captures"] == 1)//check available moves for orange captured stack
            {
                if ((curr_dice[0] != 0 && is_valid_destination(curr_dice[0] - 1))
                    || (curr_dice[1] != 0 && is_valid_destination(curr_dice[1] - 1)))
                    return true;
                else
                    return false;
            }
            for (int i = 0; i < 24; i++)
            {
                if (orange_stacks[i] != -2)
                {
                    source_triangle = orange_stacks[i];
                    if ((curr_dice[0] != 0 && is_valid_destination(i + curr_dice[0])) ||
                        curr_dice[1] != 0 && is_valid_destination(i + curr_dice[1]))
                        return true;
                }   
            }
            return false;

        }
        else if (!turn)//check available moves for green
        {
            if (board.flags["captures"] == 2)//check available moves for green captured stack
            {
                if ((curr_dice[0] != 0 && is_valid_destination(LAST_TRIANGLE - (curr_dice[0] - 1)))
                    || (curr_dice[1] != 0 && is_valid_destination(LAST_TRIANGLE - (curr_dice[1] - 1))))
                    return true;
                else
                    return false;
            }
            for (int i = 0; i < 24; i++)
            {
                if (green_stacks[i] != -2)
                {
                    source_triangle = green_stacks[i];
                    if ((curr_dice[0] != 0 && is_valid_destination(i - curr_dice[0])) ||
                        curr_dice[1] != 0 && is_valid_destination(i - curr_dice[1]))
                        return true;
                }
            }
            return false;
        }
        return false;
    }

    private void captured_turn()
    {
        Debug.Log("captured_turn");
    }

    private int[] get_stacks(char color)
    {
        int[] res = new int[24];
        for (int i = 0; i < 24; i++)
        {
            SC_Triangle curr_triangle = get_triangle_script("Triangle" + i);
            char curr_color = curr_triangle.get_stack_color();
            int curr_amount = curr_triangle.top();
            if (curr_color == color && curr_amount >= 1)
                res[i] = i;
            else
                res[i] = -2;
        }
        return res;

    }
    #endregion
}
