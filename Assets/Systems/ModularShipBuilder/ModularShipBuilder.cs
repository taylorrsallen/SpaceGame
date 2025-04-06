using UnityEngine;

public class ModularShipBuilder : MonoBehaviour {
    [SerializeField] LayerMask ui_layer;
    [SerializeField] CharacterCameraGrabMotion ship_builder_camera_anchor;

    [SerializeField] Grid3D build_grid;
    [SerializeField] Grid3D shop_grid;
    [SerializeField] Grid3D stash_grid;

    [SerializeField] Grid3DItem grid_item_prefab;

    public Grid3DItem grabbed_item;
    public Vector3 cursor_velocity;
    public Vector3 cursor_previous_position;
    public Vector3 grabbed_item_rotation_target;
    public Vector3 grabbed_item_rotation;

    private void Awake() {
        update_ui_collider();
    }

    private void Update() {
        update_ui_collider();

        cursor_velocity = Input.mousePosition - cursor_previous_position;
        cursor_previous_position = Input.mousePosition;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 50f, ui_layer)) {
            if (grabbed_item) {
                grabbed_item.transform.position = hit.point - Vector3.forward * 1.1f;
                grabbed_item_rotation_target = new Vector3(cursor_velocity.y, cursor_velocity.x, 0f) * 3f;
                grabbed_item_rotation = Vector3.Lerp(grabbed_item_rotation, grabbed_item_rotation_target, Time.deltaTime * 10f);
                grabbed_item.transform.rotation = Quaternion.Euler(grabbed_item_rotation);
            }

            Grid3D hit_grid = hit.collider.GetComponent<Grid3D>();
            if (hit_grid == null) return;

            Vector2 world_coord = hit_grid.get_world_coord_from_position(hit.point);
            Vector2Int grid_coord = hit_grid.get_grid_coord_from_world_coord(hit.point);
            int grid_id = hit_grid.get_grid_id_from_grid_coord(grid_coord);

            // Debug.Log("[" + hit_grid.name + "] WorldGridCoord: " + world_coord + " | GridCoord: " + grid_coord + " | GridID: " + grid_id + " | CalculatedWorldGridCoord: " + hit_grid.get_position_from_grid_coord(grid_coord));

            Vector3 grid_coord_corner = new Vector3(world_coord.x, world_coord.y, hit.point.z);
            Util.DrawAABB2D(grid_coord_corner, Vector2.one * 0.5f, Color.white);

            Grid3DItem hovered_item = hit_grid.get_grid_item_from_grid_coord(grid_coord);
            if (hovered_item) {
                Vector3 item_position = hit_grid.get_position_from_grid_coord(hovered_item.grid_coord);
                Util.DrawAABB2D(new Vector3(item_position.x, item_position.y, hit.point.z), (Vector2)hovered_item.dimensions * 0.5f, Color.blue);
            }

            if (grabbed_item) {
                Vector2Int placement_grid_coord = grabbed_item.get_placement_grid_coord(grid_coord);
                Vector3 item_position = hit_grid.get_position_from_grid_coord(placement_grid_coord);
                Color placement_color = hit_grid.is_item_placement_valid(grabbed_item.dimensions, placement_grid_coord) ? Color.green : Color.red;
                Util.DrawAABB2D(new Vector3(item_position.x, item_position.y, hit.point.z), (Vector2)grabbed_item.dimensions * 0.5f, placement_color);
            }

            if (Input.GetMouseButtonDown(0)) {
                if (grabbed_item) {
                    Vector2Int placement_grid_coord = grabbed_item.get_placement_grid_coord(grid_coord);
                    if (hit_grid.try_set_item(grabbed_item, placement_grid_coord)) grabbed_item = null;
                } else if (hovered_item) {
                    grabbed_item = hovered_item;
                    hit_grid.remove_item(grabbed_item);
                } else {
                    Debug.Log("Place");
                    Grid3DItem item = Instantiate(grid_item_prefab);
                    item.dimensions = new Vector2Int(2, 6);
                    if (!hit_grid.try_set_item(item, grid_coord)) Destroy(item.gameObject);
                }
            }
        }
    }

    private void update_ui_collider() {
        build_grid.update_collider();
        build_grid.transform.position = new Vector3(0f, build_grid.get_world_half_extent().y - 1f, 1f);
        shop_grid.update_collider();
        shop_grid.transform.position = new Vector3(build_grid.get_world_half_extent().x + shop_grid.get_world_half_extent().x, shop_grid.get_world_half_extent().y - 1f, 1f);
        stash_grid.update_collider();
        stash_grid.transform.position = new Vector3(-build_grid.get_world_half_extent().x - stash_grid.get_world_half_extent().x, stash_grid.get_world_half_extent().y - 1f, 1f);

        ship_builder_camera_anchor.x_bounds = new Vector2(-build_grid.get_world_half_extent().x - stash_grid.get_world_dimensions().x, build_grid.get_world_half_extent().x + shop_grid.get_world_dimensions().x);
        ship_builder_camera_anchor.y_bounds = new Vector2(-1f, Mathf.Max(Mathf.Max(build_grid.get_world_dimensions().y, shop_grid.get_world_dimensions().y), stash_grid.get_world_dimensions().y) - 1f);
    }
}
