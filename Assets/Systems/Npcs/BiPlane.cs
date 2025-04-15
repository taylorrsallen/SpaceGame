using NPCs.Base;
using UnityEngine;

public class BiPlane : NPC
{
    [SerializeField] private GameEffectData BiPlaneExplosion;
    protected override void OnDeath()
    {
        BiPlaneExplosion.Execute(new GameEffectArgs(this.gameObject, null, transform.position));
        Destroy(gameObject);
    }
}
