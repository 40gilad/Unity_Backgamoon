using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_Sprite_Triangle : MonoBehaviour
{
    public delegate void finish_move(GameObject g);
    public static finish_move turn_me_off;
    private float PieceDistance = -0.7f;
    bool turn;
    GameObject[] Pieces; //0= orange piece, 1= green piece
    SC_Board board;

    void Awake()
    {
        //Debug.Log("Awake" + name);
        Pieces= new GameObject[2];

    }
    void Start()
    {
        turn = false;
        board = GameObject.Find("Board").GetComponent<SC_Board>();
        Pieces[0] = GameObject.Find("OrangePiece");
        Pieces[1] = GameObject.Find("GreenPiece");

    }

    void OnEnable()
    {
        Debug.Log("SC_Sprit_Triangle OnEnable turn= " + turn);
        turn = GameObject.Find("SC_TriangleManeger").GetComponent<SC_TriangleManeger>().turn;
    }

    private void OnMouseDown()
    {
        Debug.Log("Pressed on " + transform.parent.name);
        Debug.Log("SC_Sprit_Triangle.OnMouseDown() with turn: " + turn);
        if (turn)//orange turn
            Move_Piece(Instantiate(Pieces[0]));
        else if (!turn)
            Move_Piece(Instantiate(Pieces[1]));
    }

    void Move_Piece(GameObject piece)
    {
        GameObject Triangle_Stack = transform.parent.transform.Find("TrianglePiecesStack").gameObject;
        int pieces_amout = Triangle_Stack.GetComponent<SC_TrianglePiecesStack>().pieces_amount;
        piece.transform.parent = Triangle_Stack.transform;
        piece.GetComponent<Transform>().localPosition = new Vector3(0, (PieceDistance * pieces_amout), 0);
        piece.GetComponent<Transform>().localScale = new Vector3(1, 1, 1);
        Triangle_Stack.GetComponent<SC_TrianglePiecesStack>().pieces_amount++;
        board.piece_press_counter++;
        Debug.Log("Move piece: piece_counter= " + board.piece_press_counter + " limit moves = " + board.limit_moves);
        if (board.piece_press_counter == board.limit_moves)
        {
            StartCoroutine(finished_turn());
        }
        else
            turn_me_off(this.gameObject);
    }

    IEnumerator finished_turn()
    {
        board.piece_press_counter = 0;
        board.limit_moves = 2;
        yield return new WaitForSeconds(1);
        turn_me_off(this.gameObject);
        board.ChangeTurn();
    }
}
