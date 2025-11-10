using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Core.Systems;
using System.Collections.Generic;
using System.Linq;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.CameraModifiers;

namespace NeoParacosm.Content.Projectiles.Friendly.Special;

public class RedLightning : ModProjectile
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

    Vector2 originalPos = Vector2.Zero;

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
        Projectile.timeLeft = 30;
        Projectile.penetrate = -1;
        Projectile.Opacity = 1f;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 30;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        //target.AddBuff(BuffID.OnFire3, 90);
        Projectile.damage /= 2;
    }

    public override void AI()
    {
        if (AITimer == 0)
        {
            originalPos = Projectile.Center;
            SoundEngine.PlaySound(ParacosmSFX.ElectricBurst with { PitchRange = (-0.2f, 0.2f) });
            SoundEngine.PlaySound(SoundID.Thunder with { PitchRange = (-0.2f, 0.2f) });
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
        if (Projectile.timeLeft < 15)
        {
            Projectile.Opacity = MathHelper.Lerp(0, 1, Projectile.timeLeft / 15f);
        }
        AITimer++;
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        float _ = float.NaN;
        Vector2 endPos = Projectile.Center;
        return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), originalPos, endPos, Projectile.width, ref _);
    }

    public override void OnKill(int timeLeft)
    {

        LemonUtils.DustCircle(Projectile.Center, 8, 10, DustID.TintableDustLighted, 5f, color: Color.Red);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        int quadCount = positions.Count / 2 - 1;
        if (quadCount <= 0) return false;
        VertexPositionColorTexture[] vertices = new VertexPositionColorTexture[quadCount * 6];
        VertexPositionColorTexture QuickVertexPCT(Vector2 pos)
        {
            //Color color = Main.rand.NextBool(10) ? Color.White : Color.Red;
            return new VertexPositionColorTexture(new Vector3(pos, 0), Color.Red * Projectile.Opacity, Vector2.Zero);
        }
        for (int i = 0; i < quadCount; i += 1)
        {
            Vector2 left0 = positions[2 * i];
            Vector2 right0 = positions[2 * i + 1];
            Vector2 left1 = positions[2 * i + 2];
            Vector2 right1 = positions[2 * i + 3];


            vertices[6 * i] = QuickVertexPCT(left0);
            vertices[6 * i + 1] = QuickVertexPCT(right0);
            vertices[6 * i + 2] = QuickVertexPCT(left1);

            vertices[6 * i + 3] = QuickVertexPCT(left1);
            vertices[6 * i + 4] = QuickVertexPCT(right0);
            vertices[6 * i + 5] = QuickVertexPCT(right1);
        }
        //for (int i = 0; i < positions.Length; i += 1)
        //{
        //    Color color = i % 2 == 0 ? Color.Red : new Color(130, 120, 200, 255);
        //    if (i < 3)
        //    {
        //        vertices[i] = new VertexPositionColorTexture(new Vector3(positions[i], 0), Color.Red * Projectile.Opacity, Vector2.Zero);
        //        vertices[i + 1] = new VertexPositionColorTexture(new Vector3(positions[i + 1], 0), Color.Red * Projectile.Opacity, Vector2.Zero);
        //        vertices[i + 2] = new VertexPositionColorTexture(new Vector3(positions[i + 2], 0), Color.Red * Projectile.Opacity, Vector2.Zero);
        //    }
        //    else
        //    {
        //        vertices[i * 3] = new VertexPositionColorTexture(new Vector3(positions[i - 2], 0), Color.Red * Projectile.Opacity, Vector2.Zero);
        //        vertices[i * 3 + 1] = new VertexPositionColorTexture(new Vector3(positions[i - 1], 0), Color.Red * Projectile.Opacity, Vector2.Zero);
        //        vertices[i * 3 + 2] = new VertexPositionColorTexture(new Vector3(positions[i], 0), Color.Red * Projectile.Opacity, Vector2.Zero);
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

        for (int i = 0; i < 2; i++)
        {
            float circleOpacity = Projectile.Opacity + 0.2f;
            LemonUtils.DrawGlow(originalPos, Color.Red, circleOpacity, 2f);
            LemonUtils.DrawGlow(originalPos, Color.Black, circleOpacity, 1f);
            LemonUtils.DrawGlow(positions.Last(), Color.Red, circleOpacity, 0.5f);
            LemonUtils.DrawGlow(positions.Last(), Color.Black, circleOpacity, 1f);
        }

        return false;
    }
}