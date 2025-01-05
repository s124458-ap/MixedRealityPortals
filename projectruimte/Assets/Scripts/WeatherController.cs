using UnityEngine;
using UnityEngine.Rendering;

public class WeatherController : MonoBehaviour
{
    [Header("Particle Systems")]
    [SerializeField] private ParticleSystem rainSystem;
    [SerializeField] private ParticleSystem fogSystem;

    [Header("Weather Settings")]
    [SerializeField] private float weatherTransitionSpeed = 0.3f;
    [SerializeField] private float maxRainEmission = 20000f;
    [SerializeField] private float maxFogEmission = 1000f;

    [Header("Lighting Settings")]
    [SerializeField] private Light directionalLight;
    [SerializeField] private Material skyboxMaterial;
    [SerializeField] private Color clearWeatherColor = new Color(1f, 0.54f, 0.4f);
    [SerializeField] private Color stormyWeatherColor = new Color(0.19f, 0.16f, 0.16f);

    [Header("Testing")]
    [SerializeField] private bool autoTestWeather = false;
    [SerializeField] private float testDuration = 180f; // 3 minutes

    [Header("Object Visibility")]
    [SerializeField] private GameObject[] objectsToHideBelow50;  // Objects that disappear in bad weather
    [SerializeField] private GameObject[] objectsToShowBelow50;  // Objects that appear in bad weather
    private float testTimer = 0f;

    private ParticleSystem.EmissionModule rainEmission;
    private ParticleSystem.EmissionModule fogEmission;
    private ParticleSystem.MainModule fogMain;
    private float currentWeatherSeverity = 0f; // 0-100 scale
    private float targetWeatherSeverity = 0f;
    private bool using3DSize = false;

    [Header("Material Settings")]
    [SerializeField] private Material materialToChange;
    [SerializeField] private Material baseMapMaterial;

    [Header("Apocalypse Effects")]
    [SerializeField] private GameObject fireParent;           // Parent object containing all fires
    [SerializeField] private float maxFireEmission = 500f;
    [SerializeField] private float fireStartThreshold = 10f;  // Fire starts when weather severity is below 10%
    [SerializeField] private float fireTransitionRange = 5f; // Range over which fire transitions (10% to 5%)
    [SerializeField] private Color treeDyingColor = new Color(0.8f, 0.4f, 0.1f);
    [SerializeField] private Material[] vegetationMaterials;

    private ParticleSystem[] fireSystems;
    private Light[] fireLights;

    private bool isWaitingAtMax = false;
    private float maxWaitTimer = 0f;
    private const float MAX_WAIT_DURATION = 10f; // 10 seconds wait at 100%

    private bool isAutoResetting = false;

    [Header("Narration")]
    [SerializeField] private AudioSource narratorAudio;
    private bool hasPlayedNarration = false;

    public bool hasCompletedCycle = false;

    private void Start()
    {
        if (rainSystem != null)
            rainEmission = rainSystem.emission;
        if (fogSystem != null)
        {
            fogEmission = fogSystem.emission;
            fogMain = fogSystem.main;

            // Enable 3D size scaling
            fogMain.startSize3D = true;
            using3DSize = true;
        }

        // Start with clear weather without triggering narration
        currentWeatherSeverity = 100f;
        targetWeatherSeverity = 100f;

        // Get all fire particle systems and lights from children
        if (fireParent != null)
        {
            fireSystems = fireParent.GetComponentsInChildren<ParticleSystem>();
            fireLights = fireParent.GetComponentsInChildren<Light>();
        }

        // Initialize all fire lights to 0 intensity
        if (fireLights != null)
        {
            foreach (Light light in fireLights)
            {
                if (light != null)
                {
                    light.intensity = 0f;
                }
            }
        }
    }

    private void Update()
    {
        // If weather is at minimum, wait and then reset
        if (currentWeatherSeverity <= 0.1f && !isWaitingAtMax && !hasCompletedCycle)
        {
            isWaitingAtMax = true;
            maxWaitTimer = 0f;
        }

        if (isWaitingAtMax)
        {
            maxWaitTimer += Time.deltaTime;
            if (maxWaitTimer >= MAX_WAIT_DURATION)
            {
                isWaitingAtMax = false;
                hasCompletedCycle = true;
                SetWeatherSeverity(100f);

                // Reset object visibility
                if (objectsToHideBelow50 != null)
                {
                    foreach (GameObject obj in objectsToHideBelow50)
                    {
                        if (obj != null)
                        {
                            obj.SetActive(true);  // Show objects that were hidden
                        }
                    }
                }

                if (objectsToShowBelow50 != null)
                {
                    foreach (GameObject obj in objectsToShowBelow50)
                    {
                        if (obj != null)
                        {
                            obj.SetActive(false);  // Hide objects that were shown
                        }
                    }
                }

                if (narratorAudio != null)
                {
                    narratorAudio.Play();
                }
            }
        }

        weatherTransitionSpeed = 0.3f;
        // Smoothly transition weather
        currentWeatherSeverity = Mathf.MoveTowards(currentWeatherSeverity, targetWeatherSeverity,
            Time.deltaTime * weatherTransitionSpeed * 100f);

        UpdateWeatherEffects();
    }

