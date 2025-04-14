using UnityEngine;

public class WaterVolume : MonoBehaviour {
    public GameObject water_enter_effect;
    public AudioClip water_enter_sound;

    public float effect_cooldown = 0.35f;
    private float effect_timer = 0f;

    public Vector2 velocity_to_splash_range = new Vector2(5f, 100f);
    public Vector2 splash_size_range = new Vector2(0.2f, 1f);
    
    private void Update() {
        effect_timer = Mathf.Min(effect_timer + Time.deltaTime, effect_cooldown);
    }

    private void OnTriggerEnter(Collider collider) {
        ModularShipComponent ship_component = collider.GetComponent<ModularShipComponent>();
        if (ship_component) {
            GameManager.instance.ship_controller.enter_water();
        }

        try_play_water_enter_effect(collider.ClosestPoint(new Vector3(collider.transform.position.x, -39.24f, collider.transform.position.z)));
    }

    private void OnTriggerExit(Collider collider) {
        ModularShipComponent ship_component = collider.GetComponent<ModularShipComponent>();
        if (ship_component) {
            GameManager.instance.ship_controller.exit_water();
        }

        try_play_water_enter_effect(collider.ClosestPoint(new Vector3(collider.transform.position.x, -39.24f, collider.transform.position.z)));
    }

    private void try_play_water_enter_effect(Vector3 position) {
        float velocity = GameManager.instance.ship_controller.get_velocity().magnitude;
        if (velocity < velocity_to_splash_range.x) return;
        if (effect_timer < effect_cooldown) return;
        effect_timer = 0f;

        Transform splash = Instantiate(water_enter_effect, position, water_enter_effect.transform.rotation).transform;
        splash.localScale = Vector3.one * Mathf.Lerp(splash_size_range.x, splash_size_range.y, Mathf.Min(velocity - 5f, velocity_to_splash_range.y) / velocity_to_splash_range.y);
        SoundManager.instance.play_sound_3d_pitched(water_enter_sound, position);
    }
}
