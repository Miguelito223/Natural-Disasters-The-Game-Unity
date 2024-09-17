using Photon.Pun;
using TMPro;
using UnityEngine;

public class PlayerName : MonoBehaviourPunCallbacks
{
    public TMP_Text playername;

    [PunRPC]
    public void SetNameText(string name)
    {
        playername.text = name;
    }
}