    private void UpdateWeatherEffects()
    {
        float normalizedSeverity = 1f - (currentWeatherSeverity / 100f);  // Invert the severity
        if (objectsToHideBelow50 != null)
        {
            foreach (GameObject obj in objectsToHideBelow50)
            {
                if (obj != null)
                {
                    obj.SetActive(currentWeatherSeverity >= 50f);
                }
            }
        }

        if (objectsToShowBelow50 != null)
        {
            foreach (GameObject obj in objectsToShowBelow50)
            {
                if (obj != null)
                {
                    obj.SetActive(currentWeatherSeverity < 50f);
                }
            }
        }

        // Rain starts very low and increases more gradually using a power curve
        float rainPercentage = Mathf.Max(0, (normalizedSeverity - 0.3f) / 0.7f);
        if (rainSystem != null)
        {
            float rainCurve = Mathf.Pow(rainPercentage, 2f);  // Added power curve for more gradual increase
            rainEmission.rateOverTime = Mathf.Lerp(5f, maxRainEmission, rainCurve);  // Start from 5 instead of 0
        }

        // Fog starts at 0 and gets denser gradually with a more natural curve
        float fogCurve = Mathf.Pow(normalizedSeverity, 5f);
        float fogIntensity = Mathf.Lerp(0f, maxFogEmission * 0.3f, fogCurve);
        fogEmission.rateOverTime = fogIntensity;

        // Gradually change fog color and size with a more natural progression
        Color fogColor = Color.Lerp(
            new Color(0.9f, 0.9f, 0.9f, 0f),    // Start: Completely transparent
            new Color(0.4f, 0.4f, 0.35f, 0.5f),  // End: Less opaque
            Mathf.Pow(normalizedSeverity, 3f)    // Steeper non-linear color transition
        );
        fogMain.startColor = fogColor;

        // Particles start very small and grow more naturally
        float fogSize = Mathf.Lerp(0.1f, 1.5f, Mathf.Pow(normalizedSeverity, 2f));
        if (using3DSize)
        {
            // Slightly vary the dimensions for more natural look
            fogMain.startSizeX = fogSize * Random.Range(0.8f, 1.2f);
            fogMain.startSizeY = fogSize * Random.Range(0.8f, 1.2f);
            fogMain.startSizeZ = fogSize * Random.Range(0.8f, 1.2f);
        }

        // Update lighting and skybox
        if (directionalLight != null)
        {
            // Increased minimum intensity from 0.2f to 0.6f for better visibility
            directionalLight.intensity = Mathf.Lerp(0.6f, 1f, currentWeatherSeverity / 100f);
            directionalLight.color = Color.Lerp(stormyWeatherColor, clearWeatherColor, currentWeatherSeverity / 100f);
        }

        if (skyboxMaterial != null)
        {
            // Increased minimum exposure from 0.2f to 0.5f
            skyboxMaterial.SetFloat("_Exposure", Mathf.Lerp(0.5f, 1f, currentWeatherSeverity / 100f));

            // Increased minimum saturation from 0f to 0.3f
            skyboxMaterial.SetFloat("_Saturation", Mathf.Lerp(0.3f, 1f, currentWeatherSeverity / 100f));

            // Lighter stormy tint color
            Color tintColor = Color.Lerp(
                new Color(0.3f, 0.3f, 0.3f, 1f), // Lighter stormy color
                new Color(0.7f, 0.7f, 0.7f, 1f), // Clear weather color
                currentWeatherSeverity / 100f
            );
            skyboxMaterial.SetColor("_Tint", tintColor);
        }

        // Update ambient lighting
        RenderSettings.ambientMode = AmbientMode.Flat;
        RenderSettings.ambientLight = Color.Lerp(
            new Color(0.3f, 0.3f, 0.3f, 1f), // Lighter stormy color
            clearWeatherColor,
            currentWeatherSeverity / 100f
        );
        // Increased minimum ambient intensity from 0.4f to 0.6f
        RenderSettings.ambientIntensity = Mathf.Lerp(0.6f, 1f, currentWeatherSeverity / 100f);
        RenderSettings.fogColor = Color.Lerp(
            new Color(0.19f, 0.16f, 0.16f, 1f),
            clearWeatherColor,
            currentWeatherSeverity / 100f
        );
        RenderSettings.fogDensity = Mathf.Lerp(0.02f, 0f, currentWeatherSeverity / 100f);

        if (materialToChange != null)
        {
            Color materialColor = Color.Lerp(
                new Color(0.19f, 0.16f, 0.16f), // Stormy color
                clearWeatherColor,               // Clear weather color
                currentWeatherSeverity / 100f
            );

            // Try all common color properties
            materialToChange.SetColor("_Color", materialColor);
            materialToChange.SetColor("_BaseColor", materialColor);
            materialToChange.color = materialColor;
        }

        if (baseMapMaterial != null)
        {
            Color baseMapColor = Color.Lerp(
                new Color(0.19f, 0.16f, 0.16f), // Stormy color
                new Color(1f, 0.54f, 0.4f),     // Clear weather color (#FF8A66)
                currentWeatherSeverity / 100f
            );

            baseMapMaterial.SetColor("_BaseColor", baseMapColor);
            baseMapMaterial.SetColor("_Color", baseMapColor);
        }

        // Update vegetation colors to look more dead/dying
        if (vegetationMaterials != null)
        {
            foreach (Material mat in vegetationMaterials)
            {
                if (mat != null)
                {
                    Color deadVegetationColor = Color.Lerp(
                        treeDyingColor,
                        mat.GetColor("_BaseColor"), // Original color
                        currentWeatherSeverity / 100f
                    );

                    // Reduce saturation as weather gets worse
                    float saturation = Mathf.Lerp(0.3f, 1f, currentWeatherSeverity / 100f);
                    deadVegetationColor = LerpSaturation(deadVegetationColor, saturation);

                    mat.SetColor("_BaseColor", deadVegetationColor);
                    mat.SetColor("_Color", deadVegetationColor);
                }
            }
        }

        // Control fire and light intensity based on weather severity threshold
        float fireIntensity = 0f;
        if (currentWeatherSeverity <= fireStartThreshold)
        {
            // Smooth transition for fire intensity
            float transitionProgress = (fireStartThreshold - currentWeatherSeverity) / fireTransitionRange;
            fireIntensity = Mathf.Clamp01(transitionProgress);
        }

        // Update all fire particle systems
        if (fireSystems != null)
        {
            foreach (ParticleSystem fireSystem in fireSystems)
            {
                if (fireSystem != null)
                {
                    var emission = fireSystem.emission;
                    emission.rateOverTime = maxFireEmission * fireIntensity;
                }
            }
        }

        // Update all fire lights
        if (fireLights != null)
        {
            foreach (Light light in fireLights)
            {
                if (light != null)
                {
                    // Calculate target intensity based on fire transition
                    float targetIntensity = fireIntensity * 4f; // 4f is max light intensity

                    // Smoothly transition light intensity
                    light.intensity = Mathf.Lerp(light.intensity, targetIntensity, Time.deltaTime);

                    // Ensure light is completely off when fire is off
                    if (fireIntensity <= 0f && light.intensity < 0.01f)
                    {
                        light.intensity = 0f;
                    }
                }
            }
        }
    }

    private Color LerpSaturation(Color color, float saturation)
    {
        float h, s, v;
        Color.RGBToHSV(color, out h, out s, out v);
        s *= saturation;
        return Color.HSVToRGB(h, s, v);
    }

    public void SetWeatherSeverity(float severity)
    {
        Debug.Log($"Weather severity received: {severity}. Current: {currentWeatherSeverity}, Target: {targetWeatherSeverity}");
        // Round very small numbers to 0 and very large numbers to 100
        if (severity < 0.1f) severity = 0f;
        if (severity > 99.5f) severity = 100f;
        targetWeatherSeverity = Mathf.Clamp(severity, 0f, 100f);
    }

    public float GetCurrentWeatherSeverity()
    {
        return currentWeatherSeverity;
    }

    public float GetCurrentWindMultiplier()
    {
        // Simple linear wind increase
        return 1f + (currentWeatherSeverity / 100f * 2f); // Wind multiplier from 1 to 3
    }

    public bool IsAutoResetting()
    {
        return isAutoResetting;
    }
}