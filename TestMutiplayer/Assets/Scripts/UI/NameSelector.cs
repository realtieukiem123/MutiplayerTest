using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NameSelector : MonoBehaviour
{
    [SerializeField] TMP_InputField nameField;
    [SerializeField] Button connectButton;
    [SerializeField] int minNameLength = 1;
    [SerializeField] int maxNameLength = 12;
    public const string PlayerNameKey = "PlayerName";

    void Start()
    {

        if (SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            return;
        }
        nameField.text = PlayerPrefs.GetString(PlayerNameKey, string.Empty);
        HandleNameChanged();
    }
    public void HandleNameChanged()
    {
        connectButton.interactable = nameField.text.Length >= minNameLength && nameField.text.Length <= maxNameLength;
    }
    public void Connect()
    {
        PlayerPrefs.SetString(PlayerNameKey, nameField.text);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    //Test
/*    void Startsss()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.inter))
        {
            Permission.RequestUserPermission(Permission.INTERNET);
        }
    }*/

/*    void Update()
    {
        if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
        {
            Debug.Log("use wwifi");
        }
        else if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
        {
            Debug.Log("use 3g");
        }
        else
        {
            Debug.Log("no Internet.");
        }
    }*/

}
