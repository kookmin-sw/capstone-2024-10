using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.Linq;
using System.Collections;
using UnityEngine.SceneManagement;
using WebSocketSharp;
using UnityEditor;
using Unity.VisualScripting;


public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{
    public NetworkRunner Runner { get; private set; }
    public string PlayerName { get; set; }
    public List<SessionInfo> Sessions = new List<SessionInfo>();
    public Action OnSessionUpdated;
    public bool IsMaster { get => Runner.IsSharedModeMasterClient; }
    public NetworkSceneManagerEx Scene { get; set; }

    private List<Pair<PlayerRef, PlayerData>> _players = new();
    public int AlienPlayerCount { get; private set; } = 0;
    public int CrewPlayerCount { get; private set; } = 0;
    public int SpawnCount { get; private set; } = 0;
    public bool IsEndGameTriggered = false;
    public bool IsEndingTriggered { get; set; } = false;

    public int NumPlayers
    {
        get
        {
            if (Runner != null)
                return Runner.ActivePlayers.Count();
            return 0;
        }
    }

    public PlayerSystem PlayerSystem { get; set; }

    public PlayerSystem.PlayState CurrentPlayState
    {
        get
        {
            if (PlayerSystem == null)
                return PlayerSystem.PlayState.None;

            return PlayerSystem.CurrentPlayState;
        }
        set
        {
            if (PlayerSystem != null)
                PlayerSystem.CurrentPlayState = value;
        }
    }

    public string DefaultRoomName;
    public Define.CreatureType Creature;
    public int PrefabNum;
    public Vector3 ReadySceneSpawnPosition;
    public Define.SectorName ReadySceneSpawnSector;

    public void Init()
    {
        if (Runner == null)
        {
            Runner = Managers.Instance.gameObject.AddComponent<NetworkRunner>();
        }

        if (ReadySceneSpawnPosition == default)
            ReadySceneSpawnPosition = new Vector3(34, 4, -8);
        if (ReadySceneSpawnSector == default)
            ReadySceneSpawnSector = Define.SectorName.Cafeteria;
        if (Creature == default)
            Creature = Define.CreatureType.Crew;

        StartCoroutine(WaitForInit());
    }

    public IEnumerator WaitForInit()
    {
        while (PlayerSystem == null)
        {
            PlayerSystem = FindAnyObjectByType<PlayerSystem>();
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void CalculatePlayerCreatures()
    {
        AlienPlayerCount = 0;
        CrewPlayerCount = 0;

        for (int i = 0; i < _players.Count; i++)
        {
            Player player = GetPlayerObject(_players[i].First);

            if (player == null)
            {
                Debug.LogWarning("Can't get the player object from the playerRef");
                return;
            }

            if (player.IsMaster)
                _players[i].Second.CreatureType = Define.CreatureType.Alien;

            if (_players[i].Second.CreatureType == Define.CreatureType.Alien)
                AlienPlayerCount++;
            else
                CrewPlayerCount++;
        }

        Debug.Log($"Alien: {AlienPlayerCount} Crew: {CrewPlayerCount}");
        string str = "";
        _players.ForEach((tuple) => str += tuple.Second.CreatureType + " ");
        Debug.Log(str + $", Total : {_players.Count}");
    }

    public PlayerData GetPlayerData(PlayerRef playerRef)
    {
        var record = _players.FirstOrDefault((pair) => pair.First == playerRef);
        if (record == null)
            return default;

        return record.Second;
    }

    public Player GetPlayerObject(PlayerRef playerRef)
    {
        if (Runner.TryGetPlayerObject(playerRef, out NetworkObject player))
            return player.GetComponent<Player>();

        return null;
    }

    public class PlayerData
    {
        public Define.CreatureType CreatureType;
        public Define.CrewState State;
    }

    public IEnumerator AddPlayer(PlayerRef playerRef)
    {
        SpawnCount++;
        Debug.Log($"Add Player = {SpawnCount} / {Define.PLAYER_COUNT} PlayerRef: {playerRef}");

        Player player;
        while ((player = GetPlayerObject(playerRef)) == null)
        {
            yield return new WaitForSeconds(0.5f);
        }

        while (player.CreatureType == Define.CreatureType.None)
        {
            yield return new WaitForSeconds(0.5f);
        }

        if (player.CreatureType == Define.CreatureType.Alien)
            AlienPlayerCount++;
        else
            CrewPlayerCount++;

        _players.Add(new (playerRef, new PlayerData { CreatureType = player.CreatureType, State = Define.CrewState.Alive }));

        string str = "";
        _players.ForEach((tuple) => str += tuple.Second.CreatureType + " ");
        Debug.Log(str);
    }

    public void RemovePlayer(PlayerRef playerRef)
    {
        SpawnCount--;
        Debug.Log($"Remove Player {SpawnCount} / {Define.PLAYER_COUNT} PlayerRef: {playerRef}");

        var record = _players.FirstOrDefault((pair) => pair.First == playerRef);
        var playerData = record.Second;
        Debug.Log($"{playerData.CreatureType} Disconnected");
        if (playerData.CreatureType == Define.CreatureType.Alien)
        {
            AlienPlayerCount--;
            OnAlienDropped();
        }
        else
        {
            CrewPlayerCount--;
            if (Managers.GameMng.GameEndSystem != null)
                Managers.GameMng.GameEndSystem.RPC_OnCrewDropped(playerRef);
        }

        Debug.Log($"Alien Count : {AlienPlayerCount}");
        Debug.Log($"Crew Count : {CrewPlayerCount}");
        _players.Remove(record);
        string str = "";
        _players.ForEach((pair) => str += pair.Second.CreatureType + " ");
        Debug.Log(str + $", Total : {_players.Count}");
    }

    private void ShowCrewEnding()
    {
        Util.ClearUIAndSound();
        Managers.SoundMng.Play($"{Define.BGM_PATH}/Panic Man", Define.SoundType.Bgm, volume: 0.8f);
        Managers.UIMng.ShowPopupUI<UI_CrewWin>();
    }

    public async void OnAlienDropped()
    {
        if (Managers.NetworkMng.IsEndGameTriggered || IsEndingTriggered)
            return;

        Player player = null;
        while ((player = GetPlayerObject(Runner.LocalPlayer)) == null)
        {
            await Task.Delay(500);
        }

        // 로딩 중간에 에일리언 탈주 시, 에일리언이 바뀔 수 있음
        if (player != null && player.CreatureType == Define.CreatureType.Crew)
        {
            StartCoroutine(Managers.ObjectMng.MyCrew.OnWin());
        }
        else
        {
            // Crew에 대한 종속성 없이 UI_CrewWin 호출
            ShowCrewEnding();
        }

        IsEndingTriggered = true;
    }

    public async void ExitGame()
    {
        await Runner.Shutdown();
        Managers.SceneMng.LoadScene(Define.SceneType.LobbyScene);
    }

    public void ConnectToLobby(string playerName)
    {
        PlayerName = playerName;

        Runner.JoinSessionLobby(SessionLobby.Shared);
    }

    public void CreateSession(string name, string password)
    {
        if (name.IsNullOrEmpty())
        {
            int randomInt = UnityEngine.Random.Range(1000, 9999);
            name = "Room-" + randomInt.ToString();
        }

        ConnectToSession(name, password);
    }

    public void ConnectToAnySession()
    {
        // 비밀번호가 없는 세션만 찾아서 입장
        List<SessionInfo> enterable = new List<SessionInfo>();
        foreach (var session in Sessions)
        {
            if (!session.Properties.ContainsKey("password"))
            {
                enterable.Add(session);
            }
        }

        if (enterable.Count == 0)
        {
            CreateSession(null, null);
            return;
        }

        int  count = enterable.Count;
        int rand = UnityEngine.Random.Range(0, count);
        string sessionName = enterable[rand].Name;
        ConnectToSession(sessionName, null);
    }

    public async void ConnectToSession(string sessionName, string password)
    {
        NetworkSceneInfo scene = new NetworkSceneInfo();
        scene.AddSceneRef(Managers.SceneMng.GetSceneRef(Define.SceneType.ReadyScene));
        Managers.Clear();
        Scene = Managers.Instance.gameObject.GetOrAddComponent<NetworkSceneManagerEx>();

        StartGameArgs startGameArgs = new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = sessionName,
            PlayerCount = Define.PLAYER_COUNT,
            SceneManager = Scene,
            Scene = scene
        };

        if (!password.IsNullOrEmpty())
        {
            startGameArgs.SessionProperties = new Dictionary<string, SessionProperty>()
            {
                {"password", password}
            };
        }

        await Runner.StartGame(startGameArgs);
    }

    private bool TryGetSceneRef(out SceneRef sceneRef)
    {
        var activeScene = SceneManager.GetActiveScene();
        if (activeScene.buildIndex < 0 || activeScene.buildIndex >= SceneManager.sceneCountInBuildSettings)
        {
            sceneRef = default;
            return false;
        }
        else
        {
            sceneRef = SceneRef.FromIndex(activeScene.buildIndex);
            return true;
        }
    }

    public void StartSharedClient()
    {
        if (TryGetSceneRef(out var sceneRef))
        {
            StartCoroutine(StartClient(GameMode.Shared, sceneRef));
        }
    }

    protected IEnumerator StartClient(GameMode serverMode, SceneRef sceneRef = default)
    {
        var clientTask = InitializeNetworkRunner(Runner, serverMode, NetAddress.Any(), sceneRef, null);
        IsEndGameTriggered = true;

        while (clientTask.IsCompleted == false)
        {
            yield return new WaitForSeconds(1f);
        }

        if (clientTask.IsFaulted)
        {
            Debug.LogWarning(clientTask.Exception);
        }

        Runner.SessionInfo.IsVisible = false;
    }

    protected Task InitializeNetworkRunner(NetworkRunner runner, GameMode gameMode, NetAddress address, SceneRef scene, Action<NetworkRunner> onGameStarted,
      INetworkRunnerUpdater updater = null)
    {
        Scene = Managers.Instance.gameObject.GetOrAddComponent<NetworkSceneManagerEx>();

        var objectProvider = runner.GetComponent<INetworkObjectProvider>();
        if (objectProvider == null)
        {
            objectProvider = runner.gameObject.AddComponent<NetworkObjectProviderDefault>();
        }

        var sceneInfo = new NetworkSceneInfo();
        if (scene.IsValid)
        {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }

        return runner.StartGame(new StartGameArgs
        {
            GameMode = gameMode,
            Address = address,
            Scene = sceneInfo,
            SessionName = DefaultRoomName,
            OnGameStarted = onGameStarted,
            SceneManager = Scene,
            Updater = updater,
            ObjectProvider = objectProvider,
        });
    }


    #region CallBack
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        Debug.Log("OnSessionListUpdated");
        Sessions.Clear();
        Sessions = sessionList;
        OnSessionUpdated?.Invoke();
    }


    public async void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (player == runner.LocalPlayer)
        {
            ReadySceneSpawnPosition += new Vector3(UnityEngine.Random.Range(0, 3), 0, UnityEngine.Random.Range(0, 3));
            SpawnPoint.SpawnPointData spawnPoint = new()
            {
                Position = ReadySceneSpawnPosition,
                SectorName = ReadySceneSpawnSector
            };
            NetworkObject playerObject = Creature == Define.CreatureType.Crew ?
                await Managers.ObjectMng.SpawnCrew(Define.CREW_CREWA_ID, spawnPoint, false) :
                await Managers.ObjectMng.SpawnAlien(Define.ALIEN_STALKER_ID, spawnPoint);

            runner.SetPlayerObject(runner.LocalPlayer, playerObject);

            if (Runner.IsSharedModeMasterClient)
            {
                NetworkObject prefab = Managers.ResourceMng.Load<NetworkObject>($"Prefabs/Systems/@PlayerSystem");
                NetworkObject no = Managers.NetworkMng.Runner.Spawn(prefab, Vector3.zero);

                PlayerSystem = no.GetComponent<PlayerSystem>();
            }
        }

        StartCoroutine(AddPlayer(player));
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log("OnPlayerLeft");
        RemovePlayer(player);
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log("OnConnectedToServer");
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        Debug.Log("OnSceneLoadDone");
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
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

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {

    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
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

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {

    }
    #endregion
}
