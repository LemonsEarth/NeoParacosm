using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Common.Utils;
using ReLogic.Content;
using System.IO;
using Terraria.Audio;

namespace NeoParacosm.Content.Projectiles.Friendly.Melee;

public class AscendedRottedForkHeldProj : ModProjectile
{
    static Asset<Texture2D> trailTexture;

    int AITimer = 0;
    ref float addsLeft => ref Projectile.ai[0];
    bool released = false;

    Vector2 savedMousePos
    {
        get
        {
            return new Vector2(Projectile.ai[1], Projectile.ai[2]);
        }
        set
        {
            Projectile.ai[1] = value.X;
            Projectile.ai[2] = value.Y;
        }
    }

    public override void Load()
    {
        trailTexture = ModContent.Request<Texture2D>("NeoParacosm/Content/Projectiles/Friendly/Melee/AscendedRottedForkTrail");
    }


    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 1;
    }

    public override void SetDefaults()
    {
        Projectile.width = 84;
        Projectile.height = 84;
        Projectile.hostile = false;
        Projectile.friendly = true;
        Projectile.ignoreWater = false;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 60;
        Projectile.DamageType = DamageClass.Melee;
        Projectile.Opacity = 0f;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 10;
    }

    public override void AI()
    {
        Player player = Main.player[Projectile.owner];
        Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter);

        if (AITimer < 30) Projectile.Opacity = MathHelper.Lerp(0, 1, AITimer / 30f);
        if (Projectile.timeLeft < 30) Projectile.Opacity = MathHelper.Lerp(0, 1, Projectile.timeLeft / 30f);

        if (player.channel && !released)
        {
            Dust.NewDustPerfect(Projectile.Center + new Vector2(-1, -1).RotatedBy(Projectile.rotation) * Projectile.height * 0.5f, DustID.Crimson).noGravity = true;
            Projectile.timeLeft = 60;
            player.heldProj = Projectile.whoAmI;
            player.SetDummyItemTime(2);
            Projectile.velocity = Vector2.Zero;
            Projectile.Center = playerCenter;
            Projectile.rotation = MathHelper.ToRadians(AITimer * 15 * player.direction);
            Projectile.spriteDirection = -player.direction;
            SoundEngine.PlaySound(SoundID.Item1, Projectile.Center);

            if (AITimer % 60 == 0 && AITimer > 0)
            {
                if (addsLeft > 0)
                {
                    if (Main.myPlayer == Projectile.owner)
                    {
                        SoundEngine.PlaySound(SoundID.DD2_DarkMageSummonSkeleton with { PitchRange = (-0.2f, 0.2f) });
                        LemonUtils.QuickProj(Projectile, Projectile.Center, Vector2.Zero, Type, Projectile.damage / (addsLeft + 1), ai0: -addsLeft);
                    }
                    addsLeft--;
                }
            }
        }
        else
        {
            released = true;
        }
        if (released)
        {
            Projectile.localNPCHitCooldown = 30;
            if (AITimer % 3 == 0 && addsLeft >= 0)
            {
                Vector2 pos = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * Projectile.height * 0.5f;
                Dust.NewDustPerfect(pos, DustID.Crimson, Projectile.velocity.RotatedBy(MathHelper.PiOver2).SafeNormalize(Vector2.Zero) * 5, Scale: 2f).noGravity = true;
                Dust.NewDustPerfect(pos, DustID.Crimson, Projectile.velocity.RotatedBy(-MathHelper.PiOver2).SafeNormalize(Vector2.Zero) * 5, Scale: 2f).noGravity = true;
            }
            if (savedMousePos == Vector2.Zero)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    savedMousePos = Main.MouseWorld;
                }
                Projectile.netUpdate = true;
                Vector2 dirToMouse = player.Center.DirectionTo(savedMousePos);
                if (addsLeft >= 0)
                {
                    Projectile.velocity = dirToMouse * (5 * (5 - addsLeft));
                }
                else
                {
                    Projectile.velocity = dirToMouse * (5 * -addsLeft);
                }
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
                Projectile.spriteDirection = -1;
                Projectile.timeLeft = 120;
            }
        }

        AITimer++;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        if (addsLeft >= 0)
        {
            return true;
        }

        Color color = Color.Red * (-addsLeft * 0.25f);
        color.A = 255;

        Main.EntitySpriteDraw(trailTexture.Value, Projectile.Center - Main.screenPosition, null, color * Projectile.Opacity, Projectile.rotation, trailTexture.Size() * 0.5f, Projectile.scale, LemonUtils.SpriteDirectionToSpriteEffects(Projectile.spriteDirection));
        return false;
    }
}
