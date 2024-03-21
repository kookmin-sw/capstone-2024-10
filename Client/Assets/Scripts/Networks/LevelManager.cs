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
        if (_loadedScene.IsValid)
        {
            SceneManager.UnloadSceneAsync(_loadedScene.AsIndex);
            _loadedScene = SceneRef.None;
        }
        base.Shutdown();
    }

    protected override IEnumerator UnloadSceneCoroutine(SceneRef prevScene)
    {
        yield return base.UnloadSceneCoroutine(prevScene);
    }

    protected override IEnumerator OnSceneLoaded(SceneRef newScene, Scene loadedScene, NetworkLoadSceneParameters sceneFlags)
    {
        yield return base.OnSceneLoaded(newScene, loadedScene, sceneFlags);

        var session = loadedScene.FindComponent<PlayerSystem>();
        Managers.NetworkMng.PlayerSystem = session;

        if (loadedScene.name == Managers.SceneMng.GetSceneName(Define.SceneType.GameScene))
        {
            session.PlayerJoined();

            yield return new WaitForSeconds(3);

            var players = Managers.NetworkMng.Runner.ActivePlayers.ToList();
            int random = Random.Range(0, players.Count);
            Player.RPC_ChangePlayerToAlien(Managers.NetworkMng.Runner, players[random], Define.ALIEN_STALKER_ID);
        }
    }
}
