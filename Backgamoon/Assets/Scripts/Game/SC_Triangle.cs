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
    public GameObject Triangle_Stack;
    SC_TrianglePiecesStack SC_Triangle_Stack;
    void Awake()
    {
        Debug.Log("SC_Triangle Awake " + name);
       SC_Triangle_Stack= Triangle_Stack.GetComponent<SC_TrianglePiecesStack>();
    }

    private void Start()
    {
        Debug.Log("SC_Triangle Start " + name);

    }


    private void OnMouseDown()
    {
        Debug.Log("pressed  "+ name);
        pressed_triangle(name);
    }

    public void change_sprite_stat()
    {
        Sprite_Triangle.SetActive(!Sprite_Triangle.activeSelf);
    }

    public bool is_stack_empty()
    {
        return SC_Triangle_Stack.is_stack_empty();
    }

    public bool is_vunarable(bool turn)
    {
        if((turn && get_stack_color()=='G') || (!turn && get_stack_color() == 'O'))
            return SC_Triangle_Stack.is_vunarable();
        return false;
    }

    public char get_stack_color()
    {
        return SC_Triangle_Stack.get_stack_color();
    }

    public void pop_piece()
    {
        SC_Triangle_Stack.pop_piece();
    }


    public void push_piece(GameObject piece,char color)
    {
        SC_Triangle_Stack.push_piece(piece,color);
    }

    public bool is_sprite_active()
    {
        if (Sprite_Triangle.activeSelf)
            return true;
        return false;
    }

    public int top()
    {
        return SC_Triangle_Stack.top;
    }

}
