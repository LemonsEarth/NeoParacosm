using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Projectiles.Hostile.Researcher;
using NeoParacosm.Core.Systems.Assets;
using Steamworks;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;

namespace NeoParacosm.Content.Projectiles.Friendly.Ranged;

public class CreminiCrematorHeldProj : ModProjectile
{
    int AITimer = 0;

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {

    }

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 1;
    }

    public override void SetDefaults()
    {
        Projectile.width = 140;
        Projectile.height = 40;
        Projectile.hostile = false;
        Projectile.friendly = false;
        Projectile.ignoreWater = false;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 180;
        Projectile.DamageType = DamageClass.Ranged;
        Projectile.Opacity = 1f;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 300;
    }

    bool releasedWhileCharged = false;
    Vector2 toMouse;
    int laserCount = 0;
    float baseLaserIncrementInterval = 60;
    float laserIncrementInterval => baseLaserIncrementInterval / Projectile.GetOwner().GetAttackSpeed(DamageClass.Ranged);
    int incrementInterval = 0;
    public override void AI()
    {
        Player player = Projectile.GetOwner();
        if (!player.Alive())
        {
            Projectile.Kill();
            return;
        }

        if (AITimer == 0)
        {
            incrementInterval = (int)laserIncrementInterval; // saving so it doesnt change mid-fire
            SoundEngine.PlaySound(ParacosmSFX.SniperScope with { Volume = 0.75f, PitchRange = (-0.2f, 0.2f) }, Projectile.Center);
        }

        if (!player.channel)
        {
            if (AITimer >= incrementInterval * 4 && !releasedWhileCharged)
            {
                releasedWhileCharged = true;
                Projectile.Center += -toMouse * 80;
                if (Main.myPlayer == Projectile.owner)
                {
                    LemonUtils.QuickProj(
                        Projectile,
                        laserBasePos + toMouse * (Projectile.width),
                        toMouse * 50,
                        ProjectileType<CrematorRound>()
                        );
                }
            }
            if (!releasedWhileCharged)
            {
                Projectile.Kill();
                return;
            }
        }

        if (AITimer % incrementInterval == 0 && AITimer > 0 && laserCount < 4)
        {
            laserCount += 1;
            SoundEngine.PlaySound(SoundID.Item15 with { Volume = 0.75f, PitchRange = (-0.1f, 0.1f) }, Projectile.Center);
        }

        if (Main.myPlayer == Projectile.owner)
        {
            toMouse = player.Center.DirectionTo(Main.MouseWorld);
        }
        player.heldProj = Projectile.whoAmI;
        if (!releasedWhileCharged)
        {
            player.SetDummyItemTime(60);
            Projectile.timeLeft = 60;
        }

        Projectile.velocity = Vector2.Zero;
        SetPositionRotationDirection(player, toMouse.ToRotation());
        //Projectile.Center = player.Center + player.Center.DirectionTo(Main.MouseWorld) * 5;

        AITimer++;
    }

    void SetPositionRotationDirection(Player player, float movedRotation = 0)
    {
        Vector2 dir = toMouse;
        float rot = dir.ToRotation();
        player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rot + -MathHelper.PiOver2);
        float spriteRot = player.direction == 1 ? 0 : MathHelper.Pi;
        Projectile.rotation = rot + spriteRot;
        if (!releasedWhileCharged)
        {
            Projectile.Center = player.Center - dir * (Projectile.width / 4); // offset so player looks like theyre holding it
        }
        else
        {
            Projectile.Center = Vector2.Lerp(Projectile.Center, player.Center - dir * (Projectile.width / 4), 1 / 2f); // offset so player looks like theyre holding it
        }
        Projectile.spriteDirection = player.direction;
        player.ChangeDir(LemonUtils.Sign(dir.X, 1));
    }

    Vector2 laserBasePos => Projectile.Center - toMouse.RotatedBy(MathHelper.PiOver2) * 3 * Projectile.spriteDirection;

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        Vector2 drawPos = Projectile.Center - Main.screenPosition;
        int xOrigin = Projectile.spriteDirection == 1 ? 0 : texture.Width;
        Vector2 drawOrigin = new Vector2(xOrigin, texture.Height * 0.5f);
        if (toMouse != Vector2.Zero && !releasedWhileCharged)
        {
            if (laserCount >= 1)
            {
                DrawLasers(MathHelper.Pi / 8, incrementInterval, incrementInterval, Color.DarkBlue, 1);
            }
            if (laserCount >= 2)
            {
                DrawLasers(2 * MathHelper.Pi / 8, incrementInterval * 2, incrementInterval, Color.DeepSkyBlue, 1);
            }
            if (laserCount >= 3)
            {
                DrawLasers(3 * MathHelper.Pi / 8, incrementInterval * 3, incrementInterval, Color.LightBlue, 1);
            }
        }
        Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, drawOrigin, Projectile.scale, LemonUtils.SpriteDirectionToSpriteEffects(Projectile.spriteDirection));
        return false;

    }

    void DrawLasers(float startAngle, int startTime, float timeToMove, Color color, float laserWidth)
    {
        for (int i = -1; i <= 1; i++)
        {
            if (i == 0) continue;
            float angleLerpT = MathHelper.Clamp((AITimer - startTime) / timeToMove, 0, 1);
            float angle = Utils.AngleLerp(i * startAngle, 0, angleLerpT);
            LemonUtils.DrawLaser(
                laserBasePos + toMouse * (Projectile.width),
                laserBasePos + toMouse.RotatedBy(angle) * 2000,
                laserWidth,
                color * angleLerpT);
        }
    }

    public override void PostDraw(Color lightColor)
    {
        //Main.spriteBatch.End();
        //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
    }
}
