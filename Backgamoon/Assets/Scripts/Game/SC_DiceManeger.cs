using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_DiceManeger : MonoBehaviour
{
    public delegate void Roll_Dice_Handler(int left, int right = 0);
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

    void Start()
    {
        times_pressed = 0;
    }

    private void OnMouseDown()
    {
        Roll();
    }

    public void Roll()
    {
        Roll_Dice(Random.Range(1, 7), Random.Range(1, 7));
        gameObject.SetActive(false);
    }


}
