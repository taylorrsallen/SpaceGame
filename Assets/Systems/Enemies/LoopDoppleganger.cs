using UnityEngine;

public class LoopDoppleganger : MonoBehaviour {
    public GameObject doppleganger_prefab;
    private Transform left_doppleganger;
    private Transform right_doppleganger;
    
    private void Start() {
        Vector3 left_position = new Vector3(transform.position.x - AtmosphereManager.instance.world_horizontal_size, transform.position.y, transform.position.z);
        Vector3 right_position = new Vector3(transform.position.x + AtmosphereManager.instance.world_horizontal_size, transform.position.y, transform.position.z);
        left_doppleganger = Instantiate(doppleganger_prefab, left_position, transform.rotation).transform;
        right_doppleganger = Instantiate(doppleganger_prefab, right_position, transform.rotation).transform;
    }

    private void FixedUpdate() {
        Vector3 left_position = new Vector3(transform.position.x - AtmosphereManager.instance.world_horizontal_size, transform.position.y, transform.position.z);
        Vector3 right_position = new Vector3(transform.position.x + AtmosphereManager.instance.world_horizontal_size, transform.position.y, transform.position.z);
        left_doppleganger.transform.position = left_position;
        left_doppleganger.transform.rotation = transform.rotation;
        right_doppleganger.transform.position = right_position;
        right_doppleganger.transform.rotation = transform.rotation;
    }
}
