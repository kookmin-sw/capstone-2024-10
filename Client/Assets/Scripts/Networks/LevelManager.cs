using DG.Tweening.Core.Easing;
using Fusion;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class LevelManager : NetworkSceneManagerDefault
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
            Vector3 position = Vector3.zero;
            GameObject spawnPoint = GameObject.FindWithTag("Respawn");
            if (spawnPoint != null)
            {
                position = spawnPoint.transform.position;
            }

            try
            {
                position = Managers.NetworkMng.PlayerSystem.SpawnPoints.Get(Managers.NetworkMng.Runner.LocalPlayer);
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }

            if (Runner.IsSharedModeMasterClient)
            {
                var players = Managers.NetworkMng.Runner.ActivePlayers.ToList();
                Player.RPC_ChangePlayerToAlien(Managers.NetworkMng.Runner, Runner.LocalPlayer, position, true);
                foreach (var player in players)
                {
                    if(player == Runner.LocalPlayer) continue;
                    Player.RPC_ChangePlayerToAlien(Managers.NetworkMng.Runner, player, position, false);
                }
            }
            //StartCoroutine(((Managers.SceneMng.CurrentScene) as GameScene).OnSceneLoaded());
        }
    }
}
