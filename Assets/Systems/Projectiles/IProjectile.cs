using UnityEngine;

public interface IProjectile
{
    public void SetRotation(Vector3 euler);
    public void SetDamage(float damage);
    public void SetForce(float force);
}
