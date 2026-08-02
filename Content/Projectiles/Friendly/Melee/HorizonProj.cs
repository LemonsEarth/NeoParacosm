using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.IO;
using Terraria.Audio;
using Terraria.GameContent;

namespace NeoParacosm.Content.Projectiles.Friendly.Melee;

public class HorizonProj : ModProjectile
{
    int AITimer = 0;

    ref float Duration => ref Projectile.ai[0];
    ref float Delay => ref Projectile.ai[1];
    ref float ClosestNPCWhoAmI => ref Projectile.ai[2];

    float direction;
    int useCounter;

    static Asset<Texture2D> trailTexture;

    public override void Load()
    {
        trailTexture = Request<Texture2D>(Texture + "Trail");
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {

    }

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 16;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 1;
    }

    public override void SetDefaults()
    {
        Projectile.width = 82;
        Projectile.height = 82;
        Projectile.hostile = false;
        Projectile.friendly = true;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.stopsDealingDamageAfterPenetrateHits = true;
        Projectile.timeLeft = 600;
        Projectile.DamageType = DamageClass.Melee;
        Projectile.Opacity = 1f;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 60;
    }

    NPC closestNPC = null;
    public override void AI()
    {
        if (AITimer == 0)
        {
            direction = Projectile.velocity.X;
            useCounter = (int)Projectile.velocity.Y;
            ClosestNPCWhoAmI = -1;
            Projectile.velocity = Vector2.Zero;
        }

        if (AITimer == Duration)
        {
            Projectile.timeLeft = 30;
            Projectile.Opacity = MathHelper.Lerp(0f, 1f, Projectile.timeLeft / 30f);
            AITimer++;
            return;
        }
        else if (AITimer > Duration)
        {
            AITimer++;
            return;
        }

        if (ClosestNPCWhoAmI == -1)
        {
            if (Main.myPlayer == Projectile.owner)
            {
                closestNPC = LemonUtils.GetClosestNPC(Main.MouseWorld, 500);
                if (closestNPC != null)
                {
                    ClosestNPCWhoAmI = closestNPC.whoAmI;
                }
            }
            Projectile.netUpdate = true;

            if (ClosestNPCWhoAmI == -1)
            {
                Projectile.Kill();
                return;
            }
            closestNPC = Main.npc[((int)ClosestNPCWhoAmI)];
        }

        AttackBehavior();

        AITimer++;
    }

    Vector2 savedPos;
    void AttackBehavior()
    {
        Player player = Projectile.GetOwner();
        float distanceToNPC = player.Distance(closestNPC.Center);
        if (AITimer < Delay)
        {
            Vector2 dir = player.Center.DirectionTo(closestNPC.Center).RotatedBy(-direction * MathHelper.Pi / 4f) * distanceToNPC;
            Vector2 targetPos = player.Center + dir;
            Projectile.Center = Vector2.Lerp(player.Center, targetPos, AITimer / Delay);
            savedPos = Projectile.Center;
        }
        else
        {
            int adjustedTime = AITimer - (int)Delay;
            int remainingDuration = (int)(Duration - Delay);
            float percentComplete = adjustedTime / (Duration - Delay);
            float rotValue = MathHelper.Lerp(0, MathHelper.PiOver2 * direction, percentComplete);
            Vector2 playerToSavedPos = savedPos - player.Center;
            Vector2 lerpTargetPos = player.Center + playerToSavedPos.RotatedBy(rotValue);
            Projectile.Center = lerpTargetPos;

            if (useCounter % 3 == 0 && adjustedTime % (int)(remainingDuration / 10f) == 0)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    LemonUtils.QuickProj(
                        Projectile,
                        Projectile.Center,
                        Vector2.Zero,
                        ProjectileID.SolarWhipSwordExplosion,
                        ai1: 1.5f
                        );
                }
            }

        }
        if (direction == 1)
        {
            Projectile.spriteDirection = 1;
            Projectile.rotation = player.DirectionTo(Projectile.Center).ToRotation() + MathHelper.PiOver4;
        }
        else
        {
            Projectile.spriteDirection = -1;
            Projectile.rotation = player.DirectionTo(Projectile.Center).ToRotation() + MathHelper.PiOver4 + MathHelper.PiOver2;
        }
    }

    public override bool? CanHitNPC(NPC target)
    {
        return AITimer >= Delay;
    }

    public override void OnKill(int timeLeft)
    {
        LemonUtils.DustBurst(20, Projectile.Center, DustID.SolarFlare, 8, 8, 1.5f, 2f);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = trailTexture.Value;
        Rectangle? sourceRect = null;
        Vector2 drawOrigin = texture.Size() * 0.5f;
        for (int k = Projectile.oldPos.Length - 1; k > 0; k--)
        {
            Vector2 drawPos = Projectile.oldPos[k] + new Vector2(Projectile.width, Projectile.height) * 0.5f - Main.screenPosition;
            Color baseColor = Color.Lerp(Color.DarkRed, Color.Gold, (float)k / Projectile.oldPos.Length);
            Color color = baseColor * Projectile.Opacity;
            Main.EntitySpriteDraw(
                texture,
                drawPos,
                sourceRect,
                color,
                Projectile.oldRot[k],
                drawOrigin,
                Projectile.scale,
                LemonUtils.SpriteDirectionToSpriteEffects(Projectile.oldSpriteDirection[k]),
                0);
        }
        return true;
    }

    public override void PostDraw(Color lightColor)
    {
        //Main.spriteBatch.End();
        //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
    }
}
