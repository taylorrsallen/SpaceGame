using UnityEngine;
using NPCs.AI.Base;
public class PlasmaBallAiType : AiType
{
    private Rigidbody _rb;
    private BoxCollider _boxCollider;
    [SerializeField] private float speed = 35;
    protected override void OnInit()
    {
        _boxCollider = gameObject.AddComponent<BoxCollider>();
        _rb = gameObject.AddComponent<Rigidbody>();
        _boxCollider.size = new Vector3(2, 2, 2);
        _rb.useGravity = false;

        _rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezePositionZ;
        _rb.mass = 0.01f;
        _rb.linearDamping = 0;

        Invoke("Thingy", 0.01f);
    }
    private void Thingy()
    {
        _rb.linearVelocity = transform.forward * speed;
    }
    public void Update()
    {
        transform.LookAt(_rb.linearVelocity + transform.position);
        CheckForKillDistance();
    }
    public void OnCollisionEnter(Collision collision)
    {
        _npcRoot.Kill();
    }
}
