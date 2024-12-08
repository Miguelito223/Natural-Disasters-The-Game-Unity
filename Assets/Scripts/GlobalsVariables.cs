using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalsVariables : MonoBehaviourPunCallbacks
{

    public static GlobalsVariables instance;

    //networking
    public bool IsNetworking = false;
    public List<GameObject> playerlist = new List<GameObject>();
    public float PlayersNumber = 0;
    public string LocalPlayerName = "Player";
    
    //weather realism
    public bool realistweather = false;


    //game
    public float points = 0;

    //nodes
    public Map map;
    public Scene CurrentScene;
    public SunManager SunDirLight;
    public MoonManager MoonDirLight;
    public Player LocalPlayer;

    //weather
    public float temp = 23;
    public float humidity = 25;
    public float wind_speed = 0;
    public Vector3 wind_direction = new Vector3(0,0,0);
    public float radiation = 0;
    public float pressure = 1000;
    public float oxygen = 100;

    public float temp_lerp = 23;
    public float humidity_lerp = 23;
    public float wind_speed_lerp = 23;
    public Vector3 wind_direction_lerp = new Vector3(0, 0, 0);
    public float radiation_lerp = 0;
    public float pressure_lerp = 1000;
    public float oxygen_lerp = 100;

    public float temp_original = 23;
    public float humidity_original = 23;
    public float wind_speed_original = 23;
    public Vector3 wind_direction_original = new Vector3(0, 0, 0);
    public float radiation_original = 0;
    public float pressure_original = 1000;
    public float oxygen_original = 100;

    public LayerMask solidLayerMask;  // Layer mask for solid objects (for trace checking)
    public LayerMask skyMask;         // Layer mask for the sky

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
        PlayersNumber = playerlist.Count;
        Atmosphere();
        AtmosphereFadeControl();
    }

    public float ConvertKmphToMetersPerSec(float kmph)
    {
        return (kmph * 1000) / 3600;
    }

    public float ConvertDegreesToHours(float degrees)
    {
        return degrees / 15;
    }

    public float convert_VectorToAngle(Vector3 Vector)
    {

        float x = Vector.x;

        float y = Vector.z;


        return (360 + (Mathf.Rad2Deg * Mathf.Atan2(y, x))) % 360;
    }

    public float ConvertHoursToDegrees(float hours)
    {
        return hours * 15;
    }

    public float ConvertDegreestoMinutes(float degrees)
    {
        return (degrees / 15) * 60;
    }
    public float ConvertMetersPerSecondToKmph(float metres)
    {
        return (metres * 39.37f) / 0.75f;
    }

    public float ConvertDegreesToHour(float degrees)
    {
        return degrees / 15;
    }

    public bool IsSomethingBlockingWind(GameObject obj)
    {
        // Lógica para determinar si algo está bloqueando el viento (sustituir con tu lógica)
        return false; // Placeholder, cambiar según tu lógica
    }
    public float CalculateArea(GameObject obj)
    {
        // Lógica para calcular el área del objeto (sustituir con tu lógica)
        return 1f; // Placeholder, cambiar según tu lógica
    }
    public RaycastHit PerformTraceWind(GameObject obj, Vector3 direction)
    {
        RaycastHit hit;
        Physics.Raycast(obj.transform.position, direction, out hit, 1000f);
        return hit;
    }

    public void Wind(GameObject obj)
    {
        if (obj.CompareTag("Player"))
        {
            bool isOutdoor = IsOutdoor(obj);
            bool isSomethingBlockingWind = IsSomethingBlockingWind(obj);

            float distanceLeftRight = PerformTraceWind(obj, Vector3.right).distance;
            float distanceForwardBehind = PerformTraceWind(obj, Vector3.forward).distance;

            float area = (distanceLeftRight * distanceForwardBehind) / 2f;
            float areaPercentage = Mathf.Clamp(area / 100f, 0f, 1f);

            // Calcular la velocidad del viento local
            float localWind = areaPercentage * wind_speed;
            if (!isOutdoor || isSomethingBlockingWind)
                localWind = 0f;

            // Aplicar la velocidad del viento local al cuerpo del objeto (jugador)
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 windVelocity = ConvertKmphToMetersPerSec(localWind) * wind_direction;
                Vector3 frictionalScalar = Vector3.ClampMagnitude(windVelocity, 400f);
                Vector3 frictionalVelocity = frictionalScalar.magnitude * -windVelocity.normalized;
                Vector3 windVelocityNew = (windVelocity + frictionalVelocity) * 0.5f;

                // Verificar si el jugador está al aire libre y no hay obstáculos bloqueando el viento
                if (isOutdoor && !isSomethingBlockingWind)
                {
                    Vector3 deltaVelocity = (rb.linearVelocity - windVelocityNew) - rb.linearVelocity;
                    if (deltaVelocity.magnitude > 0)
                    {
                        rb.linearVelocity = rb.linearVelocity + deltaVelocity * 0.3f;
                    }
                }
            }
        }
        // Verificar si el objeto es movible y es un cuerpo rígido
        else if (obj.CompareTag("MovableObject") && obj.GetComponent<Rigidbody>() != null)
        {
            bool isOutdoor = IsOutdoor(obj);
            bool isSomethingBlockingWind = IsSomethingBlockingWind(obj);
            if (isOutdoor && !isSomethingBlockingWind)
            {
                Rigidbody rb = obj.GetComponent<Rigidbody>();
                float area = CalculateArea(obj);
                float mass = rb.mass;

                float forceMultiplierArea = Mathf.Clamp(area / 680827f, 0f, 1f);
                float frictionMultiplier = Mathf.Clamp(mass / 50000f, 0f, 1f);
                float averageMultiplier = (forceMultiplierArea + frictionMultiplier) / 2f;

                Vector3 windVelocity = ConvertKmphToMetersPerSec(wind_speed / 2.9225f) * wind_direction;
                Vector3 frictionalScalar = Vector3.ClampMagnitude(windVelocity, mass);
                Vector3 frictionalVelocity = frictionalScalar.magnitude * -windVelocity.normalized;
                Vector3 windVelocityNew = (windVelocity + frictionalVelocity) * -1f;

                float windVelocityCap = windVelocityNew.magnitude - rb.linearVelocity.magnitude;

                if (windVelocityCap > 0)
                {
                    rb.AddForce(windVelocityNew * averageMultiplier, ForceMode.Force);
                }
            }
        }
    }

    public bool HitChance(float chance)
    {
        return Random.Range(0f, 1f) < chance;
    }
    public bool IsOutdoor(GameObject obj)
    {
        bool hitLeft = PerformTrace(obj, Vector3.right);
        bool hitRight = PerformTrace(obj, Vector3.left);
        bool hitForward = PerformTrace(obj, Vector3.forward);
        bool hitBehind = PerformTrace(obj, Vector3.back);
        bool hitBelow = PerformTrace(obj, Vector3.down);

        bool inTunnel = (hitLeft && hitRight && !(hitForward && hitBehind)) || (!(hitLeft && hitRight) && (hitForward && hitBehind));

        bool hitSky = IsBelowSky(obj);

        // Check for underwater or lava conditions (replace with your own system)
        if (IsUnderWater(obj) || IsUnderLava(obj))
        {
            if (obj.CompareTag("Player"))
            { 
                obj.GetComponent<Player>().isoutdoor = false; 
            }
                
            return false;
        }
        else
        {
            if (obj.CompareTag("Player"))
            {
                obj.GetComponent<Player>().isoutdoor = hitSky;
            }
           
            return hitSky;
        }
    }
    private bool PerformTrace(GameObject obj, Vector3 direction)
    {
        RaycastHit hit;
        Vector3 start = obj.transform.position;

        if (Physics.Raycast(start, direction, out hit, 1000f, solidLayerMask))
        {
            return hit.collider != null;
        }

        return false;
    }

    // Perform a trace upwards to check if the object is below the sky
    private bool IsBelowSky(GameObject obj)
    {
        RaycastHit hit;
        Vector3 start = obj.transform.position;
        Vector3 end = start + Vector3.up * 48000f;

        if (Physics.Raycast(start, Vector3.up, out hit, 48000f, skyMask))
        {
            return hit.collider.CompareTag("Sky");
        }

        return false;
    }

    private bool IsUnderWater(GameObject obj)
    {
        if (obj.CompareTag("Player"))
        {
            return obj.GetComponent<Player>().isunderwater;
        }
        else
        {
            return false;
        }
       
    }

    // Placeholder for under-lava check (replace with your actual implementation)
    private bool IsUnderLava(GameObject obj)
    {
        if (obj.CompareTag("Player"))
        {
            return obj.GetComponent<Player>().isunderlava;
        }
        else
        {
            return false;
        }
    }
    private void Atmosphere()
    {
        temp = Mathf.Clamp(temp, -273, 273);
        humidity = Mathf.Clamp(humidity, 0, 100);
        radiation = Mathf.Clamp(radiation, 0, 100);
        oxygen = Mathf.Clamp(oxygen, 0, 100);
        pressure = Mathf.Clamp(pressure, 0, 10000);
    }
    private void AtmosphereFadeControl()
    {
        temp = Mathf.Lerp(temp, temp_lerp, 0.005f);
        humidity = Mathf.Lerp(humidity, humidity_lerp, 0.005f);
        wind_speed = Mathf.Lerp(wind_speed, wind_speed_lerp, 0.005f);
        wind_direction = Vector3.Lerp(wind_direction, wind_direction_lerp, 0.005f);
        radiation = Mathf.Lerp(radiation, radiation_lerp, 0.005f);
    }
}
