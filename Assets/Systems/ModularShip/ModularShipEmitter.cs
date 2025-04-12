using UnityEngine;

public class ModularShipEmitter : ModularShipActivatable {
    GameEffectData emission_effect;
    GameEffectData malfunction_effect;

    public float emission_rate = 0.2f;
    private float emission_timer;
    public float malfunction_chance = 0f;

    public override void set_active(bool active) {
        // if (active) {
        //     if (active_effect) active_effect.SetActive(true);
        //     if (active_particles && !active_particles.isEmitting) active_particles.Play();
        //     volume_target = 1f;
        //     if (!audio_source.isPlaying) audio_source.Play();
        // } else {
        //     if (active_effect) active_effect.SetActive(false);
        //     if (active_particles && active_particles.isEmitting) active_particles.Stop();
        //     volume_target = 0f;
        // }
    }

    public override void execute(ModularShipController ship_controller) {
        // if (Random.value < malfunction_chance) {
        //     Debug.Log("Malfunction!");
        // }

        // ship_controller.rb.AddForceAtPosition(transform.up * data.thrust_force, transform.position);
        // Debug.DrawLine(transform.position, transform.position - transform.up * data.thrust_force * 0.01f, Color.red);
    }
}
