using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Common.Utils.Prim;
using NeoParacosm.Content.Projectiles.Friendly.Special;
using ReLogic.Content;
using Terraria.Audio;
using Terraria.GameContent;

namespace NeoParacosm.Content.Projectiles.Friendly.Melee;

public class StarsaberHeldProj : PrimProjectile
{
    int AITimer = 0;

    ref float special => ref Projectile.ai[0];
    ref float direction => ref Projectile.ai[1];
    ref float useCounter => ref Projectile.ai[2];

    bool alreadyHit = false;

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (special == 0 || alreadyHit) return;
        alreadyHit = true;
        for (int i = 0; i < 4; i++)
        {
            LemonUtils.QuickProj(
                Projectile,
                Projectile.Center,
                Projectile.GetOwner().DirectionTo(target.Center).RotatedBy(Main.rand.NextFloat(-MathHelper.Pi / 8, MathHelper.Pi / 8)) * Main.rand.NextFloat(12, 16),
                ProjectileType<HomingStar>(),
                Projectile.damage / 3,
                3f,
                ai0: 45f,
                ai1: 90f
                );
        }
    }

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 16;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 2;
    }

    public override void SetDefaults()
    {
        Projectile.width = 72;
        Projectile.height = 72;
        Projectile.hostile = false;
        Projectile.friendly = true;
        Projectile.ignoreWater = false;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 120;
        Projectile.DamageType = DamageClass.Melee;
        Projectile.Opacity = 1f;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 180;
    }

    float goalRotation = 270;
    float rotValue = -60;
    public override void AI()
    {
        Player player = Main.player[Projectile.owner];
        if (!player.Alive())
        {
            Projectile.Kill();
        }

        Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter);
        player.heldProj = Projectile.whoAmI;
        player.SetDummyItemTime(2);
        if (AITimer < 30) Projectile.Opacity = MathHelper.Lerp(0, 1, AITimer / 30f);
        if (Projectile.timeLeft < 30) Projectile.Opacity = MathHelper.Lerp(0, 1, Projectile.timeLeft / 30f);

        Projectile.velocity = Vector2.Zero;

        Projectile.timeLeft = 3;
        if (AITimer == 0)
        {
            if (direction == 1)
            {
                rotValue = -60;
                goalRotation = 270;
            }
            else
            {
                rotValue = 360;
                goalRotation = 0;
            }
        }
        Projectile.extraUpdates = 3;

        float lerpSpeed = 1 / 15f;
        if (special == 1)
        {
            lerpSpeed = 1 / 30f;
        }
        rotValue = MathHelper.Lerp(rotValue, goalRotation, lerpSpeed * player.GetAttackSpeed(DamageClass.Melee));
        if (direction == 1 && rotValue > goalRotation - 10) Projectile.Kill();
        else if (direction == -1 && rotValue < goalRotation + 10) Projectile.Kill();
        SetPositionRotationDirection(player, MathHelper.ToRadians(rotValue));
        AITimer++;
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {

    }

    public override void OnKill(int timeLeft)
    {

    }

    const float ThreePiOverFour = MathHelper.Pi - MathHelper.PiOver4; // dumb rotation and sprite direction stuff
    void SetPositionRotationDirection(Player player, float movedRotation = 0)
    {
        Vector2 pos = player.Center + new Vector2(-player.direction * (Projectile.width / 2), -Projectile.height / 2).RotatedBy(movedRotation * player.direction) * Projectile.scale;
        player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, movedRotation * player.direction + player.direction * ThreePiOverFour);
        Projectile.Center = pos;
        if (direction == -1)
        {
            movedRotation += MathHelper.PiOver2;
        }
        Projectile.rotation = movedRotation * player.direction + MathHelper.PiOver2 * -player.direction;
        Projectile.spriteDirection = player.direction * (int)direction;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Player player = Main.player[Projectile.owner];
        Vector2 drawPos = Projectile.Center - Main.screenPosition;
        Asset<Texture2D> textureAsset = TextureAssets.Projectile[Type];
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        Rectangle frame = textureAsset.Frame(1, 2, 0, (int)special);
        float topRotOffset = player.direction == 1 ? -MathHelper.PiOver4 : -3 * MathHelper.PiOver4;
        float botRotOffset = player.direction == 1 ? 3 * MathHelper.PiOver4 : MathHelper.PiOver4;
        if (direction == -1)
        {
            topRotOffset += MathHelper.PiOver2 * player.direction;
            botRotOffset += MathHelper.PiOver2 * player.direction;
        }

        Main.spriteBatch.End(); // Restarting spritebatch around Primitive Drawing to fix some layering issues
        Color color = special == 0 ? new Color(255, 255, 128) * 0.25f : new Color(255, 255, 128);
        PrimHelper.DrawHeldProjectilePrimTrailRectangular(Projectile, color, Color.Transparent, BasicEffect, topRotOffset, botRotOffset);
        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

        Main.EntitySpriteDraw(texture, drawPos, frame, Color.White, Projectile.rotation, frame.Size() * 0.5f, Projectile.scale, LemonUtils.SpriteDirectionToSpriteEffects(Projectile.spriteDirection));

        return false;
    }

    public override void PostDraw(Color lightColor)
    {
        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
    }
}
