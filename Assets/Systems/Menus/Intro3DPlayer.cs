using UnityEngine;

public class Intro3DPlayer : MonoBehaviour, IIntroPlayer {
    private AudioSource audio_source;

    private bool _is_playing;
    private bool _is_finished;
    private IntroState state;

    private float play_timer;
    private bool is_audio_played;

    [Header("Setup")]
    public Material render_material;
    public MeshRenderer render_target;
    public AudioClip intro_audio;

    [Header("Animation")]
    public float audio_delay = 0.3f;
    public float fade_in_time = 0.5f;
    public float hold_time = 2f;
    public float fade_out_time = 1f;
    public float emission_intensity = 1.5f;

    public void init() {
        audio_source = GetComponent<AudioSource>();
    }

    private void Update() {
        if (!_is_playing || _is_finished) return;

        play_timer += Time.deltaTime;

        if(!is_audio_played && play_timer >= audio_delay) play_audio();

        if(play_timer > (fade_in_time + hold_time + fade_out_time)) {
            // Finished
            render_material.color = Color.black;
            render_material.SetColor("_EmissionColor", render_material.color * emission_intensity);
            _is_finished = true;
        } else if(play_timer > (fade_in_time + hold_time)) {
            // Fade out
            render_material.color = Color.Lerp(Color.white, Color.black, (play_timer - fade_in_time - hold_time) / fade_out_time);
            render_material.SetColor("_EmissionColor", render_material.color * emission_intensity);
        } else if(play_timer > fade_in_time) {
            // Hold
            render_material.color = Color.white;
            render_material.SetColor("_EmissionColor", render_material.color * emission_intensity);
        } else {
            // Fade in
            render_material.color = Color.Lerp(Color.black, Color.white, play_timer / fade_in_time);
            render_material.SetColor("_EmissionColor", render_material.color * emission_intensity);
        }
    }

    private void play_audio() {
        is_audio_played = true;
        if (intro_audio) audio_source.PlayOneShot(intro_audio);
    }

    public void play() {
        render_target.material = render_material;
        state = IntroState.FADE_IN;
        play_timer = 0f;
        _is_playing = true;
    }

    public bool is_playing() { return _is_playing; }
    public bool is_finished() { return _is_finished; }
}
