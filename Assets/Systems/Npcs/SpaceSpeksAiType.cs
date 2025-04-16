using UnityEngine;
using NPCs.AI.Base;

public class SpaceSpeksAiType : AiType
{
    private Rigidbody _rb;
    private BoxCollider _boxCollider;

    protected override void OnInit()
    {
        _boxCollider = gameObject.AddComponent<BoxCollider>();
        _rb = gameObject.AddComponent<Rigidbody>();
        _boxCollider.size = new Vector3(4, 10, 4);
        _rb.useGravity = true;

        _rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezePositionZ;
        _rb.mass = 2f;
        _rb.linearDamping = 0.5f;

        
    }
    public void Update()
    {
        _rb.AddForce(0, 330 * Time.deltaTime, 0);
        CheckForKillDistance();
    }
    public void OnCollisionEnter(Collision collision)
    {
        _npcRoot.Kill();
    }
}
