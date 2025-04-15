using UnityEngine;
using NPCs.Base;
public class Meteorite : NPC
{
    [SerializeField] private GameEffectData BiPlaneExplosion;
    protected override void OnDeath()
    {
        BiPlaneExplosion.Execute(new GameEffectArgs(this.gameObject, null, transform.position));
        Destroy(gameObject);
    }
}
