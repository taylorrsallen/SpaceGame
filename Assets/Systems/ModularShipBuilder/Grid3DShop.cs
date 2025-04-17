using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopData", menuName = "Scriptable Objects/ShopData")]
public class ShopData : SerializedScriptableObject {
    float buy_multiplier = 1f;
    float sell_multiplier = 1f;
    // bool round_to_99_cents = false;

    public ulong get_buy_price(GameResource resource) { return (ulong)(resource.amount * buy_multiplier); }
    public ulong get_sell_price(GameResource resource) { return (ulong)(resource.amount * sell_multiplier); }
}
