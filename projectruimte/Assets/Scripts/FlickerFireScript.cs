using UnityEngine;

public class FlickerFireScript : MonoBehaviour
{
    [Header("Flicker Settings")]
    [SerializeField] private Light fireLight;
    [SerializeField] private float flickerSpeed = 0.1f;
    [SerializeField] private float flashChance = 0.03f;
    [SerializeField] private float dimChance = 0.05f;
    [SerializeField] private float maxMultiplier = 10f;    // Maximum multiplier
    [SerializeField] private float minMultiplier = 0.7f;    // Minimum multiplier

    [Header("Color Settings")]
    [SerializeField] private Color baseColor = new Color(1f, 0.6f, 0.1f);
    [SerializeField] private float colorVariation = 0.1f;

    private float randomOffset;
    private float nextIntensityChange;
    private float currentMultiplier = 1f;
    private float originalIntensity;

    private void Start()
    {
        if (fireLight == null)
            fireLight = GetComponent<Light>();

        randomOffset = Random.Range(0f, 100f);
        nextIntensityChange = Time.time;
    }

    private void LateUpdate()
    {
        if (fireLight != null && fireLight.intensity > 0.01f)
        {
            // Store the original intensity from WeatherController
            originalIntensity = fireLight.intensity / currentMultiplier;  // Remove previous multiplier

            // Calculate new multiplier
            if (Time.time >= nextIntensityChange)
            {
                float rand = Random.value;
                if (rand < flashChance)
                {
                    currentMultiplier = maxMultiplier;
                    nextIntensityChange = Time.time + Random.Range(0.05f, 0.1f);
                }
                else if (rand < flashChance + dimChance)
                {
                    currentMultiplier = minMultiplier;
                    nextIntensityChange = Time.time + Random.Range(0.1f, 0.3f);
                }
                else
                {
                    float noise = Mathf.PerlinNoise(Time.time * flickerSpeed, randomOffset);
                    currentMultiplier = Mathf.Lerp(minMultiplier, maxMultiplier, noise);
                    nextIntensityChange = Time.time + Random.Range(0.1f, 0.2f);
                }
            }

            // Apply multiplier to original intensity
            fireLight.intensity = originalIntensity * currentMultiplier;

            // Update color
            float colorNoise = Mathf.PerlinNoise(Time.time * flickerSpeed * 0.5f, randomOffset + 100f);
            Color targetColor = baseColor * (1f + (colorNoise - 0.5f) * colorVariation);
            fireLight.color = Color.Lerp(fireLight.color, targetColor, Time.deltaTime * 2f);
        }
    }
}
