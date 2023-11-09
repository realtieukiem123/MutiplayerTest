#if UNITY_SERVER || UNITY_EDITOR || UNITY_ANDROID
using System;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

public class ServerGameManager : IDisposable
{
    private string serverIP;
    private int serverPort;
    private int queryPort;
    private NetworkServer networkServer;
    private MultiplayAllocationService multiplayAllocationService;
    private const string GameSceneName = "Game";
    public ServerGameManager(string serverIP, int ServerPort, int queryPort, NetworkManager manager)
    {
        this.serverIP = serverIP;
        this.serverPort = ServerPort;
        this.queryPort = queryPort;
        networkServer = new NetworkServer(manager);
        multiplayAllocationService = new MultiplayAllocationService();
    }


    public async Task StartGameServerAsync()
    {
        await multiplayAllocationService.BeginServerCheck();
        if (!networkServer.OpenConnection(serverIP, serverPort))
        {
            Debug.LogWarning("Network didnot start");
            return;
        }
        NetworkManager.Singleton.SceneManager.LoadScene(GameSceneName, LoadSceneMode.Single);
    }

    public void Dispose()
    {
        multiplayAllocationService?.Dispose();
        networkServer?.Dispose();
    }
}
#endif