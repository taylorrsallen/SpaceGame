using UnityEngine;

public class CameraRig : MonoBehaviour {
    private Transform yaw;
    private Transform pitch;
    private Camera camera_3d;

    [SerializeField] Transform anchor_transform = null;
    [SerializeField] Vector3 anchor_point = Vector3.zero;
    [SerializeField] Vector3 anchor_offset = Vector3.zero;
    [SerializeField] float anchor_lerp_speed = 15f;

    [SerializeField] public Vector2 zoom_bounds = new Vector2(5f, 15f);
    [SerializeField] public float current_zoom = 5f;
    [SerializeField] public float target_zoom = 5f;
    [SerializeField] public float zoom_change_speed = 5f;

    public Vector2 vertical_look_bounds = new Vector2(-89f, 89f);
    public Vector2 current_look_rotation = new Vector2(45f, 0f);
    public Vector2 target_look_rotation = new Vector2(45f, 0f);
    public float look_lerp_speed = 15f;
    [SerializeField] public float look_sensitivity = 1f;

    public float focus_radius = 1f;

    public bool look_enabled = true;
    public bool zoom_enabled = true;
    public bool avoid_clipping = true;

    public bool follow_z_rotation = false;

    Vector3 CameraHalfExtents {
        get {
            Vector3 half_extents;
            half_extents.y = camera_3d.nearClipPlane * Mathf.Tan(0.5f * Mathf.Deg2Rad * camera_3d.fieldOfView);
            half_extents.x = half_extents.y * camera_3d.aspect;
            half_extents.z = 0f;
            return half_extents;
        }
    }

    #region Init
    void Awake() {
        yaw = transform.GetChild(0);
        pitch = yaw.GetChild(0);
        camera_3d = pitch.GetChild(0).gameObject.GetComponent<Camera>();
    }
    #endregion
    
    void LateUpdate() {
        _update_anchor_point();
        if (look_enabled) _update_look();
        _update_zoom();

        if (follow_z_rotation) camera_3d.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, anchor_transform.rotation.eulerAngles.z));
    }

    #region GetSet
    public Transform get_yaw() { return yaw; }
    public Transform get_pitch() { return pitch; }
    public Camera get_camera() { return camera_3d; }
    #endregion


    private void _update_anchor_point() {
        if (anchor_transform != null) { anchor_point = anchor_transform.position; }
        transform.position = Vector3.Lerp(transform.position, anchor_point + anchor_offset, Time.deltaTime * anchor_lerp_speed);
    }

    private void _update_look() {
        current_look_rotation.x = Mathf.Lerp(current_look_rotation.x, target_look_rotation.x, Time.deltaTime * look_lerp_speed);
        current_look_rotation.y = Mathf.Lerp(current_look_rotation.y, target_look_rotation.y, Time.deltaTime * look_lerp_speed);

        pitch.localRotation = Quaternion.identity;
        pitch.Rotate(-current_look_rotation.y, 0f, 0f);
        yaw.localRotation = Quaternion.identity;
        yaw.Rotate(0f, current_look_rotation.x, 0f);
    }

    private void _update_zoom() {
        float effective_zoom = current_zoom;

        bool clipped = false;
        if (avoid_clipping) {
            if (Physics.BoxCast(transform.position, CameraHalfExtents, -camera_3d.transform.forward, out RaycastHit hit, Quaternion.Euler(current_look_rotation), current_zoom + camera_3d.nearClipPlane, 1)) {
                effective_zoom = hit.distance - camera_3d.nearClipPlane;
                clipped = true;
            }
        }

        if (clipped) {
            target_zoom = effective_zoom;
            camera_3d.transform.localPosition = new Vector3(camera_3d.transform.localPosition.x, camera_3d.transform.localPosition.y, -effective_zoom);
        } else {
            current_zoom = Mathf.Lerp(current_zoom, target_zoom, Time.deltaTime * zoom_change_speed);
            camera_3d.transform.localPosition = new Vector3(camera_3d.transform.localPosition.x, camera_3d.transform.localPosition.y, -current_zoom);
        }
    }

    public void apply_look_rotation(Vector2 _look_rotation) {
        target_look_rotation += look_sensitivity * Time.deltaTime * _look_rotation * 20f;
        target_look_rotation.y = Mathf.Clamp(target_look_rotation.y, vertical_look_bounds.x, vertical_look_bounds.y);
        if (target_look_rotation.x > 360f) {
            target_look_rotation.x -= 360f;
            current_look_rotation.x -= 360f;
        } else if (target_look_rotation.x < -360f) {
            target_look_rotation.x += 360f;
            current_look_rotation.x += 360f;
        }
    }

    public void apply_zoom(float _zoom) {
        if (!zoom_enabled) return;
        target_zoom = Mathf.Clamp(target_zoom - _zoom, zoom_bounds.x, zoom_bounds.y);
    }
}
