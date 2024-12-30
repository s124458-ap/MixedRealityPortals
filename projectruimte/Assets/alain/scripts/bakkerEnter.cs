using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bakkerEnter : MonoBehaviour
{
    private GameObject player;
    private GameObject bakkersTv;
    private GameObject bakker;
    private AudioSource bakkersGreeting;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("speler");
        bakkersTv = GameObject.FindWithTag("bakkerstv");
        bakker = GameObject.FindWithTag("bakker");
        bakkersGreeting = bakker.GetComponent <AudioSource>();
    }

    public void GaBinnen()
    {
        player.gameObject.transform.position = new Vector3(70f, 4.9f, 55f);
        player.gameObject.transform.eulerAngles = new Vector3(0, -90, 0);
        Debug.Log("HI");
        PlayVideo bakkersfilm = bakkersTv.GetComponent<PlayVideo>();
        bakkersfilm.Play();
        bakkersGreeting.Play();
    }
}
