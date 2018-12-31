using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Player : ScriptableObject
{
    public bool AI;

    public bool IN;

    public int Id { get; set; }

    public Color pColor = Color.white;

    public string pNumber { get; set; }

    public string pName;

	public int teamNo;

    public int turnNo;

    public int type;

    public Sprite pIcon;

    public List<Guess> Guesses_Taken=new List<Guess>();
}
