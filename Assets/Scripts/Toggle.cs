using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class Toggle : MonoBehaviour {

    public Button myButton;
    public Sprite All;
    public Sprite Ene;
    public static int counter = 0;

	// Use this for initialization
	void Start () {
        myButton = GetComponent<Button>();
	}
	
	// Update is called once per frame
	public void changeButton () {
        counter ^= 1;
        if (counter == 1)
            myButton.image.overrideSprite = Ene;
        else
            myButton.image.overrideSprite = All;
    }
}
