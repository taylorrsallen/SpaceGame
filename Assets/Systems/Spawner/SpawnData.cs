using UnityEngine;

[CreateAssetMenu(fileName = "SpawnData", menuName = "Scriptable Objects/SpawnData")]
public class SpawnData : ScriptableObject {
    public GameObject spawn_prefab;

    public SpawnDirection spawn_direction;
    public AtmosphereLayer[] atmosphere_layers;
    public float spawn_weight = 1f;
    /// <summary>
    /// 1 per [spawn_rate] seconds
    /// </summary>
    public float spawn_rate = 10f;

    public uint max_spawned = 5;
}
