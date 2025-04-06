using UnityEngine;

[CreateAssetMenu(fileName = "SoundPool", menuName = "Scriptable Objects/SoundPool")]
public class SoundPool : ScriptableObject {
    [SerializeField] AudioClip[] sounds;

    public AudioClip get_sound() { return sounds[Random.Range(0, sounds.Length)]; }
}
