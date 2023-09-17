using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_Triangle : MonoBehaviour
{
    public delegate void Triangle_press_handler(string name);
    public static Triangle_press_handler pressed_triangle;
    GameObject Sprite_Triangle;
    SC_TrianglePiecesStack Triangle_Stack;
    void Awake()
    {
        Debug.Log("Awake" + name);
        Triangle_Stack= transform.Find("TrianglePiecesStack").GetComponent<SC_TrianglePiecesStack>();
    }

    private void Start()
    {
        Sprite_Triangle = transform.Find("Sprite_Triangle").gameObject;
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

    public bool is_vunarable()
    {
        return Triangle_Stack.is_vunarable();
    }

    public char get_stack_color()
    {
        return Triangle_Stack.get_stack_color();
    }
}
