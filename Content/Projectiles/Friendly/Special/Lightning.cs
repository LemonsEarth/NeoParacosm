﻿using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Common.Utils;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.GameContent;

namespace NeoParacosm.Content.Projectiles.Friendly.Special;

public class Lightning : ModProjectile
{
    ref float AITimer => ref Projectile.ai[0];

    Vector2 targetPos
    {
        get
        {
            return new Vector2(Projectile.ai[1], Projectile.ai[2]);
        }
        set
        {
            Projectile.ai[1] = value.X;
            Projectile.ai[2] = value.Y;
        }
    }

    static BasicEffect BasicEffect;
    GraphicsDevice GraphicsDevice => Main.instance.GraphicsDevice;

    List<Vector2> positions = new List<Vector2>();

    public override void Load()
    {
        if (Main.dedServ) return;
        Main.RunOnMainThread(() =>
        {
            BasicEffect = new BasicEffect(GraphicsDevice)
            {
                TextureEnabled = true,
                VertexColorEnabled = true,
            };
        });

    }

    public override void Unload()
    {
        if (Main.dedServ) return;
        Main.RunOnMainThread(() =>
        {
            BasicEffect?.Dispose();
            BasicEffect = null;
        });
    }

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
    }

    public override void SetDefaults()
    {
        Projectile.width = 24;
        Projectile.height = 24;
        Projectile.friendly = true;
        Projectile.timeLeft = 60;
        Projectile.penetrate = -1;
        Projectile.Opacity = 1f;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 30;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        //target.AddBuff(BuffID.OnFire3, 90);
    }

    public override void AI()
    {
        if (AITimer == 0)
        {
            SoundEngine.PlaySound(SoundID.Thunder);
            Vector2 projToPos = targetPos - Projectile.Center;
            float spacing = projToPos.Length() / (20 / 2);
            bool flip = true;
            int i = 0;
            while (Projectile.Center.Distance(targetPos) > 8)
            {
                Vector2 normalVector = projToPos.RotatedBy(MathHelper.PiOver2).SafeNormalize(Vector2.Zero);
                Vector2 offset = normalVector * flip.ToDirectionInt() * Main.rand.Next(8, 16);
                positions.Add(Projectile.Center + offset);
                positions.Add(Projectile.Center + offset * 0.5f);
                flip = !flip;
                Projectile.Center += Projectile.Center.DirectionTo(targetPos) * spacing * Main.rand.NextFloat(0.5f, 1.5f);
                i += 2;
            }
        }
        Projectile.velocity = Vector2.Zero;
        if (Projectile.timeLeft < 30)
        {
            Projectile.Opacity = MathHelper.Lerp(0, 1, Projectile.timeLeft / 30f);
        }
        AITimer++;
    }

    public override void OnKill(int timeLeft)
    {

        LemonUtils.DustCircle(Projectile.Center, 8, 10, DustID.TintableDustLighted, 7f, color: Color.Orange);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        int quadCount = positions.Count / 2 - 1;
        VertexPositionColorTexture[] vertices = new VertexPositionColorTexture[(positions.Count / 2 - 1) * 6];
        for (int i = 0; i < quadCount; i += 1)
        {
            Vector2 left0 = positions[2 * i];
            Vector2 right0 = positions[2 * i + 1];
            Vector2 left1 = positions[2 * i + 2];
            Vector2 right1 = positions[2 * i + 3];

            VertexPositionColorTexture QuickVertexPCT(Vector2 pos)
            {
                Color color = Main.rand.NextBool(10) ? Color.White : Color.DarkSlateBlue;
            return new VertexPositionColorTexture(new Vector3(pos, 0), color * Projectile.Opacity, Vector2.Zero);
            }

            vertices[6 * i] = QuickVertexPCT(left0);
            vertices[6 * i + 1] = QuickVertexPCT(right0);
            vertices[6 * i + 2] = QuickVertexPCT(left1);

            vertices[6 * i + 3] = QuickVertexPCT(left1);
            vertices[6 * i + 4] = QuickVertexPCT(right0);
            vertices[6 * i + 5] = QuickVertexPCT(right1);
        }
        //for (int i = 0; i < positions.Length; i += 1)
        //{
        //    Color color = i % 2 == 0 ? Color.DarkSlateBlue : new Color(130, 120, 200, 255);
        //    if (i < 3)
        //    {
        //        vertices[i] = new VertexPositionColorTexture(new Vector3(positions[i], 0), Color.DarkSlateBlue * Projectile.Opacity, Vector2.Zero);
        //        vertices[i + 1] = new VertexPositionColorTexture(new Vector3(positions[i + 1], 0), Color.DarkSlateBlue * Projectile.Opacity, Vector2.Zero);
        //        vertices[i + 2] = new VertexPositionColorTexture(new Vector3(positions[i + 2], 0), Color.DarkSlateBlue * Projectile.Opacity, Vector2.Zero);
        //    }
        //    else
        //    {
        //        vertices[i * 3] = new VertexPositionColorTexture(new Vector3(positions[i - 2], 0), Color.DarkSlateBlue * Projectile.Opacity, Vector2.Zero);
        //        vertices[i * 3 + 1] = new VertexPositionColorTexture(new Vector3(positions[i - 1], 0), Color.DarkSlateBlue * Projectile.Opacity, Vector2.Zero);
        //        vertices[i * 3 + 2] = new VertexPositionColorTexture(new Vector3(positions[i], 0), Color.DarkSlateBlue * Projectile.Opacity, Vector2.Zero);
        //    }
        //}
        BasicEffect.World = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0));
        BasicEffect.View = Main.GameViewMatrix.TransformationMatrix;
        GraphicsDevice.RasterizerState = RasterizerState.CullNone;
        Viewport viewport = GraphicsDevice.Viewport;
        BasicEffect.Projection = Matrix.CreateOrthographicOffCenter(0, viewport.Width, viewport.Height, 0, -1, 10);
        GraphicsDevice.Textures[0] = TextureAssets.MagicPixel.Value;
        BasicEffect.CurrentTechnique.Passes[0].Apply();
        GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length / 3);

        return false;
    }
}
