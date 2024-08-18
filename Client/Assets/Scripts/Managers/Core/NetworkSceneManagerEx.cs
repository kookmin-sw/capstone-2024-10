using Fusion;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkSceneManagerEx : NetworkSceneManagerDefault
{
    private SceneRef _loadedScene = SceneRef.None;
    private BaseScene _baseScene;

    public override void Shutdown()
    {
        _loadedScene = SceneRef.None;
        Managers.Clear();
        base.Shutdown();
    }

    public async void UnloadScene()
    {
        if (Runner.IsSharedModeMasterClient)
            Managers.NetworkMng.CurrentPlayState = PlayerSystem.PlayState.Transition;

        await UnloadScene(_loadedScene);
    }

    protected override IEnumerator UnloadSceneCoroutine(SceneRef prevScene)
    {
        if (Runner.IsSharedModeMasterClient)
            Managers.NetworkMng.CurrentPlayState = PlayerSystem.PlayState.Transition;

        yield return base.UnloadSceneCoroutine(prevScene);
    }

    protected override IEnumerator OnSceneLoaded(SceneRef newScene, Scene loadedScene, NetworkLoadSceneParameters sceneFlags)
    {
        yield return base.OnSceneLoaded(newScene, loadedScene, sceneFlags);

        _loadedScene = newScene;

        if (loadedScene.name == Managers.SceneMng.GetSceneName(Define.SceneType.GameScene))
        {
            SpawnPoint.SpawnPointData spawnPointTemp = GameObject.FindWithTag("Respawn").GetComponent<SpawnPoint>().Data;

            if (Runner.IsSharedModeMasterClient)
            {
                Managers.NetworkMng.CurrentPlayState = PlayerSystem.PlayState.Game;
                var players = Managers.NetworkMng.Runner.ActivePlayers.ToList();

                foreach (var player in players)
                {
              
                    if (!Managers.NetworkMng.PlayerSystem.SpawnPoints.TryGet(player, out SpawnPoint.SpawnPointData spawnPoint))
                    {
                        spawnPoint = spawnPointTemp;
                    }
    
                    // Mater client: alien & Other clients: crew
                    RPC_SpawnPlayer(Managers.NetworkMng.Runner, player, spawnPoint, player == Runner.LocalPlayer);
                }
            }
        }
        else if (loadedScene.name == Managers.SceneMng.GetSceneName(Define.SceneType.ReadyScene))
        {
            if (Runner.IsSharedModeMasterClient)
                Managers.NetworkMng.CurrentPlayState = PlayerSystem.PlayState.Ready;
        }
    }

    [Rpc]
    public static async void RPC_SpawnPlayer(NetworkRunner runner, [RpcTarget] PlayerRef player, SpawnPoint.SpawnPointData spawnPoint, bool isAlien)
    {
        NetworkObject no = isAlien
            ? await Managers.ObjectMng.SpawnAlien(Define.ALIEN_STALKER_ID, spawnPoint)
            : await Managers.ObjectMng.SpawnCrew(Define.CREW_CREWA_ID, spawnPoint, isGameScene : true);
        runner.SetPlayerObject(player, no);
    }
}
