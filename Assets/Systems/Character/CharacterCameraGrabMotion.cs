using UnityEngine;

public class CharacterCameraGrabMotion : MonoBehaviour {
    public float speed;
    public Vector2 x_bounds = new Vector2(-10f, 10f);
    public Vector2 y_bounds = new Vector2(-10f, 10f);

    public void apply_camera_grab(Vector2 look_input, float zoom) {
        float effective_speed = speed * zoom * 0.1f * Time.unscaledDeltaTime;
        transform.position = new Vector3(Mathf.Clamp(transform.position.x - look_input.x * effective_speed, x_bounds.x, x_bounds.y), Mathf.Clamp(transform.position.y - look_input.y * effective_speed, y_bounds.x, y_bounds.y));
    }
}
