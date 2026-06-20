using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria.Audio;
using Terraria.GameContent;

namespace NeoParacosm.Content.Projectiles.Friendly.Melee;

public class BloodyAftermathSlash2 : ModProjectile
{
    int AITimer = 0;
    int chargeTimer = 0;
    ref float FlipVertically => ref Projectile.ai[0];
    ref float Direction => ref Projectile.ai[1];
    ref float Delay => ref Projectile.ai[2];

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {

    }

    public override void SendExtraAI(BinaryWriter writer)
    {

    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {

    }

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 16;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 5;
    }

    public override void SetDefaults()
    {
        Projectile.width = 160;
        Projectile.height = 140;
        Projectile.hostile = false;
        Projectile.friendly = true;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = 10;
        Projectile.stopsDealingDamageAfterPenetrateHits = true;
        Projectile.timeLeft = 15;
        Projectile.DamageType = DamageClass.Melee;
        Projectile.Opacity = 1f;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 180;
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
            if (Direction == 0)
            {
                Direction = 1;
            }
        }

        if (AITimer < Delay)
        {
            AITimer++;
            Projectile.timeLeft = 15;
            return;
        }
        else if (AITimer == Delay)
        {
            SoundEngine.PlaySound(SoundID.Item71, Projectile.Center);
        }

        Projectile.spriteDirection = (int)Direction;
        Projectile.StandardAnimation(3, 5);

        AITimer++;
    }

    public override bool? CanHitNPC(NPC target)
    {
        return AITimer >= Delay;
    }

    public override void OnKill(int timeLeft)
    {

    }

    public override bool PreDraw(ref Color lightColor)
    {
        if (AITimer < Delay) return false;
        SpriteEffects spriteEffects = LemonUtils.SpriteDirectionToSpriteEffects(Projectile.spriteDirection);
        if (FlipVertically == 1)
        {
            spriteEffects = spriteEffects | SpriteEffects.FlipVertically;
        }
        Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
        Rectangle sourceRect = texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
        Vector2 drawPos = Projectile.Center - Main.screenPosition;
        Vector2 drawOrigin = new Vector2(sourceRect.Width, sourceRect.Height) * 0.5f;
        Main.EntitySpriteDraw(texture, drawPos, sourceRect, Color.White, Projectile.rotation, drawOrigin, Projectile.scale, spriteEffects, 0);

        return false;
    }

    public override void PostDraw(Color lightColor)
    {
        //Main.spriteBatch.End();
        //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
    }
}
