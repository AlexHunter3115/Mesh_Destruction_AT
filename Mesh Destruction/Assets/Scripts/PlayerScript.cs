using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class PlayerScript : MonoBehaviour
{
    public static PlayerScript instance;

    public PlayerInput playerInput;

    private InputAction move;
    private InputAction look;
    private InputAction shoot;
    private InputAction reload;
    private InputAction ability;


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

    public GameObject effect;
    private Animator animCont;
    private Vector3 mLastPosition;

    public Camera seeThroughCam;


    /// <summary>
    /// x is distance 
    /// and the y is the damage
    /// </summary>
    [SerializeField]
    public AnimationCurve weightDistribution = new AnimationCurve();


    private bool shootPressed= false;


    private void Awake()
    {


        instance = this;

        playerInput = new PlayerInput();

        animCont = GetComponent<Animator>();

        controller = GetComponent<CharacterController>();
      
    }

    private void OnEnable()
    {
        move = playerInput.playerActions.Movement;
        look = playerInput.playerActions.Look;
        shoot = playerInput.playerActions.Shoot;
        reload = playerInput.playerActions.Reload;
        ability = playerInput.playerActions.Ability;


        move.Enable();
        look.Enable();
        shoot.Enable();
        reload.Enable();
        ability.Enable();

        ability.performed += AbilitySwitch;
        reload.performed += ReloadGun;

        shoot.performed += _ => shootPressed = true;
        shoot.canceled += _ => shootPressed = false;
    }

    private void OnDisable()
    {
        move.Disable();
        look.Disable();
        shoot.Disable();
        reload.Disable();
        ability.Disable();
    }



    private void AbilitySwitch(InputAction.CallbackContext context) 
    {
        seeThroughCam.enabled = !seeThroughCam.enabled;
    }

    private void Start()
    {
        seeThroughCam.enabled = false;

        Cursor.lockState = CursorLockMode.Locked;
    }



    private void Update()
    {
        isGrouded = controller.isGrounded;

        ProcessLook(look.ReadValue<Vector2>());


        if (shootPressed)
            ShootingRayCastManager();
    }


    public void ReloadGun(InputAction.CallbackContext context) 
    {
        animCont.SetTrigger("reload");
    }


    public void ShootingRayCastManager()
    {

      
            var x = (1 - 2 * Random.value) * 0.005f;
            var y = (1 - 2 * Random.value) * 0.005f;

            Vector3 newDir = Camera.main.transform.TransformDirection(new Vector3(x, y, 1));

            if (Time.time > lastFire + fireRate && !animCont.GetCurrentAnimatorStateInfo(0).IsName("Rig|AK_Reload"))    //&& !animCont.GetCurrentAnimatorStateInfo(0).IsName("Rig|AK_Shot") &&
            {
                animCont.SetTrigger("shoot");

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

                        marchComp.ImpactReceiver(newRef.transform.localPosition, distanceEffect, newDir, 1f, weightDistribution);
                    }
                    else
                    {
                        GameObject newRef = Instantiate(bulletPrefab);
                        newRef.transform.position = outHit.point;
                        newRef.transform.parent = outHit.transform;
                    }

                    //Instantiate(effect, outHit.point, effect.transform.rotation);
                  //  Debug.DrawRay(Camera.main.transform.position, newDir * outHit.distance, Color.yellow, 90);
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

        float speed = (transform.position - mLastPosition).magnitude / Time.deltaTime;
        mLastPosition = transform.position;

        animCont.SetFloat("speed", speed);
    }



}

