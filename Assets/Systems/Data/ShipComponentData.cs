using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

public enum ShipComponentType {
    PILOT,
    PASSENGER,
    FUEL,
    EMITTER,
    MISC,
}

[CreateAssetMenu(fileName = "ShipComponentData", menuName = "Scriptable Objects/ShipComponentData")]
public class ShipComponentData : SerializedScriptableObject {
    [TabGroup("Base")] public ShipComponentType[] types;
    [TabGroup("Base")] public int id = 0;
    [TabGroup("Base")] public float mass = 5f;
    [TabGroup("Base")] public float max_health = 10f;
    [TabGroup("Base")] public float percent_armor = 0f;
    [TabGroup("Base")] public float flat_armor = 0f;

    [TabGroup("Inventory")] public Vector2Int grid_dimensions = new Vector2Int(2, 2);
    [TabGroup("Inventory")] public GameObject mesh_prefab;
    [TabGroup("Inventory")] public ModularShipComponent component_prefab;

    [TabGroup("Damage")] public float collision_velocity_damper = 0.1f;
    [TabGroup("Damage")] public GameEffectData death_effects;
    [TabGroup("Damage")] public Vector2 death_delay_range = new Vector2(0.02f, 0.07f);

    [TabGroup("Shop"), NonSerialized, OdinSerialize] public GameResource value;

    [SerializeField] public ShipComponentAudioData audio_data;

    // Activator
    [Header("Activator")]
    public bool activator = false;
    [ShowIf("activator")] public float activator_fuel_usage_per_second = 0.5f;

    // Default activator runtime data
    [ShowIf("activator")] public KeyCode activator_hotkey = KeyCode.Space;
    [ShowIf("activator")] public bool activator_can_deactivate = true;
    [ShowIf("activator")] public bool activator_toggle = false;

    // Fuel
    [Header("Fuel")]
    public bool fuel_container = false;
    [ShowIf("fuel_container")] public int fuel_capacity = 100;
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

    public void set_non_editable_defaults(ShipComponentData component_data) {
        activator_can_deactivate = component_data.activator_can_deactivate;
        fuel = component_data.fuel_capacity;
    }
}