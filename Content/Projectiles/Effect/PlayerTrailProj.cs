using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Common.Utils.Prim;
using NeoParacosm.Content.Dusts;
using NeoParacosm.Core.Systems.Assets;
using System.Collections.Generic;

namespace NeoParacosm.Content.Projectiles.Effect;

public class PlayerTrailProj : PrimProjectile
{
    int AITimer = 0;
    ref float Width => ref Projectile.ai[1];
    ref float Height => ref Projectile.ai[2];

    public override string Texture => ParacosmTextures.Empty100TexPath;

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 1;
    }

    public override void SetDefaults()
    {
        Projectile.width = 20;
        Projectile.height = 42;
        Projectile.hostile = false;
        Projectile.friendly = false;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.localNPCHitCooldown = 30;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.timeLeft = 2;
        Projectile.scale = 1f;
        Projectile.hide = true;
    }

    public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
    {
        behindNPCsAndTiles.Add(index);
    }

    public override void AI()
    {
        Player player = Projectile.GetOwner();
        Projectile.Resize(player.width, player.height);
        if (AITimer == 0)
        {

        }

        if (player.IsAlive())
        {
            Projectile.timeLeft = 2;
        }

        if (AITimer % 4 == 0)
        {
            Vector2 dustPos = Projectile.RandomPos();
            Color color = Main.rand.NextFromList(Color.DarkBlue, Color.RoyalBlue, Color.White, Color.SlateBlue, Color.MediumSlateBlue, Color.Yellow, Color.Gold);
            Dust.NewDustPerfect(dustPos, DustType<StarryDust>(), -player.velocity, newColor: color, Scale: 1f).noGravity = true;
        }

        Projectile.rotation = player.fullRotation;
        Projectile.position = player.position;
        Projectile.velocity = Vector2.Zero;

        AITimer++;
    }

    public override bool? CanHitNPC(NPC target)
    {
        return false;
    }

    public override bool CanHitPlayer(Player target)
    {
        return false;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        //Main.spriteBatch.End(); // Restarting spritebatch around Primitive Drawing to fix some layering issues
        Vector2 moveDirection = Projectile.velocity.SafeNormalize(Vector2.Zero);
        List<VertexPositionColorTexture> vertices = new List<VertexPositionColorTexture>();
        float topRot = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
        float botRot = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        int topVertexDistance = (Projectile.height / 2);
        int botVertexDistance = (Projectile.height / 2);
        Vector2 topVertexOffset = Vector2.UnitX.RotatedBy(topRot) * topVertexDistance;
        Vector2 botVertexOffset = Vector2.UnitX.RotatedBy(botRot) * botVertexDistance;
        for (int i = 0; i < Projectile.oldPos.Length; i++)
        {
            Vector2 currentPos = Projectile.oldPos[i];
            int oldPosIndex = i + 1 >= Projectile.oldPos.Length ? i : i + 1;
            Vector2 oldPos = Projectile.oldPos[oldPosIndex];
            if (currentPos == Vector2.Zero || oldPos == Vector2.Zero)
            {
                break;
            }


            currentPos += new Vector2(Projectile.width / 2, Projectile.height / 2);
            oldPos += new Vector2(Projectile.width / 2, Projectile.height / 2);
            Color startColor = Color.MidnightBlue;
            Color endColor = Color.Transparent;
            Color colorFront = Color.Lerp(startColor, endColor, (float)i / Projectile.oldPos.Length);
            Color colorBack = Color.Lerp(startColor, endColor, (float)oldPosIndex / Projectile.oldPos.Length);

            VertexPositionColorTexture topVPCT = PrimHelper.QuickVertexPositionColorTexture(currentPos + topVertexOffset, colorFront);
            VertexPositionColorTexture botVPCT = PrimHelper.QuickVertexPositionColorTexture(currentPos + botVertexOffset, colorFront);
            VertexPositionColorTexture oldTopVPCT = PrimHelper.QuickVertexPositionColorTexture(oldPos + topVertexOffset, colorBack);
            VertexPositionColorTexture oldBotVPCT = PrimHelper.QuickVertexPositionColorTexture(oldPos + botVertexOffset, colorBack);
            vertices.Add(topVPCT);
            vertices.Add(botVPCT);
            vertices.Add(oldBotVPCT);
            vertices.Add(oldBotVPCT);
            vertices.Add(topVPCT);
            vertices.Add(oldTopVPCT);
        }

        PrimHelper.DrawPrimitives(BasicEffect, vertices);
        //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        return false;
    }

    public override void PostDraw(Color lightColor)
    {
        //Main.spriteBatch.End();
        //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
    }
}
