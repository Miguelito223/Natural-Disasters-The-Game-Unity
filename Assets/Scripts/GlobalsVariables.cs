using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class GlobalsVariables : MonoBehaviourPunCallbacks
{

    public static GlobalsVariables instance;

    //networking
    public bool IsNetworking = false;
    public List<GameObject> playerlist = new List<GameObject>();
    public int PlayersNumber = 0;
    public string LocalPlayerName = "Player";

    //networking
    public int points = 0;

    //weather
    public float temp_air = 23;
    public int humidity = 25;
    public float wind_speed = 0;
    public int wind_direction = 0;
    public float radiation = 0;
    public float pressure = 1000;
    public int oxygen = 100;

    public float temp_air_lerp = 23;
    public int humidity_lerp = 23;
    public float wind_speed_lerp = 23;
    public int wind_direction_lerp = 23;
    public float radiation_lerp = 0;
    public float pressure_lerp = 1000;
    public int oxygen_lerp = 100;

    public float temp_air_original = 23;
    public int humidity_original = 23;
    public float wind_speed_original = 23;
    public int wind_direction_original = 23;
    public float radiation_original = 0;
    public float pressure_original = 1000;
    public int oxygen_original = 100;

    private void Awake()
    {
        if (GlobalsVariables.instance == null)
        {
            GlobalsVariables.instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Update()
    {
        Atmosphere();
        AtmosphereFadeControl();
    }

    private void Atmosphere()
    {
        temp_air = Mathf.Clamp(temp_air, -273, 273);
        humidity = Mathf.Clamp(humidity, 0, 100);
        radiation = Mathf.Clamp(radiation, 0, 100);
        oxygen = Mathf.Clamp(humidity, 0, 100);
        pressure = Mathf.Clamp(pressure, 0, 10000);
    }
    private void AtmosphereFadeControl()
    {
        temp_air = Mathf.Lerp(0.005f, temp_air, temp_air_lerp);
        humidity = (int)Mathf.Lerp(0.005f, humidity, humidity_lerp);
        wind_speed = Mathf.Lerp(0.005f, wind_speed, wind_speed_lerp);
        wind_direction = (int)Mathf.Lerp(0.005f, wind_direction, wind_direction_lerp);
        radiation = Mathf.Lerp(0.005f, radiation, radiation_lerp);
    }
}
