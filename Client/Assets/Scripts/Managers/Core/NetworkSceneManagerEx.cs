using DG.Tweening;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class NetworkSceneManagerEx : NetworkSceneManagerDefault
{
    private SceneRef _loadedScene = SceneRef.None;
    private BaseScene _baseScene;
    private Player _player;
    private bool _timeOut;

    public override void Shutdown()
    {
        _loadedScene = SceneRef.None;
        Managers.Clear();
        base.Shutdown();
        StopAllCoroutines();
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

            yield return StartCoroutine(CheckTime(WaitLoadingScene<GameScene>(), this));
            if (_timeOut)
                yield break;

            _baseScene = Managers.SceneMng.CurrentScene;
            yield return StartCoroutine(CheckTime(_baseScene.OnPlayerSpawn(), this));
            if (_timeOut)
                yield break;

            yield return StartCoroutine(CheckTime(WaitLoadingComplete(), this));
            if (_timeOut)
                yield break;

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

            yield return StartCoroutine(CheckTime(WaitLoadingScene<ReadyScene>(), this));
            if (_timeOut)
                yield break;

            _baseScene = Managers.SceneMng.CurrentScene;
            yield return StartCoroutine(CheckTime(_baseScene.OnPlayerSpawn(), this));
            if (_timeOut)
                yield break;

            Managers.UIMng.OnLoadingUIDown();
            #endregion
        }
        else if (loadedScene.name == Managers.SceneMng.GetSceneName(Define.SceneType.TutorialScene))
        {
            #region TutorialScene

            yield return new WaitUntil(() => MyPlayerLoaded());

            yield return StartCoroutine(CheckTime(WaitLoadingScene<TutorialScene>(), this));
            if (_timeOut)
                yield break;

            _baseScene = Managers.SceneMng.CurrentScene;
            yield return StartCoroutine(CheckTime(_baseScene.OnPlayerSpawn(), this));
            if (_timeOut)
                yield break;

            Managers.UIMng.OnLoadingUIDown();
            #endregion
        }
    }

    public IEnumerator CheckTime(IEnumerator coroutine, MonoBehaviour owner)
    {
        var wrapper = new CoroutineWrapper(coroutine, owner);
        _timeOut = false;
        wrapper.Start();
        yield return new WaitUntil(() => {
            if (wrapper.ElapsedTime > Define.LOADING_WAIT_TIME)
            {
                Managers.UIMng.OnLoadingUIDown();
                Managers.NetworkMng.OnAlienDropped();
                _timeOut = true;
                wrapper.Stop();
            }

            return wrapper.IsCompleted;
        });
    }

    public IEnumerator WaitLoadingComplete()
    {
        while (!Managers.GameMng.GameEndSystem || !Managers.GameMng.GameEndSystem.AreAllPlayersLoaded)
        {
            yield return new WaitForSeconds(0.5f);
        }
    }

    public IEnumerator WaitLoadingScene<T>() where T : BaseScene
    {
        while (Managers.SceneMng.CurrentScene is not T)
        {
            yield return new WaitForSeconds(0.5f);
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
