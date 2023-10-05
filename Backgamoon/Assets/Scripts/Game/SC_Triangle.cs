using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SC_Triangle : MonoBehaviour
{
    public delegate void Triangle_press_handler(string name);
    public static Triangle_press_handler pressed_triangle;
    public GameObject Sprite_Triangle;
    SC_TrianglePiecesStack Triangle_Stack;
    void Awake()
    {
        Triangle_Stack= transform.Find("TrianglePiecesStack").GetComponent<SC_TrianglePiecesStack>();
    }


    private void OnMouseDown()
    {
        pressed_triangle(name);
    }

    public void change_sprite_stat()
    {
        Sprite_Triangle.SetActive(!Sprite_Triangle.activeSelf);
    }

    public bool is_stack_empty()
    {
        return Triangle_Stack.is_stack_empty();
    }

    public bool is_vunarable(bool turn)
    {
        if((turn && get_stack_color()=='G') || (!turn && get_stack_color() == 'O'))
            return Triangle_Stack.is_vunarable();
        return false;
    }

    public char get_stack_color()
    {
        return Triangle_Stack.get_stack_color();
    }

    public void pop_piece()
    {
        Triangle_Stack.pop_piece();
    }


    public void push_piece(GameObject piece,char color)
    {
        Triangle_Stack.push_piece(piece,color);
    }

    public bool is_sprite_active()
    {
        if (Sprite_Triangle.activeSelf)
            return true;
        return false;
    }

    public int top()
    {
        return Triangle_Stack.top;
    }

}
