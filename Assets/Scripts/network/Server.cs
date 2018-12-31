using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System;
using System.Collections.Generic;
using System.Net;
using System.IO;

public class Server : MonoBehaviour
{
    public int port = 6321;

    private List<ServerClient> clients;
    private List<ServerClient> disconnectList;

    private TcpListener server;
    private bool serverStarted;

    int assignId;
    int assignClientsId;

    public void Init()
    {
        DontDestroyOnLoad(gameObject);
        clients = new List<ServerClient>();
        disconnectList = new List<ServerClient>();
        assignId = 1;
        assignClientsId = 1;

        try
        {
            server = new TcpListener(IPAddress.Any, port);
            server.Start();

            StartListening();
            serverStarted = true;
        }
        catch (Exception e)
        {
            Debug.Log("Socket error: " + e.Message);
        }
    }

    private void Update()
    {
        if (!serverStarted)
            return;

        foreach (ServerClient c in clients)
        {
            // Is the client still connected?
            if (!IsConnected(c.tcp))
            {
                c.tcp.Close();
                disconnectList.Add(c);
                continue;
            }
            else
            {
                NetworkStream s = c.tcp.GetStream();
                if (s.DataAvailable)
                {
                    StreamReader reader = new StreamReader(s, true);
                    string data = reader.ReadLine();

                    if (data != null)
                        OnIncomingData(c, data);
                }
            }
        }

        for (int i = 0; i < disconnectList.Count - 1; i++)
        {
            // Tell our player somebody has disconected

            clients.Remove(disconnectList[i]);
            disconnectList.RemoveAt(i);
        }
    }

    private void StartListening()
    {
        server.BeginAcceptTcpClient(AcceptTcpClient, server);
    }
    private void AcceptTcpClient(IAsyncResult ar)
    {
        TcpListener listener = (TcpListener)ar.AsyncState;


        string allUsers = "";
        foreach (ServerClient i in clients)
        {
            foreach (LobbyPlayer player in i.players)
                allUsers += player.id + "|";
        }


        ServerClient sc = new ServerClient(listener.EndAcceptTcpClient(ar));
        clients.Add(sc);

        StartListening();

        Broadcast("SWHO|" + allUsers, clients[clients.Count - 1]);
        foreach (ServerClient c in clients)
            foreach (LobbyPlayer player in c.players)
            {
                Broadcast("SEDT|" + player.id + '|' + player.name + '|' + player.number + '|' + player.image + '|' + player.teamNo, clients[clients.Count - 1]);
            }
    }

    private bool IsConnected(TcpClient c)
    {
        try
        {
            if (c != null && c.Client != null && c.Client.Connected)
            {
                if (c.Client.Poll(0, SelectMode.SelectRead))
                    return !(c.Client.Receive(new byte[1], SocketFlags.Peek) == 0);

                return true;
            }
            else
                return false;
        }
        catch
        {
            return false;
        }
    }

    // Server Send
    private void Broadcast(string data, List<ServerClient> cl)
    {
        foreach (ServerClient sc in cl)
        {
            try
            {
                StreamWriter writer = new StreamWriter(sc.tcp.GetStream());
                writer.WriteLine(data);
                writer.Flush();
            }
            catch (Exception e)
            {
                Debug.Log("Write error : " + e.Message);
            }
        }
    }
    private void Broadcast(string data, ServerClient c)
    {
        List<ServerClient> sc = new List<ServerClient> { c };
        Broadcast(data, sc);
    }
    // Server Read
    private void OnIncomingData(ServerClient c, string data)
    {
        Debug.Log("Server:" + data);

        string[] aData = data.Split('|');

        switch (aData[0])
        {
            case "CNEW":
                c.id = assignClientsId++;
                //Broadcast("SNEW|" + c.id, clients);
                addPlayer(c);
                break;
            case "CEDT":
                for (int i = 0; i < c.players.Count; i++)
                    if (c.players[i].id == int.Parse(aData[1]))
                    {
                        c.players[i].name = aData[2];
                        c.players[i].number = aData[3];
                        c.players[i].image = aData[4];
                        Broadcast("SEDT|" + c.players[i].id + '|' + c.players[i].name + '|' + c.players[i].number + '|' + c.players[i].image + '|' + c.players[i].teamNo, clients);
                        break;
                    }
                break;
            case "CLEV":
                foreach (LobbyPlayer player in c.players)
                    removePlayer(player.id);
                break;
            case "CRDY":
                c.ready = aData[1] == "1" ? true : false;
                Broadcast("SRDY|" + c.id + '|' + c.ready, clients);
                break;
            case "CGUS":
                int fromPlayer = int.Parse(aData[1]);
                int toPlayer = int.Parse(aData[2]);
                string guess = aData[3];
                Broadcast("SGUS|" + fromPlayer + '|' + toPlayer + '|' + guess , clients);
                break;
        }
    }

    public void addPlayer(ServerClient c)
    {
        c.players.Add(new LobbyPlayer(assignId++));
        Broadcast("SADD|" + c.id + '|' + c.players[c.players.Count-1].id, clients);
    }

    public void shareOrder(List<int> turns)
    {
        string data = "";
        foreach (int id in turns)
            data += id + '|';
        Broadcast("STUR|" + data, clients);
    }

    public void removePlayer(int pid)
    {
        for (int i = 0; i < clients.Count; i++)
            foreach(LobbyPlayer player in clients[i].players)
                if (player.id == pid)
                {
                    Broadcast("SREM|" + pid, clients);
                    clients[i].players.Remove(player);
                    if (clients[i].players.Count == 0)
                    {
                        clients[i].tcp.GetStream().Close();
                        clients[i].tcp.Close();
                        clients.RemoveAt(i);
                    }
                    break;
                }
    }

    public void editPlayerTeam(int pid, int team)
    {
        foreach (ServerClient c in clients)
            for(int i=0; i<c.players.Count; i++)
                if (c.players[i].id == pid)
                {
                    c.players[i].teamNo = team;
                    Broadcast("SEDT|" + c.players[i].id + '|' + c.players[i].name + '|' + c.players[i].number + '|' + c.players[i].image + '|' + c.players[i].teamNo, clients);
                    break;
                 }
    }


}

public class ServerClient
{
    public TcpClient tcp;
    public int id;
    public bool ready;
    public List<LobbyPlayer> players;

    public ServerClient(TcpClient tcp)
    {
        this.tcp = tcp;
    }
}
