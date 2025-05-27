using NeoParacosm.Common.Utils;
using NeoParacosm.Content.Biomes.TheDeep;
using NeoParacosm.Content.Buffs.Debuffs.Cooldowns;
using Terraria.Graphics.Effects;
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
        
    }

    public override void PostUpdateMiscEffects()
    {
        if (Player.InModBiome<DeepHigh>())
        {
            Filters.Scene.Activate("NeoParacosm:ScreenTintShader").GetShader().UseColor(new Color(102, 148, 255));
            Filters.Scene["NeoParacosm:ScreenTintShader"].GetShader().UseProgress(1);
        }
        else
        {
            Filters.Scene.Deactivate("NeoParacosm:ScreenTintShader");
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
