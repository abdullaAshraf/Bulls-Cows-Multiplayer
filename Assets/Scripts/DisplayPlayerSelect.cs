using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayPlayerSelect : MonoBehaviour {
	public LobbyPlayer pinfo;

	public Image imageHolder;
	public Text TeamNoHolder;
	public Text NameHolder;
	// Use this for initialization
	void Start () {
		Display();
	}
	public void Display()
	{
		imageHolder.sprite=	Resources.Load<Sprite>("players/" + pinfo.image);
		TeamNoHolder.text = pinfo.teamNo.ToString ();
		NameHolder.text = pinfo.name;
	
	}
	

}
