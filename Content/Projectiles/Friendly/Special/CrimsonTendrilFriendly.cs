using NeoParacosm.Common.Utils;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using NeoParacosm.Content.Buffs.GoodBuffs;
using System.IO;

namespace NeoParacosm.Content.Projectiles.Friendly.Special;

public class CrimsonTendrilFriendly : ModProjectile
{
    ref float AITimer => ref Projectile.ai[0];
    NPC closestNPC;

    Vector2 randomPos = Vector2.Zero;

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        Main.projFrames[Type] = 2;
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
        Projectile.timeLeft = 2;
        Projectile.penetrate = -1;
        Projectile.Opacity = 1f;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 30;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        
    }

    public override void AI()
    {
        Player player = Main.player[Projectile.owner];
        Projectile.timeLeft = 2;
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
            closestNPC = LemonUtils.GetClosestNPC(player.Center, 500);
            if (closestNPC != null)
            {
                randomPos = closestNPC.RandomPos();
            }
        }

        if (randomPos != Vector2.Zero)
        {
            float speedBoost = 1f;
            if (closestNPC != null && closestNPC.active) speedBoost = 2;
            Projectile.MoveToPos(randomPos, 0.2f, 0.2f, 0.1f * speedBoost, 0.1f * speedBoost);
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
        Player player = Main.player[Projectile.owner];
        Vector2 drawOrigin = new Vector2(9, 9);
        Vector2 drawPosition = Projectile.Center;
        Vector2 posToPlayer = drawPosition.DirectionTo(player.Center);
        float distanceToPlayer = drawPosition.Distance(player.Center);

        while (distanceToPlayer > 18)
        {
            drawPosition += posToPlayer * 18;
            distanceToPlayer = drawPosition.Distance(player.Center);
            Main.EntitySpriteDraw(tendrilTexture, drawPosition - Main.screenPosition, tendrilTexture.Frame(1, 2, 0, 1), lightColor, posToPlayer.ToRotation(), drawOrigin, Projectile.scale, SpriteEffects.None);
        }

        return true;
    }

    public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
    {
        
    }
}
