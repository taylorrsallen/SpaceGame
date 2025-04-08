using Unity.VisualScripting;
using UnityEngine;

public class ModularShipController : MonoBehaviour {
    private Character character;
    public Rigidbody rb;

    public ModularShipBlueprintData blueprint;
    public Grid3D grid;
    public Transform components;

    private ModularShipActivator[] activators;
    private ModularShipFuelContainer[] fuel_containers;

    Vector3 previous_velocity;
    Vector3 velocity_override;

    public Vector3 last_valid_camera_anchor_point;
    public Vector3 last_valid_camera_anchor_rotation;

    public float total_available_fuel;

    private void init() {
        activators = GetComponentsInChildren<ModularShipActivator>();
        fuel_containers = GetComponentsInChildren<ModularShipFuelContainer>();

        rb.mass = 0f;
        foreach (ModularShipComponent ship_component in GetComponentsInChildren<ModularShipComponent>()) {
            rb.mass += ship_component.data.mass;
            ship_component.modular_ship_controller = this;
        }
    }

    private void Awake() {
        character = GetComponent<Character>();
        rb = GetComponent<Rigidbody>();

        init();
    }

    private void Update() {
        foreach (ModularShipActivator activator in activators) {
            if (activator == null) continue;
            activator.update_active_state(total_available_fuel);
        }

        if (components.childCount > 0) {
            last_valid_camera_anchor_point = transform.position + transform.TransformDirection(rb.centerOfMass);
            last_valid_camera_anchor_rotation = new Vector3(0f, 0f, transform.rotation.eulerAngles.z);
        }
    }

    private void FixedUpdate() {
        total_available_fuel = 0f;
        foreach (ModularShipFuelContainer fuel_container in fuel_containers) {
            if (!fuel_container) continue;
            total_available_fuel += fuel_container.fuel;
        }
            
        // Debug.Log(total_available_fuel);

        float total_fuel_to_use = 0f;
        foreach (ModularShipActivator activator in activators) {
            if (activator == null) continue;
            total_fuel_to_use += activator.update_activatables_and_get_fuel_usage(this, total_available_fuel);
        }

        total_fuel_to_use *= Time.fixedDeltaTime;
        foreach (ModularShipFuelContainer fuel_container in fuel_containers) {
            if (!fuel_container) continue;
            if (fuel_container.fuel > 0f) {
                float fuel_to_use = Mathf.Min(total_fuel_to_use, fuel_container.fuel);
                total_fuel_to_use -= fuel_to_use;
                fuel_container.fuel -= fuel_to_use;
            }

            if (total_fuel_to_use == 0f) break;
        }

        float world_half_extent = AtmosphereManager.instance.world_horizontal_size * 0.5f;
        if (transform.position.x >= world_half_extent) {
            transform.position = new Vector3(transform.position.x - AtmosphereManager.instance.world_horizontal_size, transform.position.y, transform.position.z);
        } else if (transform.position.x <= -world_half_extent) {
            transform.position = new Vector3(transform.position.x + AtmosphereManager.instance.world_horizontal_size, transform.position.y, transform.position.z);
        }

        previous_velocity = rb.linearVelocity;
        if (velocity_override != Vector3.zero) {
            rb.linearVelocity = velocity_override;
            velocity_override = Vector3.zero;
        }
    }

    private void OnCollisionEnter(Collision collision) {
        ModularShipComponent ship_component = collision.GetContact(0).thisCollider.GetComponent<ModularShipComponent>();
        if (ship_component != null) {
            ship_component.apply_collision(collision);
            velocity_override = previous_velocity - (ship_component.data.collision_velocity_damper * previous_velocity) / (rb.mass * 0.25f);
        }
    }

    public void load_blueprint(ModularShipBlueprintData _blueprint) {
        clear();
        blueprint = _blueprint;
        grid.load_blueprint_as_functional(blueprint, components);
        init();
    }

    public void set_active(bool active) {
        if (active) {
            rb.isKinematic = false;
        } else {
            rb.isKinematic = true;
        }
    }

    public void clear() {
        foreach (Transform child_transform in components.GetComponentsInChildren<Transform>()) {
            if (child_transform.gameObject == components.gameObject) continue;
            Destroy(child_transform.gameObject);
        }

        grid.clear();

        transform.rotation = Quaternion.identity;
    }

    public void on_component_destroyed(ModularShipComponent ship_component) {
        rb.mass -= ship_component.data.mass;
        Destroy(ship_component.gameObject);
    }
}
