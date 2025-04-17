using TMPro;
using UnityEngine;

public class ResourceDisplay2D : MonoBehaviour {
    private TextMeshProUGUI text_mesh;

    public int resource_id;

    private void Awake() {
        init();
    }

    private void init() {
        text_mesh = GetComponent<TextMeshProUGUI>();
    }

    private void Update() {
        text_mesh.text = GameManager.instance.get_player_resource_as_string(resource_id);
    }
}
