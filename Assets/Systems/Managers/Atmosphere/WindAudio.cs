using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class WindAudio : MonoBehaviour {
    [SerializeField] bool global = false;
    private AudioSource audio_source;

    [SerializeField] Vector2 volume_range = new Vector2(0f, 1f);
    [SerializeField] float max_volume_at_wind_magnitude = 6f;
    [SerializeField] Vector2 speed_range = new Vector2(0.8f, 2f);
    [SerializeField] float max_speed_at_wind_magnitude = 10f;

    private void Awake() {
        audio_source = GetComponent<AudioSource>();
        audio_source.spatialBlend = global ? 0f : 1f;
        audio_source.loop = true;
    }

    private void Update() {
        if (GameManager.instance.ship_controller.is_in_water) {
            audio_source.volume = 0f;
            audio_source.pitch = 0f;
            return;
        }

        audio_source.volume = Mathf.Lerp(volume_range.x, volume_range.y, Mathf.Min(AtmosphereManager.instance.get_wind_strength(), max_volume_at_wind_magnitude) / max_volume_at_wind_magnitude);
        audio_source.pitch = Mathf.Lerp(speed_range.x, speed_range.y, Mathf.Min(AtmosphereManager.instance.get_wind_strength(), max_speed_at_wind_magnitude) / max_speed_at_wind_magnitude);
    }
}
