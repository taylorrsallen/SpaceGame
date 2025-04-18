using UnityEngine;

public class Bullet : ProjectileBase
{
    protected override void OnDeath()
    {
        DamageRadius(0.1f);
    }
}
