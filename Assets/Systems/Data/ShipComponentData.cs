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

    [SerializeField] public bool activator = false;
    [SerializeField] public string hotkey = "space";
    [SerializeField] public bool active = false;
    [SerializeField] public bool can_deactivate = true;
    [SerializeField] public bool toggle = false;
}
