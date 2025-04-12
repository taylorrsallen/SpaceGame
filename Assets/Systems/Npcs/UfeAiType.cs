using NPCs.AI.Base;
using System.Collections;
using UnityEngine;

public class UfeAiType : AiType
{
    private Rigidbody _rb;
    private BoxCollider _boxCollider;
    private GameObject _firePivot;

    private float _tacticTime = 3;
    private float _targetDampen = 0;
    private float _zrotation = 0f;
    private float _attackOffsetX = 14;

    private int _tick = 0;
    private int _attackId = 0;

    private Vector3 _atttackPosition;

    private string _loopableMethodName = "";

    protected override void OnInit()
    {
        _boxCollider = gameObject.AddComponent<BoxCollider>();
        _rb = gameObject.AddComponent<Rigidbody>();

        _firePivot = new GameObject();
        _firePivot.name = "SaucerFirePivot";
        _firePivot.transform.parent = this.transform;
        _firePivot.transform.localEulerAngles = Vector3.zero;
        _firePivot.transform.localPosition = Vector3.zero;

        _boxCollider.size = new Vector3(8, 1, 8);

        _rb.useGravity = false;
        _rb.constraints = RigidbodyConstraints.FreezeRotationX;
        _rb.constraints = RigidbodyConstraints.FreezeRotationY;
        _rb.constraints = RigidbodyConstraints.FreezeRotationZ;
        _rb.constraints = RigidbodyConstraints.FreezePositionZ;
        _rb.mass = 20;

        _loopableMethodName = "S_FollowPlayer";
    }

    public void FixedUpdate()
    {

        _zrotation = _rb.linearVelocity.x * -1;
        transform.eulerAngles = new Vector3(0, 0, _zrotation);

        SendMessage(_loopableMethodName);

    }

    protected void S_FollowPlayer()
    {
        _tick--;

        _tacticTime -= 1 * Time.unscaledDeltaTime;

        _rb.AddForce(GetDirectionOfTarget(GetPlayerPosition(0, 10)) * 2000 * _rb.mass * Time.fixedDeltaTime);

        float x = Mathf.Clamp(_rb.linearVelocity.x, -120, 120);
        float y = Mathf.Clamp(_rb.linearVelocity.y, -120, 120);
        float z = Mathf.Clamp(_rb.linearVelocity.z, -120, 120);
        _rb.linearVelocity = new Vector3(x, y, z);

        _rb.linearDamping = Mathf.Lerp(_rb.linearDamping, _targetDampen, 1 * Time.fixedDeltaTime);
        //if(Vector3.Distance(transform.position, GetPlayerPosition()) < 12)

        if (_tick < 1)
        {
            _tick = 2;
            CalculateStablization(0, 10);
        }
        if (_tacticTime < 0)
        {
            if (GetDistanceToPlayer(0, 10) < 15)
            {
                PickAttack();
            }
            else
            {
                _tacticTime = 3;
            }
        }
    }

    protected void S_Attack()
    {
        _targetDampen = 2;

        _rb.AddForce(GetDirectionOfTarget(_atttackPosition) * 2000 * _rb.mass * Time.fixedDeltaTime);

        float x = Mathf.Clamp(_rb.linearVelocity.x, -120, 120);
        float y = Mathf.Clamp(_rb.linearVelocity.y, -120, 120);
        float z = Mathf.Clamp(_rb.linearVelocity.z, -120, 120);

        _rb.linearVelocity = new Vector3(x, y, z);

        _rb.linearDamping = Mathf.Lerp(_rb.linearDamping, _targetDampen, 1 * Time.fixedDeltaTime);

        _firePivot.transform.LookAt(GetPlayerPosition());
    }

    protected void S_RamPlayer()
    {
        _tick--;

        _tacticTime -= 1 * Time.unscaledDeltaTime;

        _rb.AddForce(GetDirectionOfTarget(GetPlayerPosition()) * 4000 * _rb.mass * Time.fixedDeltaTime);

        float x = Mathf.Clamp(_rb.linearVelocity.x, -120, 120);
        float y = Mathf.Clamp(_rb.linearVelocity.y, -120, 120);
        float z = Mathf.Clamp(_rb.linearVelocity.z, -120, 120);
        _rb.linearVelocity = new Vector3(x, y, z);

        _rb.linearDamping = Mathf.Lerp(_rb.linearDamping, _targetDampen, 1 * Time.fixedDeltaTime);
        //if(Vector3.Distance(transform.position, GetPlayerPosition()) < 12)

        if (_tick < 1)
        {
            _tick = 2;
            CalculateStablization();
        }
        if (_tacticTime < 0)
        {
            _tacticTime = 3;
            _loopableMethodName = "S_FollowPlayer";
        }
    }

    IEnumerator TimedAttack1()
    {

        //gives time for player to move and time for the ship to slowdown for firing

        _atttackPosition = GetPlayerPosition(Random.Range(-_attackOffsetX, _attackOffsetX), 8);
        yield return new WaitForSeconds(1.5f);

        GameObject projectile = Instantiate(Resources.Load<GameObject>("Gore/SaucerCharge"), _firePivot.transform);
        projectile.transform.localPosition = new Vector3(0, 0, 5);
        //CreateChargeParticles
        yield return new WaitForSeconds(1.4f);
        //Shoot and restartLoop
        yield return new WaitForSeconds(0.4f);
        _tacticTime = 3;
        _loopableMethodName = "S_FollowPlayer";


    }

    private void PickAttack()
    {
        if (_attackId == 0)
        {
            _tacticTime = 3;
            _loopableMethodName = "S_Attack";
            StartCoroutine(TimedAttack1());
        }
        if (_attackId == 1)
        {
            _tacticTime = 1;
            _loopableMethodName = "S_RamPlayer";
        }

        _attackId++;
        if (_attackId > 1)
        {
            _attackId = 0;
        }
    }

    private void CalculateStablization(float offsetX, float offsetY)
    {
        float DampingPrePass = (7 - (GetDistanceToPlayer(offsetX, offsetY) * 0.4f));
        DampingPrePass = Mathf.Clamp(DampingPrePass, 0.4f, 7);
        _targetDampen = DampingPrePass;
    }
    private void CalculateStablization()
    {
        float DampingPrePass = (7 - (GetDistanceToPlayer() * 0.4f));
        DampingPrePass = Mathf.Clamp(DampingPrePass, 0.4f, 7);
        _targetDampen = DampingPrePass;
    }


}
