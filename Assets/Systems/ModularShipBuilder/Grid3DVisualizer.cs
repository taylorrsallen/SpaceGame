using UnityEngine;

public class Grid3DVisualizer : MonoBehaviour {
    [HideInInspector] public Grid3D grid_3d;
    [HideInInspector] public MeshRenderer grid_texture_quad;
    [HideInInspector] public NineRect3D grid_border;
    [HideInInspector] public NineRect3D highlighter;

    public Vector2Int grid_dimensions;

    public Texture2D grid_texture;
    public Texture2D border_texture;
    public int border_pixel_size;
    public int border_pixel_offset;
    public bool interactive = true;

    private MeshRenderer[] inner_border_quads = new MeshRenderer[4];

    private void Start() {
        grid_3d = GetComponentInChildren<Grid3D>();
        grid_3d.init();
        grid_texture_quad = transform.GetChild(1).GetComponentInChildren<MeshRenderer>();
        grid_border = transform.GetChild(2).GetComponentInChildren<NineRect3D>();
        highlighter = transform.GetChild(4).GetComponent<NineRect3D>();

        for (int i = 0; i < 4; i++) {
            inner_border_quads[i] = transform.GetChild(3).GetChild(i).GetComponent<MeshRenderer>();
            inner_border_quads[i].material = grid_texture_quad.material;
        }

        set_interactive(interactive);
        set_highlighter_active(false);
        refresh();
    }

    private void Update() {
        highlighter.refresh();
    }

    public void set_highlighter_active(bool active) {
        highlighter.gameObject.SetActive(active);
    }

    public void set_highlighter(Vector2Int grid_coord, Vector2Int dimensions, Color color) {
        highlighter.gameObject.SetActive(true);
        highlighter.dimensions = (Vector2)dimensions * 0.5f + new Vector2(2f, 2f);
        highlighter.refresh();
        highlighter.set_color(color);
        highlighter.transform.position = grid_3d.get_position_from_grid_coord(grid_coord) - new Vector3(1f, 1f, 0.1f);
    }

    private void refresh() {
        grid_3d.dimensions = grid_dimensions;
        grid_3d.update_collider();
        grid_3d.clear();

        grid_texture_quad.material.mainTexture = grid_texture;
        grid_texture_quad.material.mainTextureScale = new Vector2(grid_dimensions.x, grid_dimensions.y);
        grid_texture_quad.transform.localScale = new Vector3(grid_dimensions.x * 0.5f, grid_dimensions.y * 0.5f, 1f);
        grid_texture_quad.transform.localPosition = new Vector3(0f, 0f, 0.5f);

        float pixel_scale_multiplier = 1f / border_pixel_size;
        Vector2 border_pixel_offsets = new Vector2(border_pixel_offset, border_pixel_offset) * pixel_scale_multiplier;
        grid_border.base_texture = border_texture;
        grid_border.slice_dimension = border_pixel_size;
        grid_border.dimensions = new Vector2(grid_dimensions.x * 0.5f + border_pixel_offsets.x * 2f, grid_dimensions.y * 0.5f + border_pixel_offsets.y * 2f);
        grid_border.update_texture();
        grid_border.transform.localPosition = new Vector3(-grid_dimensions.x * 0.25f - border_pixel_offsets.x, -grid_dimensions.y * 0.25f - border_pixel_offsets.y, 0f);

        inner_border_quads[0].transform.localPosition = new Vector3(-grid_dimensions.x * 0.25f, 0f, 0.25f);
        inner_border_quads[1].transform.localPosition = new Vector3(grid_dimensions.x * 0.25f, 0f, 0.25f);
        inner_border_quads[2].transform.localPosition = new Vector3(0f, -grid_dimensions.y * 0.25f, 0.25f);
        inner_border_quads[3].transform.localPosition = new Vector3(0f, grid_dimensions.y * 0.25f, 0.25f);

        for (int i = 0; i < 2; i++) {
            Material inner_border_material = new Material(grid_texture_quad.material);
            inner_border_material.mainTextureScale = new Vector2(1f, grid_dimensions.x);
            inner_border_quads[i].material = inner_border_material;
            inner_border_quads[i].transform.localScale = new Vector3(0.5f, grid_dimensions.x * 0.5f, 1f);
        }

        for (int i = 2; i < 4; i++) {
            Material inner_border_material = new Material(grid_texture_quad.material);
            inner_border_material.mainTextureScale = new Vector2(1f, grid_dimensions.y);
            inner_border_quads[i].material = inner_border_material;
            inner_border_quads[i].transform.localScale = new Vector3(0.5f, grid_dimensions.y * 0.5f, 1f);
        }
    }

    public void set_interactive(bool interactive) {
        grid_3d.set_interactive(interactive);
        grid_border.set_interactive(false);
    }
}
