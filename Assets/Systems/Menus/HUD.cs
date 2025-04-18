using UnityEngine;

public class HUD : MonoBehaviour {
    [SerializeField] private Transform tab_to_return;
    [SerializeField] public float afk_time = 3f;
    private float stillness_timer = 0f;

    [SerializeField] private TimeControl2D time_control;

    public void init() {
        time_control.init();
    }

    private void Update() {
        if(GameManager.instance.ship_builder.gameObject.activeSelf) {
            tab_to_return.gameObject.SetActive(false);
            stillness_timer = 0f;
        } else {
            if(GameManager.instance.ship_controller.components.childCount == 0) {
                tab_to_return.gameObject.SetActive(true);
            } else if(GameManager.instance.ship_controller.rb.linearVelocity.magnitude < 1f) {
                stillness_timer += Time.unscaledDeltaTime;
            } else {
                tab_to_return.gameObject.SetActive(false);
                stillness_timer = 0f;
            }

            if(stillness_timer >= afk_time) {
                tab_to_return.gameObject.SetActive(true);
            }
        }
    }
}
