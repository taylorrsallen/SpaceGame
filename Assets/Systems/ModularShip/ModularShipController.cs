using UnityEngine;

public class ModularShipController : MonoBehaviour {
    private Character character;
    public Rigidbody rb;

    private ModularShipActivator[] activators;

    private float fuel;

    private void Awake() {
        character = GetComponent<Character>();
        rb = GetComponent<Rigidbody>();
        activators = GetComponentsInChildren<ModularShipActivator>();
        
        rb.mass = 0f;
        foreach (ModularShipComponent ship_component in GetComponentsInChildren<ModularShipComponent>()) {
            rb.mass += ship_component.data.mass;
            ship_component.modular_ship_controller = this;
        }
    }

    private void Update() {
        foreach (ModularShipActivator activator in activators) {
            if (activator == null) continue;
            activator.update_active_state();
        }
    }

    private void FixedUpdate() {
        foreach (ModularShipActivator activator in activators) {
            if (activator == null) continue;
            activator.update_activatables(this);
        }

        float world_half_extent = AtmosphereManager.instance.world_horizontal_size * 0.5f;
        if (transform.position.x >= world_half_extent) {
            transform.position = new Vector3(transform.position.x - AtmosphereManager.instance.world_horizontal_size, transform.position.y, transform.position.z);
        } else if (transform.position.x <= -world_half_extent) {
            transform.position = new Vector3(transform.position.x + AtmosphereManager.instance.world_horizontal_size, transform.position.y, transform.position.z);
        }
    }

    private void OnCollisionEnter(Collision collision) {
        ModularShipComponent ship_component = collision.GetContact(0).thisCollider.GetComponent<ModularShipComponent>();
        if (ship_component != null) ship_component.apply_collision(collision);
    }

    public void on_component_destroyed(ModularShipComponent ship_component) {
        rb.mass -= ship_component.data.mass;
        Destroy(ship_component.gameObject);
    }
}
