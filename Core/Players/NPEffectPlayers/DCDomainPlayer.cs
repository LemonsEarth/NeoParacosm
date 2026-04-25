using NeoParacosm.Content.Items.Weapons.Magic;
using NeoParacosm.Content.Projectiles.Friendly.Magic.HeldProjectiles;
using NeoParacosm.Core.Systems.Assets;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;

namespace NeoParacosm.Core.Players.NPEffectPlayers;

public class DCDomainPlayer : ModPlayer
{
    int Timer = 0;
    public static Projectile DCDomainProjectile;
    float effectOpacity = 0f;
    public override void ResetEffects()
    {

    }

    public override void PostUpdateMiscEffects()
    {
        DomainEffects();
        Timer++;
    }

    void DomainEffects()
    {
        if (DCDomainProjectile != null && DCDomainProjectile.active && DCDomainProjectile.ModProjectile is StaffOfTheCataclysmHeldProj staffProj)
        {
            effectOpacity = MathHelper.Lerp(effectOpacity, 1f, 1 / 60f);
            ScreenShaderData data = Filters.Scene.Activate("NeoParacosm:DCDomainEffect").GetShader();
            data.UseImage(ParacosmTextures.NoiseTexture.Value);
            data.UseTargetPosition(staffProj.DomainPos);
            data.UseOpacity(effectOpacity);
            data.Shader.Parameters["range"].SetValue(StaffOfTheCataclysm.Range);
            if (Player.DistanceSQ(staffProj.DomainPos) < StaffOfTheCataclysm.Range * StaffOfTheCataclysm.Range)
            {
                if (!SkyManager.Instance["NeoParacosm:DCDomainSky"].IsActive())
                {
                    SkyManager.Instance.Activate("NeoParacosm:DCDomainSky");
                }

                SkyManager.Instance["NeoParacosm:DCDomainSky"].Opacity = effectOpacity;
            }
        }
        else
        {
            effectOpacity = MathHelper.Lerp(effectOpacity, 0, 1 / 30f);
            Filters.Scene["NeoParacosm:DCDomainEffect"].GetShader().UseOpacity(effectOpacity);
            if (SkyManager.Instance["NeoParacosm:DCDomainSky"].IsActive())
            {
                SkyManager.Instance["NeoParacosm:DCDomainSky"].Opacity = effectOpacity;
            }
            if (effectOpacity <= 0.001f)
            {
                effectOpacity = 0f;
                Filters.Scene.Deactivate("NeoParacosm:DCDomainEffect");
                SkyManager.Instance.Deactivate("NeoParacosm:DCDomainSky");
            }
        }
    }
}
