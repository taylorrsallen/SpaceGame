using UnityEngine;
using NPCs.AI.Base;

public class SpaceSpeksAiType : AiType
{
    private Rigidbody _rb;
    private BoxCollider _boxCollider;

    private float _timeLeftTillBlowup = 0;


    protected override void OnInit()
    {
        _boxCollider = gameObject.AddComponent<BoxCollider>();
        _rb = gameObject.AddComponent<Rigidbody>();
        _boxCollider.size = new Vector3(4, 14, 2);
        _rb.useGravity = true;

        _rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezePositionZ;
        _rb.mass = 122;
        _rb.linearDamping = 0.5f;
        enabled = false;
        Invoke("S_FlyWhenRight", Random.Range(3, 12));
    }
    public void Update()
    {
        _timeLeftTillBlowup -= Time.deltaTime;

        _rb.AddRelativeForce(Vector3.up * 85000 * Time.deltaTime);

    }

    protected void S_FlyWhenRight()
    {
        enabled = true;
    }
    public void OnCollisionEnter(Collision collision)
    {
        if(Mathf.Abs(collision.impulse.magnitude) >= 800)
        {
            _npcRoot.TakeDamage(0.01f * collision.impulse.magnitude);
        }
    }
}
