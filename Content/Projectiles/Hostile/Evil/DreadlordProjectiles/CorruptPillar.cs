using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Gores;
using Terraria.GameContent;

namespace NeoParacosm.Content.Projectiles.Hostile.Evil.DreadlordProjectiles;

public class CorruptPillar : ModProjectile
{
    int AITimer = 0;
    ref float TimeLeft => ref Projectile.ai[0];
    ref float Length => ref Projectile.ai[1];
    ref float MoveTime => ref Projectile.ai[2];

    int hFrame = 0;
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 3;
    }

    public override void SetDefaults()
    {
        Projectile.width = 96;
        Projectile.height = 96;
        Projectile.hostile = false;
        Projectile.friendly = false;
        Projectile.ignoreWater = false;
        Projectile.tileCollide = false;
        Projectile.penetrate = 3;
        Projectile.timeLeft = 9999;
        Projectile.scale = 1f;
        Projectile.aiStyle = 0;
        Projectile.Opacity = 0f;
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        
    }

    public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
    {
        
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        return null;
    }

    Vector2 startPos = Vector2.Zero;
    Vector2 EndPos => startPos - Vector2.UnitY * Length;
    public override void AI()
    {
        Projectile.damage = 0;
        if (AITimer == 0)
        {
            hFrame = Main.rand.Next(0, 2);
            startPos = Projectile.position;
        }
        float t = MathHelper.Clamp(AITimer / MoveTime, 0, 1);
        Projectile.position = Vector2.SmoothStep(startPos, EndPos, t);
        Projectile.height = (int)MathF.Abs((Projectile.position.Y - startPos.Y)) + 96;
        if (MathF.Abs(Projectile.position.Y - EndPos.Y) > 96)
        {
            Vector2 randomPos = Main.rand.NextVector2FromRectangle(new Rectangle((int)Projectile.position.X, (int)Projectile.position.Y, 96, 96));
            Gore.NewGoreDirect(
                Projectile.GetSource_FromAI(),
                randomPos,
                Vector2.UnitY.RotatedByRandom(6.28f),
                GoreType<CorruptPillarGore>(),
                Main.rand.NextFloat(0.4f, 0.8f));
            for (int i = 0; i < 4; i++)
            {
                Dust.NewDustDirect(randomPos, 2, 2, DustID.Corruption, Scale: Main.rand.NextFloat(1f, 2.5f));
            }
        }
        int chance = MathF.Abs(Projectile.position.Y - EndPos.Y) > 96 ? 10 : 60;

        if (Main.rand.NextBool(chance) && !Main.dedServ)
        {
            Vector2 randomPos = Main.rand.NextVector2FromRectangle(Projectile.getRect());
            Gore.NewGoreDirect(
                Projectile.GetSource_FromAI(), 
                randomPos, 
                Vector2.UnitY.RotatedByRandom(6.28f), 
                GoreType<CorruptPillarGore>(), 
                Main.rand.NextFloat(0.4f, 0.8f));
            for (int i = 0; i < 4; i++)
            {
                Dust.NewDustDirect(randomPos, 2, 2, DustID.Corruption, Scale: Main.rand.NextFloat(1f, 2.5f));
            }
        }

        if (AITimer > TimeLeft)
        {
            Projectile.Kill();
        }
        AITimer++;
    }

    public override void OnKill(int timeLeft)
    {
        
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        Rectangle frameTop = texture.Frame(2, 4, hFrame, 0);
        Rectangle frameMiddle1 = texture.Frame(2, 4, hFrame, 1);
        Rectangle frameMiddle2 = texture.Frame(2, 4, hFrame, 2);
        Rectangle frameBottom = texture.Frame(2, 4, hFrame, 3);
        Vector2 origin = Vector2.Zero;
        Vector2 drawPosTop = Projectile.position - Main.screenPosition;
        Vector2 drawPosExtraMiddle = (startPos + (Vector2.UnitY * frameBottom.Height)) - Main.screenPosition;
        Vector2 drawPosBottom = (startPos + (Vector2.UnitY * frameBottom.Height * 2)) - Main.screenPosition;

        // Draw top part
        Main.EntitySpriteDraw(texture, drawPosTop, frameTop, Color.White, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None);

        // Draw between top and bottom
        Vector2 drawPosMiddle = Projectile.position;
        float distanceToBottom = Projectile.position.Distance(startPos);
        int count = 0;
        while (distanceToBottom > 0)
        {
            Rectangle frame = frameMiddle1;
            if (count % 2 == 0)
            {
                frame = frameMiddle2;
            }
            drawPosMiddle += Vector2.UnitY * frame.Height;
            Main.EntitySpriteDraw(texture, drawPosMiddle - Main.screenPosition, frame, Color.White, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None);
            distanceToBottom -= frame.Height;
            count++;
        }

        // Draw bottom parts (one extra middle part so it looks lest bad)
        Main.EntitySpriteDraw(texture, drawPosExtraMiddle, frameMiddle2, Color.White, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None);
        Main.EntitySpriteDraw(texture, drawPosBottom, frameBottom, Color.White, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None);
        return false;
    }

    public override void PostDraw(Color lightColor)
    {
        
    }
}
