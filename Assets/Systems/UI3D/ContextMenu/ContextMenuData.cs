using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "ContextMenuData", menuName = "Scriptable Objects/ContextMenuData")]
public class ContextMenuData : SerializedScriptableObject {
    public Texture2D frame_texture;

    public float scale = 1f;
    public float frame_x_text_multiplier = 0.69f;
    public float frame_x_base = 1f;
    public Vector2 frame_minimum_size = new Vector2(1.11f, 2f);
    public Vector2 text_offset = new Vector2(0.15f, 1.29f);

    public Vector3 follow_offset = new Vector3(-0.3f, 0.24f, -0.8f);
}
