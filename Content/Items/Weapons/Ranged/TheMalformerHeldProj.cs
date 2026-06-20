using NeoParacosm.Content.Projectiles.Friendly.Ranged;
using NeoParacosm.Core.Systems.Assets;
using Terraria.Audio;

namespace NeoParacosm.Content.Items.Weapons.Ranged;

public class TheMalformerHeldProj : ModProjectile
{
    int AITimer = 0;
    ref float ShotCount => ref Projectile.ai[0];
    bool thrown = false;

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
        Projectile.width = 50;
        Projectile.height = 28;
        Projectile.hostile = false;
        Projectile.friendly = true;
        Projectile.ignoreWater = false;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 180;
        Projectile.DamageType = DamageClass.Ranged;
        Projectile.Opacity = 1f;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 20;
    }
    float attackRate = 6;
    public override void AI()
    {
        attackRate = 6;
        Player player = Projectile.GetOwner();
        if (!player.IsAlive())
        {
            Projectile.Kill();
        }
        Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter);
        if (Projectile.timeLeft < 30) Projectile.Opacity = MathHelper.Lerp(0, 1, Projectile.timeLeft / 30f);
        if (AITimer == 0)
        {
            rot = player.direction == 1 ? normRot : oppRot;
        }

        if (ShotCount < 6)
        {
            player.heldProj = Projectile.whoAmI;
            player.SetDummyItemTime(2);
            Projectile.timeLeft = 180;
            if (AITimer % attackRate == 0)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    Vector2 dir = Projectile.Center.DirectionTo(Main.MouseWorld).RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(-5, 5)));
                    player.PickAmmo(player.HeldItem, out int proj, out float _, out int damage, out float knockback, out int usedAmmoItemID);
                    Vector2 pos = Projectile.Center + Projectile.Center.DirectionTo(Main.MouseWorld) * 5;
                    LemonUtils.QuickProj(Projectile, pos, dir * Main.rand.NextFloat(10, 16), ProjectileID.IchorBullet);
                }
                SoundEngine.PlaySound(ParacosmSFX.UndertakerGunshot with { Volume = 0.2f, SoundLimitBehavior = SoundLimitBehavior.ReplaceOldest, MaxInstances = 1 }, Projectile.Center);
                rot += -player.direction * MathHelper.ToRadians(15);
                ShotCount++;
            }
            SetPositionRotationDirection(player, player.Center.DirectionTo(Main.MouseWorld).ToRotation());
            //Projectile.Center = player.Center + player.Center.DirectionTo(Main.MouseWorld) * 5;
        }
        if (ShotCount >= 6 && !thrown && AITimer % attackRate == 0)
        {
            thrown = true;
            if (Main.myPlayer == Projectile.owner)
            {
                Projectile.velocity = Projectile.Center.DirectionTo(Main.MouseWorld) * 25;
            }
            Projectile.netUpdate = true;
        }

        if (thrown)
        {
            Projectile.rotation = MathHelper.ToRadians(AITimer * 12);
            Projectile.velocity.Y += 0.25f;
            Projectile.damage = Projectile.originalDamage * 3;

            if (AITimer % 30 == 0)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.velocity = Projectile.Center.DirectionTo(Main.MouseWorld) * 10;
                    LemonUtils.QuickProj(
                        Projectile,
                        Projectile.Center,
                        Vector2.UnitX.RotatedBy(AITimer) * Main.rand.NextFloat(10, 16),
                        ProjectileType<CursedTracerBullet>()
                        );
                }
                Projectile.netUpdate = true;
            }
        }

        AITimer++;
    }

    float rot = 0;
    float normRot = 0;
    float oppRot = MathHelper.Pi;
    void SetPositionRotationDirection(Player player, float movedRotation = 0)
    {
        float goalRot = player.Center.DirectionTo(Main.MouseWorld).ToRotation();
        rot = Utils.AngleLerp(rot, goalRot, 1f / 10f);
        player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rot + -MathHelper.PiOver2);
        float spriteRot = player.direction == 1 ? 0 : MathHelper.Pi;
        Projectile.rotation = rot + spriteRot;
        Projectile.spriteDirection = player.direction;
    }

    public override bool PreDraw(ref Color lightColor)
    {

        return true;

    }

    public override void PostDraw(Color lightColor)
    {
        //Main.spriteBatch.End();
        //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
    }
}
