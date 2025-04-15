using NPCs.AI.Base;
using UnityEngine;

public class BiPlaneAIType : AiType
{
    private Rigidbody _rb;
    private BoxCollider _boxCollider;

    protected override void OnInit()
    {
        _boxCollider = gameObject.AddComponent<BoxCollider>();
        _rb = gameObject.AddComponent<Rigidbody>();
        _boxCollider.size = new Vector3(5, 1, 5);
        _rb.useGravity = false;

        _rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionZ;
        _rb.mass = 10;
        _rb.linearDamping = 2;
    }

    public void Update()
    {
        float sinewave = Mathf.Sin(Time.time * 0.1f);

        transform.eulerAngles = new Vector3(0, 0, _rb.linearVelocity.y * 2);

        _rb.AddForce(Vector3.right * 50 + Vector3.up * sinewave * 33);

        CheckForKillDistance();
    }

    public void OnCollisionEnter(Collision collision)
    {
        _npcRoot.Kill();
    }
}
