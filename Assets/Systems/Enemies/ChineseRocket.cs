using UnityEngine;

public class ChineseRocket : MonoBehaviour {
    public float speed = 1f;

    private void Update() {
        move_forward_z_locked();
    }

    public void move_forward_z_locked() {
        Vector3 move_direction = transform.forward;
        move_direction.z = 0f;
        move_direction.Normalize();

        transform.position += move_direction * speed;
    }
}
