using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ModularShipBuilder : MonoBehaviour {
    [SerializeField] LayerMask ui_layer;
    [SerializeField] public CharacterCameraGrabMotion ship_builder_camera_anchor;

    [SerializeField] Grid3DVisualizer build_grid;
    [SerializeField] Grid3DVisualizer shop_grid;
    [SerializeField] Grid3DVisualizer stash_grid;

    [SerializeField] public Grid3DItem grid_item_prefab;
    [SerializeField] public ContextMenu3D hover_context_menu;
    [SerializeField] public ContextMenu3D context_menu_prefab;

    public Grid3DVisualizer last_hovered_grid_visualizer;

    public Grid3DItem grabbed_item;
    public Vector3 cursor_velocity;
    public Vector3 cursor_previous_position;
    public Vector3 grabbed_item_rotation_target;
    public Vector3 grabbed_item_rotation;

    public Grid3DItem set_hotkey_target;

    public NineRect3D build_zone_background;

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
                grabbed_item_rotation_target = new Vector3(-cursor_velocity.y * 3f, cursor_velocity.x * 3f, grabbed_item.data.rotation * 90f) + AtmosphereManager.instance.get_wind_force();
                grabbed_item_rotation = Vector3.Lerp(grabbed_item_rotation, grabbed_item_rotation_target, Time.deltaTime * 10f);
                grabbed_item.transform.rotation = Quaternion.Euler(grabbed_item_rotation);
            }

            Grid3D hit_grid = hit.collider.GetComponent<Grid3D>();
            if (hit_grid == null) return;
            Grid3DVisualizer visualizer = hit_grid.transform.parent.GetComponent<Grid3DVisualizer>();
            set_hovered_grid_visualizer(visualizer);

            Vector2 world_coord = hit_grid.get_world_coord_from_position(hit.point);
            Vector2Int grid_coord = hit_grid.get_grid_coord_from_world_coord(hit.point);
            int grid_id = hit_grid.get_grid_id_from_grid_coord(grid_coord);

            // Debug.Log("[" + hit_grid.name + "] WorldGridCoord: " + world_coord + " | GridCoord: " + grid_coord + " | GridID: " + grid_id + " | CalculatedWorldGridCoord: " + hit_grid.get_position_from_grid_coord(grid_coord));

            Vector3 grid_coord_corner = new Vector3(world_coord.x, world_coord.y, hit.point.z);
            Util.DrawAABB2D(grid_coord_corner, Vector2.one * 0.5f, Color.white);

            if (set_hotkey_target) {
                if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) {
                    set_hotkey_target = null;
                } else {
                    foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
                    if (Input.GetKeyDown(key)) {
                        set_hotkey_target.data.component_runtime_data.activator_hotkey = key;
                        Debug.Log(set_hotkey_target.data.component_runtime_data.activator_hotkey);
                        set_hotkey_target = null;
                    }
                }
            }

            Grid3DItem hovered_item = hit_grid.try_get_grid_item_from_grid_coord(grid_coord);
            if (hovered_item) {
                Vector3 item_position = hit_grid.get_position_from_grid_coord(hovered_item.data.grid_coord);

                if (visualizer) visualizer.set_highlighter(hovered_item.data.grid_coord, hovered_item.get_rotated_dimensions(), Color.white);
                // Util.DrawAABB2D(new Vector3(item_position.x, item_position.y, hit.point.z), (Vector2)hovered_item.get_rotated_dimensions() * 0.5f, Color.blue);

                if (hovered_item.data.component_data.activator && !grabbed_item && !set_hotkey_target) {
                    hover_context_menu.gameObject.SetActive(true);
                    hover_context_menu.set_target(hovered_item);

                    if (Input.GetMouseButtonDown(1)) {
                        set_hotkey_target = hovered_item;
                        // set_hotkey_target.data.component_runtime_data.hotkey = "SET KEY";
                    }
                } else {
                    hover_context_menu.gameObject.SetActive(false);
                }
            } else {
                if (visualizer) visualizer.set_highlighter_active(false);
                hover_context_menu.gameObject.SetActive(false);
            }

            if (grabbed_item) {
                if (Input.GetKeyDown("r")) grabbed_item.rotate();

                Vector2Int placement_grid_coord = grabbed_item.get_placement_grid_coord(grid_coord);
                Color placement_color = hit_grid.is_item_placement_valid(grabbed_item.get_rotated_dimensions(), placement_grid_coord) ? Color.green : Color.red;

                if (visualizer) visualizer.set_highlighter(placement_grid_coord, grabbed_item.get_rotated_dimensions(), placement_color);
                // Util.DrawAABB2D(new Vector3(item_position.x, item_position.y, hit.point.z), (Vector2)grabbed_item.get_rotated_dimensions() * 0.5f, placement_color);

                foreach (Vector2Int connected_coord in grabbed_item.get_connected_coords_at_coord(placement_grid_coord)) {
                    if (!hit_grid.is_grid_coord_valid(connected_coord)) continue;
                    Color connection_color = hit_grid.get_grid_item_from_grid_coord(connected_coord) ? Color.magenta : Color.cyan;
                    Util.DrawAABB2D(hit_grid.get_position_from_grid_coord(connected_coord), Vector2.one * 0.5f, connection_color);
                }
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
                    Grid3DItem item = Instantiate(grid_item_prefab, build_grid.grid_3d.transform);
                    item.init(new Grid3DItemData(GameManager.instance.ship_components[Random.Range(0, GameManager.instance.ship_components.Length)]));
                    if (!hit_grid.try_set_item(item, grid_coord)) { Destroy(item.gameObject); }
                }
            }
        } else {
            set_hovered_grid_visualizer(null);
        }
    }

    public void set_hovered_grid_visualizer(Grid3DVisualizer grid_visualizer) {
        if (grid_visualizer == last_hovered_grid_visualizer) return;
        if (last_hovered_grid_visualizer) last_hovered_grid_visualizer.set_highlighter_active(false);
        last_hovered_grid_visualizer = grid_visualizer;
    }

    public Grid3DItem spawn_grid_item(int item_id, Transform parent) {
        Grid3DItem item = Instantiate(grid_item_prefab, parent);
        item.init(new Grid3DItemData(GameManager.instance.ship_components[Random.Range(0, GameManager.instance.ship_components.Length)]));
        item.set_default_runtime_data();
        return item;
    }

    private void FixedUpdate() {
        // Debug.Log(is_ship_valid());
    }


    public bool is_ship_valid() {
        if (build_grid.grid_3d.items_list.Count == 0) return false;
        Grid3DItem root_item = build_grid.grid_3d.items_list[0];

        List<Grid3DItem> connected_items = build_grid.grid_3d.get_connected_items(root_item);

        foreach (Grid3DItem item in build_grid.grid_3d.items_list) {
            if (!connected_items.Contains(item)) return false;
        }

        return true;
    }

    public void load_blueprint(ModularShipBlueprintData blueprint) {
        build_grid.grid_3d.clear();
        build_grid.grid_3d.load_blueprint_as_inventory(blueprint);
    }

    public  ModularShipBlueprintData get_ship_blueprint() {
        if (!is_ship_valid()) return null;
        List<Grid3DItemData> item_data = new List<Grid3DItemData>();
        foreach (Grid3DItem item in build_grid.grid_3d.items_list) item_data.Add(item.data);
        return new ModularShipBlueprintData(item_data);
    }

    private void update_ui_collider() {
        // build_grid.update_collider();
        // build_grid.transform.position = new Vector3(0f, build_grid.get_world_half_extent().y - 1f, 1f);
        // shop_grid.update_collider();
        // shop_grid.transform.position = new Vector3(build_grid.get_world_half_extent().x + shop_grid.get_world_half_extent().x, shop_grid.get_world_half_extent().y - 1f, 1f);
        // stash_grid.update_collider();
        // stash_grid.transform.position = new Vector3(-build_grid.get_world_half_extent().x - stash_grid.get_world_half_extent().x, stash_grid.get_world_half_extent().y - 1f, 1f);

        // ship_builder_camera_anchor.x_bounds = new Vector2(-build_grid.get_world_half_extent().x - stash_grid.get_world_dimensions().x, build_grid.get_world_half_extent().x + shop_grid.get_world_dimensions().x);
        // ship_builder_camera_anchor.y_bounds = new Vector2(-1f, Mathf.Max(Mathf.Max(build_grid.get_world_dimensions().y, shop_grid.get_world_dimensions().y), stash_grid.get_world_dimensions().y) - 1f);
    }
}
