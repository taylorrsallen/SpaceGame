using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class TextureButton3D : MonoBehaviour, IUI3D {
    private TextMeshPro text_mesh;
    private BoxCollider box_collider;
    private MeshRenderer mesh_renderer;

    [Header("Setup")]
    [SerializeField] private string text;
    [SerializeField] private Material material;

    [Header("Sprite")]
    [SerializeField] private float sprite_scale = 0.05f;
    [SerializeField] public Color hovered_color = Color.white;
    [SerializeField] public float hovered_intensity = 1f;
    [SerializeField] public Color normal_color = Color.white;
    [SerializeField] public float normal_intensity = 0.2f;
    [SerializeField] public Color pressed_color = Color.white;
    [SerializeField] public float pressed_intensity = 0.6f;
    [SerializeField] public float button_color_lerp_speed = 10f;
    private Color button_color;
    private Color button_color_target;
    private float emission_intensity;
    private float emission_intensity_target;

    [Header("Text")]
    [SerializeField] public Vector3 text_local_offset;
    [SerializeField] public Color text_color;
    [SerializeField] private float text_size;
    [SerializeField] public Color hovered_text_color;
    [SerializeField] public Color pressed_text_color;
    private Color text_color_target;

    [Header("Rotation")]
    [SerializeField] private Vector2 rotation_multiplier = new Vector2(4f, 10f);
    [SerializeField] private float rotation_lerp_speed = 5f;
    private Vector3 target_rotation = Vector3.zero;

    [Header("Audio")]
    public AudioClip pressed_sound;
    public AudioClip hovered_sound;
    public AudioClip release_sound;

    [Header("Animation")]
    public AnimationCurve pressed_animation_curve;
    public Vector3 pressed_local_offset;
    public float animation_lerp_speed;
    private Vector3 local_position_target = Vector3.zero;
    private float normal_to_pressed = 0f;
    private float normal_to_pressed_target = 0f;

    [Header("Function")]
    public UnityEvent<int> on_pressed;
    public UnityEvent<int> on_released;
    public int button_id;
    private bool _is_pressed;

    private void Awake() {
        text_mesh = transform.GetChild(0).GetComponent<TextMeshPro>();
        box_collider = transform.GetChild(1).GetComponent<BoxCollider>();
        mesh_renderer = transform.GetChild(1).GetComponent<MeshRenderer>();

        refresh();
    }

    private void Update() {
        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(target_rotation), Time.unscaledDeltaTime * rotation_lerp_speed);
        text_mesh.transform.localPosition = text_local_offset;

        set_text_color(Color.Lerp(get_text_color(), text_color_target, Time.unscaledDeltaTime * button_color_lerp_speed));
        button_color = Color.Lerp(button_color, button_color_target, Time.unscaledDeltaTime * button_color_lerp_speed);
        emission_intensity = Mathf.Lerp(emission_intensity, emission_intensity_target, Time.unscaledDeltaTime * button_color_lerp_speed);
        mesh_renderer.material.SetColor("_EmissionColor", button_color * emission_intensity);

        normal_to_pressed = Mathf.Lerp(normal_to_pressed, normal_to_pressed_target, Time.unscaledDeltaTime * animation_lerp_speed);
        transform.localPosition = pressed_animation_curve.Evaluate(normal_to_pressed) * local_position_target;
    }

    [Button]
    public void refresh() {
        set_unhovered();
        set_text(text);
        set_text_color(text_color);
        set_text_size(text_size);

        mesh_renderer.material = new Material(material);
        mesh_renderer.transform.localScale = new Vector3(material.mainTexture.width, material.mainTexture.height, 1f) * 0.1f * sprite_scale;
    }

    public bool is_pressed() { return _is_pressed; }

    public void set_text_size(float _size) { text_mesh.fontSize = _size; }
    public float get_text_size() { return text_mesh.fontSize; }
    public void set_text_color(Color _color) { text_mesh.color = _color; }
    public Color get_text_color() { return text_mesh.color; }
    public void set_text(string _text) { text_mesh.text = _text; }
    public string get_text() { return text_mesh.text; }

    public void set_hovered(Vector3 hit_position) {
        Vector3 rotate_by = transform.position - hit_position;
        target_rotation = new Vector3(-rotate_by.y * rotation_multiplier.y, rotate_by.x * rotation_multiplier.x, 0f);

        if(!_is_pressed) {
            button_color_target = hovered_color;
            emission_intensity_target = hovered_intensity;
            text_color_target = hovered_text_color;
        } else {
            button_color_target = pressed_color;
            emission_intensity_target = pressed_intensity;
            text_color_target = pressed_text_color;
        }

        if(hovered_sound) SoundManager.instance.play_sound_3d_pitched(hovered_sound, transform.position);
    }

    public void set_unhovered() {
        target_rotation = Vector3.zero;

        button_color_target = normal_color;
        emission_intensity_target = normal_intensity;
        text_color_target = text_color;
    }

    public void press() {
        Debug.Log("Press");

        _is_pressed = true;
        local_position_target = pressed_local_offset;
        normal_to_pressed_target = 1f;
        on_pressed?.Invoke(button_id);

        button_color_target = pressed_color;
        emission_intensity_target = pressed_intensity;
        text_color_target = pressed_text_color;

        if(pressed_sound) SoundManager.instance.play_sound_3d_pitched(pressed_sound, transform.position);
    }

    public void unpress() {
        Debug.Log("Unpress");

        _is_pressed = false;
        local_position_target = Vector3.zero;
        normal_to_pressed_target = 0f;

        button_color_target = normal_color;
        emission_intensity_target = normal_intensity;
        text_color_target = text_color;
    }

    public void release() {
        Debug.Log("Release");

        _is_pressed = false;
        local_position_target = Vector3.zero;
        normal_to_pressed_target = 0f;
        on_released?.Invoke(button_id);

        button_color_target = hovered_color;
        emission_intensity_target = hovered_intensity;
        text_color_target = hovered_text_color;

        if(release_sound) SoundManager.instance.play_sound_3d_pitched(release_sound, transform.position);
    }
}
