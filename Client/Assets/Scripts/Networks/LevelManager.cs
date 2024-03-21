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

        if (loadedScene.name == Managers.SceneMng.GetSceneName(Define.SceneType.GameScene))
        {
            Vector3 position = Vector3.zero;
            Transform spawnPoint = GameObject.FindWithTag("Respawn").transform;
            if (spawnPoint != null)
            {
                position = spawnPoint.position;
            }

            NetworkObject playerObject = Managers.ObjectMng.SpawnCrew(Define.CREW_CREWA_ID, position);
            Managers.NetworkMng.Runner.SetPlayerObject(Managers.NetworkMng.Runner.LocalPlayer, playerObject);

            if (Runner.IsSharedModeMasterClient)
            {
                NetworkObject prefab = Managers.ResourceMng.Load<NetworkObject>($"Prefabs/Etc/PlayerSystem");
                NetworkObject no = Managers.NetworkMng.Runner.Spawn(prefab, Vector3.zero);
                Managers.NetworkMng.PlayerSystem = no.GetComponent<PlayerSystem>();

                var players = Managers.NetworkMng.Runner.ActivePlayers.ToList();
                int random = Random.Range(0, players.Count);
                Player.RPC_ChangePlayerToAlien(Managers.NetworkMng.Runner, players[random], Define.ALIEN_STALKER_ID);
            }
        }
    }
}
