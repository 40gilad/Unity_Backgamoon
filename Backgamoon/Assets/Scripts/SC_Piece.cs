using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;

public class SC_Piece : MonoBehaviour
{
    public delegate void Piece_Press_Handler(int n);
    public static Piece_Press_Handler Piece_Press;
    bool turn;

    #region MonoBehaviour functions and overload
    void Start()
    {
        turn = false;
    }

    void OnEnable()
    {
        SC_Board.Turn += Turn;
    }

    void OnDisable()
    {
        SC_Board.Turn -= Turn;
    }

    private void OnMouseDown()
    {
        Debug.Log("<color=purple>SC_PIECE " + name+"</Color>");
        string piece_color = name.Substring(0, 5);
            handle_after_rolling(piece_color);
    }
    #endregion

    #region My functions
    private void handle_after_rolling(string piece_color)
    {
        //turning on the relevant triangles
        if (turn && piece_color == "Orang")
            handle_piece_press_after_rolling();

        else if (!turn && piece_color == "Green")
            handle_piece_press_after_rolling();
    }
    private void handle_piece_press_after_rolling()
    {
        string triangle_name = transform.parent.transform.parent.name;
        Piece_Press(int.Parse(triangle_name.Substring(8)));
    }

    private void Turn(int t)
    {
        turn = (t == 1);
    }

    #endregion
}
