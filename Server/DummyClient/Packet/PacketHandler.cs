using DummyClient;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class PacketHandler
{
    public static void S_BroadcastEnterGameHandler(PacketSession session, IPacket packet)
    {
    }

    public static void S_BroadcastLeaveGameHandler(PacketSession session, IPacket packet)
    {
    }

    public static void S_PlayerListHandler(PacketSession session, IPacket packet)
    {
    }

    public static void S_BroadcastMoveHandler(PacketSession session, IPacket packet)
    {
    }

    public static void S_LoginReplyHandler(PacketSession session, IPacket packet)
    {
        S_LoginReply spkt = packet as S_LoginReply;
        ServerSession serverSession = session as ServerSession;
        if (spkt.answer)
        {
            serverSession.PlayerID = spkt.playerId;
        }

    }

    public static void S_RegisterRequestHandler(PacketSession session, IPacket packet)
    {
    }

    public static void S_RegisterReplyHandler(PacketSession session, IPacket packet)
    {
    }

    public static void S_SendFishListHandler(PacketSession session, IPacket packet)
    {
    }

    public static void S_BroadcastChangeHandler(PacketSession session, IPacket packet)
    {
        /*
        S_BroadcastChange spkt = packet as S_BroadcastChange;
        ServerSession serverSession = session as ServerSession;
        if (serverSession.PlayerID == spkt.senderId)
            return;

        foreach (var fish in serverSession.Aquarium)
        {
            float tmp = fish.size + spkt.size;
            if (tmp > 0.5 && tmp < 2.0)
                fish.size = tmp;
        }
        */
    }
}
