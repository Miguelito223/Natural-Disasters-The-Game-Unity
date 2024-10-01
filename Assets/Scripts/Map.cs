using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Map : MonoBehaviourPunCallbacks
{
    public PhotonView PlayerScene;
    public GameObject PlayerScene2;
    public Transform SpawnPosition;
    public GameObject MeteorModel;
    public GameObject VolcanoModel;

    public static float originaltime = 180f;
    public static int MaxValue = 10;
    public static int MinValue = 0;

    private string CurrentDisasters = "sun";
    private int CurrentDisastersInt = 0;
    private float time = originaltime;






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

    void Update()
    {
        foreach (Transform child in transform)
        {
            GameObject obj = child.gameObject;
            GlobalsVariables.instance.Wind(obj);
        }

        time -= Time.deltaTime;

        if (time <= 0.0f)
        {
            RandomdizeNaturalDisasters();
            time = originaltime;
        }

    }

    public void RandomdizeNaturalDisasters()
    {
        int RandomDisastersInt = Random.Range(MinValue, MaxValue);
        ConvertNaturalDisastersIntToString(RandomDisastersInt);
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
    public void SpawnNaturalDisasters(int currentdisasters | int currentdisasters)
    {
        if (currentdisasters.GetType() == typeof(string))
        { 
            switch (currentdisasters)
            {
                case "sun":
                    SpawnSun();
                case "tornado":
                    SpawnTornado();
                case "volcano":
                    SpawnVolcano();
                case "tsunami":
                    SpawnTsunami();
                case "meteors shower":
                    SpawnMeteors();
                case "blizzard":
                    SpawnBlizzard();
                case "sand storm":
                    SpawnSandStorm();
                default:
                    NullFunction();
            }
        }
        else if (currentdisasters.GetType() == typeof(int)) {
            switch (currentdisasters)
            {
                case 0:
                    SpawnSun();
                case "tornado":
                    SpawnTornado();
                case "volcano":
                    SpawnVolcano();
                case "tsunami":
                    SpawnTsunami();
                case "meteors shower":
                    SpawnMeteors();
                case "blizzard":
                    SpawnBlizzard();
                case "sand storm":
                    SpawnSandStorm();
                default:
                    NullFunction();
            }
        }
    }

    private void NullFunction()
    {
        Debug.Log("Its a null disasters");
    }

    private void SpawnSun()
    {
        GlobalsVariables.instance.temp = Random.RandomRange;
        GlobalsVariables.instance.humidity =
        GlobalsVariables.instance.radiation =
        GlobalsVariables.instance.oxygen =
        GlobalsVariables.instance.pressure = 
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
