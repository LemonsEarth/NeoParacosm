using NeoParacosm.Content.NPCs.Misc;
using System.Linq;
using Terraria.DataStructures;

namespace NeoParacosm.Core.Players;

public class NPArmorPlayer : ModPlayer
{
    public bool ascendedShadowArmor { get; set; } = false;
    int shadowTimer = 0;

    public override void ResetEffects()
    {
        ascendedShadowArmor = false;
    }

    public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
    {
        
    }

    public override void OnHurt(Player.HurtInfo info)
    {

    }

    public override void NaturalLifeRegen(ref float regen)
    {

    }

    public override void PostUpdateEquips()
    {
        if (shadowTimer > 0)
        {
            shadowTimer--;
        }
    }

    public override void GetHealLife(Item item, bool quickHeal, ref int healValue)
    {

    }

    public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (ascendedShadowArmor && target.CanBeChasedBy() && shadowTimer == 0 && !Main.npc.Any(n => n.active && n.type == ModContent.NPCType<ShadowOrbNPC>() && n.ai[0] == Player.whoAmI))
        {
            for (int i = 0; i < 2; i++)
            {
                NPC.NewNPCDirect(Player.GetSource_FromThis(), target.Center + Main.rand.NextVector2Circular(300, 300), ModContent.NPCType<ShadowOrbNPC>(), ai0: Player.whoAmI);
            }
            shadowTimer = 600;
        }
    }
}
