using UnityEngine;

public class Character : MonoBehaviour {
    [SerializeField] public Vector2 move_input;
    [SerializeField] public Vector3 world_move_input;
    [SerializeField] public Vector3 face_direction;

    [SerializeField] public IController controller;

    // FLAGS
    [SerializeField] public bool grounded;
    [SerializeField] public bool crouch;
    [SerializeField] public bool jump;

    // GROUND
    [SerializeField] public byte ground_matter_id;

    public bool movement_lock;
    public bool animation_action_interruptable;
    public bool animation_movement_interruptable;

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public void set_face_direction(Vector3 _face_direction) {
        if (!movement_lock) face_direction = _face_direction;
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public void primary() {
        
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public void set_attack_active() {

    }
    
    public void set_attack_inactive() {

    }
}
