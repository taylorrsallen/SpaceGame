using UnityEngine;

public class Grid3DItem : MonoBehaviour {
    [SerializeField] public Vector2Int grid_coord;
    [SerializeField] public Vector2Int dimensions = new Vector2Int(2, 2);
    [SerializeField] public bool rotated;

    public Vector3 get_center_position_offset() { return (Vector2)dimensions * 0.25f; }
    public Vector2Int get_placement_grid_coord(Vector2Int place_at) { return place_at - dimensions / 2; }
}
