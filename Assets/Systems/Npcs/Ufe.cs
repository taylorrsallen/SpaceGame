using UnityEngine;
using NPCs.Base;
public class Ufe : NPC
{
    protected override void SetDefaults()
    {
        maxHp = 150;     
        aiType = AiType.UFE;
    }
}
