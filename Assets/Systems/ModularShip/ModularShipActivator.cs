using UnityEngine;

public class ModularShipActivator : MonoBehaviour {
    public string hotkey = "space";
    public bool active = false;
    public bool can_deactivate = true;
    public bool toggle = false;

    private ModularShipActivatable[] activatables;

    private void Awake() {
        activatables = GetComponentsInChildren<ModularShipActivatable>();
    }

    public void update_active_state() {
        if (Input.GetKeyDown(hotkey)) {
            active = toggle ? !active : true;
        } else if (Input.GetKeyUp(hotkey) && can_deactivate && !toggle) {
            active = false;
        }

        foreach (ModularShipActivatable activatable in activatables) { activatable.set_active(active); }
    }

    public void update_activatables(ModularShipController ship_controller) {
        if (!active) return;
        foreach (ModularShipActivatable activatable in activatables) { activatable.execute(ship_controller); }
    }
}
