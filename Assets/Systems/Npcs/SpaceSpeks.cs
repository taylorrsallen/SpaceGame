using UnityEngine;
using NPCs.Base;
public class SpaceSpeks : NPC
{
    [SerializeField] private GameEffectData SpaceXExplosion;
    protected override void OnDeath()
    {
        SpaceXExplosion.Execute(new GameEffectArgs(this.gameObject, null, transform.position));
        Destroy(gameObject);
    }
}
