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

    Vector3 center_of_mass_target;
    float center_of_mass_lerp_speed = 15f;

    public bool is_in_water = false;

    private void init() {
        activators = GetComponentsInChildren<ModularShipActivator>();
        fuel_containers = GetComponentsInChildren<ModularShipFuelContainer>();

        foreach (ModularShipComponent ship_component in GetComponentsInChildren<ModularShipComponent>()) ship_component.modular_ship_controller = this;

        rb.WakeUp();
        rb.interpolation = RigidbodyInterpolation.None;

        force_update_center_of_mass();
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.ResetInertiaTensor();
        velocity_override = Vector3.zero;

        rb.interpolation = RigidbodyInterpolation.Interpolate;
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
        if (components.childCount == 0) {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            return;
        }

        update_center_of_mass_target();
        update_center_of_mass();

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

        rb.AddForce(AtmosphereManager.instance.get_gravity() * rb.mass);
    }

    private void OnCollisionEnter(Collision collision) {
        ModularShipComponent ship_component = collision.GetContact(0).thisCollider.GetComponent<ModularShipComponent>();
        if (ship_component != null) {
            ship_component.apply_collision(collision);
            velocity_override = previous_velocity - (ship_component.data.collision_velocity_damper * previous_velocity) / (rb.mass * 0.25f);
        }
    }

    public Vector3 get_ship_position() { return transform.position + transform.TransformDirection(rb.centerOfMass); }
    public Vector3 get_velocity() { return rb.linearVelocity; }
    public float get_ship_mass() { return rb.mass; }
    public float get_ship_radius() { return 5f; }

    public void update_center_of_mass() {
        rb.centerOfMass = Vector3.Lerp(rb.centerOfMass, center_of_mass_target, Time.fixedDeltaTime * center_of_mass_lerp_speed);
        Util.DrawAABB2D(rb.position + transform.TransformDirection(rb.centerOfMass) - Vector3.one * 0.5f, Vector2.one, Color.blue);
    }

    public void update_center_of_mass_target() {
        if (components.childCount == 0) {
            center_of_mass_target = Vector3.zero;
            rb.mass = 0;
            return;
        }

        float total_mass = 0f;
        Vector3 mass_sum = Vector3.zero;
        foreach (ModularShipComponent component in components.GetComponentsInChildren<ModularShipComponent>()) {
            mass_sum += component.transform.localPosition * component.data.mass;
            total_mass += component.data.mass;
        }

        center_of_mass_target = mass_sum / total_mass;
        rb.mass = total_mass;
    }

    public void force_update_center_of_mass() {
        if (components.childCount == 0) {
            rb.centerOfMass = Vector3.zero;
            center_of_mass_target = Vector3.zero;
            rb.mass = 0;
            return;
        }

        float total_mass = 0f;
        Vector3 mass_sum = Vector3.zero;
        foreach (ModularShipComponent component in components.GetComponentsInChildren<ModularShipComponent>()) {
            mass_sum += component.transform.localPosition * component.data.mass;
            total_mass += component.data.mass;
        }

        rb.centerOfMass = mass_sum / total_mass;
        center_of_mass_target = rb.centerOfMass;
        rb.mass = total_mass;
    }

    public void load_blueprint(ModularShipBlueprintData _blueprint) {
        clear();
        blueprint = _blueprint;
        grid.load_blueprint_as_functional(blueprint, components);
        init();
    }

    public void set_active(bool active) {
        if (active) {
            gameObject.SetActive(true);
        } else {
            gameObject.SetActive(false);
        }
    }

    public void clear() {
        foreach (Transform child_transform in components.GetComponentsInChildren<Transform>()) {
            if (child_transform.gameObject == components.gameObject) continue;
            Destroy(child_transform.gameObject);
        }

        grid.clear();

        transform.position = GameManager.instance.spawn_point;
        transform.rotation = Quaternion.identity;

        exit_water();
    }

    public void on_component_destroyed(ModularShipComponent ship_component) {
        update_center_of_mass_target();
        Destroy(ship_component.gameObject);
    }

    public void knockback_from_component(ModularShipComponent component, Vector3 force) {
        rb.AddForceAtPosition(force, component.transform.localPosition);
    }

    public void enter_water() {
        if (is_in_water) return;
        is_in_water = true;
        rb.linearDamping = 0.5f;
        rb.angularDamping = 0.5f;
        rb.linearVelocity *= 0.3f;
        rb.angularVelocity *= 0.3f;
    }

    public void exit_water() {
        if (!is_in_water) return;
        is_in_water = false;
        rb.linearDamping = 0f;
        rb.angularDamping = 0f;
    }
}
