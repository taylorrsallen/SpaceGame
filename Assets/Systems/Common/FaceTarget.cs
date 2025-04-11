using Unity.VisualScripting;
using UnityEngine;

public class FaceTarget : MonoBehaviour {
    public Transform target;

    public float turn_speed = 100f;
    private Vector3 euler_target;

    private void Update() {
        if (!target) return;
        if (!is_facing_point(target.transform.position)) {
            ModularShipController ship_controller = target.GetComponent<ModularShipController>();
            if (ship_controller) {
                face_point(ship_controller.get_ship_position());
            } else {
                face_point(target.position);
            }
        }
    }

    public void face_point(Vector3 point) {
        Quaternion target_rotation = Quaternion.LookRotation(point - transform.position);
        float turn_amount = Mathf.Min(turn_speed * Time.deltaTime, 1f);
        transform.rotation = Quaternion.Lerp(transform.rotation, target_rotation, turn_amount);
    }

    public bool is_facing_point(Vector3 point) {
        Vector3 local_point = transform.TransformPoint(point);
	    return local_point.z < 0 && Mathf.Abs(local_point.x) < 0.1f;
    }
}
//   lookPos.y = 0;
//   var rotation = Quaternion.LookRotation(lookPos);
//   transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping); 

// # (({[%%%(({[=======================================================================================================================]}))%%%]}))
// @export var turn_speed: float = 100.0

// # (({[%%%(({[=======================================================================================================================]}))%%%]}))
// func face_point(point: Vector3, delta: float) -> void:
// 	var l_point: Vector3 = to_local(point)
// 	l_point.y = 0.0
// 	var turn_direction: float = sign(l_point.x)
// 	var turn_amount: float = deg_to_rad(turn_speed * delta)
// 	var angle: float = Vector3.FORWARD.angle_to(l_point)
	
// 	if angle < turn_amount: turn_amount = angle
// 	rotate_object_local(Vector3.UP, -turn_amount * turn_direction)

// func is_facing_point(point: Vector3) -> bool:
// 	var l_point = to_local(point)
// 	return l_point.z < 0 && abs(l_point.x) < 0.1