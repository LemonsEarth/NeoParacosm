using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Projectiles.Friendly.Special;
using Terraria.GameContent;

namespace NeoParacosm.Content.Items.Weapons.Melee;

public class GravityHammerHeldProj : ModProjectile
{
    int AITimer = 0;

    ref float PushOrPull => ref Projectile.ai[0];
    ref float Hit => ref Projectile.ai[1];

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        //LemonUtils.DustBurst(8, target.RandomPos(), DustID.GemRuby, 5, 5, 1.5f, 2.5f);
        if (Hit > 0 || (rotValue > goalRotation - 75)) return;
        if (Main.myPlayer == Projectile.owner)
        {
            if (PushOrPull == -1)
            {
                LemonUtils.QuickProj(
                    Projectile,
                    Projectile.GetOwner().Center,
                    Vector2.Zero,
                    ProjectileType<GravityRepelProjFriendly>(),
                    0,
                    ai0: 300,
                    ai1: 10,
                    ai2: 2
                    );
            }
            else
            {
                LemonUtils.QuickProj(
                    Projectile,
                    target.Center,
                    Vector2.Zero,
                    ProjectileType<GravitySuckyProjFriendly>(),
                    ai0: 300,
                    ai1: 60,
                    ai2: 30
                    );
            }
        }
        Hit++;
    }

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 1;
    }

    public override void SetDefaults()
    {
        Projectile.width = 104;
        Projectile.height = 104;
        Projectile.hostile = false;
        Projectile.friendly = true;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 60;
        Projectile.DamageType = DamageClass.Melee;
        Projectile.Opacity = 1f;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 300;
    }

    float goalRotation = 270;
    float lerpSpeed = 3f / 60f;
    float rotValue;
    public override void AI()
    {
        Player player = Projectile.GetOwner();
        if (!player.IsAlive())
        {
            Projectile.Kill();
        }

        if (AITimer == 0)
        {
            rotValue = -140;
        }
        Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter);
        player.heldProj = Projectile.whoAmI;
        player.SetDummyItemTime(2);
        if (AITimer < 30) Projectile.Opacity = MathHelper.Lerp(0, 1, AITimer / 30f);
        if (Projectile.timeLeft < 30) Projectile.Opacity = MathHelper.Lerp(0, 1, Projectile.timeLeft / 30f);

        Projectile.timeLeft = 3;
        Projectile.velocity = Vector2.Zero;

        SetPositionRotationDirection(player, 0);
        float hitMul = Hit > 0 ? 2f : 1f;
        rotValue = MathHelper.Lerp(rotValue, goalRotation, lerpSpeed * player.GetAttackSpeed(DamageClass.Melee) * hitMul);
        if (rotValue > goalRotation - 10) Projectile.Kill();
        SetPositionRotationDirection(player, MathHelper.ToRadians(rotValue));

        AITimer++;
    }

    const float ThreePiOverFour = MathHelper.Pi - MathHelper.PiOver4; // dumb rotation and sprite direction stuff
    void SetPositionRotationDirection(Player player, float movedRotation = 0)
    {
        Vector2 pos = player.Center + new Vector2(-player.direction * Projectile.width / 2, -Projectile.height / 2).RotatedBy(movedRotation * player.direction);
        player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, movedRotation * player.direction + player.direction * ThreePiOverFour);
        Projectile.Center = pos;
        Projectile.rotation = movedRotation * player.direction + MathHelper.PiOver2 * -player.direction;
        Projectile.spriteDirection = player.direction;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Type].Value;

        Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, LemonUtils.SpriteDirectionToSpriteEffects(Projectile.spriteDirection));
        return false;

    }

    public override void PostDraw(Color lightColor)
    {
        //Main.spriteBatch.End();
        //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
    }
}
