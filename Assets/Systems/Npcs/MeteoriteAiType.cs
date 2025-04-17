using UnityEngine;
using NPCs.AI;
using NPCs.AI.Base;
public class MeteoriteAiType : AiType
{
    private Rigidbody _rb;
    private BoxCollider _boxCollider;

    protected override void OnInit()
    {
        _boxCollider = GetComponent<BoxCollider>();

        if (_boxCollider == null)
        {
            _boxCollider = gameObject.AddComponent<BoxCollider>();
            _boxCollider.size = new Vector3(3, 5, 3);
        }

        _rb = gameObject.AddComponent<Rigidbody>();

        _rb.useGravity = false;

        _rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezePositionZ;
        _rb.mass = 2f;
        _rb.linearDamping = 0;

        _rb.AddForce(new Vector3(Random.Range(-409, -300), Random.Range(-3000, -2500), 0));
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
