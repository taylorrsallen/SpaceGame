using UnityEngine;

public class ModularShipFuelContainer : MonoBehaviour {
    public int fuel_capacity;
    public float fuel;

    private Material fuel_material;

    private void Awake() {
        fuel_material = GetComponent<MeshRenderer>().materials[1];
    }

    private void Update() {
        fuel_material.SetFloat("_Fill", fuel / fuel_capacity);
    }
}
