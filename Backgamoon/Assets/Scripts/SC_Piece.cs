using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_Piece : MonoBehaviour
{
    public delegate void Piece_Press_Handler(int n);
    public static Piece_Press_Handler Piece_Press;

    void Start()
    {
        //Debug.Log(name +" : " + transform.parent.transform.parent.name);
    }

    // Update is called once per frame

    private void OnMouseDown()
    {
        string triangle_name= transform.parent.transform.parent.name;
        Debug.Log("calling with : "+ triangle_name);
        Piece_Press(int.Parse(triangle_name.Substring(8)));
    }
}
