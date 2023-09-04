using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_SingleDice : MonoBehaviour
{
    GameObject[] DiceNumbers = null;
    /*
    void OnEnable()
    {
        SC_DicePair.Set_Dice_Number += Set_Dice_Number;
    }


    private void OnDisable()
    {
        SC_DicePair.Set_Dice_Number -= Set_Dice_Number;

    }
    */
    void Awake()
    {
        Debug.Log("Awake "+name);
        DiceNumbers=new GameObject[6];
        for (int i=0; i < 6; i++)
            DiceNumbers[i] = transform.Find("Dice" + (i + 1)).gameObject;
    }
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start" + name);
        TurnOffAllNumbers();
        TurnOnNumber(1);
    }

    // Update is called once per frame
    void Update()
    {
        
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
