using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
public class SC_Board : MonoBehaviour
{
    private static int TRIANGLES_AMOUNT = 24;
    private float PieceStackDist = 0.7f;
    Dictionary<string, GameObject> TrianglesContainers;

    void Awake()
    {
        TrianglesContainers = new Dictionary<string, GameObject>();
        assign_values_to_TrianglesContainers();
    }

    void OnEnable()
    {
        SC_Piece.Piece_Press += Piece_press;
    }

    private void OnDisable()
    {
        SC_Piece.Piece_Press -= Piece_press;

    }

    void Update()
    {
        
    }

    private void assign_values_to_TrianglesContainers()
    {
        /* FUNCTION ALSO SETACTIVE FALSE TO ALL TRIANGLES */
        string currname;
        Transform curr_triangle_transform;
        for (int i = 0; i < TRIANGLES_AMOUNT; i++)
        {
            currname = "Triangle" + i;
            //Debug.Log("Awake" + currname);
            curr_triangle_transform = GameObject.Find(currname).transform;
            Change_TriangleState(curr_triangle_transform.Find("Sprite_Triangle").gameObject);
            TrianglesContainers.Add(currname, curr_triangle_transform.Find("TrianglePiecesStack").gameObject);
        }
    }
    void Change_TriangleState(GameObject t)
    {
        t.SetActive(!t.activeSelf);
    }

    private void Piece_press(string t_name)
    {
        //Debug.Log("Piece_press "+ t_name);
        //Change_TriangleState(TrianglesContainers[t_name].transform.parent.Find("Sprite_Triangle").gameObject);
    }
}
