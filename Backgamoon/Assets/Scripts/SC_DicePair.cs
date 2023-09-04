using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_DicePair : MonoBehaviour
{
    public delegate void Set_Dice_Number_Handler(int num,string DiceName);
    public static Set_Dice_Number_Handler Set_Dice_Number;
    GameObject[] MyDice;
    int[] DiceRolled;

    void OnEnable()
    {
        SC_DiceManeger.Roll_Dice += Roll_Dice;
    }
    private void OnDisable()
    {
        SC_DiceManeger.Roll_Dice -= Roll_Dice;

    }

    void Awake()
    {
        Debug.Log(name);
        DiceRolled = new int[2];
        MyDice = new GameObject[2];
        MyDice[0] = GameObject.Find("LeftDice");
        MyDice[1] = GameObject.Find("RightDice");

    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Roll_Dice(int left, int right=0)
    {
        Debug.Log(name+ " Dice Rolled: " + left +" , "+ right);
        Set_Dice_Number(left, MyDice[0].name);
        Set_Dice_Number(right, MyDice[1].name);

    }
}
