using UnityEngine;
using NPCs.AI.Base;
public class ChinaSpyBalloonAiType : AiType
{
    private Rigidbody _rb;
    private BoxCollider _boxCollider;

    protected override void OnInit()
    {
        _boxCollider = GetComponent<BoxCollider>();

        if (_boxCollider == null)
        {
            _boxCollider = gameObject.AddComponent<BoxCollider>();
            _boxCollider.size = new Vector3(4, 14, 2);
        }
        _rb = gameObject.AddComponent<Rigidbody>();
        _boxCollider.size = new Vector3(3, 5, 3);
        _rb.useGravity = false;

        _rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezePositionZ;
        _rb.mass = 0.25f;
        _rb.linearDamping = 0;

        _rb.AddForce(new Vector3(Random.Range(-44, 44), Random.Range(-4, 4), 0));
    }
    public void Update()
    {
        CheckForKillDistance();
    }
    public void OnCollisionEnter(Collision collision)
    {
        _npcRoot.Kill();
    }
}
