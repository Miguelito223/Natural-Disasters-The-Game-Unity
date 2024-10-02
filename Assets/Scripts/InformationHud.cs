using UnityEngine;
using TMPro;

public class InformationHud : MonoBehaviour
{
    public TMP_Text information;
    public Map map;
    public GameObject Dirlight;
    public AudioClip alarmClip;
    public AudioSource alarmSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        information.text = "Current Disasters: " + map.CurrentDisasters + "\nTimer for the next disasters: " + map.timer.ToString("f0") + "\nTime: " + GlobalsVariables.instance.ConvertHoursToDegrees(Dirlight.transform.rotation.x).ToString("f0") + ":" + GlobalsVariables.instance.ConvertAngleinDegreesToHours(Dirlight.transform.rotation.x) / 15;
        
        
        if (Mathf.Round((float)map.timer) <= 0)
        {
            alarmSource.PlayOneShot(alarmClip);
        }
    }
}
