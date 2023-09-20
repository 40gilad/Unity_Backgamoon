using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
public class SC_Board : MonoBehaviour
{
    public delegate void Turn_Handler(bool t);
    public static Turn_Handler Turn;
    public Dictionary<string, int> flags;

    /************************************************************************************************/
    /*
     * flags["turn_stage"]: 
     * 0- waiting for throw
     * 1- threw
     * 2- possible moves shown on board
     */

    GameObject camera;
    GameObject[] DiceRoller=null;
    int[] curr_dice;
    bool turn; // true= orange turn false= green turn
    public bool multiplayer = true;

    #region MonoBehaviour
    void Awake()
    {
        Debug.Log("Awake " + name);
        DiceRoller = new GameObject[2];
        DiceRoller[0] = GameObject.Find("Sprite_LeftRollDice");
        DiceRoller[1] = GameObject.Find("Sprite_RightRollDice");
        if(multiplayer)
            camera = GameObject.Find("Main Camera");
        flags= new Dictionary<string, int>();
        curr_dice = new int[2];
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

    }

    private void OnDisable()
    {
        SC_DiceManeger.Roll_Dice -= Roll_Dice;
        SC_Triangle_Maneger.finish_turn -= Finish_Turn;
    }

    #endregion

    #region Delegates
    private void Roll_Dice(int left, int right = 0)
    {
        flags["turn_stage"] = 1;
        /*
        curr_dice[0] = left;
        curr_dice[1] = right;
        */
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

    #endregion

    #region Support Functions
    private void init_flags()
    {
        flags.Add("turn_stage", 0);
        flags.Add("double", 0);
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

    public void ChangeTurn()
    {
        turn = !turn;

        rotate_camera();
        zero_flags();
        init_dice();

        DiceRoller[0].SetActive(!turn);
        DiceRoller[1].SetActive(turn);
        Turn(turn);
    }

    private void rotate_camera()
    {
        if (multiplayer)
        {
            Vector3 rotation;
            if (turn)
                rotation = new Vector3(0, 0, 0);
            else
                rotation = new Vector3(0, 0, 180);
            camera.GetComponent<Transform>().localRotation = Quaternion.Euler(rotation);

            //Change dice position()
        }

    }

    private void is_game_finish()
    {
        Debug.Log("<color=red>Write is game finished logic</color>");
    }

    #endregion

}
