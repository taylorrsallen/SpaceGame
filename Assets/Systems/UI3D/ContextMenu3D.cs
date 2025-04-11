using System;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(FollowTransform))]
public class ContextMenu3D : MonoBehaviour {
    public bool expanded = false;
    public Grid3DItem target;

    private FollowTransform follow_transform;
    public NineRect3D nine_rect;
    public TextMeshPro hotkey_text;

    public float text_to_ui_size_scale = 2f;
    public Vector2 text_offset;

    private void Awake() {
        follow_transform = GetComponent<FollowTransform>();
    }

    private void Update() {
        nine_rect.dimensions.x = Math.Max(hotkey_text.text.Length - 3, 0) * text_to_ui_size_scale;
        hotkey_text.transform.localPosition = new Vector3(text_offset.x, text_offset.y, hotkey_text.transform.localPosition.z);
    }

    public void set_target(Grid3DItem _target) {
        target = _target;
        follow_transform.follow = target.transform;
        // follow_transform.global_offset.y = _target.data.dimensions.y * 0.25f;
        follow_transform.snap();

        if (target.data.component_data.activator) {
            hotkey_text.text = target.data.component_runtime_data.activator_hotkey.ToString();
        }
    }
}
