using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Common.Utils;
using NeoParacosm.Common.Utils.Prim;
using NeoParacosm.Content.Buffs.Debuffs;
using Terraria.Audio;
using Terraria.GameContent;

namespace NeoParacosm.Content.Projectiles.Hostile;

public class LingeringDeathflame : ModProjectile
{
    int AITimer = 0;
    bool landed = false;

    ref float playerID => ref Projectile.ai[0];
    ref float duration => ref Projectile.ai[1];

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 13;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 4;
    }

    public override void SetDefaults()
    {
        Projectile.width = 32;
        Projectile.height = 32;
        Projectile.hostile = true;
        Projectile.friendly = false;
        Projectile.ignoreWater = false;
        Projectile.tileCollide = true;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 3600;
        Projectile.scale = 1f;
        Projectile.Opacity = 1f;
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        target.AddBuff(BuffType<DeathflameDebuff>(), 60);
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        return false;
    }

    public override void AI()
    {
        if (AITimer == 0)
        {
            LemonUtils.DustCircle(Projectile.Center, 8, 8, DustID.GemDiamond, 1f);
            SoundEngine.PlaySound(SoundID.DD2_WyvernDiveDown with { PitchRange = (0f, 0.2f) }, Projectile.Center);
        }

        if (Projectile.velocity.Y == 0)
        {
            landed = true;
            if (AITimer % 2 == 0)
            {
                foreach (var projectile in Main.ActiveProjectiles) // killing oldest Lingering Deathflame on the same (ish) position
                {
                    if (projectile.type == Type && projectile != Projectile && Projectile.Distance(projectile.Center) < 32)
                    {
                        if (Projectile.timeLeft < projectile.timeLeft)
                        {
                            Projectile.Kill();
                        }
                        else
                        {
                            projectile.Kill();
                        }
                    }
                }
                for (int i = 0; i < 3; i++)
                {
                    Vector2 randomPos = Projectile.Bottom + new Vector2(Main.rand.NextFloat(-Projectile.width, Projectile.width), 0);
                    Dust.NewDustPerfect(randomPos, DustID.Ash, -Vector2.UnitY * Main.rand.NextFloat(2f, 4f), Scale: 2.5f, newColor: Color.Black).noGravity = true;
                    Dust.NewDustPerfect(randomPos, DustID.GemDiamond, -Vector2.UnitY * Main.rand.NextFloat(2f, 4f), Scale: 1.5f, newColor: Color.White).noGravity = true;
                }
            }
            Projectile.velocity.X = 0;
        }
        else
        {
            if (AITimer % 3 == 0)
            {
                Dust.NewDustDirect(Projectile.RandomPos(), 2, 2, DustID.Ash, 0, 0, Scale: 1.5f, newColor: Color.Black).noGravity = true;
                Dust.NewDustDirect(Projectile.RandomPos(), 2, 2, DustID.GemDiamond, 0, 0, Scale: 1.25f, newColor: Color.White).noGravity = true;
            }
        }

        Lighting.AddLight(Projectile.Center, 0, 1, 0);

        if (AITimer > duration)
        {
            Projectile.Kill();
        }

        Projectile.velocity.Y += 0.1f;

        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        Projectile.StandardAnimation(6, 4);
        AITimer++;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        Vector2 drawOrigin = new Vector2(16, 0);
        Vector2 scale = new Vector2(1, 1 + Projectile.velocity.Y * 0.2f);
        Main.EntitySpriteDraw(texture, Projectile.Bottom - Main.screenPosition, texture.Frame(1, 4, 0, Projectile.frame), Color.White, Projectile.rotation, drawOrigin, scale, SpriteEffects.None);
        return false;
    }

    public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
    {
        if (landed || playerID == -1)
        {
            fallThrough = false;
            return true;
        }
        fallThrough = Main.player[(int)playerID].Alive() && (Main.player[(int)playerID].Bottom.Y > Projectile.Center.Y + 32);
        return true;
    }
}
