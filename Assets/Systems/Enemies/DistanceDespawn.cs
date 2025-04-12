using UnityEngine;

public class DistanceDespawn : MonoBehaviour {
    public float max_distance = 400f;
    private float check_frequency = 1f;
    private float check_timer = 0f;

    private void FixedUpdate() {
        check_timer += Time.fixedDeltaTime;
        if (check_timer < check_frequency) return;
        check_timer -= check_frequency;

        float distance = Vector3.Distance(transform.position, GameManager.instance.player_controller.character.transform.position);
        if (distance > max_distance) Destroy(gameObject);
    }
}
