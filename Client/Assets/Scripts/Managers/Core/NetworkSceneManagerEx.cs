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
        await UnloadScene(_loadedScene);
    }

    protected override IEnumerator UnloadSceneCoroutine(SceneRef prevScene)
    {
        yield return base.UnloadSceneCoroutine(prevScene);
    }

    protected override IEnumerator OnSceneLoaded(SceneRef newScene, Scene loadedScene, NetworkLoadSceneParameters sceneFlags)
    {
        yield return base.OnSceneLoaded(newScene, loadedScene, sceneFlags);

        _loadedScene = newScene;

        if (loadedScene.name == Managers.SceneMng.GetSceneName(Define.SceneType.GameScene))
        {
            GameObject spawnPoint = GameObject.FindWithTag("Respawn");

            if (Runner.IsSharedModeMasterClient)
            {
                var players = Managers.NetworkMng.Runner.ActivePlayers.ToList();

                foreach (var player in players)
                {
                    if (!Managers.NetworkMng.PlayerSystem.SpawnPoints.TryGet(player, out Vector3 position))
                    {
                        position = ( spawnPoint != null ? spawnPoint.transform.position : Vector3.zero);
                    }

                    // Mater client: alien & Other clients: crew
                    Player.RPC_SpawnPlayer(Managers.NetworkMng.Runner, player, position, player == Runner.LocalPlayer);
                }
            }
        }
    }
}
