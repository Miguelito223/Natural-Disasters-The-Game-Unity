using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviourPunCallbacks
{
    

    public float speed = 2f;
    public float jump_speed = 5f;
    public float temp = 37f;
    public int oxygen = 100;
    public float radiation = 0f;
    public float speed_camera = 0.5f;
    public float Gravity = 9.81f;

    public bool isinwater = false;
    public bool isinlava = false;
    public bool isunderwater = false;
    public bool isunderlava = false;

    public float RotationX;
    public Vector3 velocity;
    public float velocityY;

    public CharacterController controller;
    public Animator animator;
    public Camera Camera;
    public Transform esqueleto;

    void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }


    void Update()
    {
        camera_control();
        movement_control();
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
