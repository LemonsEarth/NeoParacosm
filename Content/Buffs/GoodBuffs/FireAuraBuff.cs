using NeoParacosm.Content.Items.Weapons.Magic.Spells;
using NeoParacosm.Core.Players;

namespace NeoParacosm.Content.Buffs.GoodBuffs;

public class FireAuraBuff : ModBuff
{
    public override void SetStaticDefaults()
    {
        Main.debuff[Type] = false;
    }
}

public class FireAuraPlayer : ModPlayer
{
    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        if (Player.HasBuff(BuffType<FireAuraBuff>()))
        {
            float distance = Player.Center.Distance(target.Center);
            int defReduction = (int)((1 - MathHelper.Clamp((distance / 500f), 0, 1)) * 10 * Player.GetElementalExpertiseBoostMultiplied(BaseSpell.SpellElement.Fire, 2f));
            modifiers.Defense.Flat -= defReduction;
        }
    }

    public override void UpdateEquips()
    {
        if (Player.HasBuff(BuffType<FireAuraBuff>()) && NPPlayer.timer % 120 == 0)
        {
            if (Main.myPlayer == Player.whoAmI)
            {
                LemonUtils.QuickPulse(Player, Player.Center, 1, 5, 5, Color.Red, Player);
            }
        }
    }
}
