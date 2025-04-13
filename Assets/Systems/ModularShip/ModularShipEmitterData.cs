using UnityEngine;

[CreateAssetMenu(fileName = "ModularShipEmitterData", menuName = "Scriptable Objects/ModularShipEmitterData")]
public class ModularShipEmitterData : ScriptableObject {
    public GameEffectData emission_effect;
    public GameEffectData malfunction_effect;

    public float emission_rate = 0.2f;
    public float malfunction_chance = 0f;

    public int emission_particle_burst = 1;
    public float emission_particle_velocity = 10f;
}
