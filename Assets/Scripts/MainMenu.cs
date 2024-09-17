using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviourPunCallbacks
{
    
    public TMP_InputField username;
    public TMP_Text button;

    public void PlayMultiplayer()
    {

        GlobalsVariables.instance.IsNetworking = true;
        GlobalsVariables.instance.LocalPlayerName = username.text;
        PhotonNetwork.NickName = GlobalsVariables.instance.LocalPlayerName;
        button.text = "Connecting...";
        PhotonNetwork.ConnectUsingSettings();
    }

    public void PlaySingleplayer()
    {
        GlobalsVariables.instance.IsNetworking = false;
        GlobalsVariables.instance.LocalPlayerName = username.text;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public override void OnConnectedToMaster()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
