using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class OutOfWaterAudio : MonoBehaviour {
    private AudioSource audio_source;

    private void Awake() {
        audio_source = GetComponent<AudioSource>();
    }

    private void Update() {
        if (GameManager.instance.ship_controller.is_in_water) {
            audio_source.Stop();
        } else if (!audio_source.isPlaying) {
            audio_source.Play();
        }
    }
}
