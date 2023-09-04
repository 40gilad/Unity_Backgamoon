using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_SingleDice : MonoBehaviour
{
    GameObject[] DiceNumbers = null;
    void OnEnable()
    {
        SC_DicePair.Set_Dice_Number += Set_Dice_Number;
    }


    private void OnDisable()
    {
        SC_DicePair.Set_Dice_Number -= Set_Dice_Number;

    }
    void Awake()
    {
        Debug.Log(name);
        DiceNumbers=new GameObject[6];
        for (int i=0; i < 6; i++){
            DiceNumbers[i] = GameObject.Find("Dice" + (i + 1));
        }
        TurnOffAllNumbers();
        TurnOnNumber(6);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void TurnOffAllNumbers()
    {
        for(int i = 0; i < 6; i++)
        {
            DiceNumbers[i].SetActive(false);
        }
    }

    void TurnOnNumber(int n)
    {
        Debug.Log(name +" TurnOnNumber(" + n + ")");
        DiceNumbers[n-1].SetActive(true);
    }
    private void Set_Dice_Number(int num, string DiceName)
    {
        Debug.Log("Set_Dice_Number( " + num + ", " + DiceName+". My name: "+ name);
        if (DiceName == name)
        {
            TurnOffAllNumbers();
            TurnOnNumber(num);
        }
    }
}
