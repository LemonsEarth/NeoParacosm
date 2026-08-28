using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Buffs.Debuffs;
using NeoParacosm.Content.Dusts;
using NeoParacosm.Core.Systems.Particles;
using Terraria.Audio;
using Terraria.GameContent;

namespace NeoParacosm.Content.Projectiles.Friendly.Special;

public class LingeringDeathflameFriendly : ModProjectile
{
    int AITimer = 0;

    ref float doFall => ref Projectile.ai[0];
    ref float duration => ref Projectile.ai[1];
    ref float height => ref Projectile.ai[2];

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 13;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 4;
    }

    public override void SetDefaults()
    {
        Projectile.width = 32;
        Projectile.height = 32;
        Projectile.hostile = false;
        Projectile.friendly = true;
        Projectile.ignoreWater = false;
        Projectile.tileCollide = true;
        Projectile.penetrate = 10;
        Projectile.timeLeft = 3600;
        Projectile.scale = 1f;
        Projectile.Opacity = 1f;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 10;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        return false;
    }

    float yScale = 1f;
    public override void AI()
    {
        if (AITimer == 0)
        {
            LemonUtils.DustCircle(Projectile.Center, 8, 8, DustID.GemDiamond, 1f);
            SoundEngine.PlaySound(SoundID.DD2_WyvernDiveDown with { PitchRange = (0f, 0.2f) }, Projectile.Center);
            if (height == 0) height = 0.5f;
            yScale = 1f;
        }

        if (Projectile.velocity.Y == 0)
        {
            Projectile.rotation = MathHelper.Lerp(Projectile.rotation, 0f + MathHelper.Pi, 1 / 10f);
            yScale = MathHelper.Lerp(yScale, 1f, 1 / 20f);
            if (AITimer % 2 == 0)
            {
                foreach (var projectile in Main.ActiveProjectiles) // killing oldest Lingering Deathflame on the same (ish) position
                {
                    if (projectile.type == Type && projectile != Projectile && Projectile.Distance(projectile.Center) < 32)
                    {
                        if (Projectile.timeLeft < projectile.timeLeft)
                        {
                            Projectile.Kill();
                        }
                        else
                        {
                            projectile.Kill();
                        }
                    }
                }

                Vector2 randomPos = Projectile.Bottom + new Vector2(Main.rand.NextFloat(-Projectile.width, Projectile.width), 0);
                ParticleSystem.SpawnParticle(
                        ParticleID.Gas,
                        randomPos,
                        Vector2.Zero,
                        Color.Black,
                        scale: 0.5f,
                        data0: Main.rand.NextFloat(0.05f, 0.15f)
                        );
                ParticleSystem.SpawnParticle(
                    ParticleID.Gas,
                    randomPos,
                    Vector2.Zero,
                    Color.Black,
                    scale: 0.5f,
                    data0: Main.rand.NextFloat(0.01f, 0.05f)
                    );
                Dust.NewDustPerfect(randomPos, DustID.GemDiamond, -Vector2.UnitY * Main.rand.NextFloat(2f, 4f) * height, Scale: 1.5f, newColor: Color.White).noGravity = true;
            }

            Projectile.velocity.X = 0;
        }
        else
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            yScale = 2 + Projectile.velocity.Y * 0.2f;
            if (AITimer % 8 == 0)
            {
                Dust.NewDustDirect(Projectile.RandomPos(), 2, 2, DustID.Ash, 0, 0, Scale: 1.5f, newColor: Color.Black).noGravity = true;
                Dust.NewDustDirect(Projectile.RandomPos(), 2, 2, DustID.GemDiamond, 0, 0, Scale: 1.25f, newColor: Color.White).noGravity = true;
            }
        }

        Lighting.AddLight(Projectile.Center, 0, 1, 0);

        if (AITimer > duration)
        {
            Projectile.Kill();
        }

        if (doFall == 1)
        {
            Projectile.velocity.Y += 0.1f;
        }

        Projectile.StandardAnimation(6, 4);
        AITimer++;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        /*if (Projectile.velocity.Y == 0)
        {
            return false;
        }*/
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        Vector2 drawOrigin = new Vector2(16, 0);
        Vector2 scale = new Vector2(1, yScale);
        Main.EntitySpriteDraw(texture, Projectile.Bottom - Main.screenPosition, texture.Frame(1, 4, 0, Projectile.frame), Color.White, Projectile.rotation, drawOrigin, scale, SpriteEffects.None);
        return false;
    }

    public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
    {
        fallThrough = false;
        return true;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.AddBuff(BuffType<DeathflameDebuff>(), 240);
    }
}
