using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Common.Utils;
using NeoParacosm.Content.Buffs.GoodBuffs;
using ReLogic.Content;
using System.Collections.Generic;
using System.IO;
using Terraria.Audio;
using Terraria.GameContent;

namespace NeoParacosm.Content.Projectiles.Friendly.Special;

public class CrimsonTendrilFriendly : ModProjectile
{
    ref float AITimer => ref Projectile.ai[0];
    NPC closestNPC;
    ref float randomAttackTime => ref Projectile.ai[1];
    ref float randomRangedAttack => ref Projectile.ai[2];
    Vector2 randomPos = Vector2.Zero;

    static Asset<Texture2D> bodyTexture;

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        ProjectileID.Sets.DrawScreenCheckFluff[Type] = 1000;
        Main.projFrames[Type] = 1;
    }

    public override void Load()
    {
        bodyTexture = ModContent.Request<Texture2D>("NeoParacosm/Content/Projectiles/Friendly/Special/CrimsonTendrilBody");
    }

    public override void SendExtraAI(BinaryWriter writer)
    {
        writer.WriteVector2(randomPos);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        randomPos = reader.ReadVector2();
    }

    public override void SetDefaults()
    {
        Projectile.width = 18;
        Projectile.height = 18;
        Projectile.friendly = true;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 2;
        Projectile.penetrate = -1;
        Projectile.Opacity = 1f;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 30;
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        float _ = float.NaN;
        Vector2 endPos = Main.player[Projectile.owner].Center + Main.player[Projectile.owner].DirectionTo(Projectile.Center) * Main.player[Projectile.owner].Distance(Projectile.Center);
        return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Main.player[Projectile.owner].Center, endPos, Projectile.width, ref _); ;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (damageDone > target.life)
        {
            Main.player[Projectile.owner].Heal(10);
        }
    }

    public override void AI()
    {
        Player player = Main.player[Projectile.owner];
        Projectile.timeLeft = 2;

        if (AITimer == 0)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                randomAttackTime = Main.rand.Next(45, 90);
            }
            Projectile.netUpdate = true;
        }
        if (player == null || !player.active || player.dead || player.ghost || !player.HasBuff(ModContent.BuffType<CrimsonTendrilBuff>()))
        {
            Projectile.Kill();
        }

        if (AITimer % 20 == 0 || (closestNPC != null && closestNPC.Distance(player.Center) > 500))
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                randomPos = player.Center + new Vector2(-player.direction * Main.rand.Next(20, 50), Main.rand.Next(-30, 10));
            }
            Projectile.netUpdate = true;
        }

        if (AITimer % randomAttackTime == 0)
        {
            closestNPC = LemonUtils.GetClosestNPC(Projectile.Center, 500);
            if (closestNPC != null && closestNPC.active)
            {
                if (Main.myPlayer == player.whoAmI)
                {
                    randomRangedAttack = Main.rand.NextBool() ? 1 : 0;
                    if (randomRangedAttack == 1)
                    {
                        LemonUtils.QuickProj(Projectile, Projectile.Center, Projectile.DirectionTo(closestNPC.Center) * Main.rand.NextFloat(10, 16), ModContent.ProjectileType<CrimsonThornFriendly>(), Projectile.damage / 2);
                    }
                    else
                    {
                        Projectile.velocity += Projectile.DirectionTo(closestNPC.Center) * Main.rand.NextFloat(15, 20);
                    }
                }
                Projectile.netUpdate = true;
            }
            if (closestNPC != null && closestNPC.active)
            {
                if (randomRangedAttack == 1)
                {
                    SoundEngine.PlaySound(SoundID.Item17, Projectile.Center);
                }
                else
                {
                    SoundEngine.PlaySound(SoundID.Item32, Projectile.Center);
                }
            }
            if (Projectile.Distance(player.Center) > 2000)
            {
                Projectile.Center = randomPos;
            }
        }

        if (randomPos != Vector2.Zero)
        {
            float speedMul = (closestNPC != null && closestNPC.active && randomRangedAttack == 0) ? 2 : 1;
            Projectile.MoveToPos(randomPos, 0.2f * speedMul, 0.2f * speedMul, 0.1f / speedMul, 0.1f / speedMul);
        }

        Projectile.rotation = player.Center.DirectionTo(Projectile.Center).ToRotation();

        AITimer++;
    }

    public override void OnKill(int timeLeft)
    {

    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D tendrilTexture = TextureAssets.Projectile[Type].Value;
        Texture2D bodyTex = bodyTexture.Value;
        Player player = Main.player[Projectile.owner];
        int halfHeight = 9;
        Vector2 drawOrigin = new Vector2(halfHeight, halfHeight);
        Vector2 drawOriginBody = new Vector2(5, 5);

        float projDistanceToPlayer = Projectile.Center.Distance(player.Center);
        int maxSegments = (int)projDistanceToPlayer / 5;
        maxSegments = Math.Clamp(maxSegments, 1, 100);
        float curveDirection = 1f;
        if (closestNPC == null || !closestNPC.active)
        {
            curveDirection = player.direction;
        }
        Vector2 bezierControlPoint = Projectile.Center + Projectile.Center.DirectionTo(player.Center).RotatedBy(MathHelper.ToRadians(45 * curveDirection)) * 50;
        Vector2 drawPosition = Projectile.Center;

        float posDistanceToPlayer = drawPosition.Distance(player.Center);
        int segmentCount = 1;

        segmentCount += maxSegments / 10;

        while (segmentCount < maxSegments)
        {
            drawPosition = LemonUtils.BezierCurve(Projectile.Center, player.Center, bezierControlPoint, (float)segmentCount / maxSegments);
            Vector2 nextPosition = LemonUtils.BezierCurve(Projectile.Center, player.Center, bezierControlPoint, (float)segmentCount + 1 / maxSegments);
            segmentCount++;
            posDistanceToPlayer = drawPosition.Distance(player.Center);
            Main.EntitySpriteDraw(bodyTex, drawPosition - Main.screenPosition, null, lightColor * segmentCount, drawPosition.DirectionTo(nextPosition).ToRotation() + MathHelper.Pi, drawOriginBody, Projectile.scale, SpriteEffects.None);
        }
        Vector2 secondPos = LemonUtils.BezierCurve(Projectile.Center, player.Center, bezierControlPoint, 2f / maxSegments); // used for rotation
        Main.EntitySpriteDraw(tendrilTexture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.Center.DirectionTo(secondPos).ToRotation() + MathHelper.Pi, drawOrigin, Projectile.scale, SpriteEffects.None);

        return false;
    }

    public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
    {

    }
}
