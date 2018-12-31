using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class lobbyPlayerImageSelector : MonoBehaviour
{
    private Sprite[] icons;
    Image image;
    int currentIcon;

    Text InputName, InputNum;
    // Use this for initialization
    void Start()
    {

        InputName = gameObject.transform.Find("InputName").Find("Text").gameObject.GetComponent<Text>();
        InputNum = gameObject.transform.Find("InputNum").Find("Text").gameObject.GetComponent<Text>();

        icons = Resources.LoadAll<Sprite>("players");

        currentIcon = 0;
        image = gameObject.transform.Find("Image").gameObject.GetComponent<Image>();
        image.sprite = icons[currentIcon];
    }

    public void CreateHumanPlayer()
    {
        LobbyPlayer newPlayer = ScriptableObject.CreateInstance<LobbyPlayer>();
        newPlayer.name = InputName.text;
        newPlayer.image = icons[currentIcon];
        newPlayer.guess = InputNum.text;
        newPlayer.AI = false;
        if (Validate(newPlayer))
        {
            LobbyManager.LM.CreatePlayer(newPlayer);
            Destroy();
        }
        else
        {
            //TODO Show Error
        }
    }
    bool Validate(LobbyPlayer newPlayer)//TODO dnt allow same name ?
    {
        HashSet<char> set = new HashSet<char>();

        foreach (char c in newPlayer.guess)
        {
            set.Add(c);
        }
        int res = -1;
        if (int.TryParse(newPlayer.guess, out res) && res < 0)
        {
            //Error
            return false;
        }
        else if (set.Count != newPlayer.guess.Length)
        {
            //Duplicate Error
            return false;
        }
        else if (newPlayer.name.Length == 0)
        {
            //null error
            return false;
        }
        else if (newPlayer.guess.Length < 3)
        {
            //length error
            return false;
        }
        else
        {
            return true;
        }
    }
    public void nextIcon()
    {
        currentIcon = (currentIcon + 1) % icons.Length;

        image.sprite = icons[currentIcon];
    }
    public void prevIcon()
    {
        currentIcon = ((currentIcon - 1) + icons.Length) % icons.Length;
        Debug.Log(currentIcon);
        image.sprite = icons[currentIcon];
    }
    public void Destroy()
    {
        Destroy(gameObject);
    }
}
