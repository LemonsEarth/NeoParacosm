using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Buffs.Debuffs;
using NeoParacosm.Core.Systems.Assets;
using Terraria.Audio;
using Terraria.GameContent;

namespace NeoParacosm.Content.Projectiles.Friendly.Ranged;

public class CrematorRound : ModProjectile
{
    int AITimer = 0;

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        ProjectileID.Sets.DrawScreenCheckFluff[Type] = 5000;
        Main.projFrames[Type] = 1;
    }

    public override void SetDefaults()
    {
        Projectile.width = 32;
        Projectile.height = 32;
        Projectile.hostile = false;
        Projectile.friendly = true;
        Projectile.ignoreWater = false;
        Projectile.tileCollide = true;
        Projectile.penetrate = 24;
        Projectile.stopsDealingDamageAfterPenetrateHits = true;
        Projectile.ArmorPenetration = 30;
        Projectile.timeLeft = 60;
        Projectile.scale = 1f;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 30;
        Projectile.extraUpdates = 5;
    }

    public override void AI()
    {
        if (AITimer == 0)
        {
            SoundEngine.PlaySound(ParacosmSFX.SniperShot with { Volume = 0.5f, PitchRange = (-0.2f, 0.2f) }, Projectile.Center);
        }

        for (int i = 0; i < 2; i++)
        {
            Dust.NewDustDirect(Projectile.RandomPos(), 2, 2, DustID.GemSapphire, Scale: Main.rand.NextFloat(1.5f, 2.5f)).noGravity = true;
            Dust.NewDustDirect(Projectile.RandomPos(), 2, 2, DustID.GemDiamond, Scale: Main.rand.NextFloat(1.5f, 2.5f)).noGravity = true;
        }

        if (Main.myPlayer == Projectile.owner && AITimer % 2 == 0)
        {
            LemonUtils.QuickProj(
                Projectile,
                Projectile.RandomPos(),
                Vector2.Zero,
                ProjectileID.Mushroom,
                Projectile.damage / 4f
                );
        }

        Lighting.AddLight(Projectile.Center, 0.6f, 0.6f, 1);

        /*var dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.ScourgeOfTheCorruptor);
        dust.noGravity = true;*/

        AITimer++;
    }

    public override void OnKill(int timeLeft)
    {

    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        return null;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        return false;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.AddBuff(BuffType<ShroomedDebuff>(), 300);
    }
}
