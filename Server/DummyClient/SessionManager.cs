using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DummyClient
{
    class SessionManager
    {
        static SessionManager _session = new SessionManager();
        public static SessionManager Instance { get { return _session; } }

        List<ServerSession> _sessions = new List<ServerSession>();
        object _lock = new object();
        Random _rand = new Random();

        public void SendForEach()
        {
            lock (_lock)
            {
                foreach (ServerSession session in _sessions)
                {
                    // TODO: for test
                }
            }
        }

        public void SendLoginReq()
        {
            lock (_lock)
            {
                int idx = 1;
                foreach (ServerSession session in _sessions)
                {
                    var packet = new C_LoginRequest 
                    { 
                        playerId = $"user{idx++}",
                        password = "1234",
                    };
                    session.Send(packet.Write());
                }
            }
        }

        public void SendRegisterReq()
        {
            lock (_lock)
            {
                int idx = 1;
                foreach (ServerSession session in _sessions)
                {
                    var packet = new C_RegisterRequest 
                    { 
                        playerId = $"user{idx++}",
                        password = "1234",
                    };
                    session.Send(packet.Write());
                }
            }
        }

        public ServerSession Generate()
        {
            lock (_lock)
            {
                ServerSession session = new ServerSession();
                _sessions.Add(session);
                return session;
            }
        }
    }
}
