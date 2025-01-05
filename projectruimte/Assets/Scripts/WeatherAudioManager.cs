using UnityEngine;
using UnityEngine.Rendering;

public class WeatherAudioManager : MonoBehaviour
{
    [System.Serializable]
    public class WeatherAudioEffect
    {
        public AudioSource source;
        public string soundName;
        [Range(0f, 100f)]
        public float startAt = 80f;
        [Range(0f, 100f)]
        public float fullVolumeAt = 90f;
        [Range(0f, 1f)]
        public float maxVolume = 1f;
        public float fadeSpeed = 1f;
        public bool isPositiveSound = true;
    }

    [SerializeField] private WeatherAudioEffect[] audioEffects;
    private WeatherController weatherController;

    private void Start()
    {
        weatherController = GetComponent<WeatherController>();

        // Initialize all audio sources
        foreach (var effect in audioEffects)
        {
            if (effect.source != null)
            {
                effect.source.loop = true;
                effect.source.volume = 0;
                effect.source.Play();
            }
        }
    }

    private void Update()
    {
        if (weatherController == null) return;
        UpdateAllSounds(weatherController.GetCurrentWeatherSeverity());
    }

    private void UpdateAllSounds(float weatherSeverity)
    {
        foreach (var effect in audioEffects)
        {
            if (effect.source == null) continue;

            float severity = effect.isPositiveSound ? weatherSeverity : (100f - weatherSeverity);
            float targetVolume = 0f;

            if (severity >= effect.fullVolumeAt)
                targetVolume = effect.maxVolume;
            else if (severity >= effect.startAt)
                targetVolume = effect.maxVolume * ((severity - effect.startAt) / (effect.fullVolumeAt - effect.startAt));

            effect.source.volume = Mathf.Lerp(
                effect.source.volume,
                targetVolume,
                Time.deltaTime * effect.fadeSpeed
            );
        }
    }
}