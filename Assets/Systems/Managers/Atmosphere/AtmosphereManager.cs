using System;
using Sirenix.OdinInspector;
using UnityEngine;

public enum AtmosphereLayer {
    TROPOSPHERE,
    STRATOSPHERE,
    MESOSPHERE,
    THERMOSPHERE,
    EXOSPHERE,
    SPACE,
    ALIENS,
    PEACEFUL_SPACE,
    MARS,
    COUNT,
}

public class AtmosphereManager : MonoBehaviour {
    private float wind_strength = 1f;
    private Vector2 wind_direction = new Vector2(1.0f, 0.0f);
    private Vector2 wind_target;
    [TabGroup("Wind")] public Vector2 wind_direction_horizontal_range = new Vector2(-1f, 1f);
    [TabGroup("Wind")] public Vector2 wind_direction_vertical_range = new Vector2(-0.4f, 0.4f);
    [TabGroup("Wind")] public float wind_change_speed = 5f;
    [TabGroup("Wind")] public Vector2 wind_strength_range = new Vector2(1f, 10f);
    [TabGroup("Wind")] public float wind_strength_noise_frequency = 10f;

    [TabGroup("Atmosphere")] public GameObject clouds;
    [TabGroup("Atmosphere")] public GameObject mars;
    [TabGroup("Atmosphere")] public GameObject mars_facade;
    [TabGroup("Atmosphere")] public float mars_facade_scale = 1f;
    [TabGroup("Atmosphere")] public float mars_facade_up_offset = 10f;
    [TabGroup("Atmosphere")] public float mars_facade_forward_offset = 10f;
    [TabGroup("Atmosphere")] public float troposphere_size = 700f;
    [TabGroup("Atmosphere")] public float stratosphere_size = 100f;
    [TabGroup("Atmosphere")] public float mesosphere_size = 600f;
    [TabGroup("Atmosphere")] public float thermosphere_size = 600f;
    [TabGroup("Atmosphere")] public float exosphere_size = 600f;
    [TabGroup("Atmosphere")] public float space_size = 600f;
    [TabGroup("Atmosphere")] public float aliens_size = 600f;
    [TabGroup("Atmosphere")] public float peace_size = 600f;
    [TabGroup("Atmosphere")] public float mars_size = 1000f;
    private float[] layer_sizes = new float[(int)AtmosphereLayer.COUNT];
    public AtmosphereLayer current_layer;
    private float current_layer_percent;
    private float max_height;
    private ulong height_cash;
    private float percent_to_mars;

    [HideInInspector] public float drag;
    [HideInInspector] public float visual_drag;
    [HideInInspector] public float gravity;
    
    [TabGroup("Height")] public AnimationCurve height_to_wind_curve;
    [TabGroup("Height")] public Vector2 wind_multiplier_range = new Vector2(1f, 2.5f);
    [TabGroup("Height")] public Vector2 height_to_wind_range = new Vector2(0f, 1200f);
    [TabGroup("Height")] public float max_wind_height = 1500f;
    [TabGroup("Height")] public AnimationCurve space_intensity_curve;
    [TabGroup("Height")] public float space_intensity_max = 26f;
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

        layer_sizes[0] = troposphere_size;
        layer_sizes[1] = stratosphere_size;
        layer_sizes[2] = mesosphere_size;
        layer_sizes[3] = thermosphere_size;
        layer_sizes[4] = exosphere_size;
        layer_sizes[5] = space_size;
        layer_sizes[6] = aliens_size;
        layer_sizes[7] = peace_size;
        layer_sizes[8] = mars_size;

        for(uint i = 0; i < layer_sizes.Length; i++) { max_height += layer_sizes[i]; }

