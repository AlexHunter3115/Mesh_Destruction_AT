using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;
//using UnityEngine.InputSystem.HID;Pla
using Random = UnityEngine.Random;

public class PlayerScript : MonoBehaviour
{
    public static PlayerScript instance;

    public PlayerInput playerInput;

    private InputAction move;
    private InputAction look;
    private InputAction shoot;


    //private PlayerInput.PlayerActions playerActions;


   // public InputAction playerControl;

    private CharacterController controller;
    private Vector3 playerVelocity;
    public float speed = 5f;

    public float gravity = -9.8f;
    public bool isGrouded;

    private float xRot = 0f;

    public float xSens = 30f;
    public float ySens = 30f;

    public LayerMask Hittable;

    public GameObject bulletPrefab;

    public float fireRate = 0.2f;
    public float lastFire = 0;
    public float inaccuracy = 0.1f;

    public float distanceEffect = 0.7f;


    private void Awake()
    {
        instance = this;

        playerInput = new PlayerInput();

        //playerInput = GetComponent<PlayerInput>();
        //playerInput.playerActions.Movement.performed += ProcessMove;


        controller = GetComponent<CharacterController>();
      
    }

    private void OnEnable()
    {
        move = playerInput.playerActions.Movement;
        look = playerInput.playerActions.Look;
        shoot = playerInput.playerActions.Shoot;


        move.Enable();
        look.Enable();
        shoot.Enable();

        shoot.performed += ShootingRayCastManager;
    }

    private void OnDisable()
    {
        move.Disable();
        look.Disable();
        shoot.Disable();
    }





    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
       
    }



    private void Update()
    {
        isGrouded = controller.isGrounded;


        //if (Input.GetKey(KeyCode.Mouse0))
        //{
        //    ShootingRayCastManager();
        //}



        ProcessLook(look.ReadValue<Vector2>());


    }





    public void ShootingRayCastManager(InputAction.CallbackContext context)
    {
        var x = (1 - 2 * Random.value) * 0.005f;
        var y = (1 - 2 * Random.value) * 0.005f;

        Vector3 newDir = Camera.main.transform.TransformDirection(new Vector3(x, y, 1));

        if (Time.time > lastFire + fireRate)
        {

            lastFire = Time.time;
            RaycastHit outHit;
            if (Physics.Raycast(Camera.main.transform.position, newDir, out outHit, Mathf.Infinity))
            {
                if (outHit.transform.GetComponent<MarchingSquare>() != null)
                {

                    GameObject newRef = Instantiate(bulletPrefab);

                    newRef.transform.position = outHit.point;
                    newRef.transform.parent = outHit.transform;

                    var marchComp = outHit.transform.GetComponent<MarchingSquare>();

                    marchComp.ImpactReceiver(newRef.transform.localPosition, distanceEffect, newDir);
                }
                else 
                {
                    GameObject newRef = Instantiate(bulletPrefab);
                    newRef.transform.position = outHit.point;
                    newRef.transform.parent = outHit.transform;
                }

                //Debug.DrawRay(Camera.main.transform.position, newDir * outHit.distance, Color.yellow, 90);
                //Debug.Log($"{outHit.transform.name}");
            }
        }
    }




    public void ProcessMove(Vector2 input)
    {
        Vector3 moveDir = Vector3.zero;
        moveDir.x = input.x;
        moveDir.z = input.y;

        controller.Move(transform.TransformDirection(moveDir) * speed * Time.deltaTime);

        if (isGrouded && playerVelocity.y < 0)
            playerVelocity.y = -2f;
        playerVelocity.y += gravity * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

    }

    public void ProcessLook(Vector2 input)
    {
        float mouseX = input.x;
        float mouseY = input.y;

        xRot -= (mouseY * Time.deltaTime) * ySens;
        xRot = Mathf.Clamp(xRot, -80f, 80f);

        Camera.main.transform.localRotation = Quaternion.Euler(xRot, 0, 0);
        transform.Rotate(Vector3.up * (mouseX * Time.deltaTime) * xSens);
    }

    private void FixedUpdate()
    {

        ProcessMove(move.ReadValue<Vector2>());

    }



}

