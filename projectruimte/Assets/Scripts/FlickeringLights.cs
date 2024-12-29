using UnityEngine;

public class FlickeringLights : MonoBehaviour
{
    private new Light light;
    private float nextFlickerTime;

    void Start()
    {
        light = GetComponentInChildren<Light>();
    }

    private void Update()
    {
        var curTime = Time.time * 1000;

        if (nextFlickerTime < curTime)
        {
            bool nowEnabled = !light.enabled;
            light.enabled = nowEnabled;

            nextFlickerTime = curTime + (nowEnabled ? UnityEngine.Random.Range(500, 2500) : UnityEngine.Random.Range(100, 250));
        }
    }
}
