using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.UI.Image;
using Random = UnityEngine.Random;

public class PlayerScript : MonoBehaviour
{
    public static PlayerScript instance;

    public List<GameObject> muzzleFlashObj = new List<GameObject>();

    public PlayerInput playerInput;

    private int health = 100;
    [SerializeField] Slider healthSLider;

    private InputAction move;
    private InputAction look;
    private InputAction shoot;
    private InputAction reload;
    private InputAction ability;
    private InputAction throwable;
    private InputAction interact;
    private InputAction esc;

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

    public float totalTime = 300f; // 5 minutes in seconds
    private float currentTime = 0f;
    private GUIStyle guiStyle = new GUIStyle();

    public GameObject effect;
    private Animator animCont;
    private Vector3 mLastPosition;

    public Camera seeThroughCam;

    [SerializeField] GameObject granade;
    public GameObject muzzleFlash;
    public GameObject gunPoint;
    /// <summary>
    /// x is distance 
    /// and the y is the damage
    /// </summary>
    [SerializeField]
    public AnimationCurve bulletDamage = new AnimationCurve();

    private bool shootPressed = false;


    [SerializeField] GameObject canvas;
    public GameObject messageUI;
    private Queue<Messages> messages = new Queue<Messages>();
    private bool showingText =false;
    private GameObject currentMessage;

    private bool showMenu;
    public bool hardMenuCall = false;
    public string gameMessage = "Find\n  the  \n   computer";

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
        throwable = playerInput.playerActions.Granade;
        interact = playerInput.playerActions.Interact;
        esc = playerInput.playerActions.Esc;

        move.Enable();
        look.Enable();
        shoot.Enable();
        reload.Enable();
        ability.Enable();
        throwable.Enable();
        interact.Enable();
        esc.Enable();

