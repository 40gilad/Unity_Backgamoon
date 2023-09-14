using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_DiceManeger : MonoBehaviour
{
    public delegate void Roll_Dice_Handler(int left,int right=0);
    public static Roll_Dice_Handler Roll_Dice;
    GameObject[] DicePairs;
    SC_Board board;
    int times_pressed;
    void Awake()
    {
        Debug.Log("Awake " + name);
        board = GameObject.Find("Board").GetComponent<SC_Board>();
        DicePairs = new GameObject[2];
        DicePairs[0] = GameObject.Find("Sprite_LeftDicePair");
        DicePairs[1] = GameObject.Find("Sprite_RightDicePair");

    }

    void OnEnable()
    {
        SC_Board.Turn += Turn;
    }

    void OnDisable()
    {
        SC_Board.Turn -= Turn;
    }

    void Start()
    {
        times_pressed = 0; 
    }

    private void OnMouseDown()
    {
        times_pressed++;
        if (times_pressed == 1)
        {
            //Debug.Log("SC_DiceManeger mouse with :" + name);
            Roll_Dice(Random.Range(1, 6), Random.Range(1, 6));
        }
        else if(times_pressed==2) {
            board.ChangeTurn();
            times_pressed= 0;
        }
    }

    void Turn(int t)
    {
        Debug.Log("<color=green> SC_DiceManeger Turn(" + t + "):</color>");
        DicePairs[t].SetActive(true);
        DicePairs[(t+1)%2].SetActive(false);
    }
}
