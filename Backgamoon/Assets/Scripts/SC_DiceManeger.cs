using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_DiceManeger : MonoBehaviour
{
    public delegate void Roll_Dice_Handler(int left,int right=0);
    public static Roll_Dice_Handler Roll_Dice;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Awake()
    {
        Debug.Log(name);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnMouseDown()
    {
        Debug.Log("calling with :");
        Roll_Dice(Random.Range(1,6),Random.Range(1,6));
    }
}
