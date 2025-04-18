using UnityEngine;
using NPCs.AI.Base;
using JetBrains.Annotations;

public class SpaceSpeksAiType : AiType
{
    private Rigidbody _rb;
    private BoxCollider _boxCollider;

    private float _timeLeftTillBlowup = 0;
    private float _PickedSpeed = 0;

    public bool immediateLaunch = false;


    protected override void OnInit()
    {
        _boxCollider = GetComponent<BoxCollider>();

        if (_boxCollider == null)
        {
            _boxCollider = gameObject.AddComponent<BoxCollider>();
            _boxCollider.size = new Vector3(4, 14, 2);
        }

        _rb = gameObject.AddComponent<Rigidbody>();

        _rb.useGravity = true;

        _rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezePositionZ;
        _rb.mass = 122;
        _rb.linearDamping = 0.5f;
        enabled = false;

        if (immediateLaunch)
        {
            S_FlyWhenRight();
        }
        else
        {
            Invoke("S_FlyWhenRight", Random.Range(12, 120));
        }


        _PickedSpeed = Random.Range(85000, 100000);
    }
    public void Update()
    {
        _timeLeftTillBlowup -= Time.deltaTime;

        _rb.AddRelativeForce(Vector3.up * _PickedSpeed * Time.deltaTime);

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
