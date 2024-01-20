using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Server;
using ServerCore;
using System.Collections.Concurrent;

namespace Server
{
    class GameRoom : IJobQueue
    {
        List<ClientSession> _sessions = new List<ClientSession>();
        JobQueue _jobQueue = new JobQueue();
        List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();
        ConcurrentDictionary<string, string> _players = new(); 

        public void Push(Action job)
        {
            _jobQueue.Push(job);
        }

        public void Flush()
        {
            foreach (ClientSession s in _sessions)
                s.Send(_pendingList);

            _pendingList.Clear();
        }

        public void Broadcast(ArraySegment<byte> segment)
        {
            _pendingList.Add(segment);
        }

        public void Enter(ClientSession session)
        {
            _sessions.Add(session);
            session.Room = this;

            S_PlayerList players = new S_PlayerList();
            foreach (ClientSession s in _sessions)
            {
                players.players.Add(new S_PlayerList.Player()
                {
                    isSelf = (s == session),
                    playerId = s.SessionId,
                });
            }
            session.Send(players.Write());

            S_BroadcastEnterGame enter = new S_BroadcastEnterGame();
            enter.playerId = session.SessionId;

            Broadcast(enter.Write());
        }


        public void Leave(ClientSession session)
        {
            // 플레이어 제거하고
            _sessions.Remove(session);

            // 모두에게 알린다
            S_BroadcastLeaveGame leave = new S_BroadcastLeaveGame();
            leave.playerId = session.SessionId;
            Broadcast(leave.Write());
        }

        public void Login(ClientSession clientSession, C_LoginRequest loginPacket)
        {
            S_LoginReply replyPakcet = new S_LoginReply();
            clientSession.Send(replyPakcet.Write());
        }
      
        public void RegisterId(ClientSession clientSession, C_RegisterRequest registerPacket)
        {
            S_RegisterReply replyPacket = new S_RegisterReply();
            replyPacket.answer = false;
            replyPacket.playerId = registerPacket.playerId;

            if (string.IsNullOrEmpty(registerPacket.playerId) ||
                string.IsNullOrEmpty(registerPacket.password))
                return;

            clientSession.Send(replyPacket.Write());
        }
    }
}
