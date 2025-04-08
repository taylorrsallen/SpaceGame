using UnityEngine;

public class AtmosphereManager : MonoBehaviour {
    private float wind_strength = 1f;
    private Vector2 wind_direction = new Vector2(1.0f, 0.0f);
    private Vector2 wind_target;
    public Vector2 wind_direction_horizontal_range = new Vector2(-1f, 1f);
    public Vector2 wind_direction_vertical_range = new Vector2(-0.4f, 0.4f);
    public float wind_change_speed = 5f;
    public Vector2 wind_strength_range = new Vector2(1f, 10f);
    public float wind_strength_noise_frequency = 10f;

    public float troposphere_start = 0f;
    public float stratosphere_start = 120f;
    public float mesosphere_start = 500f;
    public float thermosphere_start = 800f;
    public float exosphere_start = 7000f;

    public float world_horizontal_size = 400f;

    public static AtmosphereManager instance { get; private set; }

    private void Awake() {
        if (instance != null && instance != this) {
            Destroy(this);
        } else {
            instance = this;
        }

        wind_target = wind_direction;
    }

    private void Update() {
        wind_strength = Mathf.Max(Mathf.PerlinNoise1D(Time.realtimeSinceStartup * wind_strength_noise_frequency) * wind_strength_range.y, wind_strength_range.x);
        wind_target.x = Mathf.Clamp(wind_target.x + Random.Range(-0.1f, 0.1f), wind_direction_horizontal_range.x, wind_direction_horizontal_range.y);
        wind_target.y = Mathf.Clamp(wind_target.y + Random.Range(-0.1f, 0.1f), wind_direction_vertical_range.x, wind_direction_vertical_range.y);
        wind_direction = Vector2.Lerp(wind_direction, wind_target, Time.deltaTime * wind_change_speed);
    }

    public float get_wind_strength() { return (wind_direction * wind_strength).magnitude; }
    public Vector3 get_wind_force() { return new Vector3(wind_direction.x, wind_direction.y, 0f) * wind_strength; }
    public Vector3 get_wind_direction() { return new Vector3(wind_direction.x, wind_direction.y, 0f); }
}
