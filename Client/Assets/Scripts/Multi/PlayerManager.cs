using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerManager
{
    MyPlayer _myPlayer;
    Dictionary<int, Player> _players = new();

    public static PlayerManager Instance { get; } = new();

    public Dictionary<ushort, System.Action> UserHandler = new();

    public void Add(S_PlayerList packet)
    {
        GameObject go = new GameObject { name = "@Players" };

        foreach (S_PlayerList.Player p in packet.players)
        {
            GameObject child = new GameObject { name = "user" };

            if (p.isSelf)
            {
                MyPlayer myPlayer = child.AddComponent<MyPlayer>();
                myPlayer.PlayerId = p.playerId;
                _myPlayer = myPlayer;
            }
            else
            {
                Player player = child.AddComponent<Player>();
                player.PlayerId = p.playerId;
                _players.Add(p.playerId, player);
            }

            child.transform.parent = go.transform;
        }
    }

    public void EnterGame(S_BroadcastEnterGame packet)
    {
        if (packet.playerId == _myPlayer.PlayerId)
            return;

        GameObject go = new GameObject { name = "user" };

        Player player = go.AddComponent<Player>();
        _players.Add(packet.playerId, player);
    }

    public void LeaveGame(S_BroadcastLeaveGame packet)
    {
        if (_myPlayer.PlayerId == packet.playerId)
        {
            GameObject.Destroy(_myPlayer.gameObject);
            _myPlayer = null;
            var player = Managers.Game.GetPlayer();
        }
        else
        {
            Player player = null;
            if (_players.TryGetValue(packet.playerId, out player))
            {
                GameObject.Destroy(player.gameObject);
                _players.Remove(packet.playerId);
            }
        }
    }

    public void SignIn(S_LoginReply packet)
    {
        string str = "";
        str += $"Login {packet.playerId} : ";
        str += (packet.answer) ? "success" : "fail";
        if (packet.answer)
        {
            Managers.Game.SaveData.playerId = packet.playerId;
            var player = Managers.Game.GetPlayer();
        }
    }

    public void SignUp(S_RegisterReply packet)
    {
        string str = "";
        str += $"Register {packet.playerId} : ";
        str += (packet.answer) ? "success" : "fail";
        if (packet.answer)
        {
            Managers.Game.SaveData.playerId = packet.playerId;
            var player = Managers.Game.GetPlayer();
        }
    }
}
