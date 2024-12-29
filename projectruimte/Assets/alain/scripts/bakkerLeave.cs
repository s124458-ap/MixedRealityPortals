using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bakkerLeave : MonoBehaviour
{
    private GameObject player;
    private GameObject bakkersTv;
    private GameObject rijdendeBus;
    private Animator crashAnimatie;
    private Animator busAnimatie;
    private Animator lampAnimatie;
    private AudioSource crashGeluid;
    public GameObject crashendeAuto;
    public GameObject straatLamp;
    public GameObject achtergrondObject;
    private AudioSource achtergrondGeluid;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("speler");
        bakkersTv = GameObject.FindWithTag("bakkerstv");
        rijdendeBus = GameObject.FindWithTag("bus");
        busAnimatie = rijdendeBus.GetComponent<Animator>();
        crashAnimatie = crashendeAuto.GetComponent<Animator>();
        crashGeluid = crashendeAuto.GetComponent<AudioSource>();
        lampAnimatie = straatLamp.GetComponent<Animator>();
        lampAnimatie = straatLamp.GetComponentInChildren<Animator>();
        achtergrondGeluid = achtergrondObject.GetComponentInChildren<AudioSource>();
    }

    public void GaBuiten()
    {
        player.gameObject.transform.position = new Vector3(-50f, 1f, 59f);
        player.gameObject.transform.eulerAngles = new Vector3(0, 0, 0);
        Debug.Log("BYE");
        PlayVideo bakkersfilm = bakkersTv.GetComponent<PlayVideo>();
        bakkersfilm.Stop();
        busAnimatie.enabled = !busAnimatie.enabled;
        rijdendeBus.gameObject.transform.position = new Vector3(-29, 0f, 48);
        rijdendeBus.gameObject.transform.eulerAngles = new Vector3(0, 90, 0);
        achtergrondGeluid.Stop();
        crashendeAuto.SetActive(true);
        crashAnimatie.enabled = true;
        crashGeluid.Play();
    }

    public void buigLamp()
    {
        lampAnimatie.enabled = true;
    }
}
