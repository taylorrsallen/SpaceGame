using UnityEngine;

public class SoundManager : MonoBehaviour {
    public static SoundManager instance { get; private set; }

    [SerializeField] GameObject sound_3d_prefab;

    private void Awake() {
        if (instance != null && instance != this) Destroy(this);
        else instance = this;
        DontDestroyOnLoad(this);
    }

    public void play_sound_3d(AudioClip clip, Vector3 position) {
        AudioSource sound_3d = Instantiate(sound_3d_prefab).GetComponent<AudioSource>();

        sound_3d.transform.position = position;
        sound_3d.clip = clip;
        sound_3d.Play();
    }

    public void play_sound_3d_pitched(AudioClip clip, Vector3 position, float pitch_min = 0.8f, float pitch_max = 1.2f) {
        AudioSource sound_3d = Instantiate(sound_3d_prefab).GetComponent<AudioSource>();

        Lifetime lifetime = sound_3d.GetComponent<Lifetime>();
        lifetime.lifetime = clip.length;

        sound_3d.transform.position = position;
        sound_3d.clip = clip;
        sound_3d.pitch = Random.Range(pitch_min, pitch_max);
        sound_3d.Play();
    }
}
