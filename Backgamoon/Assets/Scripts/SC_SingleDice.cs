using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_SingleDice : MonoBehaviour
{
    GameObject[] DiceNumbers = null;

    void Awake()
    {
        Debug.Log("Awake "+name);
        DiceNumbers=new GameObject[6];
        for (int i=0; i < 6; i++)
            DiceNumbers[i] = transform.Find("Dice" + (i + 1)).gameObject;
    }

    void Start()
    {
        //Debug.Log("Start" + name);
        TurnOffAllNumbers();
        TurnOnNumber(1);
    }

    void TurnOffAllNumbers()
    {
        for(int i = 0; i < 6; i++)
           DiceNumbers[i].SetActive(false);
    }

    void TurnOnNumber(int n)
    {
        DiceNumbers[n-1].SetActive(true);
    }
    public void Set_Dice_Number(int num)
    {
            TurnOffAllNumbers();
            TurnOnNumber(num);
    }
}
