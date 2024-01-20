using Server;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class PacketHandler
{
    public static void C_LeaveGameHandler(PacketSession session, IPacket packet)
    {
        ClientSession clientSession = session as ClientSession;

        if (clientSession.Room == null)
            return;

        GameRoom room = clientSession.Room;
        room.Push(() => room.Leave(clientSession));
    }

    public static void C_LoginRequestHandler(PacketSession session, IPacket packet)
    {
        C_LoginRequest loginPacket = packet as C_LoginRequest;
        ClientSession clientSession = session as ClientSession;

        if (clientSession.Room == null)
            return;

        GameRoom room = clientSession.Room;
        room.Push(() => room.Login(clientSession, loginPacket));
    }
    public static void C_GetFishListHandler(PacketSession session, IPacket packet)
    {
        C_GetFishList fishListPacket = packet as C_GetFishList;
        ClientSession clientSession = session as ClientSession;

        if (clientSession.Room == null)
            return;

        GameRoom room = clientSession.Room;
        room.Push(() => room.GetRandomFishList(clientSession));
    }

    internal static void C_RegisterRequestHandler(PacketSession session, IPacket packet)
    {
        C_RegisterRequest registerPacket = packet as C_RegisterRequest;
        ClientSession clientSession = session as ClientSession;

        if (clientSession.Room == null)
            return;

        GameRoom room = clientSession.Room;
        room.Push(() => room.RegisterId(clientSession, registerPacket));
    }

    internal static void C_SyncFishChangeHandler(PacketSession session, IPacket packet)
    {
        C_SyncFishChange syncPacket = packet as C_SyncFishChange;
        ClientSession clientSession = session as ClientSession;

        if (clientSession.Room == null)
            return;

        Console.WriteLine($"{syncPacket.playerId} : {syncPacket.objId} to {syncPacket.size}");

        GameRoom room = clientSession.Room;
        room.Push(() => room.SyncFishChange(clientSession, syncPacket));
    }

    internal static void C_RegisterFishListHandler(PacketSession session, IPacket packet)
    {
        C_RegisterFishList regPacket = packet as C_RegisterFishList;
        ClientSession clientSession = session as ClientSession;

        if (clientSession.Room == null)
            return;

        GameRoom room = clientSession.Room;
        room.Push(() => room.RegisterFishList(clientSession, regPacket));
    }
}
