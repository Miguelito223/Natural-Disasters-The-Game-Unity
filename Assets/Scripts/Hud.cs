using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Hud : MonoBehaviour
{
    public TMP_Text information;
    public RawImage Hearth;
    public Player player;
    public AudioClip heartbeatClip;
    public AudioSource heartbeatSource;

    private float lastHeartbeatTime;
    public float heartRateMin = 0.5f;    // Latido mínimo del corazón (frecuencia)
    public float heartRateMax = 20f;     // Latido máximo del corazón (frecuencia)

    private RectTransform heartRectTransform;
    public float scaleX = 1f;            // Escala en x
    public float scaleY = 1f;            // Escala en y

    private float originalWidth;
    private float originalHeight;
    private float originalX;
    private float originalY;



    void Start()
    {
        heartRectTransform = Hearth.GetComponent<RectTransform>();
        heartbeatSource.clip = heartbeatClip;

        originalWidth = heartRectTransform.sizeDelta.x;
        originalHeight = heartRectTransform.sizeDelta.y;
        originalX = heartRectTransform.anchoredPosition.x;
        originalY = heartRectTransform.anchoredPosition.y;
    }

    // Update is called once per frame
    void Update()
    {
        information.text =
            "Health: " + player.health.ToString("f0") + "%\n" +
            "Temperature: " + GlobalsVariables.instance.temp.ToString("f2") + "ºC\n" +
            "Humidity: " + GlobalsVariables.instance.humidity.ToString("f0") + "%\n" +
            "Body Oxygen: " + player.body_oxy.ToString("f0") + "%\n" +
            "Body Temperature: " + player.body_temp.ToString("f2") + "ºC\n" +
            "Body Radiation: " + player.body_rad.ToString("f0") + "%\n" +
            "Radiation: " + GlobalsVariables.instance.radiation.ToString("f0") + "%\n" +
            "Oxygen: " + GlobalsVariables.instance.oxygen.ToString("f0") + "%\n" +
            "Pressure: " + GlobalsVariables.instance.pressure.ToString("f0") + "hPa\n" +
            "wind speed: " + GlobalsVariables.instance.wind_speed.ToString("f2") + "km/s\n" +
            "wind direction: " + GlobalsVariables.instance.convert_VectorToAngle(GlobalsVariables.instance.wind_direction).ToString("f0") + "º\n"
        ;

        float freq = Mathf.Clamp((1 - ((44 - Mathf.Round(player.body_temp)) / 20)) * (180f / 60f), heartRateMin, heartRateMax);

        // Si el jugador está muerto, ajusta la frecuencia a un valor muy bajo (simula muerte)
        if (!player.IsPlayerAlive())
        {
            freq = 0.05f;
        }

        // Escalar la imagen del corazón usando la función Mathf.Sin para crear un efecto de latido
        float scale = 1 + (Mathf.Sin(Time.time * (2 * Mathf.PI * freq)) * 0.5f);
        float w = originalWidth * scale;  // Ancho ajustado por el latido
        float h = originalHeight * scale;  // Alto ajustado por el latido
        float x = originalX;  // Posición en X ajustada
        float y = originalY;  // Posición en Y ajustada

        // Ajustar la escala y posición de la imagen del corazón
        heartRectTransform.sizeDelta = new Vector2(w * scaleX, h * scaleY);  // Ajustar tamaño
        heartRectTransform.anchoredPosition = new Vector2(x * scaleX, y * scaleY);  // Ajustar posición

        // Simular el sonido del latido si es necesario
        PlayHeartbeatSound(1 / freq, 100);
    }

    private void PlayHeartbeatSound(float interval, float pitch)
    {
        if (Time.time - lastHeartbeatTime >= interval)
        {
            heartbeatSource.pitch = pitch / 100f;  // Ajustar el pitch (tono) del sonido
            heartbeatSource.PlayOneShot(heartbeatClip);  // Reproducir el sonido del latido
            lastHeartbeatTime = Time.time;  // Actualizar el tiempo del último latido
        }
    }
}
