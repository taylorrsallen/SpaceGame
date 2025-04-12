using System;
using UnityEngine;

public enum ShipComponentType {
    PILOT,
    PASSENGER,
    FUEL,
    EMITTER,
    MISC,
}

[CreateAssetMenu(fileName = "ShipComponentData", menuName = "Scriptable Objects/ShipComponentData")]
public class ShipComponentData : ScriptableObject {
    [SerializeField] public ShipComponentType[] types;

    [SerializeField] public float mass = 5f;

    [SerializeField] public float max_health = 10f;
    [SerializeField] public float percent_armor = 0f;
    [SerializeField] public float flat_armor = 0f;

    [SerializeField] public Vector2Int grid_dimensions = new Vector2Int(2, 2);
    [SerializeField] public GameObject mesh_prefab;
    [SerializeField] public ModularShipComponent component_prefab;

    [SerializeField] public ShipComponentAudioData audio_data;
    
    [SerializeField] public float collision_velocity_damper = 0.1f;

    [SerializeField] public GameEffectData death_effects;
    [SerializeField] public Vector2 death_delay_range = new Vector2(0.02f, 0.07f);

    // Activator
    [SerializeField] public bool activator = false;
    [SerializeField] public float activator_fuel_usage_per_second = 0.5f;

    // Default activator runtime data
    [SerializeField] public KeyCode activator_hotkey = KeyCode.Space;
    [SerializeField] public bool activator_can_deactivate = true;
    [SerializeField] public bool activator_toggle = false;

    // Fuel
    [SerializeField] public bool fuel_container = false;
    [SerializeField] public int fuel_capacity = 100;
    // Default fuel container runtime data

}

public class ShipComponentRuntimeData {
    // Activator
    public KeyCode activator_hotkey = KeyCode.Space;
    public bool activator_can_deactivate = true;
    public bool activator_toggle = false;

    // Fuel
    public float fuel;

    public void set_defaults(ShipComponentData component_data) {
        activator_hotkey = component_data.activator_hotkey;
        activator_can_deactivate = component_data.activator_can_deactivate;
        activator_toggle = component_data.activator_toggle;

        fuel = component_data.fuel_capacity;
    }
}