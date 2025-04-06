using System.Linq;
using UnityEngine;

public class Grid3D : MonoBehaviour {
    public Vector2Int dimensions = new Vector2Int(1, 1);
    private BoxCollider box_collider;

    public Grid3DItem[] items;

    private void Awake() {
        box_collider = GetComponent<BoxCollider>();
        items = new Grid3DItem[dimensions.x * dimensions.y];
    }

    public void remove_item(Grid3DItem item) {
        for (int y = 0; y < item.dimensions.y; y++) { for (int x = 0; x < item.dimensions.x; x++) {
            int grid_id = get_grid_id_from_grid_coord(item.grid_coord + new Vector2Int(x, y));
            if (items[grid_id] == item) items[grid_id] = null;
        }}
    }

    public bool try_set_item(Grid3DItem item, Vector2Int place_at) {
        if (!is_item_placement_valid(item.dimensions, place_at)) return false;
        set_item(item, place_at);
        return true;
    }

    private void set_item(Grid3DItem item, Vector2Int place_at) {
        for (int y = 0; y < item.dimensions.y; y++) { for (int x = 0; x < item.dimensions.x; x++) {
            Vector2Int grid_coord = place_at + new Vector2Int(x, y);
            items[get_grid_id_from_grid_coord(grid_coord)] = item;
        }}

        item.grid_coord = place_at;
        item.transform.position = get_position_from_grid_coord(place_at) + item.get_center_position_offset();
        item.transform.rotation = Quaternion.identity;
    }

    public void update_collider() {
        box_collider.size = new Vector3(get_world_dimensions().x, get_world_dimensions().y, 1f);
    }

    public Vector2 get_world_dimensions() {
        return new Vector2(dimensions.x, dimensions.y) * 0.5f;
    }

    public Vector2 get_world_half_extent() {
        return new Vector2(dimensions.x, dimensions.y) * 0.25f;
    }


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
}
