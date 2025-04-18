using UnityEngine;

public class NightLight : MonoBehaviour {
    private Light light;
    public float light_intensity;

    private void Awake() {
        light = GetComponent<Light>();
    }

    private void Update() {
        float moon_dot = AtmosphereManager.instance.get_moon_dot();
        if(moon_dot > 0.01f) {
            light.intensity = light_intensity;
        } else {
            light.intensity = 0f;
        }
    }
}
