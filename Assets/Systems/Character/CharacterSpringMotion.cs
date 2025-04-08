using UnityEngine;

public class CharacterSpringMotion : MonoBehaviour {
    private Character character;
    private Rigidbody rb;

    [Header("Movement")]
    [SerializeField] public float max_speed = 10f;
    [SerializeField] public float max_down_slope_speed = 20f;
    [SerializeField] public float max_air_speed = 40f;
    [SerializeField] public float max_acceleration = 100f;
    [SerializeField] public float max_down_slope_acceleration = 200f;
    [SerializeField] public AnimationCurve max_acceleration_from_dot;
    [Space(10f)]
    [SerializeField] public float air_control = 0.3f;
    [SerializeField] public float gravity_multiplier = 2f;
    [SerializeField] public float jump_velocity = 10f;
    [Space(10f)]
    [SerializeField] public float ground_angle;
    [SerializeField] public Vector3 slope_direction;
    [SerializeField] public float slope_movement;

    [Header("Float")]
    [SerializeField] public Vector3 down_direction = -Vector3.up;
    [SerializeField] public float down_ray_height_offset = 1.5f;
    [SerializeField] public float down_ray_distance = 2f;
    [Space(10f)]
    [SerializeField] public float ride_height = 1.5f;
    [SerializeField] public float ride_spring_strength = 2000f;
    [SerializeField] public float ride_spring_damper = 100f;
    [SerializeField] public float torque_spring_strength = 2000f;
    [SerializeField] public float torque_spring_damper = 30f;
    [SerializeField] public Quaternion upright_joint_target_rotation = Quaternion.identity;

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    void Start() {
        character = GetComponent<Character>();
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate() {
        upright_joint_target_rotation = Quaternion.LookRotation(character.face_direction, -down_direction);

        update_ride_force();
        update_upright_force();
        if (!character.movement_lock) update_movement_force();
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    void update_movement_force() {
        Vector3 velocity = rb.linearVelocity;
        MatterData ground_matter = DataManager.get_matter(character.ground_matter_id);

        float current_max_speed = max_speed;
        if (character.grounded) {
            current_max_speed *= ground_matter.max_speed_multiplier;
            if (ground_angle < 0.999) {
                current_max_speed = Mathf.Lerp(max_speed, max_down_slope_speed, 1f - Mathf.Clamp01(ground_angle));
                velocity += Vector3.down * 5f * Time.fixedDeltaTime;
            }
        }

        Vector3 desired_velocity = new Vector3(character.world_move_input.x, 0f, character.world_move_input.z).normalized * current_max_speed;
        float max_speed_change = max_acceleration * Time.fixedDeltaTime;

        Vector3 move_velocity_temp = velocity;
        move_velocity_temp.y = 0f;
        if (character.grounded) {
            if (desired_velocity.magnitude < 0.05) {
                move_velocity_temp = Vector3.MoveTowards(move_velocity_temp, desired_velocity, max_speed_change * ground_matter.stop_friction);
            } else {
                move_velocity_temp = Vector3.MoveTowards(move_velocity_temp, desired_velocity, max_speed_change * ground_matter.move_friction);
            }
        } else {
            move_velocity_temp = Vector3.MoveTowards(move_velocity_temp, desired_velocity, max_speed_change * air_control);
            velocity.y = Mathf.Min(velocity.y - 9.8f * Time.fixedDeltaTime * gravity_multiplier, max_air_speed);
        }

        velocity.x = move_velocity_temp.x;
        velocity.z = move_velocity_temp.z;

        rb.linearVelocity = velocity;
    }

    void update_ride_force() {
        RaycastHit hit;

        if (Physics.Raycast(transform.position + Vector3.up * down_ray_height_offset, down_direction, out hit, down_ray_distance, 1)) {
            if (hit.distance < ride_height * 1.1) {
                character.grounded = true;

                IMatterCollider matter_collider = hit.collider.GetComponent<IMatterCollider>();
                character.ground_matter_id = matter_collider != null ? matter_collider.get_matter_id(hit.point) : (byte)0;

                ground_angle = Vector3.Dot(hit.normal, transform.up);
                slope_direction = Vector3.Cross(Vector3.right, hit.normal).normalized;
                slope_movement = Vector3.Dot(new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z), slope_direction);
            } else {
                character.grounded = false;
            }

            Vector3 velocity = rb.linearVelocity;

            Vector3 other_velocity = Vector3.zero;
            Rigidbody hit_body = hit.rigidbody;

            if (hit_body != null) { other_velocity = hit_body.linearVelocity; }

            float ray_direction_velocity = Vector3.Dot(down_direction, velocity);
            float other_direction_velocity = Vector3.Dot(down_direction, other_velocity);

            float relative_velocity = ray_direction_velocity - other_direction_velocity;

            float x = hit.distance - ride_height;

            float spring_force = (x * ride_spring_strength) - (relative_velocity * ride_spring_damper);
            
            rb.AddForce(down_direction * spring_force);

            if (hit_body != null) { hit_body.AddForceAtPosition(down_direction * -spring_force, hit.point); }
        } else {
            character.grounded = false;
        }
    }

    void update_upright_force() {
        Quaternion current_rotation = transform.rotation;
        Quaternion to_goal = Util.shortest_rotation(upright_joint_target_rotation, current_rotation);

        Vector3 rotation_axis;
        float rotation_degrees;

        to_goal.ToAngleAxis(out rotation_degrees, out rotation_axis);
        rotation_axis.Normalize();

        float rotation_radians = rotation_degrees * Mathf.Deg2Rad;

        rb.AddTorque((rotation_axis * (rotation_radians * torque_spring_strength)) - (rb.angularVelocity * torque_spring_damper));
    }
}
