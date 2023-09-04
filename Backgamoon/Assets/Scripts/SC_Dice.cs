using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_Dice : MonoBehaviour
{
    int[] DiceRolled;
    void OnEnable()
    {
        SC_DiceManeger.Roll_Dice += Roll_Dice;
    }

    void Awake()
    {
        DiceRolled=new int[2];
    }
    private void OnDisable()
    {
        SC_DiceManeger.Roll_Dice -= Roll_Dice;

    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Roll_Dice(int left, int right=0)
    {
        Debug.Log("DiceRolled: "+left +" , "+ right);
        DiceRolled[0]=left;
        DiceRolled[1]=right;
    }
}
