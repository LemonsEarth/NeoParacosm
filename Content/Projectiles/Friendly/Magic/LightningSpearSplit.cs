using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Common.Utils.Prim;
using NeoParacosm.Content.Items.Weapons.Magic.Spells;
using NeoParacosm.Core.Systems.Assets;
using Terraria.Audio;
using Terraria.GameContent;

namespace NeoParacosm.Content.Projectiles.Friendly.Magic;

public class LightningSpearSplit : ModProjectile
{
    public override string Texture => ParacosmTextures.Empty100TexPath;

    ref float AITimer => ref Projectile.ai[0];
    ref float Scaling => ref Projectile.ai[1];

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 42;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
    }

    public override void SetDefaults()
    {
        Projectile.width = 24;
        Projectile.height = 24;
        Projectile.friendly = true;
        Projectile.timeLeft = 600;
        Projectile.penetrate = 3;
        Projectile.Opacity = 1f;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 30;
        Projectile.tileCollide = false;
        Projectile.extraUpdates = 0;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {

    }

    public override void AI()
    {
        Player player = Projectile.GetOwner();
        if (AITimer == 0)
        {
            SoundEngine.PlaySound(ParacosmSFX.ElectricBurst with { Volume = 0.8f, PitchRange = (0.4f, 0.8f) }, Projectile.Center);
        }

        Vector2 velocityDir = Projectile.velocity.SafeNormalize(Vector2.Zero);
        Vector2 leftPos = Projectile.Center - velocityDir * Scaling * 32;
        Vector2 rightPos = Projectile.Center + velocityDir * Scaling * 32;
        if (AITimer % 2 == 0)
        {
            LemonUtils.DustLine(leftPos, rightPos, DustID.GemTopaz, 4, 1.5f);
            LemonUtils.DustLine(leftPos, rightPos, DustID.GemDiamond, 4, 0.7f);
        }

        AITimer++;
    }

    public override bool? CanHitNPC(NPC target)
    {
        return null;
    }

    public override void OnKill(int timeLeft)
    {
        LemonUtils.DustCircle(Projectile.Center, 8, 8, DustID.GemTopaz, 2.5f);
        LemonUtils.DustCircle(Projectile.Center, 6, 6, DustID.GemDiamond, 2f);
        SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        return false;
    }
}
