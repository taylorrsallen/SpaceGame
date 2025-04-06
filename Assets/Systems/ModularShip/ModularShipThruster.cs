using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ModularShipThruster : ModularShipActivatable {
    public Vector3 thrustDirection = new Vector3(0f, 1f, 0f);

    [SerializeField] public ModularShipThrusterData data;
    [SerializeField] public GameObject active_effect;
    [SerializeField] public ParticleSystem active_particles;

    private AudioSource audio_source;

    private float malfunction_cooldown;
    private float malfunction_cooldown_timer;
    private float malfunction_duration;
    private float malfunction_duration_timer;

    private float volume_target;
    [SerializeField] private float volume_lerp_speed = 15f;

    private void Awake() {
        audio_source = GetComponent<AudioSource>();
        audio_source.clip = data.thrust_sound;
        audio_source.loop = true;
        audio_source.Stop();
    }

    private void Update() {
        audio_source.volume = Mathf.Lerp(audio_source.volume, volume_target, Time.deltaTime * volume_lerp_speed);

        if (audio_source.volume < 0.01f && audio_source.isPlaying) { audio_source.Stop(); }
    }

    public override void set_active(bool active) {
        if (active) {
            if (active_effect) active_effect.SetActive(true);
            if (active_particles && !active_particles.isEmitting) active_particles.Play();
            volume_target = 1f;
            if (!audio_source.isPlaying) audio_source.Play();
        } else {
            if (active_effect) active_effect.SetActive(false);
            if (active_particles && active_particles.isEmitting) active_particles.Stop();
            volume_target = 0f;
        }
    }

    public override void execute(ModularShipController ship_controller) {
        if (Random.value < data.malfunction_chance) {
            Debug.Log("Malfunction!");
        }

        ship_controller.rb.AddForceAtPosition(transform.up * data.thrust_force, transform.position);
        Debug.DrawLine(transform.position, transform.position - transform.up * data.thrust_force * 0.01f, Color.red);
    }
}
