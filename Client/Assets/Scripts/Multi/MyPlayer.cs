using Data;
using ServerCore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPlayer : Player
{
    NetworkManager _network;
    public string _nickname;

    void Start()
    {
        StartCoroutine("CoSendPacket");
        _network = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
    }

    void Update()
    {
        
    }

    IEnumerator CoSendPacket()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.25f);
        }
    }

    public void Login(string playerId, string password)
    {
        C_LoginRequest reqPacket = new C_LoginRequest();
        reqPacket.playerId = playerId;
        reqPacket.password = password;
        Debug.Log("LoginRequset");
        _network.Send(reqPacket.Write());
    }

    public void Register(string playerId, string password)
    {
        C_RegisterRequest reqPacket = new C_RegisterRequest();
        reqPacket.playerId = playerId;
        reqPacket.password = password;
        Debug.Log("RegisterRequset");
        _network.Send(reqPacket.Write());
    }
}
