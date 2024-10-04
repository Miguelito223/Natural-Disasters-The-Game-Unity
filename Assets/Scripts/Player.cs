using Photon.Pun;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviourPunCallbacks
{
    public CharacterController controller;
    public Animator animator;
    public Camera Camera;
    public Scene mapscene;
    public TMP_Text playername;

    public float speed = 2f;
    public float health = 100f;
    public float jump_speed = 5f;
    public float body_temp = 37f;
    public float body_oxy = 100;
    public float body_rad = 0;
    public float speed_camera = 0.5f;
    public float Gravity = 9.81f;

    public float oxygenDecreaseRate = 5f; // Velocidad a la que el oxígeno se reduce

    public bool isinwater = false;
    public bool isinlava = false;
    public bool isunderwater = false;
    public bool isunderlava = false;
    public bool isunderground = false;
    public bool isoutdoor = false;

    // Factores de generación de calor
    private float bodyHeatGenK = 1f;
    private float bodyHeatGenMAX = 0.01f / 4f;
    private float fireHeatEmission = 50f;
    private float heatScale = 0f;
    private float coolScale = 0f;
    public float minTemp = 24f;        // Temperatura corporal mínima
    public float maxTemp = 44f;        // Temperatura corporal máxima
    public float min_bdradiation = 0;
    public float Max_bradiation = 100;
    public float minOxygen = 0f;
    public float maxOxygen = 100f;
    public float radiationThreshold = 80f;

    // Chance to apply damage (50%)
    private float hitChance = 0.5f;


    public LayerMask fireLayer;            // Capa que representa fuentes de calor (fuego)
    public LayerMask iceLayer;             // Capa que representa fuentes de frío (hielo)

    public Material tempEffectMaterial;

    public float RotationX;
    public Vector3 velocity;
    public float velocityY;



    void Start()
    {
        if (GlobalsVariables.instance.IsNetworking)
            if (!photonView.IsMine)
                return;

        GlobalsVariables.instance.LocalPlayer = this;

        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        Camera = transform.Find("Camera").GetComponent<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
    }


    void Update()
    {
        if (GlobalsVariables.instance.IsNetworking)
            if (!photonView.IsMine)
                return;

        UpdateTemperature(Time.deltaTime);
        UpdateOxygen(Time.deltaTime);
        UpdateRadiation(Time.deltaTime);
        camera_control();
        movement_control();
    }

    public bool IsPlayerAlive()
    {

        if (health <= 0)
            return false;
        else
            return true;
    }

    public void Damage(string damagetype, float damageamount=5)
    {
        if (GlobalsVariables.instance.IsNetworking)
            if (!photonView.IsMine)
                return;

        if (!IsPlayerAlive())
            return;


        if (damagetype == "heat")
        {
            health -= damageamount;
            if (health <= 0)
            {
                Death();
            }
        }
        else if (damagetype == "cold")
        { 
            health -= damageamount;
            if (health <= 0)
            {
                Death();
            }
        }
        else if (damagetype == "drown")
        {
            health -= damageamount;
            if (health <= 0)
            {
                Death();
            }
        }
        else
        {
            health -= damageamount;
            if (health <= 0)
            {
                Death();
            }

        }
        
    }

    public void Death()
    {
        if (GlobalsVariables.instance.IsNetworking)
            if (!photonView.IsMine)
                return;

        Debug.Log("Estas Muerto");
        transform.position = transform.parent.transform.Find("SpawnPoint").transform.position;
    }

    [PunRPC]
    public void SetNameText(string name)
    {
        playername.text = name;
    }

    private void UpdateTemperature(float deltaTime)
    {

        // Cálculos de equilibrio térmico
        float ambientTemperature = GlobalsVariables.instance.temp;
        float coreEquilibrium = Mathf.Clamp((37f - body_temp) * bodyHeatGenK, -bodyHeatGenMAX, bodyHeatGenMAX);
        float heatSourceEquilibrium = Mathf.Clamp((fireHeatEmission * heatScale) * bodyHeatGenK, 0f, bodyHeatGenMAX * 1.3f);
        float coldSourceEquilibrium = Mathf.Clamp((fireHeatEmission * coolScale) * bodyHeatGenK, bodyHeatGenMAX * -1.3f, 0f);
        float ambientEquilibrium = Mathf.Clamp((ambientTemperature - body_temp) * bodyHeatGenK, -bodyHeatGenMAX * 1.1f, bodyHeatGenMAX * 1.1f);

        // Ajuste para la temperatura ambiente dentro del rango 5 a 37 grados
        if (ambientTemperature >= 5f && ambientTemperature <= 37f)
        {
            ambientEquilibrium = 0f;
        }

        // Actualizar la temperatura corporal
        body_temp = Mathf.Clamp(body_temp + coreEquilibrium + heatSourceEquilibrium + coldSourceEquilibrium + ambientEquilibrium, minTemp, maxTemp);

        // Aplicar el efecto visual de temperatura
        if (tempEffectMaterial != null)
        {
            tempEffectMaterial.SetFloat("_Temp", body_temp);
        }

        // Cálculo de los efectos de calor y frío (alpha)
        float alphaHot = 1f - ((44f - Mathf.Clamp(body_temp, 39f, 44f)) / 5f);
        float alphaCold = ((35f - Mathf.Clamp(body_temp, 24f, 35f)) / 11f);

        // Aplicar daño por calor o frío extremo
        if (Random.Range(1, 26) == 25)
        {
            if (alphaCold != 0)
            {
                Damage("cold", alphaHot + alphaCold);
            }
            else if (alphaHot != 0)
            {
                Damage("heat", alphaHot + alphaCold);
            }
        }

        // Simular vómito si la temperatura es muy alta
        if (body_temp > 39f && Random.Range(0, 400) == 0)
        {
            Vomit();
        }

        // Simular estornudo si la temperatura es muy baja
        if (body_temp < 35f && Random.Range(0, 400) == 0)
        {
            Sneeze();
        }


    }

    // Función para simular vómito
    void Vomit()
    {
        Debug.Log("Player vomits due to overheating.");
    }

    // Función para simular estornudo
    void Sneeze()
    {
        Debug.Log("Player sneezes due to cold.");
    }
    private void UpdateRadiation(float deltaTime)
    {

        float globalRadiation = GlobalsVariables.instance.radiation;
        
        if (globalRadiation >= radiationThreshold && GlobalsVariables.instance.IsOutdoor(gameObject))
        {
            body_rad = Mathf.Clamp(body_rad + 5 * Time.deltaTime, min_bdradiation, Max_bradiation);
        }
        else
        {
            body_rad = Mathf.Clamp(body_rad - 5 * Time.deltaTime, min_bdradiation, Max_bradiation);
        }

        if (body_rad >= 100)
        {
           if (Random.Range(1, 25) == 25)
            {
                Damage("radiation", Random.Range(1, 30));
            }
        }
        
    }

    private void UpdateOxygen(float deltaTime)
    {

        float oxygenLevel = GlobalsVariables.instance.oxygen;
        // Si el oxígeno en el ambiente es bajo o el jugador está en agua/lava, disminuir el oxígeno corporal
        if (oxygenLevel <= 20f || isunderwater || isunderlava)
        {
            body_oxy = Mathf.Clamp(body_oxy - 5f * deltaTime, minOxygen, maxOxygen);
        }
        else
        {
            // De lo contrario, aumentar el oxígeno corporal
            body_oxy = Mathf.Clamp(body_oxy + 5f * deltaTime, minOxygen, maxOxygen);
        }

        // Si el oxígeno corporal se agota, causar daño al jugador
        if (body_oxy <= 0f)
        {
            if (Random.Range(1, 25) == 25) // Probabilidad de daño
            {
                Damage("drown", Random.Range(1, 31));
            }
        }
    }

    void camera_control()
    {

        if (GlobalsVariables.instance.IsNetworking)
            if (!photonView.IsMine)
                return;
            
        


        float MouseY = Input.GetAxis("Mouse Y") * speed_camera;
        float MouseX = Input.GetAxis("Mouse X") * speed_camera;
        RotationX -= MouseY;
        RotationX = Mathf.Clamp(RotationX, -90, 90);

        Camera.transform.eulerAngles = new Vector3(RotationX, Camera.transform.eulerAngles.y, Camera.transform.eulerAngles.z);
        transform.Rotate(Vector3.up * MouseX);


    }

    void movement_control()
    {
        if (GlobalsVariables.instance.IsNetworking)
            if (!photonView.IsMine)
                return;

        if (controller.isGrounded && velocity.y < 0)
        {
            velocityY = -2f;
        }

        if (Input.GetButtonDown("Jump") && controller.isGrounded)
        {
            animator.SetBool("IsJump", true);
            velocityY = jump_speed;
        }
        else
        {
            animator.SetBool("IsJump", false);
        }

        float horizontalY = Input.GetAxis("Horizontal");
        float verticalX = Input.GetAxis("Vertical");

        velocityY -= Gravity * Time.deltaTime;
        velocity = ((transform.forward * verticalX) + (transform.right * horizontalY)) * speed + Vector3.up * velocityY;

        controller.Move(velocity * Time.deltaTime);

        if (controller.isGrounded)
        {
            animator.SetBool("IsGround", true);

            if (isinwater | isunderwater)
            {
                animator.SetBool("IsSwiming", true);
            }
            else
            {
                if (controller.velocity.x > 0 || controller.velocity.z > 0)
                {
                    animator.SetBool("IsWalking", true);
                }
                else if (controller.velocity.x <= 0 || controller.velocity.z <= 0)
                {
                    animator.SetBool("IsWalking", false);
                }
            }
            
        }
        else
        {
            animator.SetBool("IsGround", false);
            if (isinwater | isunderwater)
            {
                animator.SetBool("IsSwiming", true);
            }
        }


    }
}
