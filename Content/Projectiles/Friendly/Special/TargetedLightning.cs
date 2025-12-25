using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Core.Systems.Assets;
using System.Collections.Generic;
using System.Linq;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.CameraModifiers;

namespace NeoParacosm.Content.Projectiles.Friendly.Special;

public abstract class TargetedLightning : ModProjectile
{
    protected int AITimer = 0;
    protected ref float Delay => ref Projectile.ai[0];
    protected Vector2 targetPos
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

    protected Vector2 originalPos = Vector2.Zero;
    protected List<Vector2> positions = new List<Vector2>();
    protected virtual Color ShineColor => Color.White;
    protected virtual Color DarkColor => Color.Yellow;
    protected virtual float BaseSpacingDenominator => 5;
    protected virtual float HorizontalOffsetMin => 10;
    protected virtual float HorizontalOffsetMax => 36;
    Color currentColor = Color.White;

    protected static BasicEffect BasicEffect;
    protected GraphicsDevice GraphicsDevice => Main.instance.GraphicsDevice;
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
        ProjectileID.Sets.DrawScreenCheckFluff[Type] = 6000;
    }

    public override void SetDefaults()
    {
        Projectile.width = 64;
        Projectile.height = 64;
        Projectile.friendly = true;
        Projectile.hostile = false;
        Projectile.timeLeft = 30;
        Projectile.penetrate = -1;
        Projectile.Opacity = 1f;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 30;
        Projectile.tileCollide = false;
    }

    public override void AI()
    {
        if (AITimer < Delay)
        {
            AITimer++;
            Projectile.timeLeft = 30;
            return;
        }
        if (AITimer == Delay)
        {
            PunchCameraModifier mod1 = new PunchCameraModifier(Projectile.Center, (Main.rand.NextFloat() * ((float)Math.PI * 2f)).ToRotationVector2(), 30f, 12f, 10, 1000f, FullName);
            Main.instance.CameraModifiers.Add(mod1);
            originalPos = Projectile.Center;
            float origToTargetDistance = originalPos.Distance(targetPos);
            SoundEngine.PlaySound(ParacosmSFX.ElectricBurst with { PitchRange = (-0.2f, 0.2f), MaxInstances = 1, Volume = 0.35f }, Projectile.Center);
            SoundEngine.PlaySound(ParacosmSFX.Thunder with { PitchRange = (-0.2f, 0.2f), MaxInstances = 0, Volume = 0.75f }, Projectile.Center);
            Vector2 projToPos = targetPos - Projectile.Center;
            float spacing = projToPos.Length() / BaseSpacingDenominator;

            bool flip = true;
            int i = 0;

            while (Projectile.Center.Distance(targetPos) > 32)
            {
                Vector2 normalVector = projToPos.RotatedBy(MathHelper.PiOver2).SafeNormalize(Vector2.Zero);
                Vector2 offset = normalVector * flip.ToDirectionInt() * Main.rand.NextFloat(HorizontalOffsetMin, HorizontalOffsetMax);
                if ((Projectile.Center + offset).Distance(originalPos) > origToTargetDistance)
                {
                    break;
                }
                positions.Add(Projectile.Center + offset);
                positions.Add(Projectile.Center + offset * 0.5f);

                flip = !flip;
                Projectile.Center += Projectile.Center.DirectionTo(targetPos) * spacing * Main.rand.NextFloat(0.5f, 1.5f);
                for (int dc = 0; dc < 6; dc++)
                {
                    Dust.NewDustDirect(Projectile.RandomPos(-Projectile.width / 2, -Projectile.height / 2), 2, 2, DustID.GemDiamond, Main.rand.NextFloat(-16, 16), Main.rand.NextFloat(-8, 8), Scale: Main.rand.NextFloat(1.5f, 2.5f)).noGravity = true;
                }
                i += 2;
            }
            positions.Add(Projectile.Center);
            positions.Add(Projectile.Center);
            for (int j = 0; j < 20; j++)
            {
                Vector2 randVector = new Vector2(Main.rand.NextFloat(-8, 8), Main.rand.NextFloat(-8, 2));
                Vector2 randVector2 = new Vector2(Main.rand.NextFloat(-8, 8), Main.rand.NextFloat(-8, 2));
                Dust.NewDustDirect(Projectile.RandomPos(-Projectile.width / 2, -Projectile.height / 2), 2, 2, DustID.GemDiamond, randVector.X, randVector.Y, Scale: Main.rand.NextFloat(1.5f, 2.5f)).noGravity = true;
                Dust.NewDustDirect(Projectile.RandomPos(-Projectile.width / 2, -Projectile.height / 2), 2, 2, DustID.GemAmber, randVector2.X, randVector2.Y, Scale: Main.rand.NextFloat(1.5f, 2.5f)).noGravity = true;
            }

            positions[0] = originalPos;
            positions[1] = originalPos;
        }
        Projectile.velocity = Vector2.Zero;
        if (Projectile.timeLeft < 30)
        {
            currentColor = Color.Lerp(DarkColor, ShineColor, Projectile.timeLeft / 15f);
            Projectile.Opacity = MathHelper.Lerp(0, 1, Projectile.timeLeft / 5f);
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
        LemonUtils.DustCircle(Projectile.Center, 8, 10, DustID.TintableDustLighted, 5f, color: ShineColor);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        int quadCount = positions.Count / 2 - 1;
        if (quadCount <= 0) return false;
        VertexPositionColorTexture[] vertices = new VertexPositionColorTexture[quadCount * 6];
        VertexPositionColorTexture QuickVertexPCT(Vector2 pos)
        {
            return new VertexPositionColorTexture(new Vector3(pos, 0), currentColor * Projectile.Opacity, Vector2.Zero);
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
            LemonUtils.DrawGlow(originalPos, DarkColor, circleOpacity, 2f);
            LemonUtils.DrawGlow(originalPos, ShineColor, circleOpacity, 1f);
            LemonUtils.DrawGlow(positions.Last(), DarkColor, circleOpacity, 2f);
            LemonUtils.DrawGlow(positions.Last(), ShineColor, circleOpacity, 1f);
        }

        return false;
    }
}