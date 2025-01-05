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

            if (!effect.isPositiveSound)
            {
                // For negative sounds (like wind and sirens)
                // They should get louder as weather gets worse (severity gets lower)
                if (weatherSeverity <= effect.startAt && weatherSeverity >= effect.fullVolumeAt)
                {
                    float t = (effect.startAt - weatherSeverity) / (effect.startAt - effect.fullVolumeAt);
                    targetVolume = effect.maxVolume * t;
                }
                else if (weatherSeverity <= effect.fullVolumeAt)
                {
                    targetVolume = effect.maxVolume;
                }
            }
            else
            {
                // Original calculation for positive sounds
                if (severity >= effect.startAt)
                {
                    float t = (severity - effect.startAt) / (effect.fullVolumeAt - effect.startAt);
                    t = Mathf.Clamp01(t);
                    targetVolume = effect.maxVolume * t;
                }
            }

            effect.source.volume = targetVolume;
        }
    }
}