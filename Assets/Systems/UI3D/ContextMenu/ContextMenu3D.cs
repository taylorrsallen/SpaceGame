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

    [TabGroup("UI")] public ContextMenuData data;
    public Color frame_color_override = Color.white;

    private void Awake() {
        init();
    }

    public void init() {
        follow_transform = GetComponent<FollowTransform>();
        nine_rect.init();
        refresh();
    }

    [Button]
    public void refresh() {
        if(nine_rect.base_texture != data.frame_texture) {
            nine_rect.base_texture = data.frame_texture;
            nine_rect.update_texture();
        }

        transform.localScale = Vector3.one * data.scale;
        
        nine_rect.dimensions.x = Mathf.Max(hotkey_text.text.Length * data.frame_x_text_multiplier, data.frame_minimum_size.x) + data.frame_x_base;
        nine_rect.set_color(frame_color_override);
        nine_rect.refresh();
        hotkey_text.transform.localPosition = new Vector3(data.text_offset.x, data.text_offset.y, hotkey_text.transform.localPosition.z);

        update_follow_offset();
    }

    public void update_follow_offset() {
        follow_transform.global_offset = target ? new Vector3(data.follow_offset.x + target.data.dimensions.x * 0.25f, data.follow_offset.y + target.data.dimensions.y * 0.25f, data.follow_offset.z) : data.follow_offset;
    }

    public void set_text(string text) { hotkey_text.text = text; }

    public void set_target(Grid3DItem _target) {
        target = _target;
        follow_transform.follow = target.transform;
        update_follow_offset();
        follow_transform.snap();
    }

    public void display_hotkey_text(Grid3DItem item) {
        if (target.data.component_data.activator) {
            hotkey_text.text = target.data.component_runtime_data.activator_hotkey.ToString();
        }
    }
}
