using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class onClickTest : MonoBehaviour
{
    static bool virgin = true;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void print()
    {
        Debug.Log("aaaaa");
    }
    public void SendGuess()
    {
        GameManager.GM.takeGuess();
    }
    public void nameHelperButton()
    {
        int no = int.Parse(EventSystem.current.currentSelectedGameObject.name);
        GameManager.GM.currNumHelpers[no] = (GameManager.GM.currNumHelpers[no] + 1) % 3;
        GetComponent<Image>().sprite = GameManager.GM.numHelpers[(no * 3 + GameManager.GM.currNumHelpers[no])];

        //Debug.Log("numHelpers " + no.ToString());

    }
    public void GuessSelector()
    {
        string name = EventSystem.current.currentSelectedGameObject.name;
        string curr = GameManager.GM.GuessInputBox.text;
        if (virgin) { curr = ""; virgin = false; } // :)
        if (name == "Del")
        {

            if (curr.Length > 0) { StringBuilder sb = new StringBuilder(curr); sb.Remove(curr.Length - 1, 1); curr = sb.ToString(); }

        }
        else if (name == "CLR")
        {
            curr = "";
        }
        else if (!curr.Contains(name) && curr.Length < GameManager.GM.guess_length)
        {
            curr += name;

        }

        GameManager.GM.GuessInputBox.text = curr;

    }
    public void TargetSelector()
    {
        Debug.Log(EventSystem.current.currentSelectedGameObject.transform.FindDeepChild("PlayerName").GetComponent<Text>().text);
        Color color = EventSystem.current.currentSelectedGameObject.transform.FindDeepChild("BackgroundImage").GetComponent<Image>().color;
        GameManager.GM.resetColors();
        if (color == Color.white)
            EventSystem.current.currentSelectedGameObject.transform.FindDeepChild("BackgroundImage").GetComponent<Image>().color = hexToColor("4F4F4FFF");
        //BackgroundImage
        //4F4F4FFF  FFFFFFFF
    }


    public static string colorToHex(Color32 color)
    {
        string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
        return hex;
    }


    public static Color hexToColor(string hex)
    {
        hex = hex.Replace("0x", "");//in case the string is formatted 0xFFFFFF
        hex = hex.Replace("#", "");//in case the string is formatted #FFFFFF
        byte a = 255;//assume fully visible unless specified in hex
        byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        //Only use alpha if the string has enough characters
        if (hex.Length == 8)
        {
            a = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
        }
        return new Color32(r, g, b, a);
    }
}
