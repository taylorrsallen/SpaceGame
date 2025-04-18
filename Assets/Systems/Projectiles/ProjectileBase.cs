using UnityEngine;

public abstract class ProjectileBase : MonoBehaviour , IProjectile
{
    [SerializeField] private float Damage = 1;
    [SerializeField] private float Force = 100;
    [SerializeField] private float LifeTimeAfterImpact = 0;

    public void Start()
    {
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.AddForce(transform.forward * Force);
    }

    public void SetDamage(float damage)
    {
        Damage = damage;
    }

    public void SetRotation(Vector3 euler)
    {
        transform.eulerAngles = euler;
    }

    public void SetForce(float force)
    {
        Force = force;
    }

    public void OnCollisionEnter(Collision collision)
    {
        OnDeath();
        Destroy(gameObject, LifeTimeAfterImpact);
    }

    protected void DamageRadius(float radius)
    {
        Collider[] collider = Physics.OverlapSphere(transform.position, radius);

        foreach (Collider col in collider)
        {
            IDamageable damageable = col.transform.GetComponent<IDamageable>();

            if(damageable != null)
            {
                damageable.damage(new DamageArgs(Damage));
            }
        }
    }
    protected abstract void OnDeath();

    private int _KillDistanceCheck = 6;

    protected Vector3 GetPlayerPosition()
    {
        return GameManager.instance.ship_controller.get_ship_position();
    }

    protected float GetDistanceToPlayer()
    {
        float distance = Vector3.Distance(transform.position, GetPlayerPosition());
        return distance;
    }

    protected void CheckForKillDistance()
    {
        _KillDistanceCheck--;
        if (_KillDistanceCheck <= 0)
        {
            _KillDistanceCheck = 8;
            if (GetDistanceToPlayer() >= 400)
                Destroy(gameObject);
        }
    }
}
