using UnityEngine;

public struct GameEffectArgs {
    public GameObject source;
    public GameObject target;
    public Vector3 position;
    public float multiplier;

    public GameEffectArgs(GameObject _source, GameObject _target, Vector3 _position, float _multiplier = 1f) {
        source = _source;
        target = _target;
        position = _position;
        multiplier = _multiplier;
    }
}

public enum GameEffectTarget {
    SOURCE,
    TARGET,
    POSITION,
}

public class GameEffect {
    public virtual bool Execute(GameEffectArgs args) { return true; }

    public Vector3 get_center_position(GameEffectArgs args, GameEffectTarget effect_target) {
        Vector3 center_position;
        if (effect_target == GameEffectTarget.POSITION) {
            center_position = args.position;
        } else {
            Transform center_transform = effect_target == GameEffectTarget.SOURCE ? args.source.transform : args.target.transform;
            center_position = center_transform.position;
        }

        return center_position;
    }

    public Vector3 get_center_position(GameEffectArgs args, GameEffectTarget effect_target, Vector3 local_offset) {
        Vector3 center_position;
        if (effect_target == GameEffectTarget.POSITION) {
            center_position = args.position;
        } else {
            Transform center_transform = effect_target == GameEffectTarget.SOURCE ? args.source.transform : args.target.transform;
            center_position = center_transform.position + center_transform.TransformDirection(local_offset);
        }

        return center_position;
    }
}

public class DamageGameEffect : GameEffect {
    public DamageArgs damage_args;
    public GameEffectTarget effect_target = GameEffectTarget.TARGET;

    public override bool Execute(GameEffectArgs args) {
        Transform effect_target_transform = effect_target == GameEffectTarget.SOURCE ? args.source.transform : args.target.transform;
        DamageArgs apply_damage_args = new DamageArgs(damage_args);
        apply_damage_args.damage *= args.multiplier;
        IDamageable damageable = effect_target_transform.GetComponent<IDamageable>();
        if (damageable != null) damageable.damage(apply_damage_args);
        return true;
    }
}

public class KnockbackGameEffect : GameEffect {
    public GameEffectTarget knockback_source = GameEffectTarget.SOURCE;
    public GameEffectTarget knockback_target = GameEffectTarget.TARGET;
    public float knockback_magnitude = 1f;

    public override bool Execute(GameEffectArgs args) {
        Vector3 effect_source_position = knockback_source switch {
            GameEffectTarget.SOURCE => args.source.transform.position,
            GameEffectTarget.TARGET => args.target.transform.position,
            _ => args.position,
        };

        Vector3 effect_target_position;
        Transform effect_target_transform;
        switch (knockback_target) {
            case GameEffectTarget.SOURCE:
                effect_target_position = args.source.transform.position;
                effect_target_transform = args.source.transform;
                break;
            default:
                effect_target_position = args.target.transform.position;
                effect_target_transform = args.target.transform;
                break;
        }

        Vector3 knockback_direction = effect_target_position - effect_source_position;
        IKnockbackable knockbackable = effect_target_transform.GetComponent<IKnockbackable>();
        if (knockbackable != null) knockbackable.knockback(knockback_direction * knockback_magnitude * args.multiplier);
        return true;
    }
}

public class AreaGameEffect : GameEffect {
    public GameEffectTarget effect_target = GameEffectTarget.SOURCE;
    public Vector3 local_offset = Vector3.zero;

    public float area_radius = 1f;
    public LayerMask area_layer_mask;

    public GameEffect[] effects;

    public override bool Execute(GameEffectArgs args) {
        Vector3 center_position = get_center_position(args, effect_target, local_offset);
        Collider[] colliders = Physics.OverlapSphere(center_position, area_radius, area_layer_mask);

        for (int i = 0; i < colliders.Length; i++) {
            float effect_multiplier = Vector3.Distance(colliders[i].transform.position, center_position) / area_radius;
            for (int j = 0; j < effects.Length; j++) effects[j].Execute(new GameEffectArgs(args.source, colliders[i].gameObject, center_position, effect_multiplier));
        }

        return colliders.Length > 0;
    }
}

public class RayGameEffect : GameEffect {
    public override bool Execute(GameEffectArgs args) {
        throw new System.NotImplementedException();
    }
}

public class SpawnerGameEffect : GameEffect {
    public GameEffectTarget parent = GameEffectTarget.SOURCE;
    public GameEffectTarget spawn_point = GameEffectTarget.SOURCE;
    public GameEffectTarget look_target = GameEffectTarget.TARGET;
    public Vector3 local_offset = Vector3.zero;
    public Vector3 local_direction = Vector3.zero;
    public Vector3 scale = Vector3.one;
    public GameObject prefab;

    public bool look_at_target;
    public bool face_local_direction;

    public override bool Execute(GameEffectArgs args) {
        Vector3 center_position = get_center_position(args, spawn_point, local_offset);
        Util.DrawAABB2D(center_position - Vector3.one, Vector3.one * 2f, Color.blue);

        Quaternion rotation;
        if (look_at_target) {
            Vector3 look_point = look_target switch {
                GameEffectTarget.SOURCE => args.source.transform.position,
                GameEffectTarget.TARGET => args.target.transform.position,
                _ => args.position,
            };
            rotation = Quaternion.LookRotation((look_point - center_position).normalized);
        } else if (face_local_direction) {
            Transform spawn_transform = spawn_point == GameEffectTarget.SOURCE || spawn_point == GameEffectTarget.POSITION ? args.source.transform : args.target.transform;
            rotation = Quaternion.LookRotation((center_position + spawn_transform.TransformDirection(local_direction)) - center_position);
        } else {
            rotation = prefab.transform.rotation;
        }

        Transform spawn;
        if (parent == GameEffectTarget.POSITION) {
            spawn = GameObject.Instantiate(prefab, center_position, rotation).transform;
        } else {
            spawn = GameObject.Instantiate(prefab, center_position, rotation, parent == GameEffectTarget.SOURCE ? args.source.transform : args.target.transform).transform;
        }
        
        Debug.Log("Spawned " + spawn.name + " at " + center_position);

        spawn.localScale = scale;

        return true;
    }
}

public class SoundGameEffect : GameEffect {
    public GameEffectTarget effect_target = GameEffectTarget.SOURCE;
    public Vector3 local_offset = Vector3.zero;
    public SoundPool sound_pool;
    // public float volume;
    public Vector2 pitch_range = new Vector2(0.9f, 1.1f);

    public override bool Execute(GameEffectArgs args) {
        Vector3 center_position = get_center_position(args, effect_target, local_offset);
        Util.DrawAABB2D(center_position - Vector3.one * 0.5f, Vector3.one, Color.red);
        SoundManager.instance.play_sound_3d_pitched(sound_pool.get_sound(), center_position, pitch_range.x, pitch_range.y);
        return true;
    }
}