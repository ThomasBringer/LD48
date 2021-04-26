using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class Player : MonoBehaviour
{
    [SerializeField] float speed = 1;
    [SerializeField] float distanceToDetect = 2.5f;
    [SerializeField] float distanceToEnter = 1;

    [HideInInspector] public Transform playerChild;
    Animator anim;
    //Rigidbody rb;
    Level level;

    float invRadius;

    [SerializeField] LayerMask propsLayerMask;
    [SerializeField] float collisionDistance = .1f;

    //bool onEntry=false;

    [SerializeField] GameObject uiArrowKeys;
    [SerializeField] GameObject uiSpace;
    [SerializeField] Image uiTarget;
    [SerializeField] TextMeshProUGUI digText;
    //[SerializeField] float colorBeepTime = .2f;
    [SerializeField] float minBeepFrequency = 2;
    [SerializeField] float maxBeepFrequency = 10;
    [SerializeField] Color detectedColor;
    [SerializeField] Color foundColor;

    bool controllable = true;

    [SerializeField] GameObject shovel;
    [SerializeField] GameObject dirtPrefab;

    void Awake()
    {
        playerChild = transform.GetChild(0);
        level = FindObjectOfType<Level>();
        anim = GetComponentInChildren<Animator>();
        //rb = GetComponent<Rigidbody>();
        //rb.centerOfMass = Vector3.zero;
    }

    void Start()
    {
        AdaptToRadius();
    }

    void FixedUpdate()
    {
        if (!controllable) return;

        Vector2 input = (Vector2.right * Input.GetAxisRaw("Horizontal") + Vector2.up * Input.GetAxisRaw("Vertical")).normalized;

        Move(input);
        Animate(input);
    }

    void Update()
    {
        if (!controllable || level.FinalLevel) return;

        SeekEntry();
    }

    void Move(Vector2 input)
    {
        //rb.angularVelocity = (-transform.forward * input.x + transform.right * input.y) * speed * invRadius; //* 250 * Time.fixedDeltaTime;

        //rb.angularVelocity = Vector3.Scale(rb.angularVelocity, new Vector3(1, 0, 1));

        //rb.angularVelocity = Vector3.Scale(rb.angularVelocity, (transform.forward + transform.right));

        //rb.angularVelocity = Vector3.up * speed;

        // Vector3 rotationOffset = (-Vector3.forward * input.x + Vector3.right * input.y) * speed * invRadius * 100 * Time.deltaTime;
        // rb.MoveRotation(Quaternion.Euler(rotationOffset) * rb.rotation);

        //Debug.DrawRay(playerChild.transform.position, transform.forward * input.y + transform.right * input.x, Color.yellow, 1);

        if (!Physics.Raycast(playerChild.transform.position + playerChild.transform.up * .5f, transform.forward * input.y + transform.right * input.x, collisionDistance, propsLayerMask))
            transform.Rotate((-Vector3.forward * input.x + Vector3.right * input.y) * speed * invRadius * 100 * Time.deltaTime, Space.Self);
    }

    void Animate(Vector2 input)
    {
        float inputMagnitude = input.sqrMagnitude;
        anim.SetFloat("Speed", inputMagnitude);

        if (inputMagnitude > 0) playerChild.localRotation = Quaternion.LookRotation(Vector3.right * input.x + Vector3.forward * input.y); //playerChild.right = input;

        //if (inputMagnitude > 0) playerChild.right = (-Vector3.forward * input.x + Vector3.right * input.y);
    }

    void AdaptToRadius()
    {
        float radius = level.radius;
        playerChild.transform.localPosition = radius * Vector3.up;
        invRadius = 1 / radius;
    }

    float lastBeepTime;

    void SeekEntry()
    {
        //if (Input.GetButtonDown("Enter")) Enter();

        float radius = level.radius;

        var distances = new List<float>();

        foreach (Vector3 entry in level.entries)
        {
            Vector3 thisEntry = entry * radius;
            float distance = Vector3.Distance(thisEntry, playerChild.position);
            distances.Add(distance);
        }

        float minDistance = distances.Min();
        float frequency = (Mathf.Lerp(minBeepFrequency, maxBeepFrequency, 1 - Mathf.Clamp01((minDistance - distanceToEnter) / (distanceToDetect - distanceToEnter))));
        float period = 1 / frequency;

        bool close = minDistance <= distanceToDetect;
        bool mayEnter = minDistance <= distanceToEnter;

        digText.enabled = mayEnter;

        if (level.floorCount == 0)
        {
            if (mayEnter) uiArrowKeys.SetActive(false);
            uiSpace.SetActive(mayEnter);
        }

        if (close && Time.time - lastBeepTime >= period) StartCoroutine(ColorBeep(mayEnter ? foundColor : detectedColor, period * .5f));

        if (mayEnter && Input.GetButtonDown("Enter")) Enter();
    }

    IEnumerator ColorBeep(Color thisColor, float delay)
    {
        lastBeepTime = Time.time;
        uiTarget.color = thisColor;
        SoundManager.sm.beepSource.Play();
        yield return new WaitForSeconds(delay);
        uiTarget.color = Color.white;
    }

    void Enter()
    {
        //print("enter!");
        //beepingEntry = Vector3.zero;
        digText.enabled = false;
        uiSpace.SetActive(false);

        anim.SetBool("Digging", true);
        shovel.SetActive(true);
        controllable = false;
        Dirt();
        level.Enter();
        AdaptToRadius();
    }

    void Dirt()
    {
        Instantiate(dirtPrefab, playerChild).transform.SetParent(null);
    }

    public void EndEnter()
    {
        anim.SetBool("Digging", false);
        shovel.SetActive(false);
        controllable = true;
        Dirt();
    }

    void OnTriggerEnter(Collider cldr)
    {
        int layer = cldr.gameObject.layer;
        if (layer == 8 || layer == 9)
        {
            controllable = false;

            anim.SetBool("Dead", true);
            //print("game over!");
            StartCoroutine(GameOver());
        }


        //else print("layer: " + layer);
    }

    [SerializeField] float timeAfterDeath = 2.5f;
    /*[SerializeField]*/
    string menuTextAfterDeath = "Don't worry, you'll dig deeper next time!";

    IEnumerator GameOver()
    {
        MenuText(menuTextAfterDeath);
        yield return new WaitForSeconds(timeAfterDeath);
        Utilities.LoadPreviousScene();
    }

    void MenuText(string text)
    {
        Save.menuText = text;

        // print(Save.menuText);
        // PlayerPrefs.SetString("MenuText", text);
        // PlayerPrefs.Save();
        // print(PlayerPrefs.GetString("MenuText", "no text"));
    }
}