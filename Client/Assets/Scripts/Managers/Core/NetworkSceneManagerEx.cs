using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class NetworkSceneManagerEx : NetworkSceneManagerDefault
{
    private SceneRef _loadedScene = SceneRef.None;
    private BaseScene _baseScene;
    private Player _player;

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
        yield return base.UnloadSceneCoroutine(prevScene);
    }

    protected override IEnumerator OnSceneLoaded(SceneRef newScene, Scene loadedScene, NetworkLoadSceneParameters sceneFlags)
    {
        yield return base.OnSceneLoaded(newScene, loadedScene, sceneFlags);

        _loadedScene = newScene;
        _player = null;

        if (loadedScene.name == Managers.SceneMng.GetSceneName(Define.SceneType.GameScene))
        {
            #region GameScene
            SpawnPoint.SpawnPointData spawnPointTemp = GameObject.FindWithTag("Respawn").GetComponent<SpawnPoint>().Data;
            var players = Managers.NetworkMng.Runner.ActivePlayers.ToList();

            if (Runner.IsSharedModeMasterClient)
            {
                Managers.NetworkMng.CurrentPlayState = PlayerSystem.PlayState.Game;

                foreach (var player in players)
                {
                    if (!Managers.NetworkMng.PlayerSystem.SpawnPoints.TryGet(player, out SpawnPoint.SpawnPointData spawnPoint))
                    {
                        spawnPoint = spawnPointTemp;
                    }

                    // Mater client: alien & Other clients: crew
                    Player.RPC_SpawnPlayer(Managers.NetworkMng.Runner, player, spawnPoint, player == Runner.LocalPlayer);
                }
            }

            yield return new WaitUntil(() => OtherPlayerLoaded(players));

            // Wait for loading MonoBehaviour
            while (Managers.SceneMng.CurrentScene is not GameScene)
            {
                yield return new WaitForSeconds(0.5f);
            }

            _baseScene = Managers.SceneMng.CurrentScene;
            StartCoroutine(_baseScene.OnPlayerSpawn());

            yield return new WaitUntil(() => Managers.GameMng.GameEndSystem && Managers.GameMng.GameEndSystem.AreAllPlayersLoaded);

            Managers.UIMng.OnLoadingUIDown();
            #endregion
        }
        else if (loadedScene.name == Managers.SceneMng.GetSceneName(Define.SceneType.ReadyScene))
        {
            #region ReadyScene
            if (Runner.IsSharedModeMasterClient)
            {
                Managers.NetworkMng.CurrentPlayState = PlayerSystem.PlayState.Ready;
            }

            yield return new WaitUntil(() => MyPlayerLoaded());

            yield return new WaitUntil(() => Managers.NetworkMng.PlayerSystem && Managers.NetworkMng.PlayerSystem.IsFirstLoadCompleted);

            // Wait for loading MonoBehaviour
            while (Managers.SceneMng.CurrentScene is not ReadyScene)
            {
                yield return new WaitForSeconds(0.5f);
            }

            _baseScene = Managers.SceneMng.CurrentScene;
            StartCoroutine(_baseScene.OnPlayerSpawn());

            Managers.UIMng.OnLoadingUIDown();
            #endregion
        }
        else if (loadedScene.name == Managers.SceneMng.GetSceneName(Define.SceneType.TutorialScene))
        {
            #region TutorialScene

            yield return new WaitUntil(() => MyPlayerLoaded());

            // Wait for loading MonoBehaviour
            while (Managers.SceneMng.CurrentScene is not TutorialScene)
            {
                yield return new WaitForSeconds(0.5f);
            }

            _baseScene = Managers.SceneMng.CurrentScene;
            StartCoroutine(_baseScene.OnPlayerSpawn());

            Managers.UIMng.OnLoadingUIDown();
            #endregion
        }
    }

    public bool MyPlayerLoaded()
    {
        _player = Managers.NetworkMng.GetPlayerObject(Runner.LocalPlayer);
        if (_player == null)
            return false;

        if (_player.IsSpawned == false)
            return false;

        return true;
    }

    public bool OtherPlayerLoaded(List<PlayerRef> players)
    {
        foreach (var player in players)
        {
            var po = Managers.NetworkMng.GetPlayerObject(player);
            if (po == null)
                return false;

            if (po.AreAllPlayersSpawned == false)
                return false;
        }

        return true;
    }
}
