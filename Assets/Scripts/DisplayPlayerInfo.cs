using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayPlayerInfo : MonoBehaviour
{
    public Player player;

    public Text nameHolder;
    public Image iconHolder;

    public Image colorHolder;
    //todo color

    
    // Use this for initialization
    void Start()
    {
        updateInfo();
    }
   public void updateInfo()
    {
		nameHolder.text = player.pName;
        colorHolder.color = player.pColor;
       // iconHolder.sprite = player.pIcon;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
