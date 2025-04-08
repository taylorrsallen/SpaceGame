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

    public Vector2Int dimensions = new Vector2Int(3, 3);

    public bool interactive;

    private void Awake() {
        int i = 0;
        foreach (MeshRenderer quad in GetComponentsInChildren<MeshRenderer>()) {
            quads[i] = quad;
            i++;
        }

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
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.filterMode = FilterMode.Point;

            Color[] colors = base_texture.GetPixels(texture_slice_offset[j].x, texture_slice_offset[j].y, slice_dimension, slice_dimension);
            texture.SetPixels(colors);
            texture.Apply(false);
            material.SetTexture("_MainTex", texture);
            quads[j].material = material;
        }

        set_interactive(interactive);
    }

    private void Update() {
        float scale_multiplier = 0.01f;
        Vector2 pixel_dimensions = new Vector2(dimensions.x, dimensions.y) * scale_multiplier;

        // 0 3 6 Position
        quads[0].transform.localPosition = new Vector3(0f, 1f + pixel_dimensions.y, 0f);
        quads[3].transform.localPosition = new Vector3(0f, 0.5f + pixel_dimensions.y * 0.5f, 0f);
        quads[6].transform.localPosition = new Vector3(0f, 0f, 0f);

        // 1 4 7 Position
        quads[1].transform.localPosition = new Vector3(1f * 0.5f + pixel_dimensions.x * 0.5f, 1f + pixel_dimensions.y, 0f);
        quads[4].transform.localPosition = new Vector3(1f * 0.5f + pixel_dimensions.x * 0.5f, 0.5f + pixel_dimensions.y * 0.5f, 0f);
        quads[7].transform.localPosition = new Vector3(1f * 0.5f + pixel_dimensions.x * 0.5f, 0f, 0f);

        // 2 5 8 Position
        quads[2].transform.localPosition = new Vector3(1f + pixel_dimensions.x, 1f + pixel_dimensions.y, 0f);
        quads[5].transform.localPosition = new Vector3(1f + pixel_dimensions.x, 0.5f + pixel_dimensions.y * 0.5f, 0f);
        quads[8].transform.localPosition = new Vector3(1f + pixel_dimensions.x, 0f, 0f);

        // Center Scaling
        quads[1].transform.localScale = new Vector3(pixel_dimensions.x, 1f, 1f);
        quads[3].transform.localScale = new Vector3(1f, pixel_dimensions.y, 1f);
        quads[4].transform.localScale = new Vector3(pixel_dimensions.x, pixel_dimensions.y, 1f);
        quads[5].transform.localScale = new Vector3(1f, pixel_dimensions.y, 1f);
        quads[7].transform.localScale = new Vector3(pixel_dimensions.x, 1f, 1f);
    }

    public void set_interactive(bool _interactive) {
        interactive = _interactive;
        foreach (MeshRenderer quad in quads) {
            MeshCollider quad_collider = quad.GetComponent<MeshCollider>();
            quad_collider.enabled = interactive;
        }
    }
}
