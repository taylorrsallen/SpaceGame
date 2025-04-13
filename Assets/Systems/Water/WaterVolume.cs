using UnityEngine;

public class WaterVolume : MonoBehaviour {
    public GameObject water_enter_effect;
    public AudioClip water_enter_sound;

    public float effect_cooldown = 0.35f;
    private float effect_timer = 0f;
    
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
        if (effect_timer < effect_cooldown) return;
        effect_timer = 0f;

        Instantiate(water_enter_effect, position, water_enter_effect.transform.rotation);
        SoundManager.instance.play_sound_3d_pitched(water_enter_sound, position);
    }
}
