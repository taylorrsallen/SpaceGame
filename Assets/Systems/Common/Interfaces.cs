using UnityEngine;

public struct DamageArgs {
    public float damage;

    public DamageArgs(float _damage) {
        damage = _damage;
    }

    public DamageArgs(DamageArgs args) {
        damage = args.damage;
    }
}

public interface IDamageable {
    public void damage(DamageArgs args);
}

public interface IKnockbackable {
    public void knockback(Vector3 force);
}