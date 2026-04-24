using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Items.Weapons.Magic.Spells;
using Terraria.Audio;
namespace NeoParacosm.Content.Projectiles.Friendly.Magic.HeldProjectiles;

public class DragonHeadHeldProj : ModProjectile
{
    int AITimer = 0;
    bool released = false;

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {

    }

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 2;
    }

    public override void SetDefaults()
    {
        Projectile.width = 46;
        Projectile.height = 44;
        Projectile.hostile = false;
        Projectile.friendly = false;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 60;
        Projectile.DamageType = DamageClass.Magic;
        Projectile.Opacity = 1f;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 300;
    }

    public override void AI()
    {
        Player player = Projectile.GetOwner();
        if (!player.IsAlive())
        {
            Projectile.Kill();
        }

        if (AITimer == 0)
        {
            SoundEngine.PlaySound(SoundID.Item119 with { Volume = 0.8f, PitchRange = (-0.4f, -0.2f) }, Projectile.Center);
        }

        Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter);
        player.heldProj = Projectile.whoAmI;
        SetPositionRotationDirection(player, player.Center.DirectionTo(Main.MouseWorld).ToRotation());
        if (AITimer < 60)
        {
            Projectile.timeLeft = 60;
            Projectile.frame = 0;
            AITimer++;
            return;
        }

        if (player.channel && !released)
        {
            Projectile.timeLeft = 30;
            Projectile.frame = 1;

            if (AITimer % 5 == 0)
            {
                if (!player.CheckMana(5, true, false))
                {
                    released = true;
                    return;
                }

                SoundEngine.PlaySound(SoundID.DD2_BetsyFlameBreath with { Volume = 0.4f, PitchRange = (-1f, 1f) }, Projectile.Center);
                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.NewProjectileDirect(
                        Projectile.GetSource_FromAI(),
                        Projectile.Center,
                        Projectile.DirectionTo(Main.MouseWorld) * 5 * player.GetElementalExpertiseBoostMultiplied(SpellElement.Fire, 2),
                        ProjectileID.Flames,
                        Projectile.damage,
                        Projectile.knockBack,
                        Projectile.owner
                        );
                }
            }
        }
        else
        {
            released = true;
        }

        if (released)
        {
            Projectile.frame = 0;
            Projectile.Opacity = MathHelper.Lerp(0, 1, Projectile.timeLeft / 30f);
        }
        AITimer++;
    }

    public override void OnKill(int timeLeft)
    {

    }

    void SetPositionRotationDirection(Player player, float movedRotation = 0)
    {
        Vector2 dir = player.Center.DirectionTo(Main.MouseWorld);
        player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, MathHelper.PiOver2);
        Projectile.Center = player.MountedCenter + new Vector2(0, -player.height * 2);
        Projectile.rotation = dir.ToRotation();
        if (Projectile.spriteDirection == -1)
        {
            Projectile.rotation += MathHelper.Pi;
        }

        Projectile.velocity = Vector2.Zero;

        if (!dir.HasNaNs())
        {
            player.ChangeDir(Math.Sign(dir.X));
            Projectile.spriteDirection = Math.Sign(dir.X);
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {

        return true;
    }

    public override void PostDraw(Color lightColor)
    {

    }
}
