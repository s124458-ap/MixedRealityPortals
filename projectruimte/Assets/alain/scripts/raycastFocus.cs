using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.OpenXR.Input;

public class raycastFocus : MonoBehaviour
{
    public Camera fpsCam;
    public float focusRange = 15f;
    public Transform focusEnd;
    RaycastHit hit;
    private LineRenderer laserLine;
    private float Timer;
    private int seconde = 1;
    public float moodMeter = 100f;
    public Text menuText;
    private GameObject[] npcs;
    private WeatherController weatherController;
    private float stareTimer = 0f;
    [SerializeField] private float maxStareEffect = 10f; // How much the mood changes per second
    [SerializeField] private float recoveryRate = 0.5f; // How fast mood recovers when not staring
    private float moodChangeVelocity = 0f;

    // Start is called before the first frame update
    void Start()
    {
        fpsCam = gameObject.GetComponent<Camera>();
        laserLine = GetComponentInChildren<LineRenderer>();
        menuText.text = "good";
        // alle gameobjecten die verschijnen en verdwijnen
        npcs = GameObject.FindGameObjectsWithTag("person");

        // Find the WeatherController in the scene
        weatherController = FindObjectOfType<WeatherController>();
        if (weatherController == null)
        {
            Debug.LogError("WeatherController not found in the scene!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 rayOrigin = fpsCam.ViewportToWorldPoint(new Vector3(.5f, .5f, 0));
        RaycastHit hit;
        laserLine.SetPosition(0, focusEnd.position);

        if (Physics.Raycast(rayOrigin, fpsCam.transform.forward, out hit, focusRange))
        {
            laserLine.SetPosition(1, hit.point);
            if (hit.rigidbody != null)
            {
                if (hit.transform.CompareTag("goodNews"))
                {
                    moodMeter += maxStareEffect * Time.deltaTime;
                    moodMeter = Mathf.Clamp(moodMeter, 0f, 100f);
                }
                else if (hit.transform.CompareTag("badNews"))
                {
                    moodMeter -= maxStareEffect * Time.deltaTime;
                    moodMeter = Mathf.Clamp(moodMeter, 0f, 100f);
                }
            }
        }
        else
        {
            laserLine.SetPosition(1, rayOrigin + (fpsCam.transform.forward * focusRange));
        }

        // Update UI and NPCs
        if (moodMeter >= 50)
        {
            menuText.text = "good";
            foreach (GameObject npc in npcs)
            {
                npc.SetActive(true);
            }
        }
        if (moodMeter < 50)
        {
            menuText.text = "bad";
            foreach (GameObject npc in npcs)
            {
                npc.SetActive(false);
            }
        }

        // Update weather
        if (weatherController != null)
        {
            weatherController.SetWeatherSeverity(moodMeter);
        }
    }
}
