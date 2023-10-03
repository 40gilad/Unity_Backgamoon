using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
public class SC_Board : MonoBehaviour
{
    public delegate void Turn_Handler(bool t);
    public static Turn_Handler Turn;
    public delegate void SinglePlayer_Handler();
    public static SinglePlayer_Handler play_singleplayer;
    public Dictionary<string, int> flags;

    GameObject Sprite_x;
    GameObject camera;
    GameObject[] DiceRoller=null;
    int[] curr_dice;
    bool turn; // true= orange turn false= green turn
    public bool multiplayer = false;

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
        DiceRoller = new GameObject[2];
        DiceRoller[0] = GameObject.Find("Sprite_LeftRollDice");
        DiceRoller[1] = GameObject.Find("Sprite_RightRollDice");
        if(multiplayer)
            camera = GameObject.Find("Main Camera");
        flags= new Dictionary<string, int>();
        curr_dice = new int[2];
        Sprite_x = GameObject.Find("Sprite_X");
    }

    void Start()
    {
        /* for orange to start: turn= false */
        turn = false;
        init_flags();
        ChangeTurn();
    }

    void OnEnable()
    {
        SC_DiceManeger.Roll_Dice += Roll_Dice;
        SC_Triangle_Maneger.finish_turn += Finish_Turn;
        SC_Triangle_Maneger.no_available_moves += No_Moves;
        SC_Triangle_Maneger.game_finished += finish_game;
    }

    private void OnDisable()
    {
        SC_DiceManeger.Roll_Dice -= Roll_Dice;
        SC_Triangle_Maneger.finish_turn -= Finish_Turn;
        SC_Triangle_Maneger.no_available_moves -= No_Moves;
        SC_Triangle_Maneger.game_finished -= finish_game;


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
        //add double celebration?
    }

    private void Finish_Turn()
    {
        is_game_finish();
        ChangeTurn();
    }

    private void finish_game(char color)
    {
        if (color == 'O')
            Debug.Log("<color=orange>ORANGE WON!!</color>");
        if(color=='G')
            Debug.Log("<color=green>GREEN WON!!</color>");
    }
    #endregion

    #region Support Functions
    private void init_flags()
    {
        flags.Add("turn_stage", 0);
        flags.Add("double", 0);
        flags.Add("Gcaptures", 0);
        flags.Add("Ocaptures", 0);
        flags.Add("Gendgame", 0);
        flags.Add("Oendgame", 0);
    }

    private void zero_flags()
    {
        flags["double"] = 0;
        flags["turn_stage"] = 0;
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
        rotate_camera();
        zero_flags();
        init_dice();

        DiceRoller[0].SetActive(!turn);
        DiceRoller[1].SetActive(turn);

        if(Sprite_x.activeSelf)
            Sprite_x.SetActive(false);
        Turn(turn);
        if (!turn && !multiplayer)
        {
            DiceRoller[0].GetComponent<SC_DiceManeger>().Roll();
            play_singleplayer();
        }
    }


    private void rotate_camera()
    {
        if (multiplayer)
            StartCoroutine(CR_rotate_camera());
    }

    IEnumerator CR_rotate_camera()
    {
        yield return new WaitForSeconds(1);
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
    #endregion

}
