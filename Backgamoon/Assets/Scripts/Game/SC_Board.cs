using AssemblyCSharp;
using System;
using com.shephertz.app42.gaming.multiplayer.client;
using com.shephertz.app42.gaming.multiplayer.client.events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using com.shephertz.app42.gaming.multiplayer.client.SimpleJSON;
using Unity.VisualScripting;


public class SC_Board : MonoBehaviour
{
    public delegate void Turn_Handler(bool t);
    public static Turn_Handler Turn;
    public delegate void SinglePlayer_Handler();
    public static SinglePlayer_Handler play_singleplayer;
    public delegate void Play_other_player_hanler (int[] source, int[] dest);
    public static Play_other_player_hanler play_other_player;

    //public SC_BackgamoonConnect backgamoon_connect;
    GameObject Sprite_x;
    GameObject camera;
    GameObject[] DiceRoller = new GameObject[2];
    public Dictionary<string, int> flags;
    int[] curr_dice;
    int[] dice_to_send;
    bool turn;//true= orange turn
    private string nextTurn;
    private float startTime;
    public bool multiplayer;
    public bool psudo_multiplayer;
    private SC_DicePair dice_maneger = null;
    public SC_Triangle_Maneger Triangle_Maneger;
    bool is_game_init;

    #region README
    /************************************************************************************************/
    /*
     * flags["turn_stage"]: 
     *  0- waiting for throw
     *  1- threw
     *  2- possible moves shown on board
     * 
     * flags["double"]:
     *  0- no double
     *  1- double thrwoed
     *  
     * flags[color+ "captures"]:
     *  0- no captures
     *  1- captures
     *  
     *  flags[color+"endgame"]
     *  0- no endgame
     *  1- endgame
     */

    #endregion

    #region MonoBehaviour
    void Awake()
    {
       DiceRoller[0] = GameObject.Find("Sprite_LeftRollDice");
       DiceRoller[1] = GameObject.Find("Sprite_RightRollDice");
       if(multiplayer || psudo_multiplayer)
            camera = GameObject.Find("Main Camera");
       if(multiplayer)
            dice_to_send= new int[2];
       dice_maneger=GameObject.Find("Sprite_RightDicePair").GetComponent<SC_DicePair>();
       flags= new Dictionary<string, int>();
       curr_dice = new int[2];
       Sprite_x = GameObject.Find("Sprite_X");
    }

    void Start()
    {
        is_game_init = false;
        turn = false;
        init_flags();
        ChangeTurn();
        is_game_init = true;

    }

    void OnEnable()
    {
        SC_DiceManeger.Roll_Dice += Roll_Dice;
        SC_Triangle_Maneger.finish_turn += Finish_Turn;
        SC_Triangle_Maneger.no_available_moves += No_Moves;
        SC_Triangle_Maneger.game_finished += finish_game;
        Listener.OnMoveCompleted += OnMoveCompleted;
    }

    private void OnDisable()
    {
        SC_DiceManeger.Roll_Dice -= Roll_Dice;
        SC_Triangle_Maneger.finish_turn -= Finish_Turn;
        SC_Triangle_Maneger.no_available_moves -= No_Moves;
        SC_Triangle_Maneger.game_finished -= finish_game;
        Listener.OnMoveCompleted += OnMoveCompleted;

    }



    #endregion

    #region Delegates
    private void Roll_Dice(int left, int right = 0)
    {
        flags["turn_stage"] = 1;
        curr_dice[0] = left;
        curr_dice[1] = right;
        if (curr_dice[0] == curr_dice[1])
            flags["double"] = 1;
        if (multiplayer)
        {
            dice_to_send[0] = curr_dice[0];
            dice_to_send[1] = curr_dice[1];
        }
    }


    private void Finish_Turn()
    {
        is_game_finish();
        ChangeTurn();
    }

    private void finish_game(char color)
    {
        if (color == 'O')
            Debug.Log("<color=orange>display: ORANGE WON!!</color>");
        if(color=='G')
            Debug.Log("<color=green>display: GREEN WON!!</color>");
    }

    public void StartGame(string _NextTurn)
    {
        Debug.Log("Board StartGame Room owner= " + GlobalVars.orange + "myId= "+GlobalVars.userId);
        nextTurn = _NextTurn;
        startTime = Time.time;

        if (GlobalVars.orange != GlobalVars.userId && multiplayer)
        {
            DiceRoller[1].SetActive(false);
            rotate_camera();
        }
        manage_dice_rollers();
    }
    #endregion

    #region Support Functions

