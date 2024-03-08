using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{
    public NetworkRunner Runner { get; private set; }
    public string PlayerName { get; private set; }
    public List<SessionInfo> Sessions = new List<SessionInfo>();
    public NetworkObject Player { get; private set; }

    public void Init()
    {

    }

    public void ConnectToLobby(string playerName)
    {
        PlayerName = playerName;

        if (Runner == null)
        {
            Runner = Managers.Instance.gameObject.AddComponent<NetworkRunner>();
        }

        Runner.JoinSessionLobby(SessionLobby.Shared);
    }

    public void CreateSession()
    {
        int randomInt = UnityEngine.Random.Range(1000, 9999);
        string randomSessionName = "Room-" + randomInt.ToString();
        ConnectToSession(randomSessionName);
    }

    public async void ConnectToSession(string sessionName)
    {
        Managers.SceneMng.LoadScene(Define.SceneType.GameScene);

        if (Runner == null)
        {
            Runner = Managers.Instance.gameObject.AddComponent<NetworkRunner>();
        }

        await Runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = sessionName,
            PlayerCount = Define.PLAYER_COUNT,
            SceneManager = Managers.Instance.gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }

    #region CallBack
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        Debug.Log("OnSessionListUpdated");
        Sessions.Clear();
        Sessions = sessionList;
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log("OnConnectedToServer");
        Player = Managers.ObjectMng.SpawnCrew(Define.CREW_CREWA_ID, Vector3.zero);

        runner.SetPlayerObject(runner.LocalPlayer, Player);
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {

    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {

    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {

    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {

    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {

    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {

    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {

    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {

    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {

    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log("OnPlayerJoined");
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {

    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {

    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {

    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {

    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {

    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {

    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {

    }
    #endregion
}
