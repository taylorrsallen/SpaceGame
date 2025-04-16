using NPCs.AI.Base;
using System.Collections;
using UnityEngine;

public class UfeAiType : AiType
{
    private Rigidbody _rb;
    private BoxCollider _boxCollider;
    private GameObject _firePivot;

    [SerializeField] private GameEffectData chargedEffect;
    [SerializeField] private GameObject chargedLaserEffect;
    [SerializeField] private GameObject PlasmaBallProjectile;
    [SerializeField] private GameObject LaserProjectile;
    [SerializeField] private AudioSource SoundPlayer;

    private float _tacticTime = 3;
    private float _targetDampen = 0;
    private float _zrotation = 0f;
    private float _attackOffsetX = 20;
    private float _attackSpeed = 2000;
    private float _lerpSpeed = 5;

    private Vector3 _oldPlayerPos;

    private int _tick = 0;
    private int _attackId = 0;
    private bool _trackPlayer = false;

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
        _rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionZ;
        _rb.mass = 20;

        _loopableMethodName = "S_FollowPlayer";
    }

    public void FixedUpdate()
    {
        _zrotation = _rb.linearVelocity.x * -1;
        transform.eulerAngles = new Vector3(0, 0, _zrotation);

        SendMessage(_loopableMethodName);

        CheckForKillDistance();
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

        _rb.AddForce(GetDirectionOfTarget(_atttackPosition) * _attackSpeed * _rb.mass * Time.fixedDeltaTime);

        float x = Mathf.Clamp(_rb.linearVelocity.x, -120, 120);
        float y = Mathf.Clamp(_rb.linearVelocity.y, -120, 120);
        float z = Mathf.Clamp(_rb.linearVelocity.z, -120, 120);

        _rb.linearVelocity = new Vector3(x, y, z);

        _rb.linearDamping = Mathf.Lerp(_rb.linearDamping, _targetDampen, 2 * Time.fixedDeltaTime);



        if (_trackPlayer == true)
        {
            _oldPlayerPos = Vector3.Lerp(_oldPlayerPos, GetPlayerPosition() + GameManager.instance.ship_controller.get_velocity() * 2.8f, _lerpSpeed * 22 * Time.deltaTime);

            _lerpSpeed -= 0.8f * Time.fixedDeltaTime;
            _lerpSpeed = Mathf.Clamp(_lerpSpeed, 0, 100);
        }
        else
        {
            _oldPlayerPos = GetPlayerPosition();
        }


        _firePivot.transform.LookAt(_oldPlayerPos);
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
        _trackPlayer = false;
        _atttackPosition = GetPlayerPosition(Random.Range(-_attackOffsetX, _attackOffsetX), 8);
        yield return new WaitForSeconds(1.5f);

        SoundPlayer.PlayOneShot(Resources.Load<AudioClip>("Sounds/Saucer_Charge1"));

        yield return new WaitForSeconds(0.5f);

        chargedEffect.Execute(new GameEffectArgs(_firePivot, null, Vector3.zero));

        //CreateChargeParticles
        yield return new WaitForSeconds(1.4f);
        //Shoot and restartLoop
        SoundPlayer.PlayOneShot(Resources.Load<AudioClip>("Sounds/Saucer_PlasmaFire"));
        GameObject PlasmaTemp = Instantiate(PlasmaBallProjectile);

        PlasmaTemp.transform.position = _firePivot.transform.position + (_firePivot.transform.forward * 5.8f);
        PlasmaTemp.transform.eulerAngles = _firePivot.transform.eulerAngles;

        yield return new WaitForSeconds(0.4f);
        _tacticTime = 3;
        _loopableMethodName = "S_FollowPlayer";


    }

    IEnumerator TimedAttack2()
    {

        //gives time for player to move and time for the ship to slowdown for firing

        int _side = Random.Range(0, 3);

        if (_side == 0)
        {
            _atttackPosition = GetPlayerPosition(20, 15);
        }
        if (_side == 1)
        {
            _atttackPosition = GetPlayerPosition(-20, 15);
        }
        if (_side == 2)
        {
            _atttackPosition = GetPlayerPosition(20, 5);
        }
        if (_side == 3)
        {
            _atttackPosition = GetPlayerPosition(-20, 5);
        }

        _trackPlayer = true;
        _lerpSpeed = 5;

        yield return new WaitForSeconds(1.5f);

        SoundPlayer.PlayOneShot(Resources.Load<AudioClip>("Sounds/Saucer_LaserCharge"));
       
        yield return new WaitForSeconds(1.5f); 

        GameObject lasertemp = Instantiate(chargedLaserEffect, _firePivot.transform);

        lasertemp.transform.localPosition = new Vector3(0, 0, 5.39f);
        lasertemp.transform.localEulerAngles = Vector3.zero;

        yield return new WaitForSeconds(4f);

        SoundPlayer.PlayOneShot(Resources.Load<AudioClip>("Sounds/Saucer_LaserSHOOT"));

        GameObject PlasmaTemp = Instantiate(LaserProjectile);

        PlasmaTemp.transform.position = _firePivot.transform.position + (_firePivot.transform.forward * 5.8f);
        PlasmaTemp.transform.eulerAngles = _firePivot.transform.eulerAngles;


        yield return new WaitForSeconds(0.4f);
        _tacticTime = 3;
        _loopableMethodName = "S_FollowPlayer";


    }

    private void PickAttack()
    {
        if (_attackId == 0)
        {
            _attackSpeed = 9000;
            _targetDampen = 20;
            _tacticTime = 3;
            _loopableMethodName = "S_Attack";
            StartCoroutine(TimedAttack1());
        }
        if (_attackId == 1)
        {
            if (GameManager.instance.ship_controller.get_ship_mass() > 75)
            {
                _attackId++;
            }
            else
            {
                _tacticTime = 1;
                _loopableMethodName = "S_RamPlayer";
            }
        }

        if (_attackId == 2)
        {
            _attackSpeed = 2000;
            _tacticTime = 3;
            _targetDampen = 12;
            _loopableMethodName = "S_Attack";
            StartCoroutine(TimedAttack2());
        }

        _attackId++;
        if (_attackId > 2)
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
