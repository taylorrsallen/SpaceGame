using UnityEngine;

public class ModularShipEmitter : ModularShipActivatable {
    public ModularShipEmitterData data;
    public GameObject active_effect;
    public ParticleSystem active_particles;
    public ParticleSystem emitter_particles;
    private float emission_timer;

    private void Update() {
        emission_timer = Mathf.Min(emission_timer + Time.deltaTime, data.emission_rate);
    }

    public override void set_active(bool active) {
        if (active) {
            if (active_effect) active_effect.SetActive(true);
            if (active_particles && !active_particles.isEmitting) active_particles.Play();
        //     volume_target = 1f;
        //     if (!audio_source.isPlaying) audio_source.Play();
        } else {
            if (active_effect) active_effect.SetActive(false);
            if (active_particles && active_particles.isEmitting) active_particles.Stop();
        //     volume_target = 0f;
        }
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

        data.emission_effect.Execute(new GameEffectArgs(gameObject, null, transform.position));
    }

    public void malfunction() {
        Debug.Log("Malfunction!");
    }
}
