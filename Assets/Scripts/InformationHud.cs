using UnityEngine;
using TMPro;
using Photon.Pun;

public class InformationHud : MonoBehaviourPunCallbacks
{
    public TMP_Text information;
    public AudioClip alarmClip;
    public AudioSource alarmSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

       

        if (GlobalsVariables.instance.IsNetworking)
            gameObject.SetActive(GlobalsVariables.instance.LocalPlayer.photonView.IsMine);
            if (!GlobalsVariables.instance.LocalPlayer.photonView.IsMine)
                    return;
    }

    // Update is called once per frame
    void Update()
    {
        if (GlobalsVariables.instance.IsNetworking)
            if (!GlobalsVariables.instance.LocalPlayer.photonView.IsMine)
                return;

        if (alarmClip == null)
            return;

        information.text = "Current Disasters: " + GlobalsVariables.instance.map.CurrentDisasters + "\nTimer for the next disasters: " + GlobalsVariables.instance.map.timer.ToString("f0") + "\nTime: " + GlobalsVariables.instance.SunDirLight.hours.ToString("f0") + ":" + GlobalsVariables.instance.SunDirLight.minutes.ToString("f0");
        
        
        if (GlobalsVariables.instance.map.timer <= 0.0f)
        {
            alarmSource.PlayOneShot(alarmClip);
        }
    }
}
