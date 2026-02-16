using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Projectiles.Friendly.Magic;
using Terraria.GameContent;

namespace NeoParacosm.Content.Projectiles.Friendly.Melee;

public class FlameforgedBattleAxeHeldProj : PrimProjectile
{
    int AITimer = 0;

    ref float special => ref Projectile.ai[0];
    ref float direction => ref Projectile.ai[1];

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        LemonUtils.QuickProj(Projectile, target.Center, Vector2.Zero, ProjectileType<FireballExplosion>());

    }

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 16;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 1;
    }

    public override void SetDefaults()
    {
        Projectile.width = 92;
        Projectile.height = 92;
        Projectile.hostile = false;
        Projectile.friendly = true;
        Projectile.ignoreWater = false;
        Projectile.tileCollide = false;
        Projectile.penetrate = 1;
        Projectile.timeLeft = 120;
        Projectile.DamageType = DamageClass.Melee;
        Projectile.Opacity = 1f;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 180;
        Projectile.ArmorPenetration = 10;
    }

    float goalRotation = 300;
    float lerpSpeed = 1 / 60f;
    float rotValue = -90;
    public override void AI()
    {
        Player player = Projectile.GetOwner();
        if (!player.Alive())
        {
            Projectile.Kill();
        }

        Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter);
        player.heldProj = Projectile.whoAmI;
        player.SetDummyItemTime(5);
        if (AITimer < 30) Projectile.Opacity = MathHelper.Lerp(0, 1, AITimer / 30f);
        if (Projectile.timeLeft < 30) Projectile.Opacity = MathHelper.Lerp(0, 1, Projectile.timeLeft / 30f);

        Projectile.velocity = Vector2.Zero;

        if (special != 0 && AITimer == 0)
        {
            if (Main.myPlayer == Projectile.owner)
            {
                LemonUtils.QuickProj(
                    Projectile,
                    Projectile.Center,
                    Vector2.UnitY * 5,
                    ProjectileType<Fireball>(),
                    Projectile.damage * 4,
                    ai1: 1
                    );
            }
        }

        Projectile.timeLeft = 3;
        if (AITimer == 0)
        {
            if (direction == 1)
            {
                rotValue = -90;
                goalRotation = 300;
            }
            else
            {
                rotValue = 360;
                goalRotation = -60;
            }
        }
        Projectile.extraUpdates = 3;

        if (direction == 1)
        {
            if (rotValue < 30)
            {
                Projectile.damage = 0;
            }
            else
            {
                Projectile.damage = Projectile.originalDamage;
            }
        }
        else
        {
            if (rotValue > 240)
            {
                Projectile.damage = 0;
            }
            else
            {
                Projectile.damage = Projectile.originalDamage;
            }
        }

        Dust.NewDustDirect(Projectile.RandomPos(), 2, 2, DustID.SolarFlare, Scale: Main.rand.NextFloat(1, 2)).noGravity = true;

        rotValue = MathHelper.Lerp(rotValue, goalRotation, lerpSpeed * player.GetAttackSpeed(DamageClass.Melee));
        if (direction == 1 && rotValue > goalRotation - 10) Projectile.Kill();
        else if (direction == -1 && rotValue < goalRotation + 10) Projectile.Kill();
        SetPositionRotationDirection(player, MathHelper.ToRadians(rotValue));
        AITimer++;
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
        Player player = Projectile.GetOwner();
        Vector2 drawPos = Projectile.Center - Main.screenPosition;
        Texture2D texture = TextureAssets.Projectile[Type].Value;

        Main.EntitySpriteDraw(texture, drawPos, null, Color.White, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, LemonUtils.SpriteDirectionToSpriteEffects(Projectile.spriteDirection));
        return false;
    }

    public override void PostDraw(Color lightColor)
    {
    }
}
