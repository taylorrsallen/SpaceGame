using UnityEngine;
using UnityEngine.UI;

public class CameraToggle2D : MonoBehaviour {
    private Button button;

    public Color on_color;
    public Color off_color;
    ColorBlock gay_color_block;

    private void Awake() {
        init();
    }

    private void init() {
        button = GetComponent<Button>();
        button.colors = gay_color_block;
    }

    private void Update() {
        if(GameManager.instance.player_controller.camera_rig.zero_z_rotation) set_off_colors();
        else set_on_colors();
    }

    private void set_on_colors() {
        gay_color_block.normalColor = on_color;
        gay_color_block.highlightedColor = on_color * 1.2f;
        gay_color_block.pressedColor = on_color * 0.7f;
        gay_color_block.colorMultiplier = 1f;
        gay_color_block.selectedColor = on_color;
        button.colors = gay_color_block;
    }

    private void set_off_colors() {
        gay_color_block.normalColor = off_color;
        gay_color_block.highlightedColor = off_color * 1.2f;
        gay_color_block.pressedColor = off_color * 0.7f;
        gay_color_block.colorMultiplier = 1f;
        gay_color_block.selectedColor = off_color;
        button.colors = gay_color_block;
    }

    public void toggle_camera_rotation() {
        GameManager.instance.player_controller.toggle_camera_rotation();
    }
}
