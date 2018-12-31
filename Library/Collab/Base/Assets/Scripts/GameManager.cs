using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager GM;
    public GameObject playerPrefab;
    public GameObject kplayerPrefab;
    public GameObject guessPrefab;
    public GameObject notiPrefab;
    public GameObject players_View;

    public GameObject player_input;
    public GameObject noti_list;
    public Image cur_player_image;
    public Text cur_player_name;

    public Text GuessInputBox;
    public int MainTeam;
    public int CurTurn = 0;
    public int CurPlayer = -1;
    public int guess_length = 3;
    public Dropdown selectedPlayer;
    public Sprite[] numHelpers;
    public int[] currNumHelpers;

    List<int> selectedPlayerID = new List<int>();

    List<GameObject> Players_List = new List<GameObject>();
    List<GameObject> kPlayers_List = new List<GameObject>();
    List<Player> Players = new List<Player>();

    int toggle = 0;//0 all 1 ene
    public int timeToggle = 0;
    int aiToggle = 0;
    private Sprite[] icons;
    private string[] names = { "aaron", "abdul", "abe", "abel", "abraham", "adam", "adan", "adolfo", "adolph", "adrian" };
    Color[] teamColor = new Color[8];
    AI ai;
    float aiTime = 100; 


    private void Awake()
    {
        
        if (!GM)
        {
            GM = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this);
        }

    }
    // Use this for initialization

    void Start()
    {
        numHelpers = Resources.LoadAll<Sprite>("NumHelpers");
        currNumHelpers = new int[numHelpers.Length];

        
        guess_length = LobbyManager.LM.guess_length;
        MainTeam = LobbyManager.LM.main_team;
        ai = new AI(guess_length);

        //Initialize colors for teams
        teamColor[0] = new Color(51 / 255.0f, 153 / 255.0f, 255 / 255.0f);
        teamColor[1] = new Color(229 / 255.0f, 43 / 255.0f, 80 / 255.0f);
        teamColor[2] = new Color(0 / 255.0f, 168 / 255.0f, 107 / 255.0f);
        teamColor[3] = new Color(255.0f / 255.0f, 191 / 255.0f, 0 / 255.0f);
        teamColor[4] = new Color(153 / 255.0f, 102 / 255.0f, 204 / 255.0f);
        teamColor[5] = new Color(245 / 255.0f, 245 / 255.0f, 220 / 255.0f);
        teamColor[6] = new Color(72 / 255.0f, 60 / 255.0f, 50 / 255.0f);
        teamColor[7] = new Color(0, 49 / 255.0f, 83 / 255.0f);

        //Assign players turns randomly
        int plSize = LobbyManager.LM.lobbyPlayers.Count;
        var turns = new int[plSize];
        for (int i = 1; i <= plSize; i++)
            turns[i - 1] = i;
        System.Random rng = new System.Random();
        int n = turns.Length;
        while (n > 1)
        {
            int k = rng.Next(n);
            n--;
            int temp = turns[n];
            turns[n] = turns[k];
            turns[k] = temp;
        }

        int c = 0;
        foreach (var item in LobbyManager.LM.lobbyPlayers)
        {
            //make 2 GameObject copies of every player as a player and a Kplayer
            GameObject newPlayer, newkPlayer;

            newkPlayer = Instantiate(kplayerPrefab);
            newPlayer = Instantiate(playerPrefab);

            Player player = ScriptableObject.CreateInstance<Player>();
            player.Id = c++;
            player.pName = item.name;
            player.pIcon = item.image;
            player.pNumber = item.guess;
            player.teamNo = item.teamNo;
            player.AI = item.AI;
            player.IN = true;
            player.type = item.type;
            player.turnNo = turns[player.Id];
            player.pColor = teamColor[player.teamNo - 1];
            Players.Add(player);

            //Debug.Log(player.pName);

            newPlayer.GetComponent<DisplayPlayerInfo>().player = player;
            newPlayer.SetActive(false);
            Players_List.Add(newPlayer);
            newkPlayer.GetComponent<DisplayPlayerInfo>().player = player;
            newkPlayer.SetActive(false);
            kPlayers_List.Add(newkPlayer);
        }

        Players = Players.OrderBy(x => x.turnNo).ToList();
        Players_List = Players_List.OrderBy(x => x.GetComponent<DisplayPlayerInfo>().player.teamNo).ToList();
        kPlayers_List = kPlayers_List.OrderBy(x => x.GetComponent<DisplayPlayerInfo>().player.teamNo).ToList();

        foreach (var player in Players_List)
        {
            player.transform.SetParent(players_View.transform, false);
            player.GetComponent<RectTransform>().localPosition = Vector3.zero;
        }

        foreach (var player in kPlayers_List)
        {
            player.transform.SetParent(players_View.transform, false);
            player.GetComponent<RectTransform>().localPosition = Vector3.zero;
        }

        nextTurn();

        /*
        #region //old
        
        icons = Resources.LoadAll<Sprite>("players");
        for (int i = 0; i < 10; i++)
        {
            GameObject newPlayer = Instantiate(playerPrefab);
            Player player = ScriptableObject.CreateInstance<Player>();
            player.pName = names[Random.Range(0, names.Length)];
            player.pIcon = icons[Random.Range(0, icons.Length)];


            newPlayer.GetComponent<DisplayPlayerInfo>().player = player;
            newPlayer.transform.parent = players_View.transform;

            Transform Guess_list = newPlayer.transform.FindDeepChild("Guess_list");

            
            for (int j = 0; j < 10; j++)
            {
                Guess guess = ScriptableObject.CreateInstance<Guess>();
                guess.gGuess = Random.Range(0, 10000).ToString();
                guess.gGuess = Random.Range(0, 10000).ToString();
                guess.gBulls = Random.Range(0, 10);
                guess.gCows = Random.Range(0, 10);

                GameObject newguess = Instantiate(guessPrefab);
                newguess.GetComponent<DisplayGuessInfo>().guess = guess;
                newguess.transform.parent = Guess_list;


            }
            Players_List.Add(newPlayer);
            //Debug.Log(Guess_list.name);
	}*
        #endregion
        */
    }

    public void nextTurn()
    {
        CurTurn++;
        //TODO stop game after a number of turns ?!

        //get Current player
        CurPlayer = (CurPlayer + 1) % Players.Count();
        while (!Players[CurPlayer].IN)
            CurPlayer = (CurPlayer + 1) % Players.Count();

        cur_player_image.sprite = Players[CurPlayer].pIcon;
        cur_player_name.text = Players[CurPlayer].pName;

        timeToggle = 1;
        RadialProgressBar.currentAmount = 100;

        //Switch between player and AI inputs
        if (Players[CurPlayer].AI)
        {
            player_input.SetActive(false);
            aiTime = Random.Range(1, 2);
            aiToggle = 1;
        }
        else
        {
            player_input.SetActive(true);
        }


        //assign players layouts depending on the current main team
        List<string> selectedPlayerName = new List<string>();
        selectedPlayerID.Clear();

        for (int i = 0; i < Players_List.Count; i++)
            if (Players_List[i].GetComponent<DisplayPlayerInfo>().player.IN)
            {
                if (MainTeam != Players_List[i].GetComponent<DisplayPlayerInfo>().player.teamNo)
                {
                    Players_List[i].SetActive(true);
                    kPlayers_List[i].SetActive(false);
                    selectedPlayerName.Add(Players_List[i].GetComponent<DisplayPlayerInfo>().player.pName);
                    selectedPlayerID.Add(Players_List[i].GetComponent<DisplayPlayerInfo>().player.Id);
                }
                else
                {
                    kPlayers_List[i].SetActive(true);
                    Players_List[i].SetActive(false);
                }
            }

        selectedPlayer.ClearOptions();
        selectedPlayer.AddOptions(selectedPlayerName);

        //TODO handle switching between mainTeams in multi local player teams;
    }

    public void takeGuess()
    {
        if (!validateGuess(GuessInputBox.text))
        {
            GuessInputBox.text = "";
            //TODO unvaild messege
            return;
        }

        int id = selectedPlayerID[selectedPlayer.value];
        pastTurn(id,GuessInputBox.text);
        GuessInputBox.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        //TODO el mainteam doesnt move yet
        if (toggle != Toggle.counter)
        {
            if (toggle == 0)
            {
                foreach (var item in kPlayers_List)
                {
                    if (item.GetComponent<DisplayPlayerInfo>().player.teamNo == MainTeam)
                        item.SetActive(false);
                }
            }
            else
            {
                foreach (var item in kPlayers_List)
                {
                    if (item.GetComponent<DisplayPlayerInfo>().player.teamNo == MainTeam)
                        item.SetActive(true);
                }
            }
            toggle = Toggle.counter;
        }
        if (aiToggle == 1)
        {
            if (aiTime > 0)
                aiTime -= Time.deltaTime;
            else
            {
                aiToggle = 0;
                aiTurn();
            }
        }
    }

    public void resetColors()
    {
        foreach (var player in Players_List)
        {
            player.GetComponent<DisplayPlayerInfo>().player.pColor = Color.white;
            player.GetComponent<DisplayPlayerInfo>().updateInfo();
        }

    }

    void pastTurn(int id, string guessText)
    {
        Transform Guess_list = Players_List[0].transform.FindDeepChild("Guess_list");
        Transform kGuess_list = kPlayers_List[0].transform.FindDeepChild("Guess_list");
        string num = "none";

        foreach (var player in Players_List)
            if (player.GetComponent<DisplayPlayerInfo>().player.Id == id)
            {
                num = player.GetComponent<DisplayPlayerInfo>().player.pNumber;
                Guess_list = player.transform.FindDeepChild("Guess_list");
                break;
            }

        foreach (var player in kPlayers_List)
            if (player.GetComponent<DisplayPlayerInfo>().player.Id == id)
            {
                kGuess_list = player.transform.FindDeepChild("Guess_list");
                break;
            }

        int cows = 0, bulls = 0;

        compareNumbers(ref cows, ref bulls, num, guessText);

        Guess guess = ScriptableObject.CreateInstance<Guess>();
        guess.gGuess = guessText;
        guess.gBulls = bulls;
        guess.gCows = cows;

        foreach (Player player in Players)
            if (player.Id == id)
                player.Guesses_Taken.Add(guess);

        //Players_List[selectedPlayer.value].GetComponent<DisplayPlayerInfo>().player.Guesses_Taken.Add(guess);
        //kPlayers_List[selectedPlayer.value].GetComponent<DisplayPlayerInfo>().player.Guesses_Taken.Add(guess);


        var newguess = Instantiate(guessPrefab) as GameObject;
        newguess.GetComponent<DisplayGuessInfo>().guess = guess;
        newguess.transform.parent = Guess_list;

        Instantiate(newguess, kGuess_list);
        //Instantiate(newguess, Guess_list);

        if (bulls == guess_length)
        {
            eliminatePlayer(id, Players[CurPlayer].Id);
            if (winState())
            {
                addNoti(Players[CurPlayer].pName + "'s team won");
                timeToggle = 0;
                return;
                //TODO notifiy the winner team and back to lobby
            }
        }

        nextTurn();
    }

    void aiTurn()
    {
        int type = Players[CurPlayer].type;
        //TODO randomize waiting time and add animation

        int id = pickTarget();
        string num = "";
        List<Guess> guesses = new List<Guess>();
        foreach (var player in Players)
            if (player.Id == id)
            {
                num = player.pNumber;
                guesses = player.Guesses_Taken;
                break;
            }
        string guess = ai.whatNext(guesses, type, num);
        pastTurn(id, guess);
    }

    void compareNumbers(ref int cows , ref int bulls , string number , string guess)
    {
        cows = bulls = 0;
        foreach (char a in number)
            foreach (char b in guess)
                if (a == b)
                    cows++;
        for (int i = 0; i < guess_length; i++)
            if (number[i] == guess[i])
                bulls++;
        cows -= bulls;
    }

    bool validateGuess(string guess)
    {
        if (guess.Length != guess_length)
            return false;
        for (int i = 0; i < guess.Length; i++)
            for (int j = i + 1; j < guess.Length; j++)
                if (guess[i] == guess[j])
                    return false;
        return true;
    }

    int pickTarget()
    {
        List<Player> allowed = new List<Player>();
        foreach (var player in Players)
            if (player.IN && player.teamNo != Players[CurPlayer].teamNo)
                allowed.Add(player);

        //TODO handle different types pick styles

        int rnd = Random.Range(0, allowed.Count);
        return allowed[rnd].Id;
    }

    public void eliminatePlayer(int loser , int winner)
    {
        string win = "", lose = "",losenum = "";
        foreach (var player in Players)
            if (player.Id == loser)
            {
                lose = player.pName;
                losenum = player.pNumber;
                player.IN = false;
                break;
            }else if(player.Id == winner)
            {
                win = player.pName;
            }
        addNoti(win + " x " + lose + " with " + losenum);
        foreach (var player in Players_List)
            if (player.GetComponent<DisplayPlayerInfo>().player.Id == loser)
            {
                player.GetComponent<DisplayPlayerInfo>().player.IN = false;
                break;
            }
        foreach (var player in kPlayers_List)
            if (player.GetComponent<DisplayPlayerInfo>().player.Id == loser)
            {
                player.GetComponent<DisplayPlayerInfo>().player.IN = false;
                break;
            }
    }
    
    bool winState()
    {
        HashSet<int> teams = new HashSet<int>();
        foreach (var player in Players)
            if (player.IN)
                teams.Add(player.teamNo);
        return teams.Count == 1;
    }

    public void timeOut()
    {
        //TODO handle time over in a player turn
        nextTurn();
    }

    public void addNoti(string content)
    {
        var newNoti = Instantiate(notiPrefab) as GameObject;
        newNoti.GetComponent<DisplayNotiInfo>().content.text = content;
        newNoti.transform.parent = noti_list.transform;
    }

}