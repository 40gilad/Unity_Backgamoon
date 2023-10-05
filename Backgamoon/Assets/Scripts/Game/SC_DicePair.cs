using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_DicePair : MonoBehaviour
{
    public delegate void Set_Dice_Number_Handler(int num,string DiceName);
    public static Set_Dice_Number_Handler Set_Dice_Number;
    public SC_SingleDice[] MyDice;
    int[] DiceRolled;


    void Awake()
    {
        Debug.Log("Awake " + name);
        DiceRolled = new int[2];
        MyDice = new SC_SingleDice[2];
        MyDice[0] = transform.Find("LeftDice").GetComponent<SC_SingleDice>();
        MyDice[1] = transform.Find("RightDice").GetComponent<SC_SingleDice>();

    }
        
    void OnEnable()
    {
        SC_DiceManeger.Roll_Dice += Roll_Dice;
    }
    private void OnDisable()
    {
        SC_DiceManeger.Roll_Dice -= Roll_Dice;

    }

    private void Roll_Dice(int left, int right=0)
    {
        MyDice[0].Set_Dice_Number(left);
        MyDice[1].Set_Dice_Number(right);

    }
}
