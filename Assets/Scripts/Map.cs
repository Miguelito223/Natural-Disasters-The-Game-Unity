using JetBrains.Annotations;
using Photon.Pun;
using Photon.Realtime;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Map : MonoBehaviourPunCallbacks
{
    public PhotonView PlayerScene;
    public GameObject PlayerScene2;
    public Transform SpawnPoint;


    void Start()
    {
        if (GlobalsVariables.instance.IsNetworking)
        {
            OnJoinedRoom();
        }
        else
        {
            OnSingleplayer();
        }

    }

    public override void OnJoinedRoom()
    {
        GameObject player = PhotonNetwork.Instantiate(PlayerScene.name, SpawnPoint.position, SpawnPoint.rotation);
        player.name = GlobalsVariables.instance.LocalPlayerName;
        player.GetComponent<PhotonView>().RPC("SetNameText", RpcTarget.AllBuffered, player.name);
        GlobalsVariables.instance.playerlist.Add(player);
        GlobalsVariables.instance.PlayersNumber = GlobalsVariables.instance.playerlist.Count;
    }

    public void OnSingleplayer()
    {
        GameObject player = Instantiate(PlayerScene2, SpawnPoint.position, SpawnPoint.rotation);
        player.name = GlobalsVariables.instance.LocalPlayerName;
        player.GetComponent<PlayerName>().SetNameText(player.name);
        GlobalsVariables.instance.playerlist.Add(player);
        GlobalsVariables.instance.PlayersNumber = GlobalsVariables.instance.playerlist.Count;
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}
