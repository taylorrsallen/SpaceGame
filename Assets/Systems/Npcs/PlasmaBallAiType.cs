using UnityEngine;
using NPCs.AI.Base;
public class PlasmaBallAiType : AiType
{
    private Rigidbody _rb;
    private BoxCollider _boxCollider;

    protected override void OnInit()
    {
        _boxCollider = gameObject.AddComponent<BoxCollider>();
        _rb = gameObject.AddComponent<Rigidbody>();
        _boxCollider.size = new Vector3(0.5f, 0.5f, 0.5f);
        _rb.useGravity = false;

        _rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezePositionZ;
        _rb.mass = 0.01f;
        _rb.linearDamping = 0;

        _rb.AddRelativeForce(Vector3.forward * 2);
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
