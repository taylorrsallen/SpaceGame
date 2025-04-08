using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Grid3D : MonoBehaviour {
    public Vector2Int dimensions = new Vector2Int(1, 1);
    private BoxCollider box_collider;

    public List<Grid3DItem> items_list = new List<Grid3DItem>();
    public Grid3DItem[] items;

    private void Awake() {
        box_collider = GetComponent<BoxCollider>();
        clear();
    }

    public void remove_item(Grid3DItem item) {
        for (int y = 0; y < item.get_rotated_dimensions().y; y++) { for (int x = 0; x < item.get_rotated_dimensions().x; x++) {
            int grid_id = get_grid_id_from_grid_coord(item.data.grid_coord + new Vector2Int(x, y));
            if (items[grid_id] == item) items[grid_id] = null;
        }}

        items_list.Remove(item);
    }

    public bool try_set_item(Grid3DItem item, Vector2Int place_at) {
        if (!is_item_placement_valid(item.get_rotated_dimensions(), place_at)) return false;
        set_item(item, place_at);
        return true;
    }

    private void set_item(Grid3DItem item, Vector2Int place_at) {
        for (int y = 0; y < item.get_rotated_dimensions().y; y++) { for (int x = 0; x < item.get_rotated_dimensions().x; x++) {
            Vector2Int grid_coord = place_at + new Vector2Int(x, y);
            items[get_grid_id_from_grid_coord(grid_coord)] = item;
        }}

        items_list.Add(item);
        item.data.grid_coord = place_at;
        item.transform.position = get_position_from_grid_coord(place_at) + item.get_center_position_offset();
        item.transform.rotation = Quaternion.Euler(0f, 0f, item.data.rotation * 90f);
    }

    public void update_collider() {
        box_collider.size = new Vector3(get_world_dimensions().x, get_world_dimensions().y, 1f);
    }

    public void load_blueprint_as_inventory(ModularShipBlueprintData blueprint, Transform parent = null) {
        clear();

        Transform item_parent = parent != null ? parent : transform;
        foreach (Grid3DItemData item_data in blueprint.item_datas) {
            Grid3DItem item = Instantiate(GameManager.instance.ship_builder.grid_item_prefab, item_parent);
            item.init(item_data);
            set_item(item, item.data.grid_coord);
        }
    }

    public void load_blueprint_as_functional(ModularShipBlueprintData blueprint, Transform parent = null) {
        clear();

        Transform item_parent = parent != null ? parent : transform;
        foreach (Grid3DItemData item_data in blueprint.item_datas) {
            ModularShipComponent ship_component = Instantiate(item_data.component_data.component_prefab, item_parent);
            ship_component.runtime_data = item_data.component_runtime_data;
            ship_component.transform.localPosition = get_position_from_grid_coord(item_data.grid_coord) + item_data.get_center_position_offset();
            ship_component.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, item_data.rotation * 90f));
            ship_component.init();
        }
    }

    public void clear() {
        foreach (Grid3DItem item in items_list) Destroy(item.gameObject);

        items_list = new List<Grid3DItem>();
        items = new Grid3DItem[dimensions.x * dimensions.y];
    }

    #region BOOL
    public bool is_item_placement_valid(Vector2Int dimensions, Vector2Int place_at) {
        for (int y = 0; y < dimensions.y; y++) { for (int x = 0; x < dimensions.x; x++) {
            Vector2Int grid_coord = place_at + new Vector2Int(x, y);
            if (!is_grid_coord_valid(grid_coord)) return false;
            if (get_grid_item_from_grid_coord(grid_coord) != null) return false;
        }}

        return true;
    }

    public bool is_grid_coord_valid(Vector2Int grid_coord) {
        return !(grid_coord.x < 0 || grid_coord.x > dimensions.x - 1 || grid_coord.y < 0 || grid_coord.y > dimensions.y - 1);
    }
    #endregion

    #region GET
    public List<Grid3DItem> get_connected_items(Grid3DItem item) {
        List<Grid3DItem> connected_items = new List<Grid3DItem>();
        connected_items.Add(item);

        Queue<Grid3DItem> items_to_check = new Queue<Grid3DItem>();
        items_to_check.Enqueue(item);

        while (items_to_check.Count > 0) {
            Grid3DItem item_to_check = items_to_check.Dequeue();

            foreach (Vector2Int connected_coord in item_to_check.get_connected_coords()) {
                if (!is_grid_coord_valid(connected_coord)) continue;
                Grid3DItem connected_item = get_grid_item_from_grid_coord(connected_coord);
                if (!connected_item) continue;
                if (connected_items.Contains(connected_item)) continue;
                connected_items.Add(connected_item);
                items_to_check.Enqueue(connected_item);
            }
        }

        return connected_items;
    }

    public Vector2 get_world_dimensions() {
        return new Vector2(dimensions.x, dimensions.y) * 0.5f;
    }

    public Vector2 get_world_half_extent() {
        return new Vector2(dimensions.x, dimensions.y) * 0.25f;
    }

    public Grid3DItem get_grid_item_from_grid_coord(Vector2Int grid_coord) {
        return items[get_grid_id_from_grid_coord(grid_coord)];
    }

    public Vector2 get_world_coord_from_position(Vector3 position) {
        Vector2 world_coord = new Vector2(Mathf.Floor(position.x), Mathf.Floor(position.y));
        world_coord.x = position.x - world_coord.x > 0.5f ? world_coord.x + 0.5f : world_coord.x;
        world_coord.y = position.y - world_coord.y > 0.5f ? world_coord.y + 0.5f : world_coord.y;
        return world_coord;
    }

    public Vector2Int get_grid_coord_from_position(Vector3 position) {
        return get_grid_coord_from_world_coord(get_world_coord_from_position(position));
    }

    public int get_grid_id_from_position(Vector3 position) {
        return get_grid_id_from_grid_coord(get_grid_coord_from_position(position));
    }

    public Vector3 get_position_from_grid_coord(Vector2Int grid_coord) {
        return new Vector3(grid_coord.x * 0.5f - dimensions.x * 0.25f + transform.position.x, grid_coord.y * 0.5f - dimensions.y * 0.25f + transform.position.y, transform.position.z);
    }

    public Vector2Int get_grid_coord_from_world_coord(Vector2 world_coord) {
        return new Vector2Int((int)(world_coord.x * 2f + dimensions.x * 0.5f - transform.position.x * 2f), (int)(world_coord.y * 2f + dimensions.y * 0.5f - transform.position.y * 2f));
    }

    public int get_grid_id_from_grid_coord(Vector2Int grid_coord) {
        return grid_coord.y * dimensions.x + grid_coord.x;
    }
    #endregion
}
