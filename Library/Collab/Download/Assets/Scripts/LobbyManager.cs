using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class LobbyManager : MonoBehaviour
{
    public static LobbyManager LM;
    public GameObject playerPrefab;
    public int MaxTeams;
    public Transform content;
    public List<GameObject> Players_List = new List<GameObject>();
    public List<LobbyPlayer> lobbyPlayers = new List<LobbyPlayer>();
    public GameObject humanPanel;
    public int guess_length = 3; //TODO allow more lengths
    public int main_team;
    GameObject Canvas;

    private System.Random _random = new System.Random();
    SortedList teams;
    // Use this for initialization
    void Start()
    {
        Canvas = GameObject.Find("Canvas");
        if (!LM)
        {
            LM = this;
            DontDestroyOnLoad(gameObject);
        }
       

    }

    public bool ValidateTeams() //TODO more restrictions ?
    {
        int localPlayersTeams = 0;

        SortedDictionary<int, int> s = new SortedDictionary<int, int>();
        bool AI = true;
        foreach (var item in lobbyPlayers)
        {
            if (s.ContainsKey(item.teamNo))
                s[item.teamNo] += 1;
            else s.Add(item.teamNo, 1);
            AI &= item.AI;
            if (!item.AI) {
                main_team = item.teamNo;
                localPlayersTeams++;
            }
        }

        if (localPlayersTeams > 1)//TODO handle multi players team in a single device
            return false;



        if (s.Count >= 2 && !AI) return true;

       
        return false;
    }
    public void CreateHumanPanel()
    {
        GameObject panel = Instantiate(humanPanel);
        panel.transform.SetParent(Canvas.transform, false);

        //panel.GetComponent<RectTransform>().localPosition = Vector3.zero;
        panel.transform.localPosition = Vector3.zero;

        //panel.anchorMin = new Vector2(1, 0);
        //panel.anchorMax = new Vector2(0, 1);
        //panel.pivot = new Vector2(0.5f, 0.5f);
    }

    public void CreatePlayer(LobbyPlayer newPlayer)
    {
        GameObject holder = Instantiate(playerPrefab);
        #region   //Todo assign teamNO
        /*int teamNo=1;

		if (Players_List.Count!=0) {
			teams = new SortedList ();
			foreach (var item in Players_List) {
				if(!teams.Contains (item));
				teams.Add ((item.GetComponent<DisplayPlayerSelect> ().pinfo.teamNo),0);
			}

			for (int i = 0; i < teams.Count; i++) {
				if (!teams.Contains (i+1)) {				
					teamNo = i + 1;
					break;
				}
			}
			teams.Clear ();
		 }*/
        #endregion
        MaxTeams++;
        if (MaxTeams > 8) MaxTeams = 8;
        newPlayer.teamNo = MaxTeams;
        holder.GetComponent<DisplayPlayerSelect>().pinfo = newPlayer;
        holder.GetComponent<DisplayPlayerSelect>().Display();
        Players_List.Add(holder);
        lobbyPlayers.Add(newPlayer);
        holder.transform.SetParent(content, false);
    }

    public void changeTeam(GameObject target)
    {
        //Debug.Log (target.transform.parent.gameObject.name);
        int teamNo = int.Parse(target.GetComponentInChildren<Text>().text);
        teamNo++;
        if (teamNo > MaxTeams)
            teamNo = 1;
        //target.GetComponentInChildren<Text> ().text = teamNo.ToString ();
        //Debug.Log (target.GetComponentInChildren<Text> ().text);
        for (int i = 0; i < Players_List.Count; i++)
        {
            if (Players_List[i] == target.transform.parent.gameObject)
            {
                Players_List[i].GetComponent<DisplayPlayerSelect>().pinfo.teamNo = teamNo;
                lobbyPlayers[i].teamNo = teamNo;
                Players_List[i].GetComponent<DisplayPlayerSelect>().Display();
                break;
            }
        }

    }

    public void DeletePanel(GameObject target)
    {
        for (int i = 0; i < Players_List.Count; i++)
        {
            if (Players_List[i] == target.transform.parent.gameObject)
            {
                Destroy(Players_List[i]);
                Players_List.RemoveAt(i);
                lobbyPlayers.RemoveAt(i);
                MaxTeams--;
                break;
            }
        }
    }

}
