using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Projectiles.Friendly.Special;
using System.Collections.Generic;
using Terraria.GameContent;

namespace NeoParacosm.Content.Items.Weapons.Melee;

public class FleshTwisterFollowerProj : ModProjectile
{
    int AITimer = 0;
    ref float TimeLeft => ref Projectile.ai[0];

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        /*if (chargeCount >= 2 && released)
        {
            for (int i = 0; i < chargeCount; i++)
            {
                Vector2 pos = target.Center + new Vector2(Main.rand.NextFloat(-100, 100), -500);
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), pos, Vector2.Zero, ProjectileType<PurpleLightning>(), Projectile.damage / 3, 1f, Projectile.owner, ai1: target.Center.X, ai2: target.Center.Y);
            }
        }*/
        for (int i = 0; i < 3; i++)
        {
            Vector2 pos = target.Center + new Vector2(Main.rand.NextFloat(-100, 100), -500);
            Projectile.NewProjectile(Projectile.GetSource_FromAI(), pos, Vector2.Zero, ProjectileType<PurpleLightning>(), Projectile.damage, 1f, Projectile.owner, ai1: target.Center.X, ai2: target.Center.Y);
        }
    }

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 1;
    }

    public override void SetDefaults()
    {
        Projectile.width = 66;
        Projectile.height = 66;
        Projectile.hostile = false;
        Projectile.friendly = true;
        Projectile.ignoreWater = false;
        Projectile.tileCollide = false;
        Projectile.penetrate = 1;
        Projectile.timeLeft = 600;
        Projectile.DamageType = DamageClass.Melee;
        Projectile.Opacity = 1f;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 30;
        Projectile.hide = true;
    }

    public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
    {
        overPlayers.Add(index);
    }

    public override void AI()
    {
        Player player = Projectile.GetOwner();

        if (AITimer >= TimeLeft)
        {
            Projectile.Kill();
            return;
        }

        if (TimeLeft - AITimer < 30)
        {
            Projectile.Opacity -= 1 / 30f;
        }

        Projectile.rotation = MathHelper.ToRadians(AITimer * 18);

        AITimer++;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        Vector2 drawPos = Projectile.Center - Main.screenPosition;
        LemonUtils.DrawGlow(Projectile.Center, new Color(175, 170, 255), Projectile.Opacity * 0.3f, Projectile.scale * 1.5f);
        Main.EntitySpriteDraw(texture, drawPos, null, new Color(175, 170, 255) * 0.8f * Projectile.Opacity, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, LemonUtils.SpriteDirectionToSpriteEffects(Projectile.spriteDirection), 0);
        return false;
    }

    public override void PostDraw(Color lightColor)
    {
        //Main.spriteBatch.End();
        //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
    }
}
