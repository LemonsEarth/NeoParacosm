using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Projectiles;
using NeoParacosm.Core.Systems.Drawing;
using ReLogic.Content;
using System.IO;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;

namespace NeoParacosm.Content.Projectiles.Hostile.Death.Deathbird;

public class DeathLaser : ModProjectile, IShaderProjectile
{
    public MiscShaderData ShaderData => ProjectileShaderRenderer.GetMiscShader("LaserShader");
    int AITimer = 0;
    ref float Size => ref Projectile.ai[0];
    const string NoisePath = "NeoParacosm/Common/Assets/Textures/Noise/NoiseTexture";
    static Asset<Texture2D> Noise;
    float scale = 1f;
    float laserLength = 9f;

    ref float Rotation => ref Projectile.ai[1];
    ref float RotPerSecond => ref Projectile.ai[2];

    public override void Load()
    {
        Noise = Request<Texture2D>(NoisePath);
    }

    public override void SetStaticDefaults()
    {
        Main.projFrames[Type] = 1;
        ProjectileID.Sets.DrawScreenCheckFluff[Type] = 5000;
    }

    public override void SendExtraAI(BinaryWriter writer)
    {
        writer.Write(Projectile.timeLeft);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        Projectile.timeLeft = reader.ReadInt32();
    }

    public override void SetDefaults()
    {
        Projectile.width = 280;
        Projectile.height = 280;
        Projectile.hostile = true;
        Projectile.friendly = false;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 120;
        Projectile.penetrate = -1;
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        float _ = float.NaN;
        return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Vector2.UnitY.RotatedBy(Rotation) * laserLength * MathHelper.Clamp(Size, 1, 10) * Projectile.height, Projectile.width * 0.7f * Size, ref _);
    }

    public override void OnSpawn(IEntitySource source)
    {

    }

    public override void AI()
    {
        if (AITimer == 0)
        {
            SoundEngine.PlaySound(SoundID.Item92 with { MaxInstances = 0, PitchRange = (-0.5f, -0.3f) }, Projectile.Center);
            SoundEngine.PlaySound(SoundID.Zombie103 with { MaxInstances = 0 }, Projectile.Center);
            if (Size == 0) Size = 1;
        }
        Projectile.velocity = Vector2.Zero;
        scale = AITimer / 5f * Size;
        Projectile.rotation = Rotation;
        Rotation += RotPerSecond;
        Vector2 dustPos = Projectile.Center + -Vector2.UnitY.RotatedBy(Rotation) * 16 + Main.rand.NextVector2Circular(16, 16);
        Dust.NewDustPerfect(dustPos, DustID.Ash, Vector2.UnitY.RotatedBy(Rotation) * Main.rand.NextFloat(3, 6), Scale: Main.rand.NextFloat(2f, 3f), newColor: Color.Black).noGravity = true;
        Dust.NewDustPerfect(dustPos, DustID.GemDiamond, Vector2.UnitY.RotatedBy(Rotation) * Main.rand.NextFloat(9, 15), Scale: Main.rand.NextFloat(2f, 3f), newColor: Color.White).noGravity = true;
        if (Projectile.timeLeft < 15)
        {
            scale = Projectile.timeLeft * Size / 5f;
        }
        scale = MathHelper.Clamp(scale, 0f, Size);
        AITimer++;
    }

    public override void OnKill(int timeLeft)
    {

    }

    public void DrawProjectile()
    {
        if (AITimer < 2) return;
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        Vector2 drawOrigin = new Vector2(texture.Size().X / 2, 0f);
        Vector2 drawPos = Projectile.Center - Main.screenPosition;

        ShaderData.Shader.Parameters["moveSpeed"].SetValue(-2f);
        ShaderData.Shader.Parameters["time"].SetValue(AITimer / 60f);
        ShaderData.Shader.Parameters["centerColor"].SetValue(Color.White.ToVector4());
        ShaderData.Shader.Parameters["endColor"].SetValue(Color.Black.ToVector4());
        Main.instance.GraphicsDevice.Textures[1] = Noise.Value;
        Main.EntitySpriteDraw(texture, drawPos, null, Color.Blue, Projectile.rotation, drawOrigin, new Vector2(scale, laserLength * MathHelper.Clamp(Size, 1, 10)), SpriteEffects.None, 0);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        this.QueueToShaderRenderer();
        return false;
    }

    public override void PostDraw(Color lightColor)
    {
    }
}
