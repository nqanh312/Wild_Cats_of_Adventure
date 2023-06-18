using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;                       // this gives us access to the [Serialable] attribute
using UnityEngine.UI;               // this gives us access to the Unity UI elements

/// <summary>
/// Groups all the user interface elements together for GameCtrl to use
/// </summary>

[Serializable]
public class UI 
{
    [Header("Text")]
    public Text txtCoinCount;          // for updating the number of coins collected
    public Text txtScore;
    public Text txtTimer;              // for showing the score in the HUD

    [Header("Images / Sprites")]
    public Image key0;
    public Image key1;
    public Image key2;
    public Sprite key0Full;
    public Sprite key1Full;
    public Sprite key2Full;
    public Image heart1;
    public Image heart2;
    public Image heart3;
    public Sprite emptyHeart;
    public Sprite fullHeart;

    public GameObject panelGameOver;


}
