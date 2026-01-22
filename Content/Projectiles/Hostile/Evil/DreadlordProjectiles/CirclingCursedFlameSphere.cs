using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Common.Utils.Prim;
using Terraria.Audio;
using Terraria.GameContent;

namespace NeoParacosm.Content.Projectiles.Hostile.Evil.DreadlordProjectiles;

public class CirclingCursedFlameSphere : PrimProjectile
{
    int AITimer = 0;
    ref float NPCToFollow => ref Projectile.ai[0];
    ref float CirclingOffsetAngle => ref Projectile.ai[1];
    ref float WaitTime => ref Projectile.ai[2];

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 1;
    }

    public override void SetDefaults()
    {
        Projectile.width = 32;
        Projectile.height = 32;
        Projectile.hostile = true;
        Projectile.friendly = false;
        Projectile.ignoreWater = false;
        Projectile.tileCollide = false;
        Projectile.penetrate = 3;
        Projectile.timeLeft = 1500;
        Projectile.scale = 2f;
    }

    float savedSpeed = 1f;
    public override void AI()
    {
        if (AITimer == 0)
        {
            savedSpeed = Projectile.velocity.Length();
            SoundEngine.PlaySound(SoundID.Item92 with { PitchRange = (2f, 2.3f), Volume = 0.75f }, Projectile.Center);
        }

        NPC npc = Main.npc[(int)NPCToFollow];

        if (npc == null || !npc.active)
        {
            Projectile.Kill();
            return;
        }
        Lighting.AddLight(Projectile.Center, 0.5f, 0.8f, 1f);

        if (AITimer < WaitTime)
        {
            Projectile.Center = npc.Center + Vector2.UnitX.RotatedBy(CirclingOffsetAngle + MathHelper.ToRadians(AITimer * savedSpeed * 0.1f)) * 300;
            Projectile.velocity = Vector2.Zero;
        }
        else if (AITimer == WaitTime)
        {
            SoundEngine.PlaySound(SoundID.Item92 with { PitchRange = (2f, 2.3f), Volume = 0.75f }, Projectile.Center);

            Player closestPlayer = LemonUtils.GetClosestPlayer(Projectile.Center, 4000);
            if (closestPlayer != null && closestPlayer.Alive())
            {
                Projectile.velocity = Projectile.Center.DirectionTo(closestPlayer.Center) * savedSpeed;
            }
        }

        var dust = Dust.NewDustDirect(Projectile.RandomPos(), 2, 2, DustID.CursedTorch, Scale: 2f);
        dust.noGravity = true;
        Projectile.rotation = MathHelper.ToRadians(AITimer * 12);
        AITimer++;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        PrimHelper.DrawBasicProjectilePrimTrailTriangular(Projectile, Color.DarkGreen, Color.DarkGreen, BasicEffect);
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        Vector2 drawOrigin = texture.Size() * 0.5f;
        Color color = Color.White;
        for (int i = Projectile.oldPos.Length - 1; i >= 0; i--)
        {
            Vector2 drawPos = Projectile.oldPos[i] + drawOrigin * Projectile.scale;
            Main.EntitySpriteDraw(texture, drawPos - Main.screenPosition, null, color * (((float)Projectile.oldPos.Length - i) / Projectile.oldPos.Length), Projectile.rotation, drawOrigin, Projectile.scale * (((float)Projectile.oldPos.Length - i) / Projectile.oldPos.Length), SpriteEffects.None);
        }
        Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None);
        LemonUtils.DrawGlow(Projectile.Center, Color.White, Projectile.Opacity, Projectile.scale * 0.5f);
        return false;
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {

    }

    public override void OnKill(int timeLeft)
    {
        LemonUtils.DustCircle(Projectile.Center, 8, 8, DustID.CursedTorch, 2f);
        /*for (int i = 0; i < 4; i++)
        {
            LemonUtils.QuickProj(Projectile, Projectile.Center, Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedBy(i * MathHelper.PiOver2) * (savedSpeed / 4f), ProjectileType<SavDroneProjectile>());
        }*/
    }
}
