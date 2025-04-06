using UnityEngine;

[CreateAssetMenu(fileName = "ModularShipThrusterData", menuName = "Scriptable Objects/ModularShipThrusterData")]
public class ModularShipThrusterData : ScriptableObject {
    [SerializeField] public float thrust_force = 100f;
    [SerializeField] public float thrust_fuel_per_second = 1f;
    [SerializeField] public AudioClip thrust_sound;
    
    [SerializeField] public bool can_deactivate = true;

    [SerializeField] public float malfunction_chance = 0f;
    [SerializeField] public Vector2 malfunction_duration_range = new Vector2(0.1f, 2f);
    [SerializeField] public AudioClip malfunction_sound;
    [SerializeField] public Vector2 malfunction_cooldown_range = new Vector2(0.5f, 1f);
}
