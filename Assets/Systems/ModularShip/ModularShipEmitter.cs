using UnityEngine;

public class ModularShipEmitter : ModularShipActivatable, IKnockbackable {
    private ModularShipComponent component_base;

    public ModularShipEmitterData data;
    public GameObject active_effect;
    public ParticleSystem active_particles;
    public ParticleSystem emitter_particles;
    private float emission_timer;

    public AudioSource active_source;
    public AudioClip active_one_shot;
    private float volume_target;
    [SerializeField] private float volume_lerp_speed = 15f;

    private void Update() {
        emission_timer = Mathf.Min(emission_timer + Time.deltaTime, data.emission_rate);

        if(active_source) {
            active_source.volume = Mathf.Lerp(active_source.volume, volume_target, Time.deltaTime * volume_lerp_speed);
            if(active_source.volume < 0.01f && active_source.isPlaying) { active_source.Stop(); }
        }
    }

    public override void set_active(bool active) {
        if (active) {
            if(active_effect) active_effect.SetActive(true);
            if(active_particles && !active_particles.isEmitting) active_particles.Play();
            if(active_source) {
                volume_target = 1f;
                if (!active_source.isPlaying) active_source.Play();
            }
        } else {
            if(active_effect) active_effect.SetActive(false);
            if(active_particles && active_particles.isEmitting) active_particles.Stop();
            volume_target = 0f;
        }
    }

    public override void set_component_base(ModularShipComponent component) {
        component_base = component;
    }

    public override void execute(ModularShipController ship_controller) {
        if (emission_timer >= data.emission_rate) {
            emission_timer -= data.emission_rate;
        } else {
            return;
        }

        if (Random.value < data.malfunction_chance) {
            malfunction();
            return;
        }

        if (emitter_particles) {
            Vector3 emission_velocity = GameManager.instance.ship_controller.get_velocity() + transform.forward * data.emission_particle_velocity;
            emitter_particles.Emit(data.emission_particle_burst);
        }

        data.emission_effect.Execute(new GameEffectArgs(gameObject, ship_controller.gameObject, transform.position));
    }

    public void malfunction() {
        Debug.Log("Malfunction!");
    }

    public void knockback(Vector3 force) {
        component_base.knockback(force);
    }
}
