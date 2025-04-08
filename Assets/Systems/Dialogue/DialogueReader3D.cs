using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class DialogueReader3D : MonoBehaviour {
    [SerializeField] public DialogueReaderData reader_data;
    [SerializeField] public DialogueData dialogue_data;

    private AudioClip[] letter_sounds_male = new AudioClip[26];

    public int current_line;

    AudioSource audio_source;

    private void Awake() {
        audio_source = GetComponent<AudioSource>();
        
        for (int i = 0; i < letter_sounds_male.Length; i++) {
            
        }
    }

    public void read_line() {

    }
}