        clouds.transform.position = new Vector3(clouds.transform.position.x, troposphere_size - 300f, clouds.transform.position.z);
        mars.transform.position = new Vector3(mars.transform.position.x, max_height, mars.transform.position.z);
        wind_target = wind_direction;
        update_time_scale();
    }

    private void Update() {
        wind_strength = Mathf.Max(Mathf.PerlinNoise1D(Time.realtimeSinceStartup * wind_strength_noise_frequency) * wind_strength_range.y, wind_strength_range.x);
        wind_target.x = Mathf.Clamp(wind_target.x + UnityEngine.Random.Range(-0.1f, 0.1f), wind_direction_horizontal_range.x, wind_direction_horizontal_range.y);
        wind_target.y = Mathf.Clamp(wind_target.y + UnityEngine.Random.Range(-0.1f, 0.1f), wind_direction_vertical_range.x, wind_direction_vertical_range.y);
        wind_direction = Vector2.Lerp(wind_direction, wind_target, Time.deltaTime * wind_change_speed);

        mars_facade.transform.position = GameManager.instance.ship_controller.get_ship_position() + Vector3.up * mars_facade_up_offset + Vector3.forward * mars_facade_forward_offset;
        mars_facade.transform.localScale = new Vector3(mars_facade_scale * percent_to_mars, mars_facade_scale * percent_to_mars, 0.01f);

        float ground_to_sky;
        float earth_to_space;
        float space_to_mars;
        float space_intensity;

        if(!GameManager.instance.ship_builder.gameObject.activeSelf) {
            float ship_height = GameManager.instance.ship_controller.get_ship_position().y;
            get_current_atmosphere_layer(ship_height, out ground_to_sky, out earth_to_space, out space_to_mars, out space_intensity);
            // Debug.Log("Layer: " + current_layer + " | Percent: " + current_layer_percent);

            float china_intensity = GameManager.instance.ship_controller.china_intensity;
            if (china_intensity > 0f) {
                chinese_music_lerp_target = china_intensity;
                space_music_lerp_target = Mathf.Clamp01(space_music_lerp_target - chinese_music_lerp_target);
            } else {
                chinese_music_lerp_target = 0f;
            }

            if (alien_music_lerp_target > 0f) chinese_music_lerp_target = Mathf.Max(chinese_music_lerp_target * 0.5f, 0.1f);
        } else {
            space_music_lerp_target = 0f;
            alien_music_lerp_target = 0f;
            success_music_lerp_target = 0f;
            chinese_music_lerp_target = 0f;

            ground_to_sky = 0f;
            earth_to_space = 0f;
            space_to_mars = 0f;
            space_intensity = 0f;
            drag = 0.01f;
            visual_drag = 0f;
            gravity = -9.8f;
            height_cash = 0;
            percent_to_mars = 0f;
        }

        skybox_material.SetFloat("_GroundToSky", ground_to_sky);
        skybox_material.SetFloat("_EarthToSpace", earth_to_space);
        skybox_material.SetFloat("_SpaceToMars", space_to_mars);
        skybox_material.SetFloat("_SpaceIntensity", space_intensity);

        update_music_player(space_music, space_music_lerp_target);
        update_music_player(alien_music, alien_music_lerp_target);
        update_music_player(success_music, success_music_lerp_target);
        update_music_player(chinese_music, chinese_music_lerp_target);

        update_time();
    }

    private void get_current_atmosphere_layer(float height, out float ground_to_sky, out float earth_to_space, out float space_to_mars, out float space_intensity) {
        for(int i = layer_sizes.Length - 1; i > -1; i--) {
            int layer_below_index = i - 1;
            float layer_start_height = 0f;
            if(layer_below_index >= 0) for(uint j = 0; j < i; j++) { layer_start_height += layer_sizes[j]; }

            if(height > layer_start_height) {
                // Debug.Log("StartHeight: " + layer_start_height + " | LayerBelowIndex: " + layer_below_index + " | CurrentLayer: " + i);
                current_layer_percent = (height - layer_start_height) / layer_sizes[i];

                switch(i) {
                    case 0:
                        current_layer = AtmosphereLayer.TROPOSPHERE;
                        ground_to_sky = 0f;
                        earth_to_space = 0f;
                        space_to_mars = 0f;
                        space_intensity = 0f;
                        drag = 0.01f;
                        visual_drag = 0f;
                        gravity = -9.8f;
                        height_cash = (ulong)Mathf.Lerp(0f, 5000f, current_layer_percent);
                        space_music_lerp_target = 0f;
                        alien_music_lerp_target = 0f;
                        success_music_lerp_target = 0f;
                        percent_to_mars = 0f;
                        return;
                    case 1:
                        current_layer = AtmosphereLayer.STRATOSPHERE;
                        ground_to_sky = current_layer_percent;
                        earth_to_space = current_layer_percent * 0.1f;
                        space_to_mars = 0f;
                        space_intensity = 1f;
                        drag = Mathf.Lerp(0.01f, 0.3f, current_layer_percent);
                        visual_drag = Mathf.Lerp(0f, 0.5f, current_layer_percent);
                        gravity = -9.8f;
                        height_cash = (ulong)Mathf.Lerp(5000f, 10000f, current_layer_percent);
                        space_music_lerp_target = 0f;
                        alien_music_lerp_target = 0f;
                        success_music_lerp_target = 0f;
                        percent_to_mars = 0f;
                        return;
                    case 2:
                        current_layer = AtmosphereLayer.MESOSPHERE;
                        ground_to_sky = 1f;
                        earth_to_space = Mathf.Max(0.1f, current_layer_percent * 0.3f);
                        space_to_mars = 0f;
                        space_intensity = 1f;
                        drag = Mathf.Lerp(0.3f, 0.4f, current_layer_percent);
                        visual_drag = Mathf.Lerp(0.5f, 1f, current_layer_percent);
                        gravity = Mathf.Lerp(-9.8f, -6.8f, current_layer_percent);
                        height_cash = (ulong)Mathf.Lerp(10000f, 20000f, current_layer_percent);
                        space_music_lerp_target = 0f;
                        alien_music_lerp_target = 0f;
                        success_music_lerp_target = 0f;
                        percent_to_mars = 0f;
                        return;
                    case 3:
                        current_layer = AtmosphereLayer.THERMOSPHERE;
                        ground_to_sky = 1f;
                        earth_to_space = Mathf.Max(0.3f, current_layer_percent * 0.5f);
                        space_to_mars = 0f;
                        space_intensity = 1f;
                        drag = Mathf.Lerp(0.4f, 0.7f, current_layer_percent);
                        visual_drag = Mathf.Lerp(1f, 10f, current_layer_percent);
                        gravity = Mathf.Lerp(-6.8f, -3.8f, current_layer_percent);
                        height_cash = (ulong)Mathf.Lerp(20000f, 40000f, current_layer_percent);
                        space_music_lerp_target = 0f;
                        alien_music_lerp_target = 0f;
                        success_music_lerp_target = 0f;
                        percent_to_mars = Mathf.Lerp(0f, 0.15f, current_layer_percent);
                        return;
                    case 4:
                        current_layer = AtmosphereLayer.EXOSPHERE;
                        ground_to_sky = 1f;
                        earth_to_space = Mathf.Max(0.5f, current_layer_percent);
                        space_to_mars = 0f;
                        space_intensity = 1f;
                        drag = Mathf.Lerp(0.7f, 0.3f, current_layer_percent);
                        visual_drag = Mathf.Lerp(10f, 0f, current_layer_percent);
                        gravity = Mathf.Lerp(-3.8f, -0.8f, current_layer_percent);
                        height_cash = (ulong)Mathf.Lerp(40000f, 80000f, current_layer_percent);
                        space_music_lerp_target = 1f;
                        alien_music_lerp_target = 0f;
                        success_music_lerp_target = 0f;
                        percent_to_mars = Mathf.Lerp(0.15f, 0.3f, current_layer_percent);
                        return;
                    case 5:
                        current_layer = AtmosphereLayer.SPACE;
                        ground_to_sky = 1f;
                        earth_to_space = 1f;
                        space_to_mars = 0f;
                        space_intensity = Mathf.Max(1f, space_intensity_max * space_intensity_curve.Evaluate(current_layer_percent));
                        drag = 0.3f;
                        visual_drag = 0f;
                        gravity = 0f;
                        height_cash = (ulong)Mathf.Lerp(80000f, 160000f, current_layer_percent);
                        space_music_lerp_target = 1f;
                        alien_music_lerp_target = 0f;
                        success_music_lerp_target = 0f;
                        percent_to_mars = Mathf.Lerp(0.3f, 0.45f, current_layer_percent);
                        return;
                    case 6:
                        current_layer = AtmosphereLayer.ALIENS;
                        ground_to_sky = 1f;
                        earth_to_space = 1f;
                        space_to_mars = 0f;
                        space_intensity = space_intensity_max;
                        drag = 0.3f;
                        visual_drag = 0f;
                        gravity = 0f;
                        height_cash = (ulong)Mathf.Lerp(160000f, 320000f, current_layer_percent);
                        space_music_lerp_target = 0f;
                        alien_music_lerp_target = 1f;
                        success_music_lerp_target = 0f;
                        percent_to_mars = Mathf.Lerp(0.45f, 0.6f, current_layer_percent);
                        return;
                    case 7:
                        current_layer = AtmosphereLayer.PEACEFUL_SPACE;
                        ground_to_sky = 1f;
                        earth_to_space = 1f;
                        space_to_mars = current_layer_percent;
                        space_intensity = space_intensity_max * space_intensity_curve.Evaluate(1f - current_layer_percent);
                        drag = Mathf.Lerp(0.3f, 0.5f, current_layer_percent);
                        visual_drag = Mathf.Lerp(0f, 1f, current_layer_percent);
                        gravity = Mathf.Lerp(0f, 9.8f, current_layer_percent);
                        height_cash = (ulong)Mathf.Lerp(320000f, 640000f, current_layer_percent);
                        space_music_lerp_target = 0f;
                        alien_music_lerp_target = 0f;
                        success_music_lerp_target = 0f;
                        percent_to_mars = Mathf.Lerp(0.6f, 0.75f, current_layer_percent);
                        return;
                    default:
                        current_layer = AtmosphereLayer.MARS;
                        ground_to_sky = 1f;
                        earth_to_space = 1f;
                        space_to_mars = 1f;
                        space_intensity = 0f;
                        drag = 0.5f;
                        visual_drag = Mathf.Lerp(1f, 0f, current_layer_percent);
                        gravity = 9.8f;
                        height_cash = (ulong)Mathf.Lerp(640000f, 1280000f, current_layer_percent);
                        space_music_lerp_target = 0f;
                        alien_music_lerp_target = 0f;
                        success_music_lerp_target = 1f;
                        percent_to_mars = Mathf.Lerp(0.75f, 1f, current_layer_percent);
                        return;
                }
            }
        }


        current_layer = AtmosphereLayer.TROPOSPHERE;
        ground_to_sky = 0f;
        earth_to_space = 0f;
        space_to_mars = 0f;
        space_intensity = 0f;
        drag = 0.2f;
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

    public float get_moon_dot() {
        return -Vector3.Dot(Vector3.down, sun.transform.forward);
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
            return new Vector3(0f, gravity, 0f) * underwater_gravity_multiplier;
        } else {
            return new Vector3(0f, gravity, 0f);
        }
    }

    public float get_height_cash(float height) { return height_cash; }
}
