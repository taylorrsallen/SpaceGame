using UnityEngine;

public class UFEShip : MonoBehaviour {
    [SerializeField] float rotation_speed;

    private void Update() {
        transform.Rotate(new Vector3(0f, Time.deltaTime * rotation_speed, 0f));
    }
}
