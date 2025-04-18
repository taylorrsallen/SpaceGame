using NPCs.AI.Base;
using System.Collections;
using UnityEngine;

public class FighterPlaneAIType : AiType
{
    private Rigidbody _rb;
    private BoxCollider _boxCollider;
    private float _cooldown = 2f;
    [SerializeField] private AudioSource _shoot;
    [SerializeField] private Transform _shootPivot;
    [SerializeField] private Transform _ShootSpot;
    [SerializeField] private GameObject _Bullet;
    protected override void OnInit()
    {
        _boxCollider = GetComponent<BoxCollider>();

        if (_boxCollider == null)
        {
            _boxCollider = gameObject.AddComponent<BoxCollider>();
            _boxCollider.size = new Vector3(5, 1, 5);
        }

        _rb = gameObject.AddComponent<Rigidbody>();

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

        _cooldown -= Time.deltaTime;

        if (CanFireAtPlayer() == true)
        {
            _shootPivot.LookAt(GetPlayerPosition());

            if(_cooldown <= 0)
            {
                _cooldown = 7;
                StartCoroutine(Shoot());
            }

        }
    }

    private IEnumerator Shoot()
    {
        for (int i = 0; i < 3; i++)
        {
            _shoot.Play();
            Instantiate(_Bullet , _ShootSpot.position, _ShootSpot.rotation);
            yield return new WaitForSeconds(0.15f);
            _shoot.Play();
            Instantiate(_Bullet, _ShootSpot.position, _ShootSpot.rotation);
            yield return new WaitForSeconds(0.4f);
        }
    }


    public bool CanFireAtPlayer()
    {
        if (GetPlayerPosition().x >= transform.position.x && GetPlayerPosition().x <= transform.position.x + 100)
        {
            if (GetPlayerPosition().y <= transform.position.y -5 && GetPlayerPosition().y >= transform.position.y - 60)
            {
                return true;
            }
        }
        return false;
    }

    public void OnCollisionEnter(Collision collision)
    {
        _npcRoot.Kill();
    }
}
