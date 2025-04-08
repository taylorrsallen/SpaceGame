using UnityEngine;

public class ModularShipActivator : MonoBehaviour {
    public KeyCode hotkey = KeyCode.Space;
    public bool active = false;
    public bool can_deactivate = true;
    public bool toggle = false;

    public float fuel_usage_per_second = 0.5f;

    private ModularShipActivatable[] activatables;

    private void Awake() {
        activatables = GetComponentsInChildren<ModularShipActivatable>();
    }

    public void update_active_state(float total_available_fuel) {
        if (total_available_fuel == 0f && fuel_usage_per_second > 0f) {
            active = false;
        } else {
            if (Input.GetKeyDown(hotkey)) {
                active = toggle ? !active : true;
            } else if (Input.GetKeyUp(hotkey) && can_deactivate && !toggle) {
                active = false;
            }
        }

        foreach (ModularShipActivatable activatable in activatables) { activatable.set_active(active); }
    }

    public float update_activatables_and_get_fuel_usage(ModularShipController ship_controller, float total_available_fuel) {
        if (!active) return 0f;
        if (total_available_fuel == 0f && fuel_usage_per_second > 0f) return 0f;
        foreach (ModularShipActivatable activatable in activatables) { activatable.execute(ship_controller); }
        return fuel_usage_per_second;
    }
}
