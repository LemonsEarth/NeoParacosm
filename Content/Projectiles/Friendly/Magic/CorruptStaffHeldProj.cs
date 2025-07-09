using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Common.Utils;
using NeoParacosm.Common.Utils.Prim;
using NeoParacosm.Core.Systems;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;

namespace NeoParacosm.Content.Projectiles.Friendly.Magic;

public class CorruptStaffHeldProj : ModProjectile
{
    int AITimer = 0;
    ref float chargeAmount => ref Projectile.ai[0];
    bool released = false;
    int releasedTimer = 0;

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
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
        Player player = Main.player[Projectile.owner];
        if (!player.Alive())
        {
            Projectile.Kill();
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
        if (player.channel && !released)
        {
            if (chargeAmount == 99)
            {
                LemonUtils.DustCircle(playerCenter, 4, 8, DustID.Corruption, 4f);
                SoundEngine.PlaySound(SoundID.DD2_BetsyWindAttack, playerCenter);
            }
            if (chargeAmount < 100)
            {
                Vector2 randomPos = Projectile.Center + Main.rand.NextVector2CircularEdge(50, 50);
                var dust = Dust.NewDustPerfect(randomPos, DustID.Corruption, randomPos.DirectionTo(Projectile.Center) * Main.rand.NextFloat(1, 3), Scale: Main.rand.NextFloat(1, 3));
                dust.noGravity = true;
                chargeAmount++;
            }
        }
        else
        {
            released = true;
        }

        if (released)
        {
            if (releasedTimer % 10 == 0 && chargeAmount > 0)
            {
                if (player.CheckMana(player.HeldItem.mana, true) && Main.myPlayer == Projectile.owner)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), playerCenter, playerCenter.DirectionTo(Main.MouseWorld) * 25, ModContent.ProjectileType<CorruptBolt>(), Projectile.damage, Projectile.knockBack, player.whoAmI, ai1: Main.rand.NextFloat(-90, 90));
                }
                chargeAmount -= 10;
            }

            releasedTimer++;
        }

        if (releasedTimer > 100 || (released && chargeAmount <= 0))
        {
            Projectile.Kill();
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
