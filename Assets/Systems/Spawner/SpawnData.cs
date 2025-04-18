using UnityEngine;

[CreateAssetMenu(fileName = "SpawnData", menuName = "Scriptable Objects/SpawnData")]
public class SpawnData : ScriptableObject {
    public GameObject spawn_prefab;

    public SpawnDirection spawn_direction;
    public AtmosphereLayer[] atmosphere_layers;
    public float spawn_weight = 1f;
    public float spawns_per_second = 10f;
}
