using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(FollowTransform))]
public class ContextMenu3D : MonoBehaviour {
    private FollowTransform follow_transform;

    [TabGroup("Setup")] public Grid3DItem target;
    [TabGroup("Setup")] public NineRect3D nine_rect;
    [TabGroup("Setup")] public TextMeshPro hotkey_text;

    [TabGroup("UI")] public float text_to_ui_size_scale = 2f;
    [TabGroup("UI")] public Vector2 minimum_frame_size = new Vector2(2f, 2f);
    [TabGroup("UI")] public Vector2 text_offset;
    [TabGroup("UI")] public Color color_override = Color.white;

    private void Awake() {
        init();
    }

    public void init() {
        follow_transform = GetComponent<FollowTransform>();
        nine_rect.init();
    }

    private void Update() {
        nine_rect.dimensions.x = Mathf.Max(hotkey_text.text.Length * text_to_ui_size_scale, minimum_frame_size.x);
        nine_rect.set_color(color_override);
        nine_rect.refresh();
        hotkey_text.transform.localPosition = new Vector3(text_offset.x, text_offset.y, hotkey_text.transform.localPosition.z);
    }

    public void set_target(Grid3DItem _target) {
        target = _target;
        follow_transform.follow = target.transform;
        // follow_transform.global_offset.y = _target.data.dimensions.y * 0.25f;
        follow_transform.snap();
    }

    public void display_hotkey_text(Grid3DItem item) {
        if (target.data.component_data.activator) {
            hotkey_text.text = target.data.component_runtime_data.activator_hotkey.ToString();
        }
    }
}
