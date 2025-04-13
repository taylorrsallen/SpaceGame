using TreeEditor;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ShipControllerSpeedAudio : MonoBehaviour {
    private AudioSource audio_source;

    public Vector2 volume_noise_range = new Vector2(-0.1f, 0.1f);
    public float volume_noise_frequency = 1f;
    public Vector2 volume_range = new Vector2(0f, 1f);
    public Vector2 volume_speed_range = new Vector2(20f, 120f);
    public Vector2 pitch_range = new Vector2(0.9f, 2f);
    public Vector2 pitch_speed_range = new Vector2(100f, 200f);

    private void Awake() {
        audio_source = GetComponent<AudioSource>();
        audio_source.spatialBlend = 0f;
    }

    private void Update() {
        Vector3 velocity = GameManager.instance.ship_controller.get_velocity();
        float speed = velocity.magnitude;
        float wind_dot = Vector3.Dot(velocity.normalized, AtmosphereManager.instance.get_wind_direction());
        speed -= wind_dot * AtmosphereManager.instance.get_wind_strength();
        // Debug.Log("Speed " + speed + " | Dot " + wind_dot);
        audio_source.volume = get_volume(speed);
        audio_source.pitch = get_pitch(speed);
    }

    public float get_volume(float speed) {
        if (GameManager.instance.ship_controller.is_in_water) return 0f;
        float noise = Mathf.Lerp(volume_noise_range.x, volume_noise_range.y, Mathf.PerlinNoise1D(Time.realtimeSinceStartup * volume_noise_frequency));
        if (speed < volume_speed_range.x) {
            return volume_range.x + noise;
        } else {
            return Mathf.Lerp(volume_range.x, volume_range.y, (Mathf.Min(speed, volume_speed_range.y) - volume_speed_range.x) / (volume_speed_range.y - volume_speed_range.x)) + noise;
        }
    }

    public float get_pitch(float speed) {
        if (GameManager.instance.ship_controller.is_in_water) return 0f;
        if (speed < pitch_speed_range.x) {
            return pitch_range.x;
        } else {
            return Mathf.Lerp(pitch_range.x, pitch_range.y, (Mathf.Min(speed, pitch_speed_range.y) - pitch_speed_range.x) / (pitch_speed_range.y - pitch_speed_range.x));
        }
    }
}
