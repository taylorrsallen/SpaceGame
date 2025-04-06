using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ModularShipComponent : MonoBehaviour {
    private AudioSource audio_source;

    [SerializeField] public float health = 10f;

    [SerializeField] public ShipComponentData data;
    [SerializeField] ShipComponentAudioData audio_data;
    
    [SerializeField] float collision_velocity_to_damage_multiplier = 0.5f;
    [SerializeField] float collision_velocity_to_volume_multiplier = 0.05f;

    public ModularShipController modular_ship_controller;

    private void Awake() {
        audio_source = GetComponent<AudioSource>();

        health = data.max_health;
    }

    public void damage(float amount, int type) {
        // Debug.Log("[" + name + "] took " + amount + " damage");
        health -= amount;
        if (health <= 0f) die();
    }

    public void die() {
        SoundManager.instance.play_sound_3d_pitched(audio_data.destroy_sounds.get_sound(), transform.position);
        modular_ship_controller.on_component_destroyed(this);
    }

    public void apply_collision(Collision collision) {
        // Debug.Log("[" + name + "] collided with " + collision.relativeVelocity + " velocity");
        audio_source.pitch = UnityEngine.Random.Range(0.8f, 1.2f);
        audio_source.PlayOneShot(audio_data.collision_sounds.get_sound(), Mathf.Clamp(collision.relativeVelocity.magnitude * collision_velocity_to_volume_multiplier, audio_data.collision_sound_range.x, audio_data.collision_sound_range.y));
    
        float damage_to_deal = collision.relativeVelocity.magnitude * collision_velocity_to_damage_multiplier;
        damage_to_deal -= damage_to_deal * data.percent_armor;
        damage_to_deal -= data.flat_armor;
        damage(damage_to_deal, 0);
    }
}
