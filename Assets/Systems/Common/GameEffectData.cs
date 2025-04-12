using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

[CreateAssetMenu(fileName = "GameEffectData", menuName = "Scriptable Objects/GameEffectData")]
public class GameEffectData : SerializedScriptableObject {
    [NonSerialized, OdinSerialize] public GameEffect[] game_effects;

    public void Execute(GameEffectArgs game_effect_args) {
        if (game_effects != null) foreach (GameEffect game_effect in game_effects) game_effect.Execute(game_effect_args);
    }
}
