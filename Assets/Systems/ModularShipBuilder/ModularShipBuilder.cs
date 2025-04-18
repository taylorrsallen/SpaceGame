using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class ModularShipBuilder : MonoBehaviour {
    [TabGroup("Setup"), SerializeField] private LayerMask ui_layer;
    [TabGroup("Setup")] public CharacterCameraGrabMotion ship_builder_camera_anchor;
    [TabGroup("Setup")] public Grid3DVisualizer build_grid;
    [TabGroup("Setup")] public Grid3DVisualizer shop_grid;
    [TabGroup("Setup")] public Grid3DVisualizer stash_grid;
    [TabGroup("Setup")] public Grid3DItem grid_item_prefab;
    [TabGroup("Setup")] public Transform invalid_build_text;
    [TabGroup("Setup")] public ContextMenu3D hover_context_menu;
    [TabGroup("Setup")] public ContextMenu3D context_menu_prefab;

    [TabGroup("Hotkeys")] public KeyCode[] illegal_hotkeys;
    [TabGroup("Hotkeys")] public KeyCode[] ignored_hotkeys;

    [TabGroup("Shop")] public Transform shop_money_source;
    [TabGroup("Shop")] public Vector2Int shop_restock_range = new Vector2Int(10, 20);
    private int last_day_restocked = -1;

    [HideInInspector] public Vector2Int hovered_coord;
    [HideInInspector] public Grid3D hovered_grid;
    [HideInInspector] public Grid3DVisualizer hovered_grid_visualizer;

    [HideInInspector] public Grid3DItem hovered_item;
    [HideInInspector] public Grid3DItem grabbed_item;
    [HideInInspector] public Grid3DItem set_hotkey_target;

    private Vector3 cursor_velocity;
    private Vector3 cursor_previous_position;
    private Vector3 grabbed_item_rotation_target;
    private Vector3 grabbed_item_rotation;

    public bool _is_ship_valid;

    public void init() {
        build_grid.init();
        shop_grid.init();
        stash_grid.init();
        hover_context_menu.init();
    }

    private void Update() {
        update_hover_targets();
        
        if (!try_set_hotkey()) {
            if (Input.GetKeyDown("r")) rotate();
            if (Input.GetMouseButtonDown(0)) primary();
            if (Input.GetMouseButtonDown(1)) secondary();
        }

        update_grabbed_item();
        update_hovered_item();

        if(last_day_restocked != AtmosphereManager.instance.day_number) {
            shop_grid.grid_3d.clear();
            last_day_restocked = AtmosphereManager.instance.day_number;
            int amount_to_restock = Random.Range(shop_restock_range.x, shop_restock_range.y);
            for(int i = 0; i < amount_to_restock; i++) {
                Grid3DItem item = Instantiate(grid_item_prefab, shop_grid.grid_3d.transform);
                item.init(new Grid3DItemData(GameManager.instance.ship_components[Random.Range(0, GameManager.instance.ship_components.Length)]));
                if(!shop_grid.grid_3d.try_add_item(item, new Vector2Int(Random.Range(0, shop_grid.grid_3d.dimensions.x), Random.Range(0, shop_grid.grid_3d.dimensions.y)))) Destroy(item.gameObject);
            }
        }

        if(!_is_ship_valid) {
            build_grid.set_color(Color.red);
            invalid_build_text.gameObject.SetActive(true);
        } else {
            build_grid.set_color(Color.white);
            invalid_build_text.gameObject.SetActive(false);
        }
    }

    #region  Input
    private void rotate() {
        if (!grabbed_item) return;
        grabbed_item.rotate();
    }

    private void primary() {
        if (!hovered_grid) return;

        if (grabbed_item) {
            try_place_grabbed_item(hovered_grid, hovered_coord);
        } else if (hovered_item) {
            try_pick_up_hovered_item(hovered_grid);
        } else {
            // Grid3DItem item = Instantiate(grid_item_prefab, build_grid.grid_3d.transform);
            // item.init(new Grid3DItemData(GameManager.instance.ship_components[Random.Range(0, GameManager.instance.ship_components.Length)]));
            // if (!hovered_grid.try_set_item(item, hovered_coord)) { Destroy(item.gameObject); }
        }
    }

    private void secondary() {
        if (is_valid_hotkey_target(hovered_item)) { set_hotkey_target = hovered_item; return; }
        
    }
    #endregion

    #region UpdateGrid
    private void update_hover_targets() {
        cursor_velocity = Input.mousePosition - cursor_previous_position;
        cursor_previous_position = Input.mousePosition;        

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 50f, ui_layer)) {
            if (grabbed_item) {
                grabbed_item.transform.position = hit.point - Vector3.forward * 1.1f;
                grabbed_item_rotation_target = new Vector3(-cursor_velocity.y * 3f, cursor_velocity.x * 3f, grabbed_item.data.rotation * 90f) + AtmosphereManager.instance.get_wind_force(grabbed_item.transform.position.y);
                grabbed_item_rotation = Vector3.Lerp(grabbed_item_rotation, grabbed_item_rotation_target, Time.deltaTime * 10f);
                grabbed_item.transform.rotation = Quaternion.Euler(grabbed_item_rotation);
            }

            hovered_grid = hit.collider.GetComponent<Grid3D>();
            if (!hovered_grid) {
                hovered_item = null;
                set_hovered_grid_visualizer(null);
                return;
            }

            Grid3DVisualizer visualizer = hovered_grid.transform.parent.GetComponent<Grid3DVisualizer>();
            set_hovered_grid_visualizer(visualizer);

            hovered_coord = hovered_grid.get_grid_coord_from_world_coord(hit.point);
            hovered_item = hovered_grid.try_get_grid_item_from_grid_coord(hovered_coord);
        } else {
            hovered_item = null;
            hovered_grid = null;
            set_hovered_grid_visualizer(null);
        }
    }

    public void update_grabbed_item() {
        if (!grabbed_item || !hovered_grid) return;

        Vector2Int placement_grid_coord = grabbed_item.get_placement_grid_coord(hovered_coord);
        Color placement_color = hovered_grid.is_item_placement_valid(grabbed_item.get_rotated_dimensions(), placement_grid_coord) ? Color.green : Color.red;

        if (hovered_grid_visualizer) hovered_grid_visualizer.set_highlighter(placement_grid_coord, grabbed_item.get_rotated_dimensions(), placement_color);

        foreach (Vector2Int connected_coord in grabbed_item.get_connected_coords_at_coord(placement_grid_coord)) {
            if (!hovered_grid.is_grid_coord_valid(connected_coord)) continue;
            Color connection_color = hovered_grid.get_grid_item_from_grid_coord(connected_coord) ? Color.magenta : Color.cyan;
            Util.DrawAABB2D(hovered_grid.get_position_from_grid_coord(connected_coord), Vector2.one * 0.5f, connection_color);
        }
    }

    public void update_hovered_item() {
        if(!hovered_item) {
            if (!grabbed_item && hovered_grid_visualizer) hovered_grid_visualizer.set_highlighter_active(false);
            hover_context_menu.gameObject.SetActive(false);
        } else {
            if (!grabbed_item && hovered_grid_visualizer) hovered_grid_visualizer.set_highlighter(hovered_item.data.grid_coord, hovered_item.get_rotated_dimensions(), Color.white);
        }

        if(set_hotkey_target) {
            hover_context_menu.gameObject.SetActive(true);
            hover_context_menu.set_target(set_hotkey_target);
            hover_context_menu.set_text("Press any key...");
            hover_context_menu.frame_color_override = Color.green;
            hover_context_menu.refresh();
        } else if(!grabbed_item && is_valid_hotkey_target(hovered_item) && !hovered_grid.shop) {
            hover_context_menu.gameObject.SetActive(true);
            hover_context_menu.set_target(hovered_item);
            hover_context_menu.display_hotkey_text(hovered_item);
            hover_context_menu.frame_color_override = Color.white;
            hover_context_menu.refresh();
        } else {
            hover_context_menu.gameObject.SetActive(false);
        }
    }

    private bool try_set_hotkey() {
        if (!set_hotkey_target) return false;

        foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode))) {
            if (ignored_hotkeys.Contains(key)) continue;
            if (Input.GetKeyDown(key)) {
                if (illegal_hotkeys.Contains(key)) {
                    set_hotkey_target = null;
                    return true;
                }

                set_hotkey_target.data.component_runtime_data.activator_hotkey = key;
                set_hotkey_target = null;
                return true;
            }
        }

        return false;
    }
    #endregion

    #region GridHelpers
    private void try_place_grabbed_item(Grid3D hit_grid, Vector2Int grid_coord) {
        Vector2Int placement_grid_coord = grabbed_item.get_placement_grid_coord(grid_coord);
        if(hit_grid.shop) {
            if(hit_grid.try_set_item(grabbed_item, placement_grid_coord)) {
                sell_item(grabbed_item, hit_grid.shop);
                grabbed_item = null;
                _is_ship_valid = is_ship_valid();
            }
        } else {
            if(hit_grid.try_set_item(grabbed_item, placement_grid_coord)) {
                grabbed_item = null;
                _is_ship_valid = is_ship_valid();
            }
        }
    }

    private void try_pick_up_hovered_item(Grid3D hit_grid) {
        if(hit_grid.shop) {
            if(hovered_item.data.component_data.value.can_player_afford()) {
                GameManager.instance.spawn_resource_remove_particle(hovered_item.data.component_data.value, shop_money_source);
                pick_up_hovered_item(hit_grid);
                _is_ship_valid = is_ship_valid();
            }
        } else {
            pick_up_hovered_item(hit_grid);
            _is_ship_valid = is_ship_valid();
        }
    }

    private void pick_up_hovered_item(Grid3D hit_grid) {
        grabbed_item = hovered_item;
        hit_grid.remove_item(grabbed_item);
    }

    public void sell_item(Grid3DItem item, ShopData shop_data) {
        GameResource resource_to_add = new GameResource(item.data.component_data.value);
        resource_to_add.amount = shop_data.get_sell_price(resource_to_add);
        GameManager.instance.spawn_resource_add_particle(resource_to_add, shop_money_source);
    }

    public bool is_valid_hotkey_target(Grid3DItem item) {
        if (!item) return false;
        return item.data.component_data.activator && !grabbed_item && !set_hotkey_target;
    }
    #endregion

    #region GetSet
    public void set_hovered_grid_visualizer(Grid3DVisualizer grid_visualizer) {
        if (grid_visualizer == hovered_grid_visualizer) return;
        if (hovered_grid_visualizer) hovered_grid_visualizer.set_highlighter_active(false);
        hovered_grid_visualizer = grid_visualizer;
    }
    #endregion

    #region Blueprints
    [Button]
    public void save_as(string ship_name) {
        get_ship_blueprint().save_as(ship_name);
    }

    [Button]
    public void load(string ship_name) {
        ModularShipBlueprintData blueprint = new ModularShipBlueprintData(null);
        blueprint.load(ship_name);
        load_blueprint(blueprint);
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
    #endregion
}
