using UnityEngine;

[CreateAssetMenu(fileName = "MatterData", menuName = "Scriptable Objects/MatterData")]
public class MatterData : ScriptableObject {
    [SerializeField] public string data_name;
    [SerializeField] public float stop_friction = 1f;
    [SerializeField] public float move_friction = 1f;
    [SerializeField] public float max_speed_multiplier = 1f;
}
