using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_TrianglePiecesStack : MonoBehaviour
{
    public char stack_color;
    public int top;
    private readonly float pieces_distance = -0.7f;
    // Start is called before the first frame update

    #region MonoBehaviour
    void Start()
    {
        check_my_color();
    }

    #endregion
    private void check_my_color()
    {
        if (top > 0)
            stack_color = transform.GetChild(0).gameObject.name[0];
    }

    #region Public Methods
    public bool is_stack_empty()
    {
        return (top == 0);
    }

    public bool is_vunarable()
    {

        return (top == 1);
    }
    
    public char get_stack_color()
    {
        return stack_color;
    }

    public void pop_piece()
    {
        string piece_2_destroy;
        if (stack_color == 'O')
            piece_2_destroy = "OrangePiece" + top--;
        else if (stack_color == 'G')
            piece_2_destroy = "GreenPiece" + top--;
        else
            return;
        Destroy(transform.Find(piece_2_destroy).gameObject);
    }

    public void push_piece(GameObject piece)
    {
        Debug.Log("Push piece on " + pieces_distance +"*"+ (top)+"="+ pieces_distance* (top));
        piece.transform.parent = transform;
        piece.GetComponent<Transform>().localPosition = new Vector3 (0,pieces_distance*(top),0);
        top++;

        //refer to option when top>8 (tower)
    }

    #endregion
}
