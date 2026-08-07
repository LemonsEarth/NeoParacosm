using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Items.Accessories.Combat.Summon;
using NeoParacosm.Content.Projectiles.Friendly.Magic;
using NeoParacosm.Content.Projectiles.Friendly.Summon.Sentries;
using System.Collections.Generic;
using Terraria.GameContent;

namespace NeoParacosm.Content.Projectiles.Friendly.Summon;

public class WaterSpirit : ModProjectile
{
    ref float AITimer => ref Projectile.ai[0];
    NPC closestEnemy;

    static Asset<Texture2D> trailTexture;

    public override void SetStaticDefaults()
    {
        trailTexture = Request<Texture2D>(Texture + "Trail");
        Main.projFrames[Projectile.type] = 2;
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
    }

    public override void SetDefaults()
    {
        Projectile.width = 24;
        Projectile.height = 48;
        Projectile.penetrate = -1;
        Projectile.DamageType = DamageClass.Summon;
        Projectile.tileCollide = false;

        Projectile.timeLeft = Projectile.SentryLifeTime;
        Projectile.friendly = true;
        Projectile.hide = true;
        Projectile.ContinuouslyUpdateDamageStats = true;
    }

    public override bool? CanHitNPC(NPC target)
    {
        return false;
    }

    Color trailColor;
    public override void AI()
    {
        closestEnemy = LemonUtils.GetClosestNPC(Projectile.Center, 1000);
        Player player = Projectile.GetOwner();
        Projectile.timeLeft = 3;
        if (!player.GetModPlayer<CharmOfTheLostSeaPlayer>().Active)
        {
            Projectile.Kill();
            return;
        }
        if (closestEnemy != null && closestEnemy.DistanceSQ(Projectile.Center) < 400 * 400)
        {
            Vector2 dir = Projectile.DirectionTo(closestEnemy.Center);
            Projectile.spriteDirection = LemonUtils.Sign(Projectile.Center.X - closestEnemy.Center.X, 1);
            float sinValue = MathF.Sin((AITimer / 24f)) * MathHelper.Pi / 8f;
            dir = dir.RotatedBy(sinValue);

            if (Main.myPlayer == Projectile.owner && AITimer % 4 == 0)
            {
                Projectile.NewProjectileDirect(
                    Projectile.GetSource_FromAI(),
                    Projectile.Center,
                    dir * 5,
                    ProjectileType<WaterWhip>(),
                    Projectile.damage,
                    Projectile.knockBack,
                    Projectile.owner,
                    ai1: 60
                    );
            }
        }
        else
        {
            Projectile.spriteDirection = LemonUtils.Sign(Projectile.Center.X - player.Center.X, 1);
        }
        trailColor = Color.Lerp(Color.Cyan, Color.Blue, (MathF.Sin(AITimer / 24f) + 1) * 0.5f);
        Vector2 targetPosition = player.Center + new Vector2(-player.direction * 20, -20);
        Projectile.Center = Vector2.Lerp(Projectile.Center, targetPosition, 1 / 20f);
        Projectile.StandardAnimation(20, 2);
        AITimer++;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = trailTexture.Value;
        Vector2 drawOrigin = texture.Size() * 0.5f;
        for (int k = Projectile.oldPos.Length - 1; k > 0; k--)
        {
            Vector2 drawPos = Projectile.oldPos[k] + new Vector2(Projectile.width, Projectile.height) * 0.5f - Main.screenPosition;
            Color color = trailColor * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
            Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.oldRot[k], drawOrigin, Projectile.scale, LemonUtils.SpriteDirectionToSpriteEffects(-Projectile.spriteDirection), 0);
        }
        Texture2D tex = Projectile.GetTexture();
        Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, tex.Frame(1, 2, 0, Projectile.frame), Color.White, Projectile.rotation, drawOrigin, Projectile.scale, LemonUtils.SpriteDirectionToSpriteEffects(-Projectile.spriteDirection), 0);

        return false;
    }

    public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
    {
        behindProjectiles.Add(index);
    }

    public override void OnKill(int timeLeft)
    {

    }
}
