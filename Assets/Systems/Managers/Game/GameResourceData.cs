using UnityEngine;

public struct GameResource {
    public uint id;
    public ulong amount;

    public GameResource(GameResource _resource) {
        id = _resource.id;
        amount = _resource.amount;
    }

    public string to_string() { return GameManager.instance.game_resources[id].get_amount_as_string(amount); }
    public bool can_player_afford() { return GameManager.instance.player_resources[id] >= amount; }
}

public enum GameResourceDisplayStyle {
    PLAIN,
    CENTS,
}

[CreateAssetMenu(fileName = "GameResourceData", menuName = "Scriptable Objects/GameResourceData")]
public class GameResourceData : ScriptableObject {
    public string resource_name;
    public string resource_display_name;
    public string resource_display_symbol;
    public GameResourceDisplayStyle resource_display_style;
    public bool symbol_before_amount = true;

    public float universal_value_exchange_rate;

    public SoundPool purchase_sounds;
    public SoundPool collect_sounds;

    public Mesh collectable_mesh;

    public Color base_color;
    public Color add_color;

    public string get_amount_as_string(ulong amount) {
        string amount_string = amount.ToString();
        if(resource_display_style == GameResourceDisplayStyle.CENTS) {
            if(amount_string.Length == 1) {
                amount_string = "0.0" + amount_string;
            } else if(amount_string.Length == 2) {
                amount_string = "0." + amount_string;
            } else {
                amount_string = amount_string.Insert(amount_string.Length - 2, ".");
            }
        }

        return symbol_before_amount ? resource_display_symbol + amount_string : amount_string + resource_display_symbol;
    }
}