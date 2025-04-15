using UnityEngine;
using NPCs.Base;
public class PlasmaBall : NPC
{
    [SerializeField] private GameEffectData PlasmaImpact;
    protected override void OnDeath()
    {
        PlasmaImpact.Execute(new GameEffectArgs(this.gameObject, null, transform.position));
        Destroy(gameObject);
    }
}
