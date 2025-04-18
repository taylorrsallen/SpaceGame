using TMPro;
using UnityEngine;

public enum ResourceDisplayType {
    GAME_RESOURCE,
    HEIGHT_PAY,
}

public class ResourceDisplay2D : MonoBehaviour {
    private TextMeshProUGUI text_mesh;

    public int resource_id;
    public ResourceDisplayType type = ResourceDisplayType.GAME_RESOURCE;

    private void Awake() {
        init();
    }

    private void init() {
        text_mesh = GetComponent<TextMeshProUGUI>();
    }

    private void Update() {
        if(type == ResourceDisplayType.GAME_RESOURCE) {
            text_mesh.text = GameManager.instance.get_player_resource_as_string(resource_id);
        } else {
            text_mesh.text = "+ " + GameManager.instance.resource_for_height.to_string();
            text_mesh.color = GameManager.instance.game_resources[GameManager.instance.resource_for_height.id].add_color;
        }
    }
}
