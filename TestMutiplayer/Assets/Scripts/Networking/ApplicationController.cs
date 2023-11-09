#if UNITY_SERVER || UNITY_EDITOR || UNITY_ANDROID
using System.Threading.Tasks;
using UnityEngine;

public class ApplicationController : MonoBehaviour
{
    [SerializeField] private ClientSingleton clientPrefab;
    [SerializeField] private HostSingleton hostPrefab;
    [SerializeField] private ServerSingleton serverPrefab;
    private async void Start()
    {
        DontDestroyOnLoad(gameObject);
        await LauchInMode(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null);
    }

    private async Task LauchInMode(bool isDedicatedServer)
    {
        if (isDedicatedServer)
        {
            ServerSingleton serverSingleton = Instantiate(serverPrefab);
            await serverSingleton.CreateServer();
            await serverSingleton.GameManager.StartGameServerAsync();

        }
        else
        {
            HostSingleton hostSingleton = Instantiate(hostPrefab);
            hostSingleton.CreateHost();


            ClientSingleton clientSingleton = Instantiate(clientPrefab);
            bool authenticated = await clientSingleton.CreateClient();

            if (authenticated)
            {

                clientSingleton.GameManager.GoToMenu();
            }
        }
    }

}
#endif