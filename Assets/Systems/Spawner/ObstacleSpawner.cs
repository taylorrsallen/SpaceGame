using UnityEngine;

public enum SpawnDirection {
    LEFT_RIGHT,
    ABOVE,
    CORNER,
    ANYWHERE,
}

public class ObstacleSpawner : MonoBehaviour {
    [SerializeField] GameObject temp_prefab;
    public float spawn_rate = 5f;
    private float spawn_timer = 0f;

    public Vector2 no_spawn_zone = new Vector2(40f, 40f);
    public Vector2 max_spawn_zone = new Vector2(150f, 150f);

    private void FixedUpdate() {
        Util.DrawAABB2D(transform.position - new Vector3(no_spawn_zone.x * 0.5f, no_spawn_zone.y * 0.5f), no_spawn_zone, Color.red);
        
        spawn_timer += Time.deltaTime;
        if (spawn_timer >= spawn_rate) {
            spawn_timer -= spawn_rate;
            Vector3 spawn_point = get_corner_spawn_point();
            Instantiate(temp_prefab, spawn_point, Quaternion.identity);
        }
    }

    private Vector3 get_any_spawn_point() {
        float axis = Random.value;
        return Vector3.zero;
    }

    private Vector3 get_corner_spawn_point() {
        float horizontal = Random.value;
        float vertical = Random.value;

        float x = horizontal < 0.5f ? Random.Range(-max_spawn_zone.x, -no_spawn_zone.x) : Random.Range(no_spawn_zone.x, max_spawn_zone.x);
        float y = vertical < 0.5f ? Random.Range(-max_spawn_zone.y, -no_spawn_zone.y) : Random.Range(no_spawn_zone.y, max_spawn_zone.y);

        return new Vector3(x, y, 0f);
    }

    private Vector3 get_spawn_point(SpawnDirection spawn_direction, Vector2 perpindicular_axis_range) {
        // if (spawn_direction == SpawnDirection.HORIZONTAL) {
        //     float horizontal = Random.value;
        //     float x = horizontal < 0.5f ? Random.Range(-max_spawn_zone.x, -no_spawn_zone.x) : Random.Range(no_spawn_zone.x, max_spawn_zone.x);
        //     float y = Random.Range(Mathf.Min(perpindicular_axis_range.x, max_spawn_zone))
        // }

        return Vector3.zero;
    }
    
}
