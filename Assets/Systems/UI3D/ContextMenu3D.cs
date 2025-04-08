using TMPro;
using UnityEngine;

[RequireComponent(typeof(FollowTransform))]
public class ContextMenu3D : MonoBehaviour {
    public bool expanded = false;
    public Grid3DItem target;

    private FollowTransform follow_transform;
    public NineRect3D nine_rect;
    public TextMeshPro hotkey_text;

    private void Awake() {
        follow_transform = GetComponent<FollowTransform>();
    }

    private void Update() {
        nine_rect.dimensions.x = (hotkey_text.text.Length - 3) * 30;
    }

    public void set_target(Grid3DItem _target) {
        target = _target;
        follow_transform.follow = target.transform;
        follow_transform.global_offset.y = _target.data.dimensions.y * 0.25f;
    }
}
