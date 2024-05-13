using Fusion;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkSceneManagerEx : NetworkSceneManagerDefault
{
    private SceneRef _loadedScene = SceneRef.None;

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
            SpawnPoint spawnPointTemp = GameObject.FindWithTag("Respawn").GetComponent<SpawnPoint>();

            if (Runner.IsSharedModeMasterClient)
            {
                Managers.NetworkMng.CurrentPlayState = PlayerSystem.PlayState.Game;
                var players = Managers.NetworkMng.Runner.ActivePlayers.ToList();

                foreach (var player in players)
                {
              
                    if (!Managers.NetworkMng.PlayerSystem.SpawnPositions.TryGet(player, out Vector3 spawnPos))
                    {
                        spawnPos = spawnPointTemp.gameObject.transform.position;
                    }
                    if (!Managers.NetworkMng.PlayerSystem.SpawnSectors.TryGet(player, out Define.SectorName spawnSector))
                    {
                        spawnSector = spawnPointTemp.SectorName;
                    }
    
                    // Mater client: alien & Other clients: crew
                    Player.RPC_SpawnPlayer(Managers.NetworkMng.Runner, player, spawnPos, spawnSector, player == Runner.LocalPlayer);
                }
            }
        }
        else if (loadedScene.name == Managers.SceneMng.GetSceneName(Define.SceneType.ReadyScene))
        {
            if (Runner.IsSharedModeMasterClient)
                Managers.NetworkMng.CurrentPlayState = PlayerSystem.PlayState.Ready;
        }
    }
}