        throwable.performed += ThrowGranade;
        ability.performed += AbilitySwitch;
        reload.performed += ReloadGun;
        interact.performed += Interact;
        esc.performed += MenuCall;

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
        interact.Disable();
        esc.Disable();
    }

    private float cooldownTime = 2f;
    private float lastActivationTime = 0;
    public float throwForce = 10f;

    private bool showText = false;

    public bool stopTimer = false;

    [SerializeField] Transform soundPos;

    private void ThrowGranade(InputAction.CallbackContext context)
    {
        if (lastActivationTime + cooldownTime > Time.time)
        {
            return;
        }

        lastActivationTime = Time.time;

        GameObject grenade = Instantiate(granade, transform.position, transform.rotation); // Create a new instance of the grenade prefab
        Rigidbody rb = grenade.GetComponent<Rigidbody>(); // Get the rigidbody component of the grenade
        rb.AddForce(transform.forward * throwForce, ForceMode.VelocityChange); // Add force to the rigidbody to throw the grenade

    }

    private void AbilitySwitch(InputAction.CallbackContext context)
    {
        seeThroughCam.enabled = !seeThroughCam.enabled;
    }

    private void Start()
    {
        animCont.SetTrigger("reload");

        seeThroughCam.enabled = false;

        Cursor.lockState = CursorLockMode.Locked;

        SetMessage("Your goal is to deactivate the computer before the Timer runs out", Color.blue);
        SetMessage("Use your ability to check where the enemies are", Color.blue);
        SetMessage("They seemed to have locked all the doors, break them down!", Color.red);

        currentTime = totalTime;
        guiStyle.fontSize = 24;
        guiStyle.normal.textColor = Color.white;
        guiStyle.alignment = TextAnchor.UpperCenter;
    }

    private void Update()
    {
        isGrouded = controller.isGrounded;

        ProcessLook(look.ReadValue<Vector2>());

        if (shootPressed)
        {
            ShootingRayCastManager();
            muzzleFlash.SetActive(false);
        }

        LookingAtInteractables();

        if (!showingText && messages.Count > 0) 
        {
            showingText = true;
            var comp = messages.Dequeue();
            var obj = Instantiate(messageUI, canvas.transform);
            obj.GetComponent<MessageUI>().CallSetMessage(comp.text, comp.color);
        }

        if (!stopTimer) 
        {
            currentTime -= Time.deltaTime;
            if (currentTime <= 0f)
            {
                currentTime = 0f;
                hardMenuCall = true;
                gameMessage = "You Let the\ntime run out\n\n you are dead";
            }
        }
    }

    private void MenuCall(InputAction.CallbackContext context) 
    {
        showMenu = !showMenu;

        if (showMenu) 
        {
            Time.timeScale = 0;
        }
        else 
        {
            Time.timeScale = 1;
        }
    }

    private void Interact(InputAction.CallbackContext context)
    {
        RaycastHit hit;
        if (showText)
        {
            if (Physics.Raycast(transform.position, transform.forward, out hit, 10, LayerMask.GetMask("Interactable")))
            {
                IInteractable io = hit.collider.GetComponent<IInteractable>();

                if (io != null)
                {
                    io.Interact();
                }
            }
        }
    }

    private void LookingAtInteractables()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 10, LayerMask.GetMask("Interactable")))
        {
            showText = true;

        }
        else
        {
            showText = false;
        }
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

                    marchComp.ImpactReceiver(newRef.transform.localPosition, distanceEffect, newDir, 1f, bulletDamage);
                }
                else if (outHit.transform.GetComponent<Enemy>())
                {
                    outHit.transform.GetComponent<Enemy>().TakeDamage();
                }

                MakeNoise();
                Instantiate(effect, outHit.point, Quaternion.LookRotation(outHit.normal));

                var obj = Instantiate(muzzleFlashObj[Random.Range(0,muzzleFlashObj.Count)], gunPoint.transform.position, gunPoint.transform.rotation);
                obj.transform.parent = transform;

                Debug.DrawRay(Camera.main.transform.position, newDir * outHit.distance, Color.yellow, 90);
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

    public void TakeDamage(int val)
    {
        health -= val;

        healthSLider.value = health;

        if (health <= 0)
        {
            gameMessage = "you are dead";
            hardMenuCall = true;
        }
    }

    private void FixedUpdate()
    {
        ProcessMove(move.ReadValue<Vector2>());

        float speed = (transform.position - mLastPosition).magnitude / Time.deltaTime;
        mLastPosition = transform.position;

        animCont.SetFloat("speed", speed);
    }

    public void MakeNoise()
    {
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, 10, transform.forward);

        foreach (RaycastHit hit in hits)
        {
            if (hit.transform.GetComponent<Enemy>())
            {
                hit.transform.GetComponent<Enemy>().HearNoiseAt(soundPos.position);
            }
        }
    }

    private void OnGUI()
    {

        int minutes = Mathf.FloorToInt(currentTime / 60f);
        int seconds = Mathf.FloorToInt(currentTime - minutes * 60f);
        string timeText = string.Format("{0:00}:{1:00}", minutes, seconds);
        GUI.Label(new Rect(Screen.width / 2 - 50, 10, 100, 30), timeText, guiStyle);

        if (showMenu || hardMenuCall)
        {
            GUI.Box(new Rect((Screen.width / 2) - 50, (Screen.height / 2) - 50, 150, 100), $"{gameMessage}");

            // Create a button below the text box
            if (GUI.Button(new Rect((Screen.width / 2) - 50, (Screen.height / 2) + 50, 150, 30), "Restart the Game"))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }

            return;
        }

        if (showText)
        {
            GUI.Label(new Rect(Screen.width / 2 - 50, Screen.height - 50, 100, 30), "E to interact");
        }
    }

    public void SetMessage(string text, Color color) => messages.Enqueue(new Messages(text, color));

    public void SetDoneText() 
    {
        Destroy(currentMessage);
        showingText = false;
    }



    public class Messages 
    {
        public string text;
        public Color color;

        public Messages(string text, Color color)
        {
            this.text = text;
            this.color = color;
        }
    }


}

