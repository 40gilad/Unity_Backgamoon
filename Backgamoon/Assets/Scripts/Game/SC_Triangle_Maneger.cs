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
    public delegate void No_Moves_Handler();
    public static No_Moves_Handler no_available_moves;
    public delegate void Game_Finished_Handler(char color,bool is_left=false);
    public static Game_Finished_Handler game_finished;

    /**************************************** CONSTANTS ****************************************/

    private const int TRIANGLES_AMOUNT = 25; //triangle 24- green captured, triangle -1 - orange captured
    private const int LAST_TRIANGLE = 23;
    private const int FIRST_TRIANGLE = 0;
    private const int FIRST_G_TRIANGLE = 5;
    private const int FIRST_O_TRIANGLE = 18;
    private const int ORANGE_CAPTURED_STACK = -1;
    private const int GREEN_CAPTURED_STACK = 24;

    /*******************************************************************************************/

    bool turn;
    int turn_moves;
    int direction_accelerator = 0;
    SC_Board board;
    public int[] curr_dice;
    int source_triangle;
    int[] dest_triangles;
    public GameObject orange_piece;
    public GameObject green_piece;
    Dictionary<string, GameObject> Triangles;
    Dictionary<string, Dictionary<string, string>> moves_to_send;
    List<int> key_source;
    List<int> val_dest;


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
        direction_accelerator = 0;//needs to be set to 0
    }

    private void OnEnable()
    {
        Debug.Log("OnEnable");
        SC_Board.Turn += Turn;
        SC_Board.play_singleplayer += play_singleplayer;
        SC_Triangle.pressed_triangle += pressed_triangle;
        SC_DiceManeger.Roll_Dice += Roll_Dice;
        SC_Board.play_other_player += play_other_player;
    }

    private void OnDisable()
    {
        Debug.Log("SC_Triangle_Maneger OnDisable");
        SC_Board.Turn -= Turn;
        SC_Board.play_singleplayer -= play_singleplayer;
        SC_Triangle.pressed_triangle -= pressed_triangle;
        SC_DiceManeger.Roll_Dice -= Roll_Dice;
        SC_Board.play_other_player += play_other_player;

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

    void pressed_triangle(string name,bool is_from_other_player=false)
    {      
        if (get_triangle_number(name) == source_triangle)
        {
            Debug.Log("<color=red>repressed, cancele the press</color>");
        }
        Debug.Log("<color=orange>84. pressed_triangle" + name + " Turn_moves= " + turn_moves + "</color>");
        turn_off_dest_triangles();
        if (get_triangle_number(name) == dest_triangles[0]
            || get_triangle_number(name) == dest_triangles[1])
            handle_press_as_new_location(name,is_from_other_player);
        else if (board.flags["turn_stage"] == 1)//pressing on source triangle
            handle_press_after_throw(name,is_from_other_player);
    }

    private void Roll_Dice(int left, int right = 0, bool is_from_other_player=false)
    {
        Debug.Log("Roll_Dice " + left + ", " + right);
        curr_dice[0] = left;
        curr_dice[1] = right;
        if (!is_from_other_player)
        {
            if (!check_available_moves())
            {
                if (board.multiplayer)
                    board.send_data(moves_to_send);
                no_available_moves();

            }
        }
    }

    private void play_other_player(int[] source, int[] dest)
    {
        if (curr_dice[0] == curr_dice[1])
        {
            Debug.Log("2. got double!! " + curr_dice[0] + "," + curr_dice[1]);
            board.flags["double"] = 1;
        }
        Debug.Log("turn= " + turn);
        StartCoroutine(CR_play_recieved_moves(source, dest));
        
    }

    private IEnumerator CR_play_recieved_moves(int[] source, int[] dest)
    {
        for (int i = 0; i < 4; i++)
        {
            Debug.Log("i= " + i);
            Debug.Log("Playing " + source[i] + " -> " + dest[i]);
            if (source[i] != -2)
            {
                yield return new WaitForEndOfFrame();
                Debug.Log("Moves a piece");
                board.flags["turn_stage"] = 1;
                pressed_triangle("Triangle" + source[i],true);
                board.flags["turn_stage"] = 2;
                pressed_triangle("Triangle" + dest[i],true);
            }
        }
    }
    #endregion

    #region Mouse Click Handlers
    private void handle_press_after_throw(string name,bool is_from_other_player=false)
    {
        Debug.Log("<color=blue> handle_press_after_throw triangle " + name + "</color>");
        source_triangle = get_triangle_number(name);
        if (is_valid_press(name))
        {
            dest_triangles[0] = source_triangle + (curr_dice[0] * direction_accelerator);
            dest_triangles[1] = source_triangle + (curr_dice[1] * direction_accelerator);
            if (board.flags["double"] == 1)
            {
                if (curr_dice[0] != 0 && is_valid_destination(dest_triangles[0]))
                    get_triangle_script("Triangle" + (dest_triangles[0])).change_sprite_stat();

            }
            else
            {   // activate sprite for destination triangles
                if (curr_dice[0] != 0 && is_valid_destination(dest_triangles[0]))
                    get_triangle_script("Triangle" + (dest_triangles[0])).change_sprite_stat();
                if (curr_dice[1] != 0 && is_valid_destination(dest_triangles[1]))
                    get_triangle_script("Triangle" + (dest_triangles[1])).change_sprite_stat();
            }
            if (turn && board.flags["Oendgame"] == 1)
            {
                if (board.flags["double"] != 1)
                {
                    if (dest_triangles[0] > LAST_TRIANGLE)
                        handle_press_as_new_location("Triangle" + dest_triangles[0],is_from_other_player);
                    if (dest_triangles[1] > LAST_TRIANGLE)
                        handle_press_as_new_location("Triangle" + dest_triangles[1],is_from_other_player);
                }
                else
                    handle_press_as_new_location("Triangle" + dest_triangles[0], is_from_other_player);
            }
            else if (!turn && board.flags["Gendgame"] == 1)
            {
                if (board.flags["double"] != 1)
                {
                    if (dest_triangles[0] < FIRST_TRIANGLE)
                        handle_press_as_new_location("Triangle" + dest_triangles[0], is_from_other_player);
                    if (dest_triangles[1] < FIRST_TRIANGLE)
                        handle_press_as_new_location("Triangle" + dest_triangles[1], is_from_other_player);
                }
                else
                    handle_press_as_new_location("Triangle" + dest_triangles[0], is_from_other_player);
            }
        }

    }

    private void handle_press_as_new_location(string name,bool is_from_other_player=false)
    {
        Debug.Log("handle_press_as_new_location " + name + "is other player= " + (is_from_other_player));
        int dest_triangle = get_triangle_number(name);
        if (dest_triangle >= FIRST_TRIANGLE && dest_triangle <= LAST_TRIANGLE) //triangle number is in game scope
        {
            SC_Triangle sc_triangle = get_triangle_script(name);
            get_triangle_script("Triangle" + source_triangle).pop_piece();
            if (sc_triangle.is_vunarable(turn))
                captured(name);
            push_piece(name);
        }
        else //dest_triangle is outside scope- endgame
        {
            if (turn && board.flags["Oendgame"] == 1)//orange valdiated as being at endgame
            {
                if (dest_triangle == LAST_TRIANGLE + 1)//taking out from the exact triangle
                    get_triangle_script("Triangle" + source_triangle).pop_piece();
                else if (dest_triangle > LAST_TRIANGLE+1)
                {//takes out smaller triangle than cube. check if there are greater triangles
                    for (int i = FIRST_O_TRIANGLE; i < source_triangle; i++)
                    {
                        SC_Triangle curr = get_triangle_script("Triangle" + i);
                        if (!curr.is_stack_empty() && curr.get_stack_color()=='O')
                        { 
                            //stacks from larger triangles arnt empty with own color. illeagel
                            end_move(-1);
                            return;
                        }
                    }
                    get_triangle_script("Triangle" + source_triangle).pop_piece();
                }
            }
            else if (!turn && board.flags["Gendgame"] == 1)
            {
                if (dest_triangle == FIRST_TRIANGLE - 1)
                    get_triangle_script("Triangle" + source_triangle).pop_piece();
                else if (dest_triangle < FIRST_G_TRIANGLE-1)
                {//takes out smaller triangle than cube. check if there are greater triangles
                    for (int i = FIRST_G_TRIANGLE; i > source_triangle; i--)
                    {
                        SC_Triangle curr = get_triangle_script("Triangle" + i);
                        if (!curr.is_stack_empty() && curr.get_stack_color()=='G')
                        {
                            //stacks from larger triangles arnt empty with own color. illeagel
                            end_move(-1);
                            return;
                        }
                    }
                    get_triangle_script("Triangle" + source_triangle).pop_piece();
                }
            }
        }
        if (board.flags["double"] != 1)
        {
            if (turn)
                update_dice(dest_triangle - source_triangle);
            else if (!turn)
                update_dice(source_triangle - dest_triangle);
        }
        if (board.multiplayer && board.is_my_turn())
        {
            Debug.Log("added source= " + source_triangle + " dest= " + dest_triangle);
            add_moves_to_dict(source_triangle, dest_triangle);
        }
        turn_off_dest_triangles();
        turn_moves++;
        end_move(dest_triangle);
        if(!is_from_other_player)
            StartCoroutine(CR_check_available_moves());

    }
    #endregion

    #region Support functions

    private void add_moves_to_dict(int source_triangle, int dest_triangle)
    {
        for(int i = 0; i < 4; i++)//4 is maximum moves
        {
            if (key_source[i]==-2 && val_dest[i]==-2)//-2 represent that theres no value there
            {
                key_source[i] = source_triangle;
                val_dest[i] = dest_triangle;
                break;
            }
        }
    }

    public void play_singleplayer()
    {
        Debug.Log("play_singleplayer");
        int moves = 0;
        if (board.flags["double"] == 0)
            moves = 2;
        else if (board.flags["double"] == 1)
            moves = 4;

        for (int i = 0; i < moves; i++)
        {
            if (board.flags["Gendgame"] == 1)
                Debug.Log("implement decrement any piece");
            int[] green_move = get_green_move();
            if (green_move[0] == -1)
                no_available_moves();
            else
            {
                pressed_triangle("Triangle" + green_move[0]);
                pressed_triangle("Triangle" + green_move[1]);
            }

        }
    }


    private bool is_valid_press(string name)
    {
        Debug.Log("is_valid_press " + name);
        SC_Triangle Tcurr = get_triangle_script(name);
        // check if the triangle was pressed to move a piece matches the turn (if orange pieces triangle when turn=True)
        // also check if current player has captured pieces. if so, he has to press on his captured stack
        char pressed_pieces_color = Tcurr.get_stack_color();
        if (((pressed_pieces_color == 'O') && turn && (board.flags["Ocaptures"] != 1 || name == "Triangle-1")
            || (pressed_pieces_color == 'G') && !turn && (board.flags["Gcaptures"] != 1 || name == "Triangle24")))
            return true;
        else
        {
            Debug.Log(name + " Not valid press!");
            return false;
        }
    }

    private bool is_valid_destination(int dest)
    {
        if (dest > LAST_TRIANGLE || dest < FIRST_TRIANGLE)
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
                GameObject curr = GameObject.Find(currname);
                Triangles.Add(currname, GameObject.Find(currname));
                get_triangle_script(currname).change_sprite_stat();
            }
    }

    SC_Triangle get_triangle_script(string name)
    {
        return Triangles[name].GetComponent<SC_Triangle>();
    }

    int get_triangle_number(string name)
    {
        string number = name.Substring("Triangle".Length);
        int num = int.Parse(number);
        //if (num >= -1 && num <= 24)
        return num;
        //return -2;
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
        StartCoroutine(CR_wait_frame());
        Debug.Log("end_move " + triangle_number);
        if ((turn_moves >= 4 && board.flags["double"] == 1)
            || (turn_moves == 2 && board.flags["double"] == 0))
        {
            if (board.is_my_turn() && board.multiplayer)
            {
                string Skey_source = string.Join(",", key_source);
                string Sval_dest = string.Join(",", val_dest);
                moves_to_send["moves"].Add(Skey_source, Sval_dest);
                board.send_data(moves_to_send);
            }
            init_vars();
            is_endgame();
            is_finish();
            finish_turn();
            return;
        }
        // if turn is not done:
        if (turn_moves > 0)//zero capture flag if captured_flag is empty
        {
            if (turn && get_triangle_script("Triangle" + ORANGE_CAPTURED_STACK).is_stack_empty())
            {

                board.flags["Ocaptures"] = 0;
                Debug.Log("<color=purple>Ocaptures=0</color>");
            }
            else if (!turn && get_triangle_script("Triangle" + GREEN_CAPTURED_STACK).is_stack_empty())
            {
                board.flags["Gcaptures"] = 0;
                Debug.Log("<color=purple>Gcaptures=0</color>");
            }

        }
        board.flags["turn_stage"] = 1;
        if (board.flags["double"] == 1)
            dest_triangles[0] = dest_triangles[1] = -2;
        else if (dest_triangles[0] == triangle_number)
            triangle_number = dest_triangles[1];
        else if (dest_triangles[1] == triangle_number)
            triangle_number = dest_triangles[0];
        dest_triangles[1] = -2;
        dest_triangles[0] = -2;
        is_endgame();
        is_finish();
    }

    private void is_endgame()
    {
        Debug.Log("is_endgame");
        if (turn)
        {
            int[] Ostacks = new int[24];
            Ostacks = get_stacks('O');
            if (board.flags["Ocaptures"] == 1)
            {
                board.flags["Oendgame"] = 0;
                return;
            }
            for (int i = FIRST_TRIANGLE; i < FIRST_O_TRIANGLE; i++)//check all triangles exept orange home triangles
            {
                if (Ostacks[i] != -2)
                    return;
            }
            board.flags["Oendgame"] = 1;
        }

        else if (!turn)
        {
            int[] Gstacks = new int[24];
            Gstacks = get_stacks('G');
            if (board.flags["Gcaptures"] == 1)
            {
                board.flags["Gendgame"] = 0;
                return;
            }
            for (int i = LAST_TRIANGLE; i > FIRST_G_TRIANGLE; i--)//check all triangles exept orange home triangles
            {
                if (Gstacks[i] != -2)
                    return;
            }
            board.flags["Gendgame"] = 1;
        }
    }

    void turn_off_dest_triangles()
    {
        Debug.Log("turn_off_dest_triangles");
        for (int i = 0; i < 2; i++)
        {
            if (dest_triangles[i] != -2 && is_valid_destination(dest_triangles[i]))
            {
                SC_Triangle curr = get_triangle_script("Triangle" + dest_triangles[i]);
                if (curr.is_sprite_active())
                    curr.change_sprite_stat();
            }
        }
    }

    private void is_finish()
    {
        Debug.Log("is_finish");
        if (turn)
        {
            int[] Ostacks = new int[24];
            Ostacks = get_stacks('O');
            if (board.flags["Ocaptures"] == 1)
                return;
            for (int i = FIRST_TRIANGLE; i <= LAST_TRIANGLE; i++)
            {
                if (Ostacks[i] != -2)
                    return;
            }
            game_finished('O');
        }

        else if (!turn)
        {
            int[] Gstacks = new int[24];
            Gstacks = get_stacks('G');
            if (board.flags["Gcaptures"] == 1)
                return;
            for (int i = FIRST_TRIANGLE; i <= LAST_TRIANGLE; i++)
            {
                if (Gstacks[i] != -2)
                    return;
            }
            game_finished('G');
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
        init_moves_dict();
    }

    private void init_moves_dict()
    {
        moves_to_send = new Dictionary<string, Dictionary<string, string>>();
        moves_to_send.Add("moves", new Dictionary<string, string>());
        key_source = new List<int> { -2, -2, -2, -2 };
        val_dest = new List<int> { -2, -2, -2, -2 };

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
            board.flags["Ocaptures"] = 1;
            board.flags["Oendgame"] = 0;
            Debug.Log("<color=purple>captures=1</color>");
        }
        else if (!turn)
        {
            push_piece("Triangle24");
            board.flags["Gcaptures"] = 1;
            board.flags["Gendgame"] = 0;
            Debug.Log("<color=purple>captures=2</color>");

        }
        turn = !turn;
    }

    private int[] get_green_move()
    {
        //int[0]= source, int[1]= dest
        int[] no_moves = new int[2] { -1, -1 };
        int[] green_stacks = new int[24];
        green_stacks = get_stacks('G');
        if (board.flags["Gendgame"] == 1)
        {
            for (int i = FIRST_G_TRIANGLE; i >= 0; i--)
            {
                if (green_stacks[i] != -2)
                    return new int[] { i, -2 };
            }
        }

        if (board.flags["Gcaptures"] == 1)//check available moves for green captured stack
        {
            source_triangle = GREEN_CAPTURED_STACK;
            if (curr_dice[0] != 0 && is_valid_destination(LAST_TRIANGLE - (curr_dice[0] - 1)))
                return new int[] { source_triangle, LAST_TRIANGLE - (curr_dice[0] - 1) };

            else if (curr_dice[1] != 0 && is_valid_destination(LAST_TRIANGLE - (curr_dice[1] - 1)))
                return new int[] { source_triangle, LAST_TRIANGLE - (curr_dice[1] - 1) };
            else
                return no_moves;
        }
        for (int i = 0; i < 24; i++)
        {
            if (green_stacks[i] != -2)
            {
                source_triangle = green_stacks[i];
                if (curr_dice[0] != 0 && is_valid_destination(i - curr_dice[0]))
                    return new int[] { source_triangle, i - curr_dice[0] };
                else if (curr_dice[1] != 0 && is_valid_destination(i - curr_dice[1]))
                    return new int[] { source_triangle, i - curr_dice[1] };
            }
        }
        return no_moves;

    }

    private bool check_available_moves()
    {
        Debug.Log("<color=purple>check_available_moves()</color>");
        if (turn)//check available moves for orange
        {
            if (board.flags["Oendgame"] == 1)
                return true;
            int[] orange_stacks = new int[24];
            orange_stacks = get_stacks('O');
            if (board.flags["Ocaptures"] == 1)//check available moves for orange captured stack
            {
                source_triangle = ORANGE_CAPTURED_STACK;
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
            if (board.flags["Gendgame"] == 1)
                return true;
            int[] green_stacks = new int[24];
            green_stacks = get_stacks('G');
            if (board.flags["Gcaptures"] == 1)//check available moves for green captured stack
            {
                source_triangle = GREEN_CAPTURED_STACK;
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

    private IEnumerator CR_check_available_moves()
    {
        yield return null; // wait until the next frame
        if (turn_moves >= 1 && !check_available_moves())
        {
            no_available_moves();
            string Skey_source = string.Join(",", key_source);
            string Sval_dest = string.Join(",", val_dest);
            moves_to_send["moves"].Add(Skey_source, Sval_dest);
            board.send_data(moves_to_send);
        }
    }

    public IEnumerator CR_wait_frame(int frames=1)
    {
            yield return new WaitForEndOfFrame();
            Debug.Log("waiting frame " + frames);
    }

    private void endgame_press(string name)
    {
        if (turn)
        {

        }

        else if (!turn)
        {

        }
    }

    public void set_curr_dice(int left,int right)
    {
        curr_dice[0] = left;
        curr_dice[1]=right;
    }
    #endregion

}
