using NeoParacosm.Content.NPCs.Friendly.Quest.Researcher;
using NeoParacosm.Content.Projectiles.Hostile.Death;
using NeoParacosm.Content.Projectiles.Hostile.Evil.DreadlordProjectiles;
using NeoParacosm.Core.UI.ResearcherUI.Ascension;
using System.Collections.Generic;

namespace NeoParacosm.Core.Players;

public abstract class CoatingPlayer : ModPlayer
{
    public bool Active { get; set; } = false;

    public int Timer { get; private set; } = 0;
    public abstract int BaseCD { get; }

    public abstract void OnHitEffect(NPC target, NPC.HitInfo hit);

    public override void ResetEffects()
    {
        Active = false;
    }

    public override void PostUpdateEquips()
    {
        if (Active && Timer > 0)
        {
            Timer--;
        }
    }

    public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (Active && Timer == 0)
        {
            OnHitEffect(target, hit);
            Timer = BaseCD;
        }
    }

    public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (Active && Timer == 0 && proj.CountsAsTrueMelee())
        {
            OnHitEffect(target, hit);
            Timer = BaseCD;
        }
    }
}