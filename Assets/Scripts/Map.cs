using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using ExitGames.Client.Photon;
using System;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using UnityEngine.UIElements;
using Unity.VisualScripting;


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

    public int cellSize = 100;
    public float updateInterval = 1f;
    private float nextUpdateGrid = Time.time;
    private float nextUpdateGridPlayer = Time.time;

    public Dictionary<Vector3Int, CellData> gridMap = new Dictionary<Vector3Int, CellData>();
    private List<Vector3Int> cellsToUpdate = new List<Vector3Int>();

    ExitGames.Client.Photon.Hashtable CustomeValue;


    void Start()
    {
        GlobalsVariables.instance.map = this;

        if (GlobalsVariables.instance.IsNetworking)
        {
            PhotonNetwork.OfflineMode = false;
            PhotonNetwork.JoinRandomOrCreateRoom();
            timer = starttime;
        }
        else
        {
            PhotonNetwork.OfflineMode = true;
            PhotonNetwork.CreateRoom("Singleplayer Room");
            timer = starttime;
        }

        if (GlobalsVariables.instance.realistweather == true)
        {
            GenerateGrid();
            StartCoroutine(UpdateGridCoroutine());
        }
    }

    void Update()
    {
        foreach (Transform child in transform)
        {
            GameObject obj = child.gameObject;
            GlobalsVariables.instance.Wind(obj);
        }

        if (GlobalsVariables.instance.realistweather == false)
        {
            timer -= Time.deltaTime;

            if (timer <= 0.0f)
            {
                RandomdizeNaturalDisasters();
                timer = starttime;
            }
        }
        


    }

    public void GenerateGrid()
    {
        Vector3Int minBounds = new Vector3Int(-5000, -5000, -5000); // Example values
        Vector3Int maxBounds = new Vector3Int(5000, 5000, 5000); // Example values

        Debug.Log("Generating grid...");
        for (int x = minBounds.x; x <= maxBounds.x; x += cellSize)
        {
            for (int y = minBounds.y; y <= maxBounds.y; y += cellSize)
            {
                for (int z = minBounds.z; z <= maxBounds.z; z += cellSize)
                {
                    Vector3Int position = new Vector3Int(x, y, z);
                    gridMap[position] = new CellData();

                    // Inicializar valores climáticos
                    CalculateTemperature(position);
                    CalculateHumidity(position);
                    CalculatePressure(position);
                    CalculateWind(position);
                    // Otros cálculos según tu lógica

                    cellsToUpdate.Add(position);
                    Debug.Log($"Cell generated at {position}");
                }
            }
        }
        SaveGrid();
        Debug.Log("Grid generated.");
    }

    private IEnumerator UpdateGridCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(updateInterval);
            UpdateGrid();
            UpdatePlayerCell();
        }
    }


    public void UpdateGrid()
    {
        if (cellsToUpdate.Count == 0)
        {
            cellsToUpdate.AddRange(gridMap.Keys);
        }

        for (int i = 0; i < 10 && cellsToUpdate.Count > 0; i++) // Process batch of 10 cells
        {
            Vector3Int cellPosition = cellsToUpdate[0];
            cellsToUpdate.RemoveAt(0);

            if (gridMap.ContainsKey(cellPosition))
            {
                CalculateTemperature(cellPosition);
                CalculateHumidity(cellPosition);
                CalculatePressure(cellPosition);
                CalculateWind(cellPosition);
                // Otros cálculos según tu lógica
            }
        }

        SaveGrid();
    }

    private void SaveGrid()
    {
        string filePath = Path.Combine(Application.persistentDataPath, "gridData.json");
        string json = JsonUtility.ToJson(new SerializableGrid(gridMap));
        File.WriteAllText(filePath, json);
        Debug.Log($"Grid saved to {filePath}");
    }

    private void LoadGrid()
    {
        string filePath = Path.Combine(Application.persistentDataPath, "gridData.json");
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            SerializableGrid data = JsonUtility.FromJson<SerializableGrid>(json);
            gridMap = data.ToDictionary();
            Debug.Log("Grid loaded.");
        }
    }

    private void CalculateTemperature(Vector3Int position)
    {
        // Lógica para calcular temperatura
        gridMap[position].Temperature = UnityEngine.Random.Range(-10f, 40f); // Ejemplo
        
    }

    private void CalculateHumidity(Vector3Int position)
    {
        // Lógica para calcular humedad
        gridMap[position].Humidity = UnityEngine.Random.Range(0f, 100f); // Ejemplo
    }

    private void CalculatePressure(Vector3Int position)
    {
        // Lógica para calcular humedad
        gridMap[position].Pressure = UnityEngine.Random.Range(998f, 1200f); // Ejemplo
    }

    private void CalculateWind(Vector3Int position)
    {
        // Lógica para calcular humedad
        gridMap[position].Wind_Speed = UnityEngine.Random.Range(0f, 1000f); // Ejemplo
        gridMap[position].Wind_Dir = new Vector3(UnityEngine.Random.Range(0f, 180f), UnityEngine.Random.Range(0f, 180f), UnityEngine.Random.Range(0f, 180f)); // Ejemplo

    }

    public void UpdatePlayerCell()
    {
       foreach (GameObject i in GlobalsVariables.instance.playerlist)
        {
            Vector3 Pos = new Vector3(Mathf.Floor(i.transform.position.x / cellSize), Mathf.Floor(i.transform.position.y / cellSize), Mathf.Floor(i.transform.position.z / cellSize));

            Vector3Int PosInt = Vector3Int.FloorToInt(Pos);


            if (gridMap[PosInt] != null)
            {
                CellData cell = gridMap[PosInt];
                if (cell.Temperature != null & cell.Humidity != null & cell.Pressure != null & cell.Wind_Dir.x != null & cell.Wind_Speed != null)
                {
                    GlobalsVariables.instance.temp_lerp = cell.Temperature;
                    GlobalsVariables.instance.humidity_lerp = cell.Humidity;
                    GlobalsVariables.instance.pressure_lerp = cell.Pressure;
                    GlobalsVariables.instance.wind_speed_lerp = cell.Wind_Speed;
                    GlobalsVariables.instance.wind_direction_lerp = cell.Wind_Dir;
                }
                else
                {

                    cell.Temperature = GlobalsVariables.instance.temp_lerp;
                    cell.Humidity = GlobalsVariables.instance.humidity_lerp;
                    cell.Pressure = GlobalsVariables.instance.pressure_lerp;
                    cell.Wind_Speed = GlobalsVariables.instance.wind_speed_lerp;
                    cell.Wind_Dir = GlobalsVariables.instance.wind_direction_lerp;
                }


            }
            else
            {
                print("Error: Posición fuera de los límites de la cuadrícula.");
            }
        }
    }


    public void RandomdizeNaturalDisasters()
    {
        int RandomDisastersInt = UnityEngine.Random.Range(MinValue, MaxValue);
        photonView.RPC("SpawnNaturalDisasters", RpcTarget.AllBuffered, RandomDisastersInt);
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

        GlobalsVariables.instance.temp_lerp = UnityEngine.Random.Range(20, 31);
        GlobalsVariables.instance.humidity_lerp = UnityEngine.Random.Range(0, 20);
        GlobalsVariables.instance.radiation_lerp = 0;
        GlobalsVariables.instance.oxygen_lerp = 100;
        GlobalsVariables.instance.pressure_lerp = UnityEngine.Random.Range(10000, 10020);
        GlobalsVariables.instance.wind_direction_lerp = new Vector3(UnityEngine.Random.Range(-1, 1), 0, UnityEngine.Random.Range(-1, 1));
        GlobalsVariables.instance.wind_speed_lerp = UnityEngine.Random.Range(0, 10);
    }

    void SpawnAcidRain()
    {
        CurrentDisastersInt = 7;
        CurrentDisasters = ConvertNaturalDisastersIntToString(CurrentDisastersInt);

        GlobalsVariables.instance.temp_lerp = UnityEngine.Random.Range(20, 31);
        GlobalsVariables.instance.humidity_lerp = UnityEngine.Random.Range(0, 20);
        GlobalsVariables.instance.radiation_lerp = 0;
        GlobalsVariables.instance.oxygen_lerp = 100;
        GlobalsVariables.instance.pressure_lerp = UnityEngine.Random.Range(10000, 10020);
        GlobalsVariables.instance.wind_direction_lerp = new Vector3(UnityEngine.Random.Range(-1, 1), 0, UnityEngine.Random.Range(-1, 1));
        GlobalsVariables.instance.wind_speed_lerp = UnityEngine.Random.Range(0, 10);
    }

    void SpawnVolcano()
    {
        CurrentDisastersInt = 2;
        CurrentDisasters = ConvertNaturalDisastersIntToString(CurrentDisastersInt);

        GlobalsVariables.instance.temp_lerp = UnityEngine.Random.Range(20, 31);
        GlobalsVariables.instance.humidity_lerp = UnityEngine.Random.Range(0, 20);
        GlobalsVariables.instance.radiation_lerp = 0;
        GlobalsVariables.instance.oxygen_lerp = 100;
        GlobalsVariables.instance.pressure_lerp = UnityEngine.Random.Range(10000, 10020);
        GlobalsVariables.instance.wind_direction_lerp = new Vector3(UnityEngine.Random.Range(-1, 1), 0, UnityEngine.Random.Range(-1, 1));
        GlobalsVariables.instance.wind_speed_lerp = UnityEngine.Random.Range(0, 10);
    }

    void SpawnTsunami()
    {
        CurrentDisastersInt = 3;
        CurrentDisasters = ConvertNaturalDisastersIntToString(CurrentDisastersInt);

        GlobalsVariables.instance.temp_lerp = UnityEngine.Random.Range(20, 31);
        GlobalsVariables.instance.humidity_lerp = UnityEngine.Random.Range(0, 20);
        GlobalsVariables.instance.radiation_lerp = 0;
        GlobalsVariables.instance.oxygen_lerp = 100;
        GlobalsVariables.instance.pressure_lerp = UnityEngine.Random.Range(10000, 10020);
        GlobalsVariables.instance.wind_direction_lerp = new Vector3(UnityEngine.Random.Range(-1, 1), 0, UnityEngine.Random.Range(-1, 1));
        GlobalsVariables.instance.wind_speed_lerp = UnityEngine.Random.Range(0, 10);
    }

    void SpawnMeteors()
    {
        CurrentDisastersInt = 4;
        CurrentDisasters = ConvertNaturalDisastersIntToString(CurrentDisastersInt);

        GlobalsVariables.instance.temp_lerp = UnityEngine.Random.Range(20, 31);
        GlobalsVariables.instance.humidity_lerp = UnityEngine.Random.Range(0, 20);
        GlobalsVariables.instance.radiation_lerp = 0;
        GlobalsVariables.instance.oxygen_lerp = 100;
        GlobalsVariables.instance.pressure_lerp = UnityEngine.Random.Range(10000, 10020);
        GlobalsVariables.instance.wind_direction_lerp = new Vector3(UnityEngine.Random.Range(-1, 1), 0, UnityEngine.Random.Range(-1, 1));
        GlobalsVariables.instance.wind_speed_lerp = UnityEngine.Random.Range(0, 10);
    }

    void SpawnBlizzard()
    {
        CurrentDisastersInt = 5;
        CurrentDisasters = ConvertNaturalDisastersIntToString(CurrentDisastersInt);

        GlobalsVariables.instance.temp_lerp = UnityEngine.Random.Range(-20, -35);
        GlobalsVariables.instance.humidity_lerp = 0;
        GlobalsVariables.instance.radiation_lerp = 100;
        GlobalsVariables.instance.oxygen_lerp = UnityEngine.Random.Range(8000, 9020);
        GlobalsVariables.instance.pressure_lerp = UnityEngine.Random.Range(5, 15);
        GlobalsVariables.instance.wind_direction_lerp = new Vector3(UnityEngine.Random.Range(-1, 1), 0, UnityEngine.Random.Range(-1, 1));
        GlobalsVariables.instance.wind_speed_lerp = UnityEngine.Random.Range(40, 50);
    }
    void SpawnSandStorm()
    {
        CurrentDisastersInt = 6;
        CurrentDisasters = ConvertNaturalDisastersIntToString(CurrentDisastersInt);

        GlobalsVariables.instance.temp_lerp = UnityEngine.Random.Range(30, 35);
        GlobalsVariables.instance.humidity_lerp = UnityEngine.Random.Range(0, 5);
        GlobalsVariables.instance.radiation_lerp = 0;
        GlobalsVariables.instance.oxygen_lerp = 100;
        GlobalsVariables.instance.pressure_lerp = UnityEngine.Random.Range(10000, 10020);
        GlobalsVariables.instance.wind_direction_lerp = new Vector3(UnityEngine.Random.Range(-1, 1), 0, UnityEngine.Random.Range(-1, 1));
        GlobalsVariables.instance.wind_speed_lerp = UnityEngine.Random.Range(30, 50);
    }
    private void SpawnTornado()
    {
        CurrentDisastersInt = 1;
        CurrentDisasters = ConvertNaturalDisastersIntToString(CurrentDisastersInt);

        GlobalsVariables.instance.temp_lerp = UnityEngine.Random.Range(5, 15);
        GlobalsVariables.instance.humidity_lerp = UnityEngine.Random.Range(30, 40);
        GlobalsVariables.instance.radiation_lerp = 0;
        GlobalsVariables.instance.oxygen_lerp = 100;
        GlobalsVariables.instance.pressure_lerp = UnityEngine.Random.Range(8000, 9000);
        GlobalsVariables.instance.wind_direction_lerp = new Vector3(UnityEngine.Random.Range(-1, 1), 0, UnityEngine.Random.Range(-1, 1));
        GlobalsVariables.instance.wind_speed_lerp = UnityEngine.Random.Range(0, 30);
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

[Serializable]
public class SerializableGrid
{
    public List<Vector3Int> Keys = new List<Vector3Int>();
    public List<CellData> Values = new List<CellData>();

    public SerializableGrid(Dictionary<Vector3Int, CellData> dictionary)
    {
        foreach (var kvp in dictionary)
        {
            Keys.Add(kvp.Key);
            Values.Add(kvp.Value);
        }
    }

    public Dictionary<Vector3Int, CellData> ToDictionary()
    {
        Dictionary<Vector3Int, CellData> dictionary = new Dictionary<Vector3Int, CellData>();
        for (int i = 0; i < Keys.Count; i++)
        {
            dictionary[Keys[i]] = Values[i];
        }
        return dictionary;
    }
}

[Serializable]
public class CellData
{
    public float Temperature;
    public float Humidity;
    public float Pressure;
    public float Wind_Speed;
    public Vector3 Wind_Dir;
    // Otros atributos climáticos
}
