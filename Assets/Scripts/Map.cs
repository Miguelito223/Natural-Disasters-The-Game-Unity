using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using ExitGames.Client.Photon;

public class Map : MonoBehaviourPunCallbacks
{
    public PhotonView PlayerScene;
    public GameObject PlayerScene2;
    public Transform SpawnPosition;
    public GameObject MeteorPrefab;
    public GameObject VolcanoPrefab;
    public GameObject TornadoPrefab;
    public GameObject TsunamiPrefab;

    public static int MaxValue = 10;
    public static int MinValue = 0;
    public double starttime = 180f;

    public string CurrentDisasters = "sun";
    public int CurrentDisastersInt = 0;
    public double timer;

    Hashtable CustomeValue;


    void Start()
    {
        GlobalsVariables.instance.map = this;

        if (GlobalsVariables.instance.IsNetworking)
        {
            PhotonNetwork.JoinRandomOrCreateRoom();
            timer = starttime;
        }
        else
        {
            OnSingleplayer();
        }

    }

    void Update()
    {
        foreach (Transform child in transform)
        {
            GameObject obj = child.gameObject;
            GlobalsVariables.instance.Wind(obj);
        }

        if (GlobalsVariables.instance.IsNetworking) 
        { 
            timer -= Time.deltaTime;

            if (timer <= 0.0f)
            {
                RandomdizeNaturalDisasters();
                timer = starttime;
            }
        }
        else
        {
            timer -= Time.deltaTime;

            if (timer <= 0.0f)
            {
                RandomdizeNaturalDisasters();
                timer = starttime;
            }
        }

    }

    public void RandomdizeNaturalDisasters()
    {
        if (GlobalsVariables.instance.IsNetworking)
        {
            int RandomDisastersInt = Random.Range(MinValue, MaxValue);
            photonView.RPC("SpawnNaturalDisasters", RpcTarget.AllBuffered, RandomDisastersInt);
        }
        else
        {
            int RandomDisastersInt = Random.Range(MinValue, MaxValue);
            SpawnNaturalDisasters(RandomDisastersInt);
        }
    }

    public string ConvertNaturalDisastersIntToString(int currentdisasters)
    {
        switch (currentdisasters)
        {
            case 0:
                return "sun";
            case 1:
                return "tornado";
            case 2:
                return "volcano";
            case 3:
                return "tsunami";
            case 4:
                return "meteors shower";
            case 5:
                return "blizzard";
            case 6:
                return "sand storm";
            case 7:
                return "acid rain";
            default:
                return "null";
        }
    }

    public int ConvertNaturalDisastersStringToInt(string currentdisasters)
    {
        switch (currentdisasters)
        {
            case "sun":
                return 0;
            case "tornado":
                return 1;
            case "volcano":
                return 2;
            case "tsunami":
                return 3;
            case "meteors shower":
                return 4;
            case "blizzard":
                return 5;
            case "sand storm":
                return 6;
            case "acid rain":
                return 7;
            default:
                return -1;
        }
    }

    [PunRPC]
    public void SpawnNaturalDisasters(int currentdisasters_index)
    {

        switch (currentdisasters_index)
        {
            case 0:
                SpawnSun();
                break;

            case 1:
                SpawnTornado();
                break;

            case 2:
                SpawnVolcano();
                break;

            case 3:
                SpawnTsunami();
                break;

            case 4:
                SpawnMeteors();
                break;

            case 5:
                SpawnBlizzard();
                break;

            case 6:
                SpawnSandStorm();
                break;

            case 7:
                SpawnAcidRain();
                break;

            default:
                NullFunction();
                break;
        }

    }

    private void NullFunction()
    {
        CurrentDisastersInt = -1;
        CurrentDisasters = ConvertNaturalDisastersIntToString(CurrentDisastersInt);

        Debug.Log("Its a null disasters");
    }

