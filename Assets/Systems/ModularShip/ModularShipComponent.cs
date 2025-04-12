using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ModularShipComponent : MonoBehaviour, IDamageable, IKnockbackable {
    private AudioSource audio_source;

    [SerializeField] public float health = 10f;

    [SerializeField] public ShipComponentData data;
    [SerializeField] public ShipComponentRuntimeData runtime_data;

    float collision_velocity_to_damage_multiplier = 0.5f;
    float collision_velocity_to_volume_multiplier = 0.05f;

    public ModularShipController modular_ship_controller;

    public void init() {
        ModularShipActivator activator = GetComponent<ModularShipActivator>();
        if (activator) {
            activator.hotkey = runtime_data.activator_hotkey;
            activator.can_deactivate = runtime_data.activator_can_deactivate;
            activator.toggle = runtime_data.activator_toggle;
            activator.fuel_usage_per_second = data.activator_fuel_usage_per_second;
        }

        ModularShipFuelContainer fuel_container = GetComponent<ModularShipFuelContainer>();
        if (fuel_container) {
            fuel_container.fuel_capacity = data.fuel_capacity;
            fuel_container.fuel = data.fuel_capacity;
        }
    }

    private void Awake() {
        audio_source = GetComponent<AudioSource>();

        health = data.max_health;
    }

    public void damage(DamageArgs args) {
        health -= args.damage;
        if (health <= 0f) die();
    }

    public void knockback(Vector3 force) {
        modular_ship_controller.knockback_from_component(this, force);
    }

    public void die() {
        SoundManager.instance.play_sound_3d_pitched(data.audio_data.destroy_sounds.get_sound(), transform.position);
        Transform explosion = Instantiate(data.audio_data.explosion_effect).transform;
        explosion.position = transform.position;
        modular_ship_controller.on_component_destroyed(this);
    }

    public void apply_collision(Collision collision) {
        // Debug.Log("[" + name + "] collided with " + collision.relativeVelocity + " velocity");
        audio_source.pitch = UnityEngine.Random.Range(0.8f, 1.2f);
        audio_source.PlayOneShot(data.audio_data.collision_sounds.get_sound(), Mathf.Clamp(collision.relativeVelocity.magnitude * collision_velocity_to_volume_multiplier, data.audio_data.collision_sound_range.x, data.audio_data.collision_sound_range.y));
    
        float damage_to_deal = collision.relativeVelocity.magnitude * collision_velocity_to_damage_multiplier;
        damage_to_deal -= damage_to_deal * data.percent_armor;
        damage_to_deal -= data.flat_armor;
        damage(new DamageArgs(damage_to_deal));
    }
}
