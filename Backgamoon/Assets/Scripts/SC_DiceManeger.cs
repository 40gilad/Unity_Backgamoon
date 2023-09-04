using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_DiceManeger : MonoBehaviour
{
    public delegate void Roll_Dice_Handler(int left,int right=0);
    public static Roll_Dice_Handler Roll_Dice;
    GameObject[] DicePairs;

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
        
    }

    void Awake()
    {
        Debug.Log(name);
        DicePairs=new GameObject[2];
        DicePairs[0] = GameObject.Find("Sprite_LeftDicePair");
        DicePairs[1] = GameObject.Find("Sprite_RightDicePair");

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnMouseDown()
    {
        Debug.Log("SC_DiceManeger mouse with :"+name);
        Roll_Dice(Random.Range(1,6),Random.Range(1,6));
    }

    void Turn(int t)
    {
        Debug.Log("SC_DiceManeger Turn(" + t + "):");
        DicePairs[t].SetActive(true);
        DicePairs[(t+1)%2].SetActive(false);
    }
}
