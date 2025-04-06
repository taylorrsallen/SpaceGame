using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class WindBlown : MonoBehaviour {
    private Rigidbody rb;

    void Awake() { rb = GetComponent<Rigidbody>(); }
    void FixedUpdate() { rb.AddForce(AtmosphereManager.instance.get_wind_force()); }
}
