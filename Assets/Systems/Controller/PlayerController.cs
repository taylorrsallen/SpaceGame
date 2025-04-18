using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, IController {
    private PlayerControls player_controls;
    private InputAction jump_action;
    private InputAction crouch_action;
    private InputAction grab_camera_action;

    public Vector2 look_input;
    public Vector2 move_input;
    public Vector3 world_move_input;

    [SerializeField] public CameraRig camera_rig;
    [SerializeField] public Character character;

    void Start() {
        player_controls = new PlayerControls();
        player_controls.Enable();
        jump_action = player_controls.FindAction("Jump");
        jump_action.Enable();
        crouch_action = player_controls.FindAction("Crouch");
        crouch_action.Enable();
        grab_camera_action = player_controls.FindAction("GrabCamera");
        grab_camera_action.Enable();

        camera_rig.init();
    }

    void Update() {
        if (character != null) {
            if (grab_camera_action.IsPressed()) {
                CharacterCameraGrabMotion grab_motion = character.GetComponent<CharacterCameraGrabMotion>();
                if (grab_motion != null) grab_motion.apply_camera_grab(look_input, camera_rig.current_zoom);
            }
        }

        if(Input.GetKeyDown(KeyCode.F1)) toggle_camera_rotation();
    }

    void FixedUpdate() {
        if (camera_rig != null) {
            Transform camera_yaw = camera_rig.get_yaw();
            world_move_input = move_input.x * camera_yaw.right + world_move_input.y * camera_yaw.up + move_input.y * camera_yaw.forward;
        } else {
            world_move_input = Vector3.zero;
        }

        if (character != null) { update_character(); }
    }

    private void update_character() {
        character.move_input = move_input;
        character.world_move_input = world_move_input;
        if (world_move_input != Vector3.zero) character.set_face_direction(world_move_input);

        character.jump = jump_action.IsPressed() ? true : false;
        character.crouch = crouch_action.IsPressed() ? true : false;
    }

    private void OnPrimary() { character.primary(); }

    private void OnMove(InputValue input_value) {
        move_input = input_value.Get<Vector2>();
    }

    private void OnLook(InputValue input_value) {
        look_input = input_value.Get<Vector2>();
        if (camera_rig != null) { camera_rig.apply_look_rotation(look_input); }
    }

    private void OnZoom(InputValue input_value) {
        if (camera_rig != null) { camera_rig.apply_zoom(input_value.Get<float>()); }
    }

    private void OnToggleBuildMode(InputValue input_value) {
        Debug.Log("Toggle");
        GameManager.instance.toggle_ship_builder();
    }

    public void toggle_camera_rotation() {
        camera_rig.follow_z_rotation = !camera_rig.follow_z_rotation;
        camera_rig.zero_z_rotation = !camera_rig.zero_z_rotation;
    }
}
