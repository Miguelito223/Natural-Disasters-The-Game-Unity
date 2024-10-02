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
        if (GlobalsVariables.instance.IsNetworking)
        {
            OnJoinedRoom();
            timer = starttime;
            CustomeValue = new Hashtable();
            CustomeValue.Add("StartTime", starttime);
            PhotonNetwork.CurrentRoom.SetCustomProperties(CustomeValue);
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
            timer -= PhotonNetwork.Time;

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
            PhotonView.Get(this).RPC("SpawnNaturalDisasters", RpcTarget.AllBuffered, RandomDisastersInt);
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

        GlobalsVariables.instance.temp = Random.Range(20, 31);
        GlobalsVariables.instance.humidity = Random.Range(0, 20);
        GlobalsVariables.instance.radiation = 0;
        GlobalsVariables.instance.oxygen = 100;
        GlobalsVariables.instance.pressure = Random.Range(10000, 10020);
        GlobalsVariables.instance.wind_direction = new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1));
        GlobalsVariables.instance.wind_speed = Random.Range(0, 10);
    }

    void SpawnVolcano()
    {
        CurrentDisastersInt = 2;
        CurrentDisasters = ConvertNaturalDisastersIntToString(CurrentDisastersInt);
        GlobalsVariables.instance.temp = Random.Range(20, 31);
        GlobalsVariables.instance.humidity = Random.Range(0, 20);
        GlobalsVariables.instance.radiation = 0;
        GlobalsVariables.instance.oxygen = 100;
        GlobalsVariables.instance.pressure = Random.Range(10000, 10020);
        GlobalsVariables.instance.wind_direction = new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1));
        GlobalsVariables.instance.wind_speed = Random.Range(0, 10);
    }

    void SpawnTsunami()
    {
        CurrentDisastersInt = 3;
        CurrentDisasters = ConvertNaturalDisastersIntToString(CurrentDisastersInt);
        GlobalsVariables.instance.temp = Random.Range(20, 31);
        GlobalsVariables.instance.humidity = Random.Range(0, 20);
        GlobalsVariables.instance.radiation = 0;
        GlobalsVariables.instance.oxygen = 100;
        GlobalsVariables.instance.pressure = Random.Range(10000, 10020);
        GlobalsVariables.instance.wind_direction = new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1));
        GlobalsVariables.instance.wind_speed = Random.Range(0, 10);
    }

    void SpawnMeteors()
    {
        CurrentDisastersInt = 4;
        CurrentDisasters = ConvertNaturalDisastersIntToString(CurrentDisastersInt);
        GlobalsVariables.instance.temp = Random.Range(20, 31);
        GlobalsVariables.instance.humidity = Random.Range(0, 20);
        GlobalsVariables.instance.radiation = 0;
        GlobalsVariables.instance.oxygen = 100;
        GlobalsVariables.instance.pressure = Random.Range(10000, 10020);
        GlobalsVariables.instance.wind_direction = new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1));
        GlobalsVariables.instance.wind_speed = Random.Range(0, 10);
    }

    void SpawnBlizzard()
    {
        CurrentDisastersInt = 5;
        CurrentDisasters = ConvertNaturalDisastersIntToString(CurrentDisastersInt);
        GlobalsVariables.instance.temp = Random.Range(-20, -35);
        GlobalsVariables.instance.humidity = 0;
        GlobalsVariables.instance.radiation = 100;
        GlobalsVariables.instance.oxygen = Random.Range(8000, 9020);
        GlobalsVariables.instance.pressure = Random.Range(5, 15);
        GlobalsVariables.instance.wind_direction = new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1));
        GlobalsVariables.instance.wind_speed = Random.Range(40, 50);
    }
    void SpawnSandStorm()
    {
        CurrentDisastersInt = 6;
        CurrentDisasters = ConvertNaturalDisastersIntToString(CurrentDisastersInt);
        GlobalsVariables.instance.temp = Random.Range(30, 35);
        GlobalsVariables.instance.humidity = Random.Range(0, 5);
        GlobalsVariables.instance.radiation = 0;
        GlobalsVariables.instance.oxygen = 100;
        GlobalsVariables.instance.pressure = Random.Range(10000, 10020);
        GlobalsVariables.instance.wind_direction = new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1));
        GlobalsVariables.instance.wind_speed = Random.Range(30, 50);
    }
    private void SpawnTornado()
    {
        CurrentDisastersInt = 1;
        CurrentDisasters = ConvertNaturalDisastersIntToString(CurrentDisastersInt);
        GlobalsVariables.instance.temp = Random.Range(5, 15);
        GlobalsVariables.instance.humidity = Random.Range(30, 40);
        GlobalsVariables.instance.radiation = 0;
        GlobalsVariables.instance.oxygen = 100;
        GlobalsVariables.instance.pressure = Random.Range(8000, 9000);
        GlobalsVariables.instance.wind_direction = new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1));
        GlobalsVariables.instance.wind_speed = Random.Range(0, 30);
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
        player.GetComponent<PlayerName>().SetNameText(player.name);
        GlobalsVariables.instance.playerlist.Add(player);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}
