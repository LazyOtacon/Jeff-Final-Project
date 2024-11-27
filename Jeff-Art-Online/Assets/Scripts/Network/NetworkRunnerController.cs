using Fusion;
using Fusion.Addons.Physics;
using Fusion.Sockets;

using System;
using System.Collections.Generic;

using Unity.VisualScripting;

using UnityEngine.SceneManagement;
using UnityEngine;

public class NetworkRunnerController : MonoBehaviour, INetworkRunnerCallbacks
{
    private NetworkRunner runner;

    public Action OnRunnerStartGame;
    public Action OnPlayerJoinSuccess;

    public void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public async void StartGame(GameMode gameMode, string roomName)
    {
        OnRunnerStartGame?.Invoke();

        runner = gameObject.AddComponent<NetworkRunner>();
        RunnerSimulatePhysics2D simulation = runner.AddComponent<RunnerSimulatePhysics2D>();
        simulation.ClientPhysicsSimulation = ClientPhysicsSimulation.SimulateAlways;

        runner.ProvideInput = true;

        StartGameArgs args = new StartGameArgs()
        {
            GameMode = gameMode,
            SessionName = roomName,
            PlayerCount = 16,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        };

        StartGameResult result = await runner.StartGame(args);

        if (result.Ok)
        {
            if (runner.IsSceneAuthority)
            {
                await runner.LoadScene("GameScene", LoadSceneMode.Single);
            }
        }
        else
        {
            Debug.LogError($"NetworkRunnerController: Failed to start game. {result.ShutdownReason}");
        }
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        OnPlayerJoinSuccess?.Invoke();
    }

    public void ShutdownRunner()
    {
        runner.Shutdown();
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        SceneManager.LoadScene("MenuScene");
        Destroy(gameObject);
    }

    #region UNUSED
    public void OnConnectedToServer(NetworkRunner runner)
    {
        // throw new NotImplementedException();
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        // throw new NotImplementedException();
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        // throw new NotImplementedException();
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        // throw new NotImplementedException();
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        // throw new NotImplementedException();
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        // throw new NotImplementedException();
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        //throw new NotImplementedException();
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        // throw new NotImplementedException();
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        // throw new NotImplementedException();
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        // throw new NotImplementedException();
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        // throw new NotImplementedException();
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
        // throw new NotImplementedException();
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
        // throw new NotImplementedException();
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        //throw new NotImplementedException();
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        //throw new NotImplementedException();
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        // throw new NotImplementedException();
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        // throw new NotImplementedException();
    }
    #endregion
}
