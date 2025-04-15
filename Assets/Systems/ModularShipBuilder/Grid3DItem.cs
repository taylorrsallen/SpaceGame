using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Vector2IntSerialized {
    public int x;
    public int y;

    public Vector2IntSerialized(Vector2Int vector) {
        x = vector.x;
        y = vector.y;
    }

    public Vector2Int to_vector2int() {
        return new Vector2Int(x, y);
    }
}

[Serializable]
public class Grid3DItemDataSerialized {
    public Vector2IntSerialized grid_coord;
    public int rotation;
    public int component_data_id;

    public KeyCode activator_hotkey;
    public bool activator_toggle;

    public Grid3DItemDataSerialized(Grid3DItemData item_data) {
        grid_coord = new Vector2IntSerialized(item_data.grid_coord);
        rotation = item_data.rotation;
        component_data_id = item_data.component_data.id;

        activator_hotkey = item_data.component_runtime_data.activator_hotkey;
        activator_toggle = item_data.component_runtime_data.activator_toggle;
    }
}

// Data saved to blueprints
public class Grid3DItemData {
    public Vector2Int grid_coord;
    public Vector2Int dimensions = new Vector2Int(2, 2);
    public int rotation;
    public ShipComponentData component_data;
    public ShipComponentRuntimeData component_runtime_data = new ShipComponentRuntimeData();

    public Grid3DItemData(ShipComponentData _component_data) {
        set_component_data(_component_data);
    }

    public Grid3DItemData(Grid3DItemDataSerialized serialized) {
        load_serialized(serialized);
    }

    public void set_component_data(ShipComponentData _component_data) {
        component_data = _component_data;
        dimensions = component_data.grid_dimensions;
        component_runtime_data.set_non_editable_defaults(component_data);
    }
    
    public Vector2Int get_rotated_dimensions() {
        if (rotation == 0 || rotation == 2) return dimensions;
        return new Vector2Int(dimensions.y, dimensions.x);
    }

    public Vector3 get_center_position_offset() { return (Vector2)get_rotated_dimensions() * 0.25f; }
    public void rotate() { rotation = (rotation + 1) % 4; }
    public Vector2Int get_placement_grid_coord(Vector2Int place_at) { return place_at - get_rotated_dimensions() / 2; }

    public Grid3DItemDataSerialized get_serialized() {
        return new Grid3DItemDataSerialized(this);
    }

    public void load_serialized(Grid3DItemDataSerialized serialized) {
        grid_coord = serialized.grid_coord.to_vector2int();
        rotation = serialized.rotation;
        set_component_data(GameManager.instance.ship_components[serialized.component_data_id]);
        component_runtime_data.activator_hotkey = serialized.activator_hotkey;
        component_runtime_data.activator_toggle = serialized.activator_toggle;
    }
}

public class Grid3DItem : MonoBehaviour {
    public Grid3DItemData data;
    public ContextMenu3D menu;
    

    public void init(Grid3DItemData _data) {
        data = _data;
        refresh();
    }

    public void set_default_runtime_data() {
        data.component_runtime_data.set_defaults(data.component_data);
    }

    public void refresh() {
        foreach (Transform child_transform in GetComponentsInChildren<Transform>()) {
            if (child_transform.gameObject == gameObject) continue;
            Destroy(child_transform.gameObject);
        }

        Instantiate(data.component_data.mesh_prefab, transform);
    }

    public Vector3 get_center_position_offset() { return data.get_center_position_offset(); }
    public Vector2Int get_rotated_dimensions() { return data.get_rotated_dimensions(); }
    public void rotate() { data.rotate(); }
    public Vector2Int get_placement_grid_coord(Vector2Int place_at) { return data.get_placement_grid_coord(place_at); }

    public List<Vector2Int> get_connecting_coord_offsets() {
        List<Vector2Int> connecting_coord_offsets = new List<Vector2Int>();

        for (int y = 0; y < get_rotated_dimensions().y; y++) {
            connecting_coord_offsets.Add(new Vector2Int(-1, y));
            connecting_coord_offsets.Add(new Vector2Int(get_rotated_dimensions().x, y));
        }

        for (int x = 0; x < get_rotated_dimensions().x; x++) {  
            connecting_coord_offsets.Add(new Vector2Int(x, -1));
            connecting_coord_offsets.Add(new Vector2Int(x, get_rotated_dimensions().y));
        }

        return connecting_coord_offsets;
    }

    public List<Vector2Int> get_connected_coords() {
        return get_connected_coords_at_coord(data.grid_coord);
    }

    public List<Vector2Int> get_connected_coords_at_coord(Vector2Int coord) {
        List<Vector2Int> connecting_coord_offsets = new List<Vector2Int>();

        for (int y = 0; y < get_rotated_dimensions().y; y++) {
            connecting_coord_offsets.Add(new Vector2Int(-1, y) + coord);
            connecting_coord_offsets.Add(new Vector2Int(get_rotated_dimensions().x, y) + coord);
        }

        for (int x = 0; x < get_rotated_dimensions().x; x++) {  
            connecting_coord_offsets.Add(new Vector2Int(x, -1) + coord);
            connecting_coord_offsets.Add(new Vector2Int(x, get_rotated_dimensions().y) + coord);
        }

        return connecting_coord_offsets;
    }
}
