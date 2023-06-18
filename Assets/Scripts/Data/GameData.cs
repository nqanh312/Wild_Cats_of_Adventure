using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;               // gives access to the [Serialazble] attribute

/// <summary>
/// The Data Model for your game data
/// </summary>

[Serializable]
public class GameData
{
    public int coinCount; 
    public int lives;           // tracks the number of coins collected
    public int score;
    public bool[] keyFound;
    public bool isFirstBoot;
}
