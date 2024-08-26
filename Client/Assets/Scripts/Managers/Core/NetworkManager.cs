using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections;
using UnityEngine.SceneManagement;
using WebSocketSharp;

public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{
    #region Fields
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
    public bool IsEndGameTriggered { get; set; } = false;
    public bool IsTestScene { get; set; } = false;
    public bool IsTutorialScene { get; set; } = false;
    public int RoomPlayersCount { get; private set; }
    public bool IsGameLoading {  get; set; } = false;

    public Queue<Action<GameEndSystem>> EndSystemQueue = new Queue<Action<GameEndSystem>>();

    public Player Player { get; set; }
    public List<Player> AllPlayers
    {
        get
        {
            return Managers.NetworkMng.Runner.GetAllBehaviours<Player>();
        }
    }

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
    public Vector3 TutorialSceneSpawnPosition;
    public Define.SectorName TutorialSceneSpawnSector;
    public Vector3 TestSceneSpawnPosition;
    public Define.SectorName TestSceneSpawnSector;
    #endregion

    #region Init
    public void Init()
    {
        if (Runner == null)
        {
            Runner = Managers.Instance.gameObject.AddComponent<NetworkRunner>();
        }

        IsGameLoading = false;

        ReadySceneSpawnPosition = new Vector3(34, 4, -8);
        ReadySceneSpawnSector = Define.SectorName.Cafeteria;

        TutorialSceneSpawnPosition = new Vector3(42, 0, -28);
        TutorialSceneSpawnSector = Define.SectorName.F1_Corridor_D;

        TestSceneSpawnPosition = new Vector3(1.2f, 0, 0);
        TestSceneSpawnSector = Define.SectorName.F1_Corridor_A;

        RoomPlayersCount = Define.PLAYER_COUNT;

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
    #endregion

    #region Update
    private void Update()
    {
        if (Managers.GameMng.GameEndSystem)
        {
            while (EndSystemQueue.Count > 0)
            {
                var action = EndSystemQueue.Dequeue();
                action.Invoke(Managers.GameMng.GameEndSystem);
            }
        }
    }
    #endregion

    #region Player

    public class PlayerData
    {
        public Define.CreatureType CreatureType;
        public Define.CrewState State;
    }

    public void InitializeCreatureCount()
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
        if (!Runner.IsRunning)
            return null;

        if (Runner.TryGetPlayerObject(playerRef, out NetworkObject player))
            return player.GetComponent<Player>();

        return null;
    }

    public IEnumerator AddPlayer(PlayerRef playerRef)
    {
        SpawnCount++;
        Debug.Log($"Add Player = {SpawnCount} / {Define.PLAYER_COUNT} PlayerRef: {playerRef}");

        Player player;
        while ((player = GetPlayerObject(playerRef)) == null)
        {
            yield return null;
        }

        while (player.CreatureType == Define.CreatureType.None)
        {
            yield return null;
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
            if (IsGameLoading == false)
                OnAlienDropped();
        }
        else if (playerData.CreatureType == Define.CreatureType.Crew)
        {
            CrewPlayerCount--;

            // 게임씬에서 크루가 탈주했을 때 실행
            if (IsMaster && Managers.GameMng.GameEndSystem != null)
                Managers.GameMng.GameEndSystem.OnCrewDropped(playerRef);
        }

        Debug.Log($"Alien Count : {AlienPlayerCount}");
        Debug.Log($"Crew Count : {CrewPlayerCount}");
        _players.Remove(record);
        string str = "";
        _players.ForEach((pair) => str += pair.Second.CreatureType + " ");
        Debug.Log(str + $", Total : {_players.Count}");
    }
    #endregion

    #region Ending
    public void OnAlienDropped()
    {
        if (IsEndGameTriggered || IsTestScene || IsTutorialScene)
            return;

        IsEndGameTriggered = true;

        Managers.UIMng.ClosePanelUI<UI_CameraPanel>();
        Util.ShowCursor();

        Managers.GameMng.GameResult = Define.GameResultType.Disconnected;
        Managers.SceneMng.LoadScene(Define.SceneType.EndingScene);
    }
    #endregion

    #region LobbyScene
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

    public async Task<bool> CreateSession(string name, string password)
    {
        if (name.IsNullOrEmpty())
        {
            int randomInt = UnityEngine.Random.Range(1000, 9999);
            name = "Room-" + randomInt.ToString();
        }

        return await ConnectToSession(name, password);
    }

    public async Task<bool> ConnectToAnySession()
    {
        // 비밀번호가 없는 세션만 찾아서 입장
        List<SessionInfo> enterable = new List<SessionInfo>();
        foreach (var session in Sessions)
        {
            if (!session.Properties.ContainsKey("password") && session.PlayerCount < Define.PLAYER_COUNT)
            {
                enterable.Add(session);
            }
        }

        if (enterable.Count == 0)
        {
            return await CreateSession(null, null);
        }

        int  count = enterable.Count;
        int rand = UnityEngine.Random.Range(0, count);
        string sessionName = enterable[rand].Name;
        return await ConnectToSession(sessionName, null);
    }

    public async Task<bool> ConnectToSession(string sessionName, string password)
    {
        NetworkSceneInfo scene = new NetworkSceneInfo();
        scene.AddSceneRef(Managers.SceneMng.GetSceneRef(Define.SceneType.ReadyScene));
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

        StartGameResult result = await Runner.StartGame(startGameArgs);
        return result.Ok;
    }
    #endregion

    #region TestScene
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

    public void StartTestSharedClient()
    {
        if (TryGetSceneRef(out var sceneRef))
        {
            IsTestScene = true;
            StartCoroutine(StartClient(GameMode.Shared, LoadSceneMode.Additive, sceneRef));
        }
    }

    public void StartTutorialSharedClient()
    {
        Define.SceneType sceneType = Define.SceneType.TutorialScene;
        var sceneRef = Managers.SceneMng.GetSceneRef(sceneType);
        IsTutorialScene = true;
        DefaultRoomName = "Tutorial-" + UnityEngine.Random.Range(0, 9999);
        RoomPlayersCount = 1;
        StartCoroutine(StartClient(GameMode.Shared, LoadSceneMode.Single, sceneRef));
    }

    protected IEnumerator StartClient(GameMode serverMode, LoadSceneMode sceneMode, SceneRef sceneRef = default)
    {
        var clientTask = InitializeNetworkRunner(Runner, serverMode, NetAddress.Any(), sceneRef, null, sceneMode);

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
      LoadSceneMode sceneMode, INetworkRunnerUpdater updater = null)
    {
        Scene = Managers.Instance.gameObject.GetOrAddComponent<NetworkSceneManagerEx>();

        var objectProvider = runner.gameObject.GetOrAddComponent<NetworkObjectProviderDefault>();

        var sceneInfo = new NetworkSceneInfo();
        if (scene.IsValid)
        {
            sceneInfo.AddSceneRef(scene, sceneMode);
        }

        return runner.StartGame(new StartGameArgs
        {
            GameMode = gameMode,
            Address = address,
            Scene = sceneInfo,
            SessionName = DefaultRoomName,
            PlayerCount = RoomPlayersCount,
            OnGameStarted = onGameStarted,
            SceneManager = Scene,
            Updater = updater,
            ObjectProvider = objectProvider,
        });
    }
    #endregion

    void SetSceneSpawnInfo(Define.SceneType sceneType, ref SpawnData spawnData)
    {
        switch (sceneType)
        {
            case Define.SceneType.TutorialScene:
                spawnData.SpawnPoint.Position = TutorialSceneSpawnPosition;
                spawnData.SpawnPoint.SectorName = TutorialSceneSpawnSector;
                spawnData.CreatureType = Define.CreatureType.TutoCrew;
                break;
            case Define.SceneType.ReadyScene:
                spawnData.SpawnPoint.Position = ReadySceneSpawnPosition + new Vector3(UnityEngine.Random.Range(0, 3), 0, UnityEngine.Random.Range(0, 3));
                spawnData.SpawnPoint.SectorName = ReadySceneSpawnSector;
                spawnData.CreatureType = Define.CreatureType.Crew;
                break;
            // 테스트 씬만 사용, 본 게임의 게임 씬은 NetworkSceneManager에서 처리
            case Define.SceneType.GameScene:
                spawnData.SpawnPoint.Position = TestSceneSpawnPosition;
                spawnData.SpawnPoint.SectorName = TestSceneSpawnSector;
                spawnData.CreatureType = Creature;
                break;
        }
    }

    #region CallBack
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        Debug.Log("OnSessionListUpdated");
        Sessions.Clear();
        Sessions = sessionList;
        OnSessionUpdated?.Invoke();
    }

    class SpawnData
    {
        public SpawnPoint.SpawnPointData SpawnPoint;
        public Define.CreatureType CreatureType;
    }

    public async void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (player == runner.LocalPlayer)
        {
            SpawnData spawnData = new SpawnData();
            SetSceneSpawnInfo(Managers.SceneMng.CurrentScene.SceneType, ref spawnData);

            NetworkObject playerObject = null;
            switch (spawnData.CreatureType)
            {
                case Define.CreatureType.Crew:
                    playerObject = await Managers.ObjectMng.SpawnCrew(Define.CREW_CREWA_ID, spawnData.SpawnPoint, isGameScene : false);
                    break;

                case Define.CreatureType.Alien:
                    playerObject = await Managers.ObjectMng.SpawnAlien(Define.ALIEN_STALKER_ID, spawnData.SpawnPoint);
                    break;

                case Define.CreatureType.TutoCrew:
                    playerObject = await Managers.ObjectMng.SpawnCrew(Define.CREW_CREWA_ID, spawnData.SpawnPoint, isGameScene : false, isTutorial : true);
                    break;
            }

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
    #endregion

    #region NotUsed
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
