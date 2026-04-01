using NeoParacosm.Content.Dusts;
using NeoParacosm.Content.Items.Weapons.Magic.Spells;
using NeoParacosm.Core.Players;

namespace NeoParacosm.Content.Buffs.GoodBuffs;

public class GreenAuraBuff : ModBuff
{
    public override void SetStaticDefaults()
    {
        Main.debuff[Type] = false;
    }
}

public class GreenAuraPlayer : ModPlayer
{
    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {

    }

    public override void UpdateEquips()
    {
        if (Player.HasBuff(BuffType<GreenAuraBuff>()))
        {
            Player.AddBuff(BuffID.Stinky, 2);

            if (Main.myPlayer == Player.whoAmI && NPPlayer.timer % 30 == 0)
            {
                foreach (var npc in Main.ActiveNPCs)
                {
                    if (npc.CanBeChasedBy() && npc.Distance(Player.Center) < 160 * Player.GetElementalExpertiseBoostMultiplied(SpellElement.Nature, 8))
                    {
                        npc.AddBuff(BuffID.Poisoned, ((int)(60 * Player.GetElementalExpertiseBoostMultiplied(SpellElement.Nature, 5))));
                    }
                }
            }
        }
    }
}
