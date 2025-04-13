using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;

public class AtmosphereManager : MonoBehaviour {
    private float wind_strength = 1f;
    private Vector2 wind_direction = new Vector2(1.0f, 0.0f);
    private Vector2 wind_target;
    [TabGroup("Wind")] public Vector2 wind_direction_horizontal_range = new Vector2(-1f, 1f);
    [TabGroup("Wind")] public Vector2 wind_direction_vertical_range = new Vector2(-0.4f, 0.4f);
    [TabGroup("Wind")] public float wind_change_speed = 5f;
    [TabGroup("Wind")] public Vector2 wind_strength_range = new Vector2(1f, 10f);
    [TabGroup("Wind")] public float wind_strength_noise_frequency = 10f;

    [TabGroup("Atmosphere")] public float troposphere_start = 0f;
    [TabGroup("Atmosphere")] public float stratosphere_start = 120f;
    [TabGroup("Atmosphere")] public float mesosphere_start = 500f;
    [TabGroup("Atmosphere")] public float thermosphere_start = 800f;
    [TabGroup("Atmosphere")] public float exosphere_start = 7000f;
    
    [TabGroup("Height")] public AnimationCurve height_to_wind_curve;
    [TabGroup("Height")] public Vector2 height_to_wind_range = new Vector2(0f, 1200f);
    [TabGroup("Height")] public AnimationCurve height_to_gravity_curve;
    [TabGroup("Height")] public Vector2 height_to_gravity_range = new Vector2(0f, 2000f);
    [TabGroup("Height")] public AnimationCurve height_to_cash_curve;
    [TabGroup("Height")] public Vector2 height_to_cash_range = new Vector2(0f, 60000f);
    [TabGroup("Height")] public Vector2 space_intensity_range = new Vector2(500f, 20000f);
    [TabGroup("Height")] public float space_intensity_max = 4f;
    [TabGroup("Height")] public Material skybox_material;

    [TabGroup("Audio")] public AudioClip birds;
    [TabGroup("Audio")] public AudioClip crickets;

    [TabGroup("DayNight")] public Transform sun_moon_pivot;
    [TabGroup("DayNight")] public Light sun;
    [TabGroup("DayNight")] public Light moon;
    [TabGroup("DayNight")] public Vector2 skybox_intensity_range = new Vector2(0.2f, 1.1f);
    [TabGroup("DayNight")] public float minutes_in_day;
    [TabGroup("DayNight")] public float time_of_day = 0f;
    [TabGroup("DayNight")] public int day_number = 0;
    private float day_time_scale;

    public ReactiveGlobalVolume global_volume;

    public float underwater_gravity_multiplier = 0.1f;

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

        float ground_to_sky = 1f;
        float earth_to_space = 0f;
        float space_to_mars = 0f;
        ModularShipController ship_controller = GameManager.instance.ship_controller;
        if (ship_controller.transform.position.y < stratosphere_start) {
            ground_to_sky = GameManager.instance.ship_controller.transform.position.y / stratosphere_start;
        } else if (ship_controller.transform.position.y < mesosphere_start) {
            earth_to_space = ((GameManager.instance.ship_controller.transform.position.y - stratosphere_start) / (mesosphere_start - stratosphere_start)) * 0.3f;
        } else if (ship_controller.transform.position.y < thermosphere_start) {
            earth_to_space = Mathf.Max(((GameManager.instance.ship_controller.transform.position.y - mesosphere_start) / (thermosphere_start - mesosphere_start)), 0.3f);
        } else {
            earth_to_space = 1f;
        }

        float space_intensity = 1f;
        if (ship_controller.transform.position.y > space_intensity_range.x) {
            space_intensity = (Mathf.Min(ship_controller.transform.position.y, space_intensity_range.y) - space_intensity_range.x) / (space_intensity_range.y - space_intensity_range.x) * space_intensity_max;
        }

        skybox_material.SetFloat("_GroundToSky", ground_to_sky);
        skybox_material.SetFloat("_EarthToSpace", earth_to_space);
        skybox_material.SetFloat("_SpaceIntensity", space_intensity);
    }

    public float get_wind_strength() { return (wind_direction * wind_strength).magnitude; }
    public Vector3 get_wind_force() { return new Vector3(wind_direction.x, wind_direction.y, 0f) * wind_strength; }
    public Vector3 get_wind_direction() { return new Vector3(wind_direction.x, wind_direction.y, 0f).normalized; }

    public Vector3 get_gravity() {
        if (GameManager.instance.ship_controller.is_in_water) {
            return new Vector3(0f, -9.8f, 0f) * underwater_gravity_multiplier;
        } else {
            return new Vector3(0f, -9.8f, 0f) * height_to_gravity_curve.Evaluate(Mathf.Min(GameManager.instance.ship_controller.transform.position.y, height_to_gravity_range.y) / height_to_gravity_range.y);
        }
    }
}
