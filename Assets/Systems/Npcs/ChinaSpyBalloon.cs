using UnityEngine;
using NPCs.Base;
public class ChinaSpyBalloon : NPC
{
    [SerializeField] private GameEffectData Explosion;
    protected override void OnDeath()
    {
        Explosion.Execute(new GameEffectArgs(this.gameObject, null, transform.position));
        Destroy(gameObject);
    }
}
