using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_TrianglePiecesStack : MonoBehaviour
{
    public char stack_color;
    public int pieces_amount;
    // Start is called before the first frame update

    #region MonoBehaviour
    void Start()
    {
        check_my_color();
    }

    #endregion
    private void check_my_color()
    {
        if (pieces_amount > 0)
            stack_color = transform.GetChild(0).gameObject.name[0];
        Debug.Log("SC_TrianglePiecesStack color = " + stack_color);
    }

    #region Public Methods
    public bool is_stack_empty()
    {
        return (pieces_amount == 0);
    }

    public bool is_vunarable()
    {
        return (pieces_amount == 1);
    }
    
    public char get_stack_color()
    {
        return stack_color;
    }

    #endregion
}
