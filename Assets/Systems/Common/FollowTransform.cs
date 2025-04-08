using UnityEngine;

public class FollowTransform : MonoBehaviour {
    public Transform follow;
    public Vector3 global_offset;
    public Vector3 local_offset;

    public bool follow_rotation = false;

    private void Update() {
        if (!follow) return;
        transform.position = follow.position + global_offset + follow.TransformDirection(local_offset);
        if (follow_rotation) transform.rotation = follow.rotation;
    }
}
