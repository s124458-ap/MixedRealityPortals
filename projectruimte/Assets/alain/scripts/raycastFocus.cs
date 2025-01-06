using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.OpenXR.Input;

public class raycastFocus : MonoBehaviour
{
    public Camera fpsCam;
    public float focusRange = 30f;
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
    private const float MAX_CONTRIBUTION_PER_OBJECT = 50f;
    private float moodChangeVelocity = 0f;
    private GameObject lastHitObject = null;

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
            if (hit.transform != null && !weatherController.hasCompletedCycle)
            {
                if (hit.transform.CompareTag("badNews"))
                {
                    if (lastHitObject != hit.transform.gameObject)
                    {
                        lastHitObject = hit.transform.gameObject;
                        stareTimer = 0f;
                    }

                    // Only update if we haven't reached the maximum contribution
                    if (stareTimer < MAX_CONTRIBUTION_PER_OBJECT)
                    {
                        stareTimer += Time.deltaTime;
                        float change = -Time.deltaTime * 10f;

                        // Calculate remaining contribution allowed
                        float remainingContribution = MAX_CONTRIBUTION_PER_OBJECT - stareTimer;
                        if (Mathf.Abs(change) > remainingContribution)
                        {
                            change = -remainingContribution;
                        }

                        moodMeter = Mathf.Clamp(moodMeter + change, 0f, 100f);
                        weatherController.AdjustWeather(change, hit.transform.gameObject);
                    }
                }
            }
        }
        else
        {
            lastHitObject = null;
            stareTimer = 0f;
            laserLine.SetPosition(1, rayOrigin + (fpsCam.transform.forward * focusRange));
        }

        // Update UI based on mood
        if (moodMeter >= 50)
        {
            menuText.text = "good";
            foreach (GameObject npc in npcs)
            {
                npc.SetActive(true);
            }
        }
        else
        {
            menuText.text = "bad";
            foreach (GameObject npc in npcs)
            {
                npc.SetActive(false);
            }
        }
    }
}
