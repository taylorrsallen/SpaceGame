using UnityEngine;

public abstract class ModularShipActivatable : MonoBehaviour {
    public abstract void set_active(bool active);
    public abstract void set_component_base(ModularShipComponent component);
    public abstract void execute(ModularShipController ship_controller);
}