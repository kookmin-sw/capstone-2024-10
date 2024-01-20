using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class PacketHandler
{
    public static void S_BroadcastEnterGameHandler(PacketSession session, IPacket packet)
    {
        S_BroadcastEnterGame pkt = packet as S_BroadcastEnterGame;
        ServerSession serverSession = session as ServerSession;

        PlayerManager.Instance.EnterGame(pkt);
    }

    public static void S_BroadcastLeaveGameHandler(PacketSession session, IPacket packet)
    {
        S_BroadcastLeaveGame pkt = packet as S_BroadcastLeaveGame;
        ServerSession serverSession = session as ServerSession;

        PlayerManager.Instance.LeaveGame(pkt);
    }

    public static void S_PlayerListHandler(PacketSession session, IPacket packet)
    {
        S_PlayerList pkt = packet as S_PlayerList;
        ServerSession serverSession = session as ServerSession;

        PlayerManager.Instance.Add(pkt);
    }

    public static void S_LoginReplyHandler(PacketSession session, IPacket packet)
    {
        S_LoginReply pkt = packet as S_LoginReply;
        ServerSession serverSession = session as ServerSession;

        PlayerManager.Instance.SignIn(pkt);
    }

    public static void S_RegisterReplyHandler(PacketSession session, IPacket packet)
    {
        S_RegisterReply pkt = packet as S_RegisterReply;
        ServerSession serverSession = session as ServerSession;

        PlayerManager.Instance.SignUp(pkt);
    }
}
