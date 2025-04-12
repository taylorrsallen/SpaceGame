using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ChineseRocket : MonoBehaviour {
    private AudioSource audio_source;
    private Rigidbody rb;
    private FaceTarget face_target;

    public GameObject rocket_particle;
    public GameObject damage_particle;
    public Collider explosion_collider;
    public Collider engine_collider;

    public AudioClip collision_sound;
    public SoundPool engine_explode_sound;
    public GameObject engine_explode_effect;

    public float speed = 1f;
    public float max_speed = 5f;

    public float explosion_delay = 0.2f;
    private float explosion_timer = 0f;
    private bool is_exploding;
    private bool is_engine_exploded;

    public GameEffectData collision_effect;
    

    private void Start() {
        audio_source = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
        face_target = GetComponent<FaceTarget>();
        face_target.target = GameManager.instance.player_controller.character.transform;
    }

    private void Update() {
        if (!is_engine_exploded) move_forward_z_locked();

        if (is_exploding) {
            explosion_timer += Time.deltaTime;
            if (explosion_timer >= explosion_delay) explode();
        }
    }

    private void FixedUpdate() {
        if (is_engine_exploded) {
            rb.AddForceAtPosition(engine_collider.transform.position, engine_collider.transform.right);
            return;
        }

        face_target.update_facing();
        Vector3 velocity = rb.linearVelocity;
        if (velocity.magnitude > max_speed) velocity = velocity.normalized * max_speed;
        rb.linearVelocity = velocity;
    }

    public void OnCollisionEnter(Collision collision) {
        Collider rocket_collider = collision.GetContact(0).thisCollider.GetComponent<Collider>();
        if (rocket_collider == explosion_collider) {
            is_exploding = true;
        } else {
            if (is_engine_exploded) {
                explode();
            } else {
                destroy_engine();
            }
        }
    }

    public void move_forward_z_locked() {
        Vector3 move_direction = transform.forward;
        move_direction.z = 0f;
        move_direction.Normalize();

        rb.AddForce(move_direction * speed);
    }

    private void destroy_engine() {
        if (is_engine_exploded) return;
        is_engine_exploded = true;

        SoundManager.instance.play_sound_3d_pitched(engine_explode_sound.get_sound(), engine_collider.transform.position);
        Instantiate(engine_explode_effect, engine_collider.transform.position, Quaternion.identity);

        rb.useGravity = true;
        rb.mass = 30f;
        rb.AddForce((transform.position - engine_collider.transform.position).normalized * Random.Range(5f, 15f));

        damage_particle.SetActive(true);
        rocket_particle.SetActive(false);
    }

    private void explode() {
        collision_effect.Execute(new GameEffectArgs(gameObject, null, transform.position));
        Destroy(gameObject);
    }
}
