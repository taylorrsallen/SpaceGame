using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

[CreateAssetMenu(fileName = "GameEffectData", menuName = "Scriptable Objects/GameEffectData")]
public class GameEffectData : SerializedScriptableObject {
    [NonSerialized, OdinSerialize] public GameEffect[] game_effects;
}
