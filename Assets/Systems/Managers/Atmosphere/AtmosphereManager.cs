using System;
using Sirenix.OdinInspector;
using UnityEngine;

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
    [TabGroup("Atmosphere")] public float aliens_start = 20000f;
    [TabGroup("Atmosphere")] public float mars_start = 50000f;
    [TabGroup("Atmosphere")] public float mars_full = 55000f;
    
    [TabGroup("Height")] public AnimationCurve height_to_wind_curve;
    [TabGroup("Height")] public Vector2 wind_multiplier_range = new Vector2(1f, 2.5f);
    [TabGroup("Height")] public Vector2 height_to_wind_range = new Vector2(0f, 1200f);
    [TabGroup("Height")] public float max_wind_height = 1500f;
    [TabGroup("Height")] public AnimationCurve height_to_gravity_curve;
    [TabGroup("Height")] public Vector2 height_to_gravity_range = new Vector2(0f, 2000f);
    [TabGroup("Height")] public AnimationCurve height_to_cash_curve;
    [TabGroup("Height")] public Vector2 height_to_cash_range = new Vector2(0f, 60000f);
    [TabGroup("Height")] public AnimationCurve space_intensity_curve;
    [TabGroup("Height")] public Vector2 space_intensity_range = new Vector2(500f, 20000f);
    [TabGroup("Height")] public float space_intensity_max = 4f;
    [TabGroup("Height")] public Material skybox_material;

    [TabGroup("Audio")] public AudioSource birds;
    [TabGroup("Audio")] public AudioSource crickets;
    [TabGroup("Audio")] public AnimationCurve ambient_sounds_curve;
    [TabGroup("Audio")] public float ambient_volume;
    [TabGroup("Audio")] public AudioSource space_music;
    [TabGroup("Audio")] public AudioSource alien_music;
    [TabGroup("Audio")] public AudioSource success_music;
    [TabGroup("Audio")] public AudioSource chinese_music;
    [TabGroup("Audio")] public float bgm_lerp_speed = 5f;
    private float space_music_lerp_target;
    private float alien_music_lerp_target;
    private float success_music_lerp_target;
    private float chinese_music_lerp_target;

    [TabGroup("DayNight")] public Transform sun_moon_pivot;
    [TabGroup("DayNight")] public Light sun;
    [TabGroup("DayNight")] public Light moon;
    [TabGroup("DayNight")] public Material sun_material;
    [TabGroup("DayNight")] public Material moon_material;
    [TabGroup("DayNight")] public AnimationCurve sun_intensity_curve;
    [TabGroup("DayNight")] public Vector2 sun_intensity_range = new Vector2(0f, 10f);
    [TabGroup("DayNight")] public AnimationCurve sun_light_intensity_curve;
    [TabGroup("DayNight")] public AnimationCurve moon_intensity_curve;
    [TabGroup("DayNight")] public Vector2 sun_red_dot_range = new Vector2(0f, 0.2f);
    [TabGroup("DayNight")] public Color sun_red_color;
    [TabGroup("DayNight")] public Color sun_normal_color;
    [TabGroup("DayNight")] public Gradient day_ambient_light;
    [TabGroup("DayNight")] public Gradient night_ambient_light;
    [TabGroup("DayNight")] public AnimationCurve skybox_intensity_curve;
    [TabGroup("DayNight")] public Vector2 skybox_intensity_range = new Vector2(0.2f, 1.1f);
    [TabGroup("DayNight")] public Vector2 skybox_reflection_intensity_range = new Vector2(0.4f, 1f);
    [TabGroup("DayNight")] public float minutes_in_day;
    [HideInInspector] public float time_of_day = 0f;
    [HideInInspector] public int day_number = 0;
    private float day_time_scale;

    public ReactiveGlobalVolume global_volume;

    public float underwater_gravity_multiplier = 0.1f;

    public float world_horizontal_size = 400f;

    public static AtmosphereManager instance { get; private set; }

    private void Awake() {
        if (instance != null && instance != this) Destroy(this);
        else instance = this;
        DontDestroyOnLoad(this);

        wind_target = wind_direction;
        update_time_scale();
    }

    private void Update() {
        wind_strength = Mathf.Max(Mathf.PerlinNoise1D(Time.realtimeSinceStartup * wind_strength_noise_frequency) * wind_strength_range.y, wind_strength_range.x);
        wind_target.x = Mathf.Clamp(wind_target.x + UnityEngine.Random.Range(-0.1f, 0.1f), wind_direction_horizontal_range.x, wind_direction_horizontal_range.y);
        wind_target.y = Mathf.Clamp(wind_target.y + UnityEngine.Random.Range(-0.1f, 0.1f), wind_direction_vertical_range.x, wind_direction_vertical_range.y);
        wind_direction = Vector2.Lerp(wind_direction, wind_target, Time.deltaTime * wind_change_speed);

        space_music_lerp_target = 0f;
        float ground_to_sky = 1f;
        float earth_to_space = 0f;
        float space_to_mars = 0f;
        ModularShipController ship_controller = GameManager.instance.ship_controller;
        if (ship_controller.transform.position.y < stratosphere_start) {
            ground_to_sky = GameManager.instance.ship_controller.transform.position.y / stratosphere_start;
        } else if (ship_controller.transform.position.y < mesosphere_start) {
            earth_to_space = ((GameManager.instance.ship_controller.transform.position.y - stratosphere_start) / (mesosphere_start - stratosphere_start)) * 0.2f;
        } else if (ship_controller.transform.position.y < thermosphere_start) {
            earth_to_space = Mathf.Max(((GameManager.instance.ship_controller.transform.position.y - mesosphere_start) / (thermosphere_start - mesosphere_start)) * 0.4f, 0.2f);
        } else if (ship_controller.transform.position.y < exosphere_start) {
            earth_to_space = Mathf.Max(((GameManager.instance.ship_controller.transform.position.y - thermosphere_start) / (exosphere_start - thermosphere_start)), 0.4f);
        } else {
            earth_to_space = 1f;
            space_music_lerp_target = 1f;
        }

        if (ship_controller.transform.position.y > mars_start) {
            space_to_mars = Mathf.Min(ship_controller.transform.position.y, mars_full) / mars_full;
        }

        float space_intensity = 1f;
        if (ship_controller.transform.position.y > space_intensity_range.x) {
            space_intensity = space_intensity_curve.Evaluate(((Mathf.Min(ship_controller.transform.position.y, space_intensity_range.y) - space_intensity_range.x) / (space_intensity_range.y - space_intensity_range.x))) * space_intensity_max;
        }

        skybox_material.SetFloat("_GroundToSky", ground_to_sky);
        skybox_material.SetFloat("_EarthToSpace", earth_to_space);
        skybox_material.SetFloat("_SpaceToMars", space_to_mars);
        skybox_material.SetFloat("_SpaceIntensity", space_intensity);

        float china_intensity = GameManager.instance.ship_controller.china_intensity;
        if (china_intensity > 0f) {
            chinese_music_lerp_target = china_intensity;
            space_music_lerp_target = Mathf.Clamp01(space_music_lerp_target - chinese_music_lerp_target);
        } else {
            chinese_music_lerp_target = 0f;
        }

        if (alien_music_lerp_target > 0f) {
            space_music_lerp_target = 0f;
            chinese_music_lerp_target = Mathf.Max(chinese_music_lerp_target * 0.5f, 0.1f);
        }

        update_music_player(space_music, space_music_lerp_target);
        update_music_player(alien_music, alien_music_lerp_target);
        update_music_player(success_music, success_music_lerp_target);
        update_music_player(chinese_music, chinese_music_lerp_target);

        update_time();
    }

    private void update_music_player(AudioSource player, float lerp_target) {
        player.volume = Mathf.Lerp(player.volume, lerp_target, Time.deltaTime * bgm_lerp_speed);
        if (player.volume > 0f && !player.isPlaying) { player.Play(); } else if (player.volume == 0f && player.isPlaying) { player.Stop(); }
    }

    private void update_time_scale() {
        day_time_scale = 24f / (minutes_in_day / 60f);
    }

    private void update_time() {
        time_of_day += Time.deltaTime * day_time_scale / 86400f;
        if (time_of_day > 1) new_day();

        float sun_moon_pivot_angle = time_of_day * -360f;
        sun_moon_pivot.rotation = Quaternion.identity;
        sun_moon_pivot.Rotate(new Vector3(sun_moon_pivot_angle, 41f, 0f));

        float sun_dot = Mathf.Clamp01(Vector3.Dot(Vector3.down, sun.transform.forward));
        float sun_intensity = Mathf.Lerp(sun_intensity_range.x, sun_intensity_range.y, sun_intensity_curve.Evaluate(sun_dot));
        float moon_intensity = moon_intensity_curve.Evaluate(Mathf.Clamp01(-Vector3.Dot(Vector3.down, sun.transform.forward)));
        sun_material.color = Color.Lerp(sun_red_color, sun_normal_color, Mathf.Lerp(0f, 1f, Mathf.Clamp(sun_dot, sun_red_dot_range.x, sun_red_dot_range.y) / sun_red_dot_range.y)) * sun_intensity;
        sun.intensity = sun_dot * sun_light_intensity_curve.Evaluate(sun_dot);
        moon.intensity = moon_intensity;

        RenderSettings.reflectionIntensity = Mathf.Lerp(skybox_reflection_intensity_range.x, skybox_reflection_intensity_range.y, sun_dot);
        if (GameManager.instance.ship_builder.isActiveAndEnabled) RenderSettings.reflectionIntensity = Mathf.Lerp(0.8f, 1f, sun_dot);;

        float skybox_intensity = Mathf.Lerp(skybox_intensity_range.x, skybox_intensity_range.y, skybox_intensity_curve.Evaluate(sun_dot));
        skybox_material.SetFloat("_Intensity", skybox_intensity);

        if (GameManager.instance.ship_controller.is_in_water) {
            birds.volume = 0f;
            crickets.volume = 0f;
        } else {
            float wind_max_height_multiplier = get_wind_max_height_multiplier(GameManager.instance.ship_controller.transform.position.y);
            float ambience_dot = Vector3.Dot(Vector3.down, sun.transform.forward);
            if (ambience_dot > 0f) {
                RenderSettings.ambientLight = day_ambient_light.Evaluate(ambience_dot);

                if (!birds.isPlaying) {
                    birds.Play();
                    crickets.Stop();
                }
                birds.volume = ambient_sounds_curve.Evaluate(ambience_dot) * ambient_volume * wind_max_height_multiplier;
            } else {
                RenderSettings.ambientLight = night_ambient_light.Evaluate(-ambience_dot);

                if (!crickets.isPlaying) {
                    crickets.Play();
                    birds.Stop();
                }
                crickets.volume = ambient_sounds_curve.Evaluate(-ambience_dot) * ambient_volume * wind_max_height_multiplier;
            }
        }
    }

    private void new_day() {
        day_number++;
        time_of_day = 0f;
    }

    public float get_wind_multiplier(float height) {
        if (height < height_to_wind_range.y) {
            return Mathf.Lerp(wind_multiplier_range.x, wind_multiplier_range.y, height_to_wind_curve.Evaluate(Mathf.Min(height, height_to_wind_range.y) / height_to_wind_range.y));
        } else {
            return get_wind_max_height_multiplier(height);
        }
    }

    public float get_wind_max_height_multiplier(float height) {
        return Mathf.Lerp(1f, 0f, Mathf.Min(Mathf.Max(height, height_to_wind_range.y) - height_to_wind_range.y, max_wind_height - height_to_wind_range.y) / (max_wind_height - height_to_wind_range.y));
    }

    public float get_wind_strength(float height) { return (wind_direction * wind_strength).magnitude * get_wind_multiplier(height); }
    public Vector3 get_wind_force(float height) { return new Vector3(wind_direction.x, wind_direction.y, 0f) * wind_strength * get_wind_multiplier(height); }
    public Vector3 get_wind_direction() { return new Vector3(wind_direction.x, wind_direction.y, 0f).normalized; }

    public Vector3 get_gravity() {
        if (GameManager.instance.ship_controller.is_in_water) {
            return new Vector3(0f, -9.8f, 0f) * underwater_gravity_multiplier;
        } else {
            return new Vector3(0f, -9.8f, 0f) * height_to_gravity_curve.Evaluate(Mathf.Min(GameManager.instance.ship_controller.transform.position.y, height_to_gravity_range.y) / height_to_gravity_range.y);
        }
    }
}
