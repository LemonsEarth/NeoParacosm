using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using Terraria.GameContent;

namespace NeoParacosm.Content.Projectiles.Friendly.Magic.HeldProjectiles;

public class BloodBloomHeldProjMagic : BaseStaffHeldProj
{
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 1;
    }

    public override void SetDefaults()
    {
        Projectile.width = 128;
        Projectile.height = 128;
        Projectile.hostile = false;
        Projectile.friendly = false;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 60;
        Projectile.DamageType = DamageClass.Magic;
        Projectile.Opacity = 1f;
    }

    public override void AI()
    {
        HeldProjectileControl(Main.MouseWorld, false);
        if (!Main.mouseRight)
        {
            Projectile.Kill();
            return;
        }
        Player player = Projectile.GetOwner();
        player.SetDummyItemTime(60);

        int attackCD = (int)(60 / player.GetAttackSpeed(DamageClass.Magic));
        if (AITimer % attackCD == 0)
        {
            player.manaRegenDelay = 120;
            SoundEngine.PlaySound(SoundID.Item84 with { PitchRange = (-0.6f, -0.3f), MaxInstances = 0 }, Projectile.Center);
            if (Main.myPlayer == Projectile.owner)
            {
                Vector2 dirToMouse = Main.player[Projectile.owner].Center.DirectionTo(Main.MouseWorld);
                Vector2 pos = Main.player[Projectile.owner].Center + dirToMouse * Projectile.width * 0.8f;
                pos += dirToMouse.RotatedBy(MathHelper.PiOver2) * Main.rand.NextFloat(-6, 6);
                LemonUtils.DustBurst(8, pos, DustID.GemRuby, 5, 5, 1.5f, 2.5f);
                //Dust.NewDustDirect(pos, 2, 2, DustID.GemRuby, dirToMouse.X, dirToMouse.Y).noGravity = true;
                for (int i = 0; i < 5; i++)
                {
                    LemonUtils.QuickProj(
                        Projectile,
                        pos,
                        dirToMouse.RotatedBy(Main.rand.NextFloat(-MathHelper.Pi / 8, MathHelper.Pi / 8)) * Main.rand.NextFloat(6, 10),
                        ProjectileType<BloodBloomProj>(),
                        ai0: Main.rand.NextFloat(0.95f, 0.98f),
                        ai1: Main.rand.Next(60, 120)
                        );
                }
            }

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
