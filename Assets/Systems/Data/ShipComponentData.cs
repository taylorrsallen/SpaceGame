using UnityEngine;

[CreateAssetMenu(fileName = "ShipComponentData", menuName = "Scriptable Objects/ShipComponentData")]
public class ShipComponentData : ScriptableObject {
    [SerializeField] public float mass = 5f;

    [SerializeField] public float max_health = 10f;
    [SerializeField] public float percent_armor = 0f;
    [SerializeField] public float flat_armor = 0f;
}
