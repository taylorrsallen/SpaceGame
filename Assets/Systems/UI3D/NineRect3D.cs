using UnityEngine;

public class NineRect3D : MonoBehaviour {
    private MeshRenderer[] quads = new MeshRenderer[9];
    private Texture[] textures = new Texture[9];

    // 0 1 2
    // 3 4 5
    // 6 7 8

    public Texture2D base_texture;
    public Material base_material;
    public int slice_dimension;

    public Vector2 dimensions = new Vector2(3f, 3f);

    public bool interactive;

    private void Awake() {
        init();
    }

    public void init() {
        int i = 0;
        foreach (MeshRenderer quad in GetComponentsInChildren<MeshRenderer>(true)) {
            quads[i] = quad;
            i++;
        }

        update_texture();
        set_interactive(interactive);
    }

    public void update_texture() {
        Vector2Int[] texture_slice_offset = new Vector2Int[] {
            new Vector2Int(0, slice_dimension * 2),
            new Vector2Int(slice_dimension, slice_dimension * 2),
            new Vector2Int(slice_dimension * 2, slice_dimension * 2),
            new Vector2Int(0, slice_dimension),
            new Vector2Int(slice_dimension, slice_dimension),
            new Vector2Int(slice_dimension * 2, slice_dimension),
            new Vector2Int(0, 0),
            new Vector2Int(slice_dimension, 0),
            new Vector2Int(slice_dimension * 2, 0),
        };

        for (int j = 0; j < 9; j++) {
            Material material = new Material(base_material);
            Texture2D texture = Instantiate(new Texture2D(slice_dimension, slice_dimension, TextureFormat.RGBA32, false));
            texture.wrapMode = TextureWrapMode.Repeat;
            texture.filterMode = FilterMode.Point;

            Color[] colors = base_texture.GetPixels(texture_slice_offset[j].x, texture_slice_offset[j].y, slice_dimension, slice_dimension);
            texture.SetPixels(colors);
            texture.Apply(false);
            material.SetTexture("_MainTex", texture);
            quads[j].material = material;
        }

        refresh();
    }

    public void set_color(Color color) {
        foreach (MeshRenderer quad in quads) quad.material.color = color;
    }

    public void refresh() {
        float pixel_scale_multiplier = 1f / (float)slice_dimension;
        Vector2 half_sprite_dimensions = new Vector2(slice_dimension, slice_dimension) * 0.5f * pixel_scale_multiplier;

        float mid_x = 0.5f + dimensions.x * 0.5f - half_sprite_dimensions.x;
        float right_x = 1f + dimensions.x - half_sprite_dimensions.x * 3f;
        float mid_y = 0.5f + dimensions.y * 0.5f - half_sprite_dimensions.y;
        float top_y = 1f + dimensions.y - half_sprite_dimensions.y * 3f;

        // 0 3 6 Position
        quads[0].transform.localPosition = new Vector3(half_sprite_dimensions.x, top_y, 0f);
        quads[3].transform.localPosition = new Vector3(half_sprite_dimensions.x, mid_y, 0f);
        quads[6].transform.localPosition = new Vector3(half_sprite_dimensions.x, half_sprite_dimensions.y, 0f);

        // 1 4 7 Position
        quads[1].transform.localPosition = new Vector3(mid_x, top_y, 0f);
        quads[4].transform.localPosition = new Vector3(mid_x, mid_y, 0f);
        quads[7].transform.localPosition = new Vector3(mid_x, half_sprite_dimensions.y, 0f);

        // 2 5 8 Position
        quads[2].transform.localPosition = new Vector3(right_x, top_y, 0f);
        quads[5].transform.localPosition = new Vector3(right_x, mid_y, 0f);
        quads[8].transform.localPosition = new Vector3(right_x, half_sprite_dimensions.y, 0f);

        // Center Scaling
        Vector2 scaling = new Vector2(dimensions.x * slice_dimension * pixel_scale_multiplier - slice_dimension * pixel_scale_multiplier * 2f, dimensions.y * slice_dimension * pixel_scale_multiplier - slice_dimension * pixel_scale_multiplier * 2f);
        quads[1].transform.localScale = new Vector3(scaling.x, 1f, 1f);
        quads[1].material.mainTextureScale = new Vector2(dimensions.x, 1f);

        quads[3].transform.localScale = new Vector3(1f, scaling.y, 1f);
        quads[3].material.mainTextureScale = new Vector2(1f, scaling.y);

        quads[4].transform.localScale = new Vector3(scaling.x, scaling.y, 1f);
        quads[4].material.mainTextureScale = new Vector2(scaling.x, scaling.y);

        quads[5].transform.localScale = new Vector3(1f, scaling.y, 1f);
        quads[5].material.mainTextureScale = new Vector2(1f, scaling.y);

        quads[7].transform.localScale = new Vector3(scaling.x, 1f, 1f);
        quads[7].material.mainTextureScale = new Vector2(scaling.x, 1f);
    }

    public void set_interactive(bool _interactive) {
        interactive = _interactive;
        foreach (MeshRenderer quad in quads) {
            MeshCollider quad_collider = quad.GetComponent<MeshCollider>();
            quad_collider.enabled = interactive;
        }
    }
}
