using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.IO;
using System;
using System.Collections.Generic;

public class Client : MonoBehaviour
{
    public bool isHost;
    /*
    public string clientName;
    public string clientNumber;
    public string clientImage;
    */
    public List<LobbyPlayer> players;

    private bool socketReady;
    private TcpClient socket;
    private NetworkStream stream;
    private StreamWriter writer;
    private StreamReader reader;

    public List<GameClient> clients = new List<GameClient>();

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public bool ConnectToServer(string host, int port)
    {
        if (socketReady)
            return false;

        try
        {
            socket = new TcpClient(host, port);
            stream = socket.GetStream();
            writer = new StreamWriter(stream);
            reader = new StreamReader(stream);

            socketReady = true;
        }
        catch (Exception e)
        {
            Debug.Log("Socket error " + e.Message);
        }

        return socketReady;
    }

    private void Update()
    {
        if (socketReady)
        {
            if (stream.DataAvailable)
            {
                string data = reader.ReadLine();
                if (data != null)
                    OnIncomingData(data);
            }
        }
    }

    // Sending messaged to the server
    public void Send(string data)
    {
        if (!socketReady)
            return;

        writer.WriteLine(data);
        writer.Flush();
    }

    // Read messages from the server
    private void OnIncomingData(string data)
    {
        Debug.Log("Client:" + data);

        string[] aData = data.Split('|');

        switch (aData[0])
        {
            case "SWHO":
                for (int i = 1; i < aData.Length - 1; i++)
                {
                    UserConnected(int.Parse(aData[i]));
                }
                Send("CNEW|");
                break;

            case "SNEW":
                //UserConnected(int.Parse(aData[1]));
                break;
            case "SADD":
                UserConnected(int.Parse(aData[1]) , int.Parse(aData[2]));
                break;
            case "SEDT":

                break;
            case "SREM":

                break;
            case "SRDY":
                setReady(int.Parse(aData[1]), aData[2] == "1" ? true : false);
                break;
            case "SGUS":

                break;
            case "STUR":

                break;
        }

    }

    private void setReady(int pid, bool value)
    {
        foreach (GameClient c in clients)
        {
            if (c.clientId == pid)
            {
                c.ready = value;
            }
        }
    }

    private void editPlayer(int pid, string name, string number, string image, int teamNo)
    {
        //update lobby manger
        foreach (GameClient c in clients)
        {
            if (c.player.id == pid)
            {
                c.player.name = name;
                c.player.number = number;
                c.player.image = image;
                c.player.teamNo = teamNo;
            }
        }
    }

    private void UserConnected(int cid)
    {

        GameClient c = new GameClient();
        c.player.id = pid;
        c.isHost = false;
        c.ready = false;
        c.player.AI = false;

        clients.Add(c);

        LobbyManager.LM.CreatePlayer(c.player);
    }

    private void PlayerAdded(int pid , int cid)
    {

    }


    private void OnApplicationQuit()
    {
        CloseSocket();
    }
    private void OnDisable()
    {
        CloseSocket();
    }
    private void CloseSocket()
    {
        if (!socketReady)
            return;

        writer.Close();
        reader.Close();
        socket.Close();
        socketReady = false;
    }
}

public class GameClient
{
    public LobbyPlayer player;
    public bool isHost;
    public int clientId;
    public bool ready;
    /*
    public string name;
    public string number;
    public string image;
    */
    //TODO add all player info

}