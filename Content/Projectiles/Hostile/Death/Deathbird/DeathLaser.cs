using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Core.Systems.Assets;
using NeoParacosm.Core.Systems.Drawing;
using System.IO;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;

namespace NeoParacosm.Content.Projectiles.Hostile.Death.Deathbird;

public class DeathLaser : ModProjectile, IShaderProjectile
{
    string NoisePath = "NeoParacosm/Common/Assets/Textures/Noise/NoiseTexture";
    int MaxProjectileRange = 5000;
    float LaserLength = 20f;
    float CollisionWidth = 0.7f;

    int InitialDuration = 120;
    int FadeOutDuration = 15;
    int ScaleRampUpFrames = 5;

    int DustSpawnRadius = 16;
    float DustAshSpeedMin = 3f;
    float DustAshSpeedMax = 6f;
    float DustDiamondSpeedMin = 9f;
    float DustDiamondSpeedMax = 15f;
    float DustScaleMin = 2f;
    float DustScaleMax = 3f;

    float ShaderMoveSpeed = -2f;

    static Asset<Texture2D> NoiseTexture;

    public MiscShaderData ShaderData => ProjectileShaderRenderer.GetMiscShader("LaserShader");
    public override string Texture => ParacosmTextures.Empty100TexPath;

    int AITimer = 0;
    float currentScale = 1f;

    ref float SizeMultiplier => ref Projectile.ai[0];
    ref float Rotation => ref Projectile.ai[1];
    ref float RotationPerSecond => ref Projectile.ai[2];

    public override void Load()
    {
        NoiseTexture = Request<Texture2D>(NoisePath);
    }

    public override void SetStaticDefaults()
    {
        Main.projFrames[Type] = 1;
        ProjectileID.Sets.DrawScreenCheckFluff[Type] = MaxProjectileRange;
    }

    public override void SetDefaults()
    {
        Projectile.width = 100;
        Projectile.height = 100;
        Projectile.hostile = true;
        Projectile.friendly = false;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.timeLeft = InitialDuration;
        Projectile.penetrate = -1;
    }

    public override void SendExtraAI(BinaryWriter writer)
    {
        writer.Write(Projectile.timeLeft);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        Projectile.timeLeft = reader.ReadInt32();
    }

    public override void AI()
    {
        // Initialize on first frame
        if (AITimer == 0)
        {
            SoundEngine.PlaySound(SoundID.Item92 with { MaxInstances = 0, PitchRange = (-0.5f, -0.3f) }, Projectile.Center);
            SoundEngine.PlaySound(SoundID.Zombie103 with { MaxInstances = 0 }, Projectile.Center);
            if (SizeMultiplier == 0)
                SizeMultiplier = 1;
        }

        Projectile.velocity = Vector2.Zero;
        Projectile.rotation = Rotation;
        Rotation += RotationPerSecond;

        currentScale = MathHelper.Clamp(AITimer / (float)ScaleRampUpFrames * SizeMultiplier, 0, SizeMultiplier);

        if (Projectile.timeLeft < FadeOutDuration)
            currentScale *= Projectile.timeLeft * SizeMultiplier / (float)ScaleRampUpFrames;

        DustEffects();
        AITimer++;
    }

    void DustEffects()
    {
        Vector2 laserDirection = Vector2.UnitY.RotatedBy(Rotation);
        Vector2 baseSpawnPos = Projectile.Center - laserDirection * DustSpawnRadius;
        Vector2 spawnOffset = Main.rand.NextVector2Circular(DustSpawnRadius, DustSpawnRadius);
        Vector2 dustSpawnPos = baseSpawnPos + spawnOffset;

        Dust ashDust = Dust.NewDustPerfect(dustSpawnPos, DustID.Ash,
            laserDirection * Main.rand.NextFloat(DustAshSpeedMin, DustAshSpeedMax),
            Scale: Main.rand.NextFloat(DustScaleMin, DustScaleMax),
            newColor: Color.Black);
        ashDust.noGravity = true;

        Dust diamondDust = Dust.NewDustPerfect(dustSpawnPos, DustID.GemDiamond,
            laserDirection * Main.rand.NextFloat(DustDiamondSpeedMin, DustDiamondSpeedMax),
            Scale: Main.rand.NextFloat(DustScaleMin, DustScaleMax),
            newColor: Color.White);
        diamondDust.noGravity = true;
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        Vector2 laserDirection = Vector2.UnitY.RotatedBy(Rotation);
        Vector2 laserEnd = Projectile.Center + laserDirection
            * LaserLength * MathHelper.Clamp(SizeMultiplier, 1, 10) * Projectile.height;

        float laserWidth = Projectile.width * CollisionWidth * SizeMultiplier;
        float unused = float.NaN;

        return Collision.CheckAABBvLineCollision(
            targetHitbox.TopLeft(),
            targetHitbox.Size(),
            Projectile.Center,
            laserEnd,
            laserWidth,
            ref unused);
    }

    public override void OnSpawn(IEntitySource source) { }

    public override void OnKill(int timeLeft) { }

    public override bool PreDraw(ref Color lightColor)
    {
        this.QueueToShaderRenderer();
        return false;
    }

    public override void PostDraw(Color lightColor) { }

    public void DrawProjectile()
    {
        Texture2D texture = ParacosmTextures.Empty100Tex.Value;
        Vector2 drawOrigin = new Vector2(texture.Width / 2f, 0f);
        Vector2 drawPosition = Projectile.Center - Main.screenPosition;

        // Setup shader parameters
        SetupShader();

        // Draw laser
        Vector2 laserScale = new Vector2(currentScale, LaserLength * MathHelper.Clamp(SizeMultiplier, 1, 10));
        Main.EntitySpriteDraw(texture, drawPosition, null, Color.Blue, Projectile.rotation, drawOrigin, laserScale, SpriteEffects.None, 0);
    }

    void SetupShader()
    {
        ShaderData.Shader.Parameters["moveSpeed"].SetValue(ShaderMoveSpeed);
        ShaderData.Shader.Parameters["time"].SetValue(AITimer / 60f);
        ShaderData.Shader.Parameters["centerColor"].SetValue(Color.White.ToVector4());
        ShaderData.Shader.Parameters["endColor"].SetValue(Color.Black.ToVector4());
        Main.instance.GraphicsDevice.Textures[1] = NoiseTexture.Value;
        ShaderData.Apply();
    }
}
