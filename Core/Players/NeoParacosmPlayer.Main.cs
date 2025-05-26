using NeoParacosm.Common.Utils;
using NeoParacosm.Content.Biomes.TheDeep;
using NeoParacosm.Content.Buffs.Debuffs.Cooldowns;
using Terraria.GameContent.Generation;
using Terraria.WorldBuilding;

namespace NeoParacosm.Core.Players;

public partial class NeoParacosmPlayer : ModPlayer
{
    public override void ResetEffects()
    {
        ResetAccessoryFields();
    }

    public override void PostUpdate()
    {
        //LemonUtils.DebugPlayerCenter(Player);
        //Main.NewText("World Surface: " + (int)Main.worldSurface);
        if (Player.InModBiome<DeepHigh>())
        {
            
        }
    }

    public override void OnHurt(Player.HurtInfo info)
    {
        if (roundShield && !Player.HasBuff(ModContent.BuffType<KnockbackCooldown>()))
        {
            Player.AddBuff(ModContent.BuffType<KnockbackCooldown>(), 1800);
        }
    }
}
