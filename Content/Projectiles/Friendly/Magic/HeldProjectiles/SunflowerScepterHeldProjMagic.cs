using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using Terraria.GameContent;

namespace NeoParacosm.Content.Projectiles.Friendly.Magic.HeldProjectiles;

public class SunflowerScepterHeldProjMagic : ModProjectile
{
    int AITimer = 0;

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 1;
    }

    public override void SetDefaults()
    {
        Projectile.width = 78;
        Projectile.height = 78;
        Projectile.hostile = false;
        Projectile.friendly = false;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 60;
        Projectile.DamageType = DamageClass.Magic;
        Projectile.Opacity = 1f;
    }

    void SetPositionRotationDirection(Player player, float movedRotation = 0)
    {
        Vector2 dir = player.Center.DirectionTo(Main.MouseWorld);
        float armRotValue = player.direction == 1 ? -MathHelper.PiOver2 : -MathHelper.PiOver2;
        player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, movedRotation + armRotValue);
        Projectile.Center = player.Center + dir * 28;
        Projectile.rotation = movedRotation + MathHelper.PiOver4;
        Projectile.spriteDirection = 1;
        if (!dir.HasNaNs())
        {
            player.ChangeDir(Math.Sign(dir.X));
        }
    }

    public override void AI()
    {
        Player player = Projectile.GetOwner();
        if (!player.Alive() || (Main.myPlayer == Projectile.owner && !Main.mouseRight))
        {
            Projectile.Kill();
            return;
        }
        Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter);
        player.heldProj = Projectile.whoAmI;

        player.SetDummyItemTime(2);


        Projectile.timeLeft = 2;

        if (AITimer == 0)
        {

        }

        if (Main.myPlayer == Projectile.owner)
        {
            Vector2 mouseDir = player.Center.DirectionTo(Main.MouseWorld);
            SetPositionRotationDirection(player, mouseDir.ToRotation());
        }

        int attackCD = (int)(10 / player.GetAttackSpeed(DamageClass.Magic));
        if (AITimer % attackCD == 0 && player.CheckMana(15, true, false))
        {
            player.manaRegenDelay = 60;
            SoundEngine.PlaySound(SoundID.Item39 with { PitchRange = (0.3f, 0.5f) }, Projectile.Center);
            if (Main.myPlayer == Projectile.owner)
            {
                Vector2 dirToMouse = Main.player[Projectile.owner].Center.DirectionTo(Main.MouseWorld);
                Vector2 pos = Main.player[Projectile.owner].Center + dirToMouse * Projectile.width * 0.8f;
                pos += dirToMouse.RotatedBy(MathHelper.PiOver2) * Main.rand.NextFloat(-6, 6);
                Dust.NewDustDirect(pos, 2, 2, DustID.Dirt, dirToMouse.X, dirToMouse.Y).noGravity = true;
                LemonUtils.QuickProj(
                    Projectile,
                    pos,
                    dirToMouse * 20,
                    ProjectileType<SunflowerSeed>()
                    );
            }

        }
        else if (!player.CheckMana(player.GetManaCost(player.HeldItem), false, false))
        {
            Projectile.Kill();
            return;
        }


        AITimer++;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D tex = TextureAssets.Projectile[Type].Value;
        Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, tex.Size() * 0.5f, Projectile.scale, LemonUtils.SpriteDirectionToSpriteEffects(Projectile.spriteDirection));
        return false;
    }
}
