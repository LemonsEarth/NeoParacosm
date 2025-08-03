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

    float controlPointDirection = 1;
    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D tendrilTexture = TextureAssets.Projectile[Type].Value;
        Texture2D bodyTex = bodyTexture.Value;
        Player player = Main.player[Projectile.owner];

        Vector2 playerToProj = player.Center.DirectionTo(Projectile.Center);
        Vector2 drawPos = player.Center;
        float maxDistance = player.Distance(Projectile.Center);
        float distanceLeft = maxDistance;
        float segmentCount = 0;
        int maxSegments = (int)(maxDistance / (bodyTex.Size().Length() * 0.3f));
        int goalControlPointDirection = playerToProj.X >= 0 ? 1 : -1;
        controlPointDirection = MathHelper.Lerp(controlPointDirection, goalControlPointDirection, 1 / 20f);
        Vector2 bezierControlPoint = player.Center + playerToProj.RotatedBy(MathHelper.ToRadians(45 * controlPointDirection)) * (maxDistance / 2);
        while (segmentCount < maxSegments)
        {
            float segmentProgress = segmentCount / maxSegments;
            float nextSegmentProgress = (segmentCount + 1) / maxSegments;
            distanceLeft -= bodyTex.Height;
            drawPos = LemonUtils.BezierCurve(player.Center, Projectile.Center, bezierControlPoint, segmentProgress);
            Vector2 nextPos = LemonUtils.BezierCurve(player.Center, Projectile.Center, bezierControlPoint, nextSegmentProgress);
            float rotation = drawPos.DirectionTo(nextPos).ToRotation() + MathHelper.PiOver2;
            float scale = Math.Clamp(segmentCount, 0, maxSegments / 3) / (maxSegments / 3);
            Main.EntitySpriteDraw(bodyTex, drawPos - Main.screenPosition, null, Color.White, rotation + MathHelper.PiOver2, bodyTex.Size() * 0.5f, Projectile.scale, SpriteEffects.None);
            segmentCount++;
        }
        Vector2 secondPos = LemonUtils.BezierCurve(player.Center, Projectile.Center, bezierControlPoint, 1.2f);
        Main.EntitySpriteDraw(tendrilTexture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.DirectionTo(secondPos).ToRotation(), tendrilTexture.Size() * 0.5f, Projectile.scale, SpriteEffects.None);

        return false;
    }

    public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
    {

    }
}
