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
    public void check_my_color()
    {
        if (top > 0)
            stack_color = transform.GetChild(0).gameObject.name[0];
        else if (top == 0)
            stack_color = 'N';
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

        if (top == 0)
            stack_color = 'N';
    }

    public void push_piece(GameObject piece,char color)
    {
        piece.transform.parent = transform;
        piece.GetComponent<Transform>().localPosition = new Vector3 (0,pieces_distance*(top),0);
        piece.GetComponent<Transform>().localScale = new Vector3(1, 1, 1);
        piece.GetComponent<SC_Piece>().change_piece_name(++top,color);
        StartCoroutine(update_stack_color());

        //refer to option when top>8 (tower)
    }

    private IEnumerator update_stack_color()
    {
        yield return null; // wait until the next frame
        check_my_color();
    }


    #endregion
}
