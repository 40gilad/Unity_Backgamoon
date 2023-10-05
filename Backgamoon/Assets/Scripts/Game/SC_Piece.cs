using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;

public class SC_Piece : MonoBehaviour
{
    private string OrangeName = "OrangePiece";
    private string OreenName = "GreenPiece";
   public void change_piece_name(int index, char color)
    {
        if (color == 'O')
            name = OrangeName + index;
        else if(color == 'G')
            name = OreenName + index;
    }
}
