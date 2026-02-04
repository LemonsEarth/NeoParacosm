using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Common.Utils.Prim;
using NeoParacosm.Core.Players;
using Terraria.Audio;
using Terraria.GameContent;

namespace NeoParacosm.Content.Projectiles.Hostile.Evil.DreadlordProjectiles;

public class IchorSphere : PrimProjectile
{
    int AITimer = 0;
    ref float WaitTime => ref Projectile.ai[0];
    ref float SpeedUP => ref Projectile.ai[1];
    ref float TimeLeft => ref Projectile.ai[2];

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 40;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 1;
        ProjectileID.Sets.DrawScreenCheckFluff[Type] = 2000;
    }

    public override void SetDefaults()
    {
        Projectile.width = 32;
        Projectile.height = 32;
        Projectile.hostile = true;
        Projectile.friendly = false;
        Projectile.ignoreWater = false;
        Projectile.tileCollide = true;
        Projectile.penetrate = 3;
        Projectile.timeLeft = 9999;
        Projectile.scale = 1f;
        Projectile.extraUpdates = 10;
    }

    Vector2 savedVelocity;
    Vector2 savedPlayerPos;
    public override void AI()
    {
        if (AITimer == 0)
        {
            savedVelocity = Projectile.velocity;
        }

        if (AITimer < WaitTime * (Projectile.extraUpdates + 1))
        {
            Projectile.scale = AITimer / (WaitTime * (Projectile.extraUpdates + 1));
            Projectile.velocity = Vector2.Zero;
            if (AITimer < WaitTime * (Projectile.extraUpdates + 1) * 0.75f)
            {
                Player closestPlayer = LemonUtils.GetClosestPlayer(Projectile.Center, 2000);
                if (closestPlayer != null)
                {
                    savedPlayerPos = closestPlayer.Center;
                }
            }
            if (AITimer % 20 == 0)
            {
                Dust.NewDustDirect(Projectile.RandomPos(), 2, 2, DustID.GemTopaz, Scale: 1f).noGravity = true;
            }
            Projectile.hostile = false;
        }
        else if (AITimer == WaitTime * (Projectile.extraUpdates + 1))
        {
            SoundEngine.PlaySound(SoundID.Item92 with { PitchRange = (3f, 3.3f), Volume = 0.75f }, Projectile.Center);
            SoundEngine.PlaySound(SoundID.DD2_FlameburstTowerShot with { PitchRange = (1f, 1.3f), Volume = 0.75f }, Projectile.Center);
            if (savedPlayerPos != Vector2.Zero)
            {
                Projectile.velocity = Projectile.DirectionTo(savedPlayerPos) * savedVelocity.Length();
            }
            else
            {
                Projectile.velocity = savedVelocity;
            }
        }
        else
        {
            Projectile.hostile = true;
            Projectile.velocity *= SpeedUP;
            if (AITimer % 5 == 0)
            {
                Dust.NewDustDirect(Projectile.RandomPos(), 2, 2, DustID.GemTopaz, Scale: 2f).noGravity = true;
            }

            foreach (Projectile proj in NPPlayer.BlockProjectileInstances)
            {
                if (Projectile.Hitbox.IntersectsExact(proj.Hitbox))
                {
                    proj.ai[0] -= 50;
                    Projectile.Kill();
                    return;
                }
            }
        }

        Lighting.AddLight(Projectile.Center, 1f, 1f, 0f);
        if (SpeedUP == 0)
        {
            SpeedUP = 1f;
        }

        if (AITimer > TimeLeft * (Projectile.extraUpdates + 1))
        {
            Projectile.Kill();
        }

        Projectile.rotation = MathHelper.ToRadians(AITimer * 12);
        AITimer++;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        //PrimHelper.DrawBasicProjectilePrimTrailRectangular(Projectile, Color.Gold, Color.Transparent, BasicEffect);
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        Vector2 drawOrigin = texture.Size() * 0.5f;
        Color color = Color.White;
        for (int i = Projectile.oldPos.Length - 1; i >= 0; i--)
        {
            Vector2 drawPos = Projectile.oldPos[i] + drawOrigin;
            Main.EntitySpriteDraw(texture, drawPos - Main.screenPosition, null, color * (((float)Projectile.oldPos.Length - i) / Projectile.oldPos.Length), Projectile.rotation, drawOrigin, Projectile.scale * (((float)Projectile.oldPos.Length - i) / Projectile.oldPos.Length), SpriteEffects.None);
        }
        Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None);

        LemonUtils.DrawGlow(Projectile.Center, Color.White, Projectile.Opacity, Projectile.scale);
        return false;
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {

    }

    public override void OnKill(int timeLeft)
    {
        LemonUtils.DustCircle(Projectile.Center, 8, 8, DustID.Gold, 2f);
        if (LemonUtils.NotClient())
        {
            LemonUtils.QuickPulse(Projectile, Projectile.Center, 2, 2f, 5f, Color.Yellow);
        }
        SoundEngine.PlaySound(SoundID.NPCHit52 with { PitchRange = (0.8f, 1f), Volume = 0.8f }, Projectile.Center);
        SoundEngine.PlaySound(SoundID.Item14 with { PitchRange = (0.8f, 1f), Volume = 0.8f }, Projectile.Center);
        /*for (int i = 0; i < 4; i++)
        {
            LemonUtils.QuickProj(Projectile, Projectile.Center, Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedBy(i * MathHelper.PiOver2) * (savedSpeed / 4f), ProjectileType<SavDroneProjectile>());
        }*/
    }
}
