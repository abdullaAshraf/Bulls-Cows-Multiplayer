using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class OnClickLobby : MonoBehaviour
{

    public int type;

    // Use this for initialization
    void Start()
    {


    }



    public void TargetSelector()
    {
        GameObject target = EventSystem.current.currentSelectedGameObject;
        if (target.name == "Player")
        {
            LobbyManager.LM.CreateHumanPanel();
            return;
        }
        LobbyPlayer newPlayer = ScriptableObject.CreateInstance<LobbyPlayer>();
        newPlayer.name = target.name;
        newPlayer.image = target.transform.Find("Image").gameObject.GetComponent<Image>().sprite.name;
        newPlayer.teamNo = 1;
        newPlayer.AI = true;
        newPlayer.type = type;
        newPlayer.number = genRandomGuess();
        LobbyManager.LM.CreatePlayer(newPlayer);
    }
    string genRandomGuess()
    {
        List<string> nums = new List<string>();
        for (int i = 0; i < 10; i++)
        {
            nums.Add(i.ToString());
        }
        string guess = "";
        for (int i = 0; i < LobbyManager.LM.guess_length; i++)
        {
            int r = Random.Range(0, nums.Count);
            guess += nums[r];
            nums.RemoveAt(r);

        }
        return guess;
    }

    public void changeTeam()
    {
        GameObject target = EventSystem.current.currentSelectedGameObject;

        LobbyManager.LM.changeTeam(target);

    }
    public void OnStartButton()
    {
        if (LobbyManager.LM.ValidateTeams())
            SceneManager.LoadScene("GameRoom");
        else
        {
            //Error
        }
    }
    public void DeletePanel()
    {
        GameObject target = EventSystem.current.currentSelectedGameObject;
        int pid = target.GetComponent<DisplayPlayerSelect>().pinfo.id;
        //removePlayer
        LobbyManager.LM.DeletePanel(target);
    }


}
