using UnityEngine;

public class ModularShipController : MonoBehaviour {
    private Character character;
    public Rigidbody rb;

    public ModularShipBlueprintData blueprint;
    public Grid3D grid;
    public Transform components;

    private ModularShipActivator[] activators;

    Vector3 previous_velocity;
    Vector3 velocity_override;

    public Vector3 last_valid_camera_anchor_point;
    public Vector3 last_valid_camera_anchor_rotation;

    private void init() {
        activators = GetComponentsInChildren<ModularShipActivator>();

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
            activator.update_active_state();
        }

        if (components.childCount > 0) {
            last_valid_camera_anchor_point = transform.position + transform.TransformDirection(rb.centerOfMass);
            last_valid_camera_anchor_rotation = new Vector3(0f, 0f, transform.rotation.eulerAngles.z);
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