    private void SpawnSun()
    {
        CurrentDisastersInt = 0;
        CurrentDisasters = ConvertNaturalDisastersIntToString(CurrentDisastersInt);

        GlobalsVariables.instance.temp_lerp = Random.Range(20, 31);
        GlobalsVariables.instance.humidity_lerp = Random.Range(0, 20);
        GlobalsVariables.instance.radiation_lerp = 0;
        GlobalsVariables.instance.oxygen_lerp = 100;
        GlobalsVariables.instance.pressure_lerp = Random.Range(10000, 10020);
        GlobalsVariables.instance.wind_direction_lerp = new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1));
        GlobalsVariables.instance.wind_speed_lerp = Random.Range(0, 10);
    }

    void SpawnAcidRain()
    {
        CurrentDisastersInt = 7;
        CurrentDisasters = ConvertNaturalDisastersIntToString(CurrentDisastersInt);

        GlobalsVariables.instance.temp_lerp = Random.Range(20, 31);
        GlobalsVariables.instance.humidity_lerp = Random.Range(0, 20);
        GlobalsVariables.instance.radiation_lerp = 0;
        GlobalsVariables.instance.oxygen_lerp = 100;
        GlobalsVariables.instance.pressure_lerp = Random.Range(10000, 10020);
        GlobalsVariables.instance.wind_direction_lerp = new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1));
        GlobalsVariables.instance.wind_speed_lerp = Random.Range(0, 10);
    }

    void SpawnVolcano()
    {
        CurrentDisastersInt = 2;
        CurrentDisasters = ConvertNaturalDisastersIntToString(CurrentDisastersInt);

        GlobalsVariables.instance.temp_lerp = Random.Range(20, 31);
        GlobalsVariables.instance.humidity_lerp = Random.Range(0, 20);
        GlobalsVariables.instance.radiation_lerp = 0;
        GlobalsVariables.instance.oxygen_lerp = 100;
        GlobalsVariables.instance.pressure_lerp = Random.Range(10000, 10020);
        GlobalsVariables.instance.wind_direction_lerp = new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1));
        GlobalsVariables.instance.wind_speed_lerp = Random.Range(0, 10);
    }

    void SpawnTsunami()
    {
        CurrentDisastersInt = 3;
        CurrentDisasters = ConvertNaturalDisastersIntToString(CurrentDisastersInt);

        GlobalsVariables.instance.temp_lerp = Random.Range(20, 31);
        GlobalsVariables.instance.humidity_lerp = Random.Range(0, 20);
        GlobalsVariables.instance.radiation_lerp = 0;
        GlobalsVariables.instance.oxygen_lerp = 100;
        GlobalsVariables.instance.pressure_lerp = Random.Range(10000, 10020);
        GlobalsVariables.instance.wind_direction_lerp = new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1));
        GlobalsVariables.instance.wind_speed_lerp = Random.Range(0, 10);
    }

    void SpawnMeteors()
    {
        CurrentDisastersInt = 4;
        CurrentDisasters = ConvertNaturalDisastersIntToString(CurrentDisastersInt);

        GlobalsVariables.instance.temp_lerp = Random.Range(20, 31);
        GlobalsVariables.instance.humidity_lerp = Random.Range(0, 20);
        GlobalsVariables.instance.radiation_lerp = 0;
        GlobalsVariables.instance.oxygen_lerp = 100;
        GlobalsVariables.instance.pressure_lerp = Random.Range(10000, 10020);
        GlobalsVariables.instance.wind_direction_lerp = new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1));
        GlobalsVariables.instance.wind_speed_lerp = Random.Range(0, 10);
    }

    void SpawnBlizzard()
    {
        CurrentDisastersInt = 5;
        CurrentDisasters = ConvertNaturalDisastersIntToString(CurrentDisastersInt);

        GlobalsVariables.instance.temp_lerp = Random.Range(-20, -35);
        GlobalsVariables.instance.humidity_lerp = 0;
        GlobalsVariables.instance.radiation_lerp = 100;
        GlobalsVariables.instance.oxygen_lerp = Random.Range(8000, 9020);
        GlobalsVariables.instance.pressure_lerp = Random.Range(5, 15);
        GlobalsVariables.instance.wind_direction_lerp = new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1));
        GlobalsVariables.instance.wind_speed_lerp = Random.Range(40, 50);
    }
    void SpawnSandStorm()
    {
        CurrentDisastersInt = 6;
        CurrentDisasters = ConvertNaturalDisastersIntToString(CurrentDisastersInt);

        GlobalsVariables.instance.temp_lerp = Random.Range(30, 35);
        GlobalsVariables.instance.humidity_lerp = Random.Range(0, 5);
        GlobalsVariables.instance.radiation_lerp = 0;
        GlobalsVariables.instance.oxygen_lerp = 100;
        GlobalsVariables.instance.pressure_lerp = Random.Range(10000, 10020);
        GlobalsVariables.instance.wind_direction_lerp = new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1));
        GlobalsVariables.instance.wind_speed_lerp = Random.Range(30, 50);
    }
    private void SpawnTornado()
    {
        CurrentDisastersInt = 1;
        CurrentDisasters = ConvertNaturalDisastersIntToString(CurrentDisastersInt);

        GlobalsVariables.instance.temp_lerp = Random.Range(5, 15);
        GlobalsVariables.instance.humidity_lerp = Random.Range(30, 40);
        GlobalsVariables.instance.radiation_lerp = 0;
        GlobalsVariables.instance.oxygen_lerp = 100;
        GlobalsVariables.instance.pressure_lerp = Random.Range(8000, 9000);
        GlobalsVariables.instance.wind_direction_lerp = new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1));
        GlobalsVariables.instance.wind_speed_lerp = Random.Range(0, 30);
    }


    public override void OnJoinedRoom()
    {
        GameObject player = PhotonNetwork.Instantiate(PlayerScene.name, SpawnPosition.position, SpawnPosition.rotation);
        player.name = GlobalsVariables.instance.LocalPlayerName;
        player.GetComponent<PhotonView>().RPC("SetNameText", RpcTarget.AllBuffered, player.name);
        GlobalsVariables.instance.playerlist.Add(player);
    }

    public void OnSingleplayer()
    {
        GameObject player = Instantiate(PlayerScene2, SpawnPosition.position, SpawnPosition.rotation);
        player.name = GlobalsVariables.instance.LocalPlayerName;
        player.GetComponent<Player>().SetNameText(player.name);
        GlobalsVariables.instance.playerlist.Add(player);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        GlobalsVariables.instance.temp_lerp = GlobalsVariables.instance.temp_original;
        GlobalsVariables.instance.humidity_lerp = GlobalsVariables.instance.humidity_original;
        GlobalsVariables.instance.radiation_lerp = GlobalsVariables.instance.radiation_original;
        GlobalsVariables.instance.oxygen_lerp = GlobalsVariables.instance.oxygen_original;
        GlobalsVariables.instance.pressure_lerp = GlobalsVariables.instance.pressure_original;
        GlobalsVariables.instance.wind_direction_lerp = GlobalsVariables.instance.wind_direction_original;
        GlobalsVariables.instance.wind_speed_lerp = GlobalsVariables.instance.wind_speed_original;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}
