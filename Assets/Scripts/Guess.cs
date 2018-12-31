using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Guess : ScriptableObject
{

    public string gGuess { get; set; }

    public int gBulls { get; set; }

    public int gCows { get; set; }
}
