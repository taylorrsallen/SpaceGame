using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ReactiveGlobalVolume : MonoBehaviour {
    private Volume volume;

    public Vector2 chromatic_aberration_range = new Vector2(0.1f, 1f);
    public Vector2 lens_distortion_range = new Vector2(0f, 1f);
    public Vector2 motion_blur_range = new Vector2(0f, 1f);
    public Vector2 bloom_intensity_range = new Vector2(4f, 40f);
    public Vector2 lens_flare_streak_range = new Vector2(0.25f, 1f);

    private ChromaticAberration chromatic_aberration;
    private LensDistortion lens_distortion;
    private MotionBlur motion_blur;
    private Bloom bloom;
    private ScreenSpaceLensFlare lens_flare;

    public float intensity_fade_rate = 1f;
    public float lerp_speed = 25f;

    public float explosion_intensity;

    private void Awake() {
        volume = GetComponent<Volume>();
        volume.profile.TryGet(out chromatic_aberration);
        volume.profile.TryGet(out lens_distortion);
        volume.profile.TryGet(out motion_blur);
        volume.profile.TryGet(out bloom);
        volume.profile.TryGet(out lens_flare);
    }

    private void Update() {
        explosion_intensity = Mathf.Max(explosion_intensity - Time.deltaTime * intensity_fade_rate, 0f);
        chromatic_aberration.intensity.value = Mathf.Lerp(chromatic_aberration_range.x, chromatic_aberration_range.y, explosion_intensity);
        lens_distortion.intensity.value = Mathf.Lerp(lens_distortion_range.x, lens_distortion_range.y, explosion_intensity);
        motion_blur.intensity.value = Mathf.Lerp(motion_blur_range.x, motion_blur_range.y, explosion_intensity);
        bloom.intensity.value = Mathf.Lerp(bloom_intensity_range.x, bloom_intensity_range.y, explosion_intensity);
        lens_flare.streaksIntensity.value = Mathf.Lerp(lens_flare_streak_range.x, lens_flare_streak_range.y, explosion_intensity);
    }

    public void apply_explosion_post_processing(float intensity) {
        explosion_intensity = Mathf.Min(explosion_intensity + intensity, 1f);
    }
}
