using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu]
public class LobbyPlayer : ScriptableObject {

    //public Sprite image;
    public string image = "9";
	public int teamNo;
	public new string name = "new player";
    public string number;
    public bool AI;
    public int type;
    public int id;
    public bool local;

    public LobbyPlayer(int _id)
    {
        id = _id;
    }

    public LobbyPlayer()
    {
    }
}
