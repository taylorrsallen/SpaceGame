using UnityEngine;

[CreateAssetMenu(fileName = "ShipComponentAudioData", menuName = "Scriptable Objects/ShipComponentAudioData")]
public class ShipComponentAudioData : ScriptableObject {
    [SerializeField] public SoundPool collision_sounds;
    [SerializeField] public SoundPool destroy_sounds;

    [SerializeField] public Vector2 collision_sound_range = new Vector2(0.05f, 2f);
}
