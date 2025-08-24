using NeoParacosm.Common.Utils;
using Terraria.Audio;

namespace NeoParacosm.Content.Projectiles.Friendly.Melee;

public class ChainsawGunHeldProjectile : ModProjectile
{
    ref float AITimer => ref Projectile.ai[0];
    bool sharp = false;

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 1;
    }

    public override void SetDefaults()
    {
        Projectile.width = 48;
        Projectile.height = 48;
        Projectile.hostile = false;
        Projectile.friendly = false;
        Projectile.ignoreWater = false;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 60;
        Projectile.hide = true;
        Projectile.DamageType = DamageClass.Melee;
    }

    public override void AI()
    {
        Player player = Main.player[Projectile.owner];
        if (!player.Alive())
        {
            Projectile.Kill();
        }
        Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter);

        Projectile.timeLeft = 60;


        if (Main.myPlayer == Projectile.owner)
        {
            if (player.channel)
            {
                float holdoutDistance = player.HeldItem.shootSpeed;
                Vector2 holdoutOffset = holdoutDistance * playerCenter.DirectionTo(Main.MouseWorld);
                Projectile.velocity = holdoutOffset;
                if (AITimer % 3 == 0)
                {
                    Projectile.netUpdate = true;
                }
            }
            else
            {
                Projectile.Kill();
            }
        }
        if (AITimer % 30 == 0)
        {
            if (Main.rand.NextBool())
            {
                SoundEngine.PlaySound(SoundID.Item22);
            }
            else
            {
                SoundEngine.PlaySound(SoundID.Item23);

            }
        }
        Projectile.Center = playerCenter;
        if (Projectile.velocity.X > 0)
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
        }
        else
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Pi;

        }
        Projectile.velocity.Y = Main.rand.NextFloat(-1f, 1f);
        Projectile.spriteDirection = Projectile.direction;
        player.ChangeDir(Projectile.direction);
        player.heldProj = Projectile.whoAmI;
        player.SetDummyItemTime(2);
        if (AITimer == 0)
        {
            if (Main.myPlayer == Projectile.owner)
            {
                LemonUtils.QuickProj(Projectile, Projectile.Center + Projectile.velocity, Vector2.Zero, ProjectileType<SawProjectile>(), owner: Projectile.owner, ai2: Projectile.whoAmI, knockback: 5);
            }
        }

        AITimer++;
    }
}
