using NeoParacosm.Content.Dusts;
using NeoParacosm.Content.Items.Weapons.Magic.Spells;
using NeoParacosm.Core.Players;
using Terraria.DataStructures;

namespace NeoParacosm.Content.Buffs.GoodBuffs;

public class MerchantsBlessingBuff : ModBuff
{
    public override void SetStaticDefaults()
    {
        Main.debuff[Type] = false;
    }
}

public class MerchantsBlessingBuffPlayer : ModPlayer
{
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (Player.HasBuff(BuffType<MerchantsBlessingBuff>()) && target.life <= 0)
        {
            if (Main.rand.NextBool(4))
            {
                Player.QuickSpawnItem(Player.GetSource_FromThis(), ItemID.CopperCoin, (int)target.value);
            }
        }
    }
}
