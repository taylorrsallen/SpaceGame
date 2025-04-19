using UnityEngine;

public class WindRotate : MonoBehaviour {
    Vector3 rotation_target;
    float lerp_speed = 10f;

    private void Update() {
        rotation_target = Quaternion.LookRotation(AtmosphereManager.instance.get_wind_direction()).eulerAngles;
        transform.rotation = Quaternion.Euler(Vector3.Lerp(transform.rotation.eulerAngles, new Vector3(0f, rotation_target.x, 0f), lerp_speed));
        
    }
}
