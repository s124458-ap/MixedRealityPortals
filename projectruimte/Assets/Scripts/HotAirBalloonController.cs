using UnityEngine;

public class HotAirBalloonController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float baseFloatSpeed = 2f;        // Basis zweefsnelheid
    [SerializeField] private float maxHeight = 100f;           // Maximum hoogte
    [SerializeField] private float minHeight = 10f;            // Minimum hoogte
    [SerializeField] private float horizontalSpeed = 3f;       // Horizontale bewegingssnelheid
    [SerializeField] private float wobbleAmount = 0.5f;        // Hoeveelheid "wiebelen"
    [SerializeField] private float wobbleSpeed = 1f;           // Snelheid van "wiebelen"

    [Header("Wind Settings")]
    [SerializeField] private float windStrength = 2f;          // Windsterkte
    [SerializeField] private float windChangeSpeed = 0.5f;     // Hoe snel de wind verandert

    private float currentHeight;
    private Vector3 windDirection;
    private float timeOffset;

    private void Start()
    {
        currentHeight = transform.position.y;
        timeOffset = Random.Range(0f, 1000f); // Random startpunt voor Perlin noise
        windDirection = Random.insideUnitCircle.normalized; // Random startrichting voor wind
    }

    private void Update()
    {
        // Update hoogte
        UpdateHeight();

        // Update horizontale positie met wind en wobble
        UpdateHorizontalPosition();

        // Update windrichting
        UpdateWind();
    }

    private void UpdateHeight()
    {
        // Voeg een lichte op-en-neer beweging toe met Perlin noise
        float heightNoise = Mathf.PerlinNoise(Time.time * wobbleSpeed, timeOffset) * wobbleAmount;

        // Zorg dat de ballon binnen de min/max hoogte blijft
        currentHeight = Mathf.Clamp(transform.position.y + heightNoise * Time.deltaTime * baseFloatSpeed,
                                  minHeight,
                                  maxHeight);

        // Update de Y-positie
        Vector3 newPosition = transform.position;
        newPosition.y = currentHeight;
        transform.position = newPosition;
    }

    private void UpdateHorizontalPosition()
    {
        // Combineer wind en horizontale beweging
        Vector3 movement = windDirection * (windStrength * Time.deltaTime);

        // Voeg wat willekeurige beweging toe met Perlin noise
        float noiseX = Mathf.PerlinNoise(Time.time * wobbleSpeed, timeOffset) - 0.5f;
        float noiseZ = Mathf.PerlinNoise(Time.time * wobbleSpeed, timeOffset + 100) - 0.5f;

        movement += new Vector3(noiseX, 0, noiseZ) * horizontalSpeed * Time.deltaTime;

        // Update positie
        transform.position += movement;
    }

    private void UpdateWind()
    {
        // Verander windrichting geleidelijk met Perlin noise
        float windX = Mathf.PerlinNoise(Time.time * windChangeSpeed, timeOffset) - 0.5f;
        float windZ = Mathf.PerlinNoise(Time.time * windChangeSpeed, timeOffset + 100) - 0.5f;

        Vector3 targetWindDirection = new Vector3(windX, 0, windZ).normalized;
        windDirection = Vector3.Lerp(windDirection, targetWindDirection, Time.deltaTime);
    }

    // Optioneel: Voeg wat rotatie toe aan de ballon
    private void UpdateRotation()
    {
        float rotationX = Mathf.PerlinNoise(Time.time * wobbleSpeed * 0.5f, timeOffset) - 0.5f;
        float rotationZ = Mathf.PerlinNoise(Time.time * wobbleSpeed * 0.5f, timeOffset + 100) - 0.5f;

        Quaternion targetRotation = Quaternion.Euler(rotationX * 5f, transform.rotation.eulerAngles.y, rotationZ * 5f);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime);
    }
}