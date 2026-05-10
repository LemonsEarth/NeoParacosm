using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Common.Utils.Prim;
using NeoParacosm.Content.Dusts;
using NeoParacosm.Content.Projectiles.Friendly.Special;
using NeoParacosm.Core.Systems.Assets;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.GameContent;

namespace NeoParacosm.Content.Projectiles.Friendly.Melee.HeldProjectiles;

public class VoidGreatswordHeldProj : PrimProjectile
{
    int AITimer = 0;
    ref float direction => ref Projectile.ai[1];

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {

    }

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 16;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 1;
    }

    public override void SetDefaults()
    {
        Projectile.width = 120;
        Projectile.height = 120;
        Projectile.hostile = false;
        Projectile.friendly = true;
        Projectile.ignoreWater = false;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 120;
        Projectile.DamageType = DamageClass.Melee;
        Projectile.Opacity = 1f;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 180;
    }

    float goalRotation = 270;
    float lerpSpeed = 1 / 30f;
    float rotValue = -60;
    public override void AI()
    {
        Player player = Projectile.GetOwner();
        if (!player.IsAlive())
        {
            Projectile.Kill();
        }
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 40;


        Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter);
        player.heldProj = Projectile.whoAmI;
        player.SetDummyItemTime(2);
        if (AITimer % 4 == 0)
        {
            Vector2 dustPos = Projectile.oldPos[10] + new Vector2(60, 60) + player.DirectionTo(Projectile.Center) * Main.rand.NextFloat(-40, 80);
            Color color = Main.rand.NextFromList(Color.DarkBlue, Color.RoyalBlue, Color.White, Color.SlateBlue, Color.MediumSlateBlue, Color.Yellow, Color.Gold);
            Dust.NewDustPerfect(dustPos, DustType<StarryDust>(), Main.rand.NextVector2Circular(0.5f, 0.5f), newColor:color, Scale: 1f).noGravity = true;
        }
        Projectile.velocity = Vector2.Zero;

        Projectile.timeLeft = 3;
        if (AITimer == 0)
        {
            if (direction == 1)
            {
                rotValue = -60;
                goalRotation = 270;
            }
            else
            {
                rotValue = 360;
                goalRotation = 0;
            }
        }
        Projectile.extraUpdates = 3;
        rotValue = MathHelper.Lerp(rotValue, goalRotation, lerpSpeed * player.GetAttackSpeed(DamageClass.Melee));
        if (direction == 1 && rotValue > goalRotation - 10) Projectile.Kill();
        else if (direction == -1 && rotValue < goalRotation + 10) Projectile.Kill();
        SetPositionRotationDirection(player, MathHelper.ToRadians(rotValue));
        AITimer++;
    }

    public override void OnKill(int timeLeft)
    {

    }

    const float ThreePiOverFour = MathHelper.Pi - MathHelper.PiOver4; // dumb rotation and sprite direction stuff
    void SetPositionRotationDirection(Player player, float movedRotation = 0)
    {
        Vector2 pos = player.Center + new Vector2(-player.direction * (Projectile.width / 2), -Projectile.height / 2).RotatedBy(movedRotation * player.direction) * Projectile.scale;
        player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, movedRotation * player.direction + player.direction * ThreePiOverFour);
        Projectile.Center = pos;
        if (direction == -1)
        {
            movedRotation += MathHelper.PiOver2;
        }
        Projectile.rotation = movedRotation * player.direction + MathHelper.PiOver2 * -player.direction;
        Projectile.spriteDirection = player.direction * (int)direction;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Player player = Projectile.GetOwner();
        Vector2 drawPos = Projectile.Center - Main.screenPosition;
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        float topRotOffset = player.direction == 1 ? -MathHelper.PiOver4 : -3 * MathHelper.PiOver4;
        float botRotOffset = player.direction == 1 ? 3 * MathHelper.PiOver4 : MathHelper.PiOver4;
        if (direction == -1)
        {
            topRotOffset += MathHelper.PiOver2 * player.direction;
            botRotOffset += MathHelper.PiOver2 * player.direction;
        }


        Main.spriteBatch.End(); // Restarting spritebatch around Primitive Drawing to fix some layering issues
        //PrimHelper.DrawHeldProjectilePrimTrailRectangular(Projectile, Color.DarkSlateBlue, Color.Transparent, BasicEffect, topRotOffset, botRotOffset, (int)(Projectile.height * 0.7f), (int)(Projectile.height * 0.7f));
        DrawTrail(Color.MidnightBlue, Color.Transparent, topRotOffset, botRotOffset, (int)(Projectile.height * 0.7f), (int)(Projectile.height * 0.7f));
        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);


        /*for (int k = Projectile.oldPos.Length - 1; k > 0; k--)
        {
            Vector2 afterimagePos = Projectile.oldPos[k] + texture.Size() * 0.5f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
            Color color = Color.Black * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length) * 0.2f;
            Main.EntitySpriteDraw(texture, afterimagePos, null, color, Projectile.oldRot[k], texture.Size() * 0.5f, Projectile.scale, LemonUtils.SpriteDirectionToSpriteEffects(Projectile.oldSpriteDirection[k]), 0);
        }*/
        Main.EntitySpriteDraw(texture, drawPos, null, Color.White, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, LemonUtils.SpriteDirectionToSpriteEffects(Projectile.spriteDirection));

        return false;
    }

    private void DrawPrimitiveTrail(
        Color startColor,
        Color endColor,
        BasicEffect basicEffect,
        Texture2D noiseTexture,
        float topVertexRotationOffset,
        float bottomVertexRotationOffset,
        float uvScale = 32f,
        float uvScrollSpeed = 2f,
        int? topDistance = null,
        int? bottomDistance = null)
    {
        Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

        Rectangle frame = texture.Frame(
            1,
            Main.projFrames[Projectile.type],
            0,
            Projectile.frame);

        List<VertexPositionColorTexture> vertices =
            new();

        float[] trailLengths =
            new float[Projectile.oldPos.Length];

        float totalLength = 0f;

        // Calculate accumulated world-space distance
        for (int i = 1; i < Projectile.oldPos.Length; i++)
        {
            Vector2 current = Projectile.oldPos[i];
            Vector2 previous = Projectile.oldPos[i - 1];

            if (current == Vector2.Zero ||
                previous == Vector2.Zero)
            {
                break;
            }

            totalLength += Vector2.Distance(
                current,
                previous);

            trailLengths[i] = totalLength;
        }

        float scroll =
            Main.GlobalTimeWrappedHourly *
            uvScrollSpeed;

        for (int i = 0; i < Projectile.oldPos.Length - 1; i++)
        {
            Vector2 currentPos = Projectile.oldPos[i];
            Vector2 oldPos = Projectile.oldPos[i + 1];

            if (currentPos == Vector2.Zero ||
                oldPos == Vector2.Zero)
            {
                break;
            }

            currentPos += new Vector2(
                frame.Width / 2f,
                frame.Height / 2f);

            oldPos += new Vector2(
                frame.Width / 2f,
                frame.Height / 2f);

            float topRot =
                topVertexRotationOffset +
                Projectile.oldRot[i];

            float botRot =
                bottomVertexRotationOffset +
                Projectile.oldRot[i];

            float oldTopRot =
                topVertexRotationOffset +
                Projectile.oldRot[i + 1];

            float oldBotRot =
                bottomVertexRotationOffset +
                Projectile.oldRot[i + 1];

            int topVertexDistance =
                topDistance ?? (frame.Height / 2);

            int botVertexDistance =
                bottomDistance ?? (frame.Height / 2);

            Vector2 topOffset =
                Vector2.UnitX.RotatedBy(topRot) *
                topVertexDistance;

            Vector2 botOffset =
                Vector2.UnitX.RotatedBy(botRot) *
                botVertexDistance;

            Vector2 oldTopOffset =
                Vector2.UnitX.RotatedBy(oldTopRot) *
                topVertexDistance;

            Vector2 oldBotOffset =
                Vector2.UnitX.RotatedBy(oldBotRot) *
                botVertexDistance;

            Color colorFront =
                Color.Lerp(
                    startColor,
                    endColor,
                    (float)i / Projectile.oldPos.Length);

            Color colorBack =
                Color.Lerp(
                    startColor,
                    endColor,
                    (float)(i + 1) / Projectile.oldPos.Length);

            // Distance-based seamless UVs
            float currentV =
                trailLengths[i] / uvScale +
                scroll;

            float oldV =
                trailLengths[i + 1] / uvScale +
                scroll;

            VertexPositionColorTexture topCurrent =
                new(
                    new Vector3(currentPos + topOffset, 0f),
                    colorFront,
                    new Vector2(0f, currentV));

            VertexPositionColorTexture botCurrent =
                new(
                    new Vector3(currentPos + botOffset, 0f),
                    colorFront,
                    new Vector2(1f, currentV));

            VertexPositionColorTexture topOld =
                new(
                    new Vector3(oldPos + oldTopOffset, 0f),
                    colorBack,
                    new Vector2(0f, oldV));

            VertexPositionColorTexture botOld =
                new(
                    new Vector3(oldPos + oldBotOffset, 0f),
                    colorBack,
                    new Vector2(1f, oldV));

            // Triangle 1
            vertices.Add(topCurrent);
            vertices.Add(botCurrent);
            vertices.Add(botOld);

            // Triangle 2
            vertices.Add(botOld);
            vertices.Add(topCurrent);
            vertices.Add(topOld);
        }

        if (vertices.Count == 0)
        {
            return;
        }

        GraphicsDevice gd =
            Main.graphics.GraphicsDevice;

        gd.BlendState =
            BlendState.Additive;

        gd.SamplerStates[0] =
            SamplerState.LinearWrap;

        gd.RasterizerState =
            RasterizerState.CullNone;

        gd.DepthStencilState =
            DepthStencilState.None;

        gd.Textures[0] =
            noiseTexture;

        basicEffect.TextureEnabled = true;
        basicEffect.VertexColorEnabled = true;

        basicEffect.World =
            Matrix.CreateTranslation(
                new Vector3(-Main.screenPosition, 0f));

        basicEffect.View =
            Main.GameViewMatrix.TransformationMatrix;

        basicEffect.Projection =
            Matrix.CreateOrthographicOffCenter(
                0,
                Main.screenWidth,
                Main.screenHeight,
                0,
                0,
                1);

        foreach (EffectPass pass
            in basicEffect.CurrentTechnique.Passes)
        {
            pass.Apply();

            gd.DrawUserPrimitives(
                PrimitiveType.TriangleList,
                vertices.ToArray(),
                0,
                vertices.Count / 3);
        }
    }


    void DrawTrail(Color startColor, Color endColor, float topVertexRotationOffset, float bottomVertexRotationOffset, int? topDistance, int? bottomDistance)
    {
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        Rectangle frame = texture.Frame(1, Main.projFrames[Type], 0, 0);
        List<VertexPositionColorTexture> vertices = new List<VertexPositionColorTexture>();
        for (int i = 0; i < Projectile.oldPos.Length; i++)
        {
            Vector2 currentPos = Projectile.oldPos[i];
            int oldPosIndex = (i + 1 >= Projectile.oldPos.Length) ? i : i + 1;
            Vector2 oldPos = Projectile.oldPos[oldPosIndex];
            if (currentPos == Vector2.Zero || oldPos == Vector2.Zero)
            {
                break;
            }

            float topRot = topVertexRotationOffset + Projectile.oldRot[i];
            float botRot = bottomVertexRotationOffset + Projectile.oldRot[i];

            float oldTopRot = topVertexRotationOffset + Projectile.oldRot[oldPosIndex];
            float oldBotRot = bottomVertexRotationOffset + Projectile.oldRot[oldPosIndex];
            int topVertexDistance = topDistance ?? (frame.Height / 2);
            int botVertexDistance = bottomDistance ?? (frame.Height / 2);
            Vector2 topVertexOffset = Vector2.UnitX.RotatedBy(topRot) * topVertexDistance;
            Vector2 botVertexOffset = Vector2.UnitX.RotatedBy(botRot) * botVertexDistance;
            Vector2 oldTopVertexOffset = Vector2.UnitX.RotatedBy(oldTopRot) * topVertexDistance;
            Vector2 oldBotVertexOffset = Vector2.UnitX.RotatedBy(oldBotRot) * botVertexDistance;

            currentPos += new Vector2(frame.Width / 2, frame.Height / 2);
            oldPos += new Vector2(frame.Width / 2, frame.Height / 2);

            Color colorFront = Color.Lerp(startColor, endColor, (float)i / Projectile.oldPos.Length);
            Color colorBack = Color.Lerp(startColor, endColor, (float)oldPosIndex / Projectile.oldPos.Length);

            float currentV = (float)i / Projectile.oldPos.Length;
            float oldV = (float)oldPosIndex / Projectile.oldPos.Length;

            VertexPositionColorTexture topVPCT =
                new VertexPositionColorTexture(
                    new Vector3(currentPos + topVertexOffset, 0),
                    colorFront,
                    new Vector2(0, currentV)
                );

            VertexPositionColorTexture botVPCT =
                new VertexPositionColorTexture(
                    new Vector3(currentPos + botVertexOffset, 0),
                    colorFront,
                    new Vector2(1, currentV)
                );

            VertexPositionColorTexture oldTopVPCT =
                new VertexPositionColorTexture(
                    new Vector3(oldPos + oldTopVertexOffset, 0),
                    colorBack,
                    new Vector2(0, oldV)
                );

            VertexPositionColorTexture oldBotVPCT =
                new VertexPositionColorTexture(
                    new Vector3(oldPos + oldBotVertexOffset, 0),
                    colorBack,
                    new Vector2(1, oldV)
                );

            vertices.Add(topVPCT);
            vertices.Add(botVPCT);
            vertices.Add(oldBotVPCT);
            vertices.Add(oldBotVPCT);
            vertices.Add(topVPCT);
            vertices.Add(oldTopVPCT);
        }

        if (vertices.Count == 0)
        {
            return;
        }

        BasicEffect.World = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0));
        BasicEffect.View = Main.GameViewMatrix.TransformationMatrix;
        GraphicsDevice.RasterizerState = RasterizerState.CullNone;
        var GDViewport = GraphicsDevice.Viewport;
        BasicEffect.Projection = Matrix.CreateOrthographicOffCenter(0, GDViewport.Width, GDViewport.Height, 0, -1, 10);
        GraphicsDevice.Textures[0] = TextureAssets.MagicPixel.Value;
        BasicEffect.CurrentTechnique.Passes[0].Apply();
        GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, vertices.ToArray(), 0, vertices.Count / 3);
    }

    public override void PostDraw(Color lightColor)
    {
        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
    }
}