    private void OnMoveCompleted(MoveEvent _Move)
    {
        Debug.Log("Got data from other player: " + _Move.getMoveData());

        if (_Move.getSender() != GlobalVars.userId && _Move.getMoveData() != null) // other player sent and it's not null
        {
            Dictionary<string, object> data = (Dictionary<string, object>)MiniJSON.Json.Deserialize(_Move.getMoveData());
            if (data != null)
            {
                if (data.ContainsKey("dice"))
                {
                    // Extract the "dice" dictionary from the JSON data
                    string diceData = MiniJSON.Json.Serialize(data["dice"]);
                    diceData = diceData.Replace("{", "").Replace("}", "").Replace("\"", "");
                    string[] parts = diceData.Split(':');
                    int leftDiceValue = int.Parse(parts[0]);
                    int rightDiceValue = int.Parse(parts[1]);
                    dice_maneger.Roll_Dice(leftDiceValue, rightDiceValue);
                    Triangle_Maneger.curr_dice[0] = leftDiceValue;
                    Triangle_Maneger.curr_dice[1] = rightDiceValue;



                }
                if (data.ContainsKey("moves"))
                {
                    if (flags["did_play_other"] == 0)
                    {

                        string moves_string = MiniJSON.Json.Serialize(data["moves"]);

                        /******************** convert string to int[] ****************************/
                        moves_string = moves_string.Replace("{", "").Replace("}", "").Replace("\"", "");
                        string[] parts = moves_string.Split(':');
                        int[] source_data = ExtractIntArray(parts[0]);
                        int[] dest_data = ExtractIntArray(parts[1]);
                        /************************************************************************/
                        Debug.Log("First Array: " + string.Join(",", source_data));
                        Debug.Log("Second Array: " + string.Join(",", dest_data));

                        play_other_player(source_data, dest_data);
                        flags["did_play_other"] = 1;
                    }

                }
            }
        }
    }


    /***************************************** send_data overloads *****************************************/

    public void send_data(Dictionary<string, Dictionary<string, string>> data)
    {
        data.Add("dice", new Dictionary<string, string>());
        data["dice"].Add(dice_to_send[0].ToString(), dice_to_send[1].ToString());
        if (is_my_turn())
        {
            Debug.Log("changing move");
            string jsonData = MiniJSON.Json.Serialize(data);
            Debug.Log(jsonData);
            WarpClient.GetInstance().sendMove(jsonData);
        }
    }

    /*******************************************************************************************************/

    private void init_flags()
    {
        flags.Add("turn_stage", 0);
        flags.Add("double", 0);
        flags.Add("Gcaptures", 0);
        flags.Add("Ocaptures", 0);
        flags.Add("Gendgame", 0);
        flags.Add("Oendgame", 0);
        flags.Add("did_play_other", 0);

    }

    private void zero_flags()
    {
        Debug.Log("zero flags");
        flags["double"] = 0;
        flags["turn_stage"] = 0;
        flags["did_play_other"] = 0;
    }

    private void init_dice()
    {
        curr_dice[0] = 0;
        curr_dice[1] = 0;
    }

    private void No_Moves()
    {
        StartCoroutine(CR_No_Moves());
    }

    private IEnumerator CR_No_Moves()
    {
        yield return new WaitForSeconds(1);
        Sprite_x.SetActive(true);
        yield return new WaitForSeconds(2);
        Sprite_x.SetActive(false);
        ChangeTurn();
    }
    public void ChangeTurn()
    {
        turn = !turn;
        if (turn)
            Debug.Log("<color=orange>ORANGE TURN</color>");
        else if (!turn)
            Debug.Log("<color=green>GREEN TURN</color>");
        if(psudo_multiplayer && !is_game_init)
            rotate_camera();
        zero_flags();
        init_dice();

        manage_dice_rollers();

        if(Sprite_x.activeSelf)
            Sprite_x.SetActive(false);
        Turn(turn);
        if (!turn && !multiplayer && !psudo_multiplayer)
        {
            DiceRoller[0].GetComponent<SC_DiceManeger>().Roll();
            play_singleplayer();
        }
    }

    public void rotate_camera()
    {
        Vector3 rotation;
        if (turn)
            rotation = new Vector3(0, 0, 0);
        else
            rotation = new Vector3(0, 0, 180);
        camera.GetComponent<Transform>().localRotation = Quaternion.Euler(rotation);
    }

    private void is_game_finish()
    {
        Debug.Log("<color=red>Write is game finished logic</color>");
    }

    public void exit_game()
    {
        SceneManager.LoadScene("SampleScene");
    }

    int[] ExtractIntArray(string input)
    {
        // Split the string into individual values
        string[] values = input.Split(',');

        // Convert values to integers
        int[] result = new int[values.Length];
        for (int i = 0; i < values.Length; i++)
        {
            result[i] = int.Parse(values[i]);
        }

        return result;
    }
    #endregion

    private void manage_dice_rollers()
    {
        if (multiplayer && !is_my_turn() && is_game_init)
        {
            DiceRoller[0].SetActive(false);
            DiceRoller[1].SetActive(false);
        }
        else if (is_game_init)
        {
            DiceRoller[0].SetActive(!turn);
            DiceRoller[1].SetActive(turn);
        }

    }
    public bool is_my_turn()
    {
        return (GlobalVars.orange == GlobalVars.userId && turn) || 
            (GlobalVars.orange != GlobalVars.userId && !turn);
    }
}
