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
    public int moodMeter = 50;
    public Text menuText;
    private GameObject[] npcs;
    // Start is called before the first frame update
    void Start()
    {
        fpsCam = gameObject.GetComponent<Camera>();
        laserLine = GetComponentInChildren<LineRenderer>();
        menuText.text = "good";
        // alle gameobjecten die verschijnen en verdwijnen
        npcs = GameObject.FindGameObjectsWithTag("person");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 rayOrigin = fpsCam.ViewportToWorldPoint(new Vector3(.5f, .5f, 0));
        RaycastHit hit;
        laserLine.SetPosition(0, focusEnd.position);
        // Check if our raycast has hit anything
        if (Physics.Raycast(rayOrigin, fpsCam.transform.forward, out hit, focusRange))
        {
            // Set the end position for our laser line 
            laserLine.SetPosition(1, hit.point);
            // Check if the object we hit has a rigidbody attached
            if (hit.rigidbody != null)
            {
                Timer += Time.deltaTime;
                // Check if the object has the right tag attached
                if (hit.transform.CompareTag("goodNews")) { 
                    if (Timer >= seconde)
                    {
                        Timer = 0f;
                        moodMeter = moodMeter + 1;
                        if (moodMeter > 100)
                        {
                            moodMeter = 100;
                        }
                        Debug.Log(moodMeter);
                    }
                }
                if (hit.transform.CompareTag("badNews"))
                {
                    if (Timer >= seconde)
                    {
                        Timer = 0f;
                        moodMeter = moodMeter - 1;
                        if (moodMeter < 0)
                        {
                            moodMeter = 0;
                        }
                        Debug.Log(moodMeter);
                    }
                }
            }
        }
        else
        {
            // If we did not hit anything, set the end of the line to a position directly in front of the camera at the distance of weaponRange
            laserLine.SetPosition(1, rayOrigin + (fpsCam.transform.forward * focusRange));
        }

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
    }
}
