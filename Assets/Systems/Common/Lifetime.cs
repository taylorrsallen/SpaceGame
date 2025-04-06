using UnityEngine;

public class Lifetime : MonoBehaviour {
    [SerializeField] public float lifetime;
    private float lifetime_timer;

    private void Update() {
        lifetime_timer += Time.deltaTime;
        if (lifetime_timer >= lifetime) Destroy(gameObject);
    }
}
