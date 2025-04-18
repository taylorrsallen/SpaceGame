using UnityEngine;
using UnityEngine.UI;

public class TimeControl2D : MonoBehaviour {
    [SerializeField] private Button one_x_speed;
    [SerializeField] private Button two_x_speed;
    [SerializeField] private Button three_x_speed;
    private int speed_value = 1;
    
    public Color on_color;
    public Color off_color;
    ColorBlock on_color_block = new ColorBlock();
    ColorBlock off_color_block = new ColorBlock();

    public void set_speed_one() { speed_value = 1; Time.timeScale = 1; }
    public void set_speed_two() { speed_value = 2; Time.timeScale = 2; }
    public void set_speed_three() { speed_value = 3; Time.timeScale = 3; }

    public void init() {
        on_color_block.normalColor = on_color;
        on_color_block.highlightedColor = on_color * 1.2f;
        on_color_block.pressedColor = on_color * 0.7f;
        on_color_block.colorMultiplier = 1f;
        on_color_block.selectedColor = on_color;
        one_x_speed.colors = on_color_block;

        off_color_block.normalColor = off_color;
        off_color_block.highlightedColor = off_color * 1.2f;
        off_color_block.pressedColor = off_color * 0.7f;
        off_color_block.colorMultiplier = 1f;
        off_color_block.selectedColor = off_color;
        two_x_speed.colors = off_color_block;
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.F2)) set_speed_one();
        if(Input.GetKeyDown(KeyCode.F3)) set_speed_two();
        if(Input.GetKeyDown(KeyCode.F4)) set_speed_three();

        if(speed_value == 1) {
            one_x_speed.colors = on_color_block;
            two_x_speed.colors = off_color_block;
            three_x_speed.colors = off_color_block;
        } else if(speed_value == 2) {
            one_x_speed.colors = off_color_block;
            two_x_speed.colors = on_color_block;
            three_x_speed.colors = off_color_block;
        } else {
            one_x_speed.colors = off_color_block;
            two_x_speed.colors = off_color_block;
            three_x_speed.colors = on_color_block;
        }
    }
}
