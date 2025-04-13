using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class UnderwaterAudio : MonoBehaviour {
    private AudioSource audio_source;

    private float volume_lerp_target = 0f;
    public float audio_lerp_speed = 5f;

    private void Awake() {
        audio_source = GetComponent<AudioSource>();
    }

    private void Update() {
        volume_lerp_target = GameManager.instance.ship_controller.is_in_water ? 1f : 0f;
        audio_source.volume = Mathf.Lerp(audio_source.volume, volume_lerp_target, Time.deltaTime * audio_lerp_speed);
    }
}
