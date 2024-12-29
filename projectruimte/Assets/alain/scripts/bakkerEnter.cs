using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bakkerEnter : MonoBehaviour
{
    private GameObject player;
    private GameObject bakkersTv;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("speler");
        bakkersTv = GameObject.FindWithTag("bakkerstv");
    }

    public void GaBinnen()
    {
        player.gameObject.transform.position = new Vector3(70f, 4.9f, 55f);
        player.gameObject.transform.eulerAngles = new Vector3(0, -90, 0);
        Debug.Log("HI");
        PlayVideo bakkersfilm = bakkersTv.GetComponent<PlayVideo>();
        bakkersfilm.Play();
    }
}
