using Photon.Pun;
using UnityEngine;

public class Player : MonoBehaviourPunCallbacks
{
    public CharacterController controller;
    public Animator animator;
    public Camera Camera;

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

    public float compensationMax = 10f;    // Compensación máxima en grados
    public float bodyHeatGenK = 1f;        // Generación de calor corporal (1 grado por segundo)
    public float bodyHeatGenMax = 0.01f / 4;
    public float fireHeatEmission = 50f;   // Emisión de calor por fuego
    public float minBodyTemp = 24f;        // Temperatura corporal mínima
    public float maxBodyTemp = 44f;        // Temperatura corporal máxima
    public float BRadiation = 0f;
    public float radiationThreshold = 80f;

    // Chance to apply damage (50%)
    private float hitChance = 0.5f;


    public LayerMask fireLayer;            // Capa que representa fuentes de calor (fuego)
    public LayerMask iceLayer;             // Capa que representa fuentes de frío (hielo)

    private float nextUpdateTime = 0f;     // Tiempo de la siguiente actualización de temperatura

    public float RotationX;
    public Vector3 velocity;
    public float velocityY;



    void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        Camera = transform.Find("Camera").GetComponent<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
    }


    void Update()
    {
        if (Time.time >= nextUpdateTime)
        {
            UpdateTemperature();
            UpdateOxygen();
            UpdateRadiation();
            nextUpdateTime = Time.time + 1f; // Actualiza cada segundo
        }

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

    public void Damage(string damagetype)
    {
        if (!IsPlayerAlive())
            return;

        if (damagetype == "heat")
        {
            health -= 1;
            if (health <= 0)
            {
                Death();
            }
        }
        else if (damagetype == "cold")
        { 
            health -= 1;
            if (health <= 0)
            {
                Death();
            }
        }
        else if (damagetype == "Drown")
        {
            health -= 5;
            if (health <= 0)
            {
                Death();
            }
        }
        else
        {
            health -= 3;
            if (health <= 0)
            {
                Death();
            }

        }
        
    }

    public void Death()
    {
        Debug.Log("Estas Muerto");
        transform.position = transform.parent.transform.Find("SpawnPoint").transform.position;
    }


    private void UpdateTemperature()
    {
        float globalTemperature = GlobalsVariables.instance.temp;
        Debug.Log("Temperatura Global: " + globalTemperature);

        // Encuentra la fuente de calor más cercana (fuego)
        Collider[] fireColliders = Physics.OverlapSphere(transform.position, 10f, fireLayer);
        float heatScale = 0f;
        foreach (Collider fire in fireColliders)
        {
            float distance = Vector3.Distance(transform.position, fire.transform.position);
            heatScale = Mathf.Clamp(200f / (distance * distance), 0f, 1f);
        }

        // Encuentra la fuente de frío más cercana (hielo)
        Collider[] iceColliders = Physics.OverlapSphere(transform.position, 10f, iceLayer);
        float coolScale = 0f;
        foreach (Collider ice in iceColliders)
        {
            float distance = Vector3.Distance(transform.position, ice.transform.position);
            coolScale = Mathf.Clamp(200f / (distance * distance), 0f, 1f) * -1f;
        }

        // Cálculo de la temperatura
        float coreEquilibrium = Mathf.Clamp((37f - body_temp) * bodyHeatGenK, -bodyHeatGenMax, bodyHeatGenMax);
        float heatSourceEquilibrium = Mathf.Clamp((fireHeatEmission * heatScale) * bodyHeatGenK, 0f, bodyHeatGenMax * 1.3f);
        float coldSourceEquilibrium = Mathf.Clamp((fireHeatEmission * coolScale) * bodyHeatGenK, bodyHeatGenMax * -1.3f, 0f);
        float ambientEquilibrium = Mathf.Clamp(((globalTemperature - body_temp) * bodyHeatGenK), -bodyHeatGenMax * 1.1f, bodyHeatGenMax * 1.1f);

        // Condición para la zona de confort térmico (5°C - 37°C)
        if (globalTemperature >= 5f && globalTemperature <= 37f)
        {
            ambientEquilibrium = 0f;
        }

        // Actualiza la temperatura corporal del jugador
        body_temp = Mathf.Clamp(body_temp + coreEquilibrium + heatSourceEquilibrium + coldSourceEquilibrium + ambientEquilibrium, minBodyTemp, maxBodyTemp);

        Debug.Log("Temperatura Corporal: " + body_temp);

        TemperatureDamage();
    }

    void TemperatureDamage()
    {
        // Apply damage based on extreme temperatures
        if (body_temp >= maxBodyTemp)
        {
            Damage("heat");
        }
        else if (body_temp <= minBodyTemp)
        {
            Damage("cold");
        }

        // If body temperature exceeds lethal thresholds, handle player death
        if (body_temp >= 44f)
        {
            Damage("heat");
        }
        else if (body_temp <= 24f)
        {
            Damage("cold");
        }
        
    }

    private void UpdateRadiation()
    {

        if (BRadiation >= radiationThreshold && GlobalsVariables.instance.IsOutdoor(gameObject))
        {
            if (GlobalsVariables.instance.HitChance(hitChance))
            {
                Damage("acid");
            }
        }
        
    }

    private void UpdateOxygen()
    {
        if (isunderwater || isunderlava || isunderground || GlobalsVariables.instance.oxygen <= 20)
        {
            // Reduce el oxígeno
            body_oxy = Mathf.Clamp(body_oxy - (oxygenDecreaseRate * Time.deltaTime), 0, 100);

            if (body_oxy <= 0)
            {
                if (GlobalsVariables.instance.HitChance(hitChance))
                {
                    // Aplica daño si el oxígeno está en 0
                    Damage("Drown");
                }
            }
        }
        else
        {
            // Recupera oxígeno si el jugador no está en un entorno hostil
            body_oxy = Mathf.Clamp(body_oxy + (oxygenDecreaseRate * Time.deltaTime), 0, 100);
        }
    }

    void camera_control()
    {
        if (GlobalsVariables.instance.IsNetworking)
        { 
            if (!photonView.IsMine)
            {
                return;
            }
        }


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
        {
            if (!photonView.IsMine)
            {
                return;
            }
        }

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
