
using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Dusts;
using NeoParacosm.Content.Projectiles;
using NeoParacosm.Core.Systems.Assets;
using NeoParacosm.Core.Systems.Drawing;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;

namespace NeoParacosm.Content.Projectiles.Friendly.Special;

public class GravitySuckyProjFriendly : ModProjectile, IShaderProjectile
{
    public MiscShaderData ShaderData => ProjectileShaderRenderer.GetMiscShader("ShieldPulseShader");
    public override string Texture => "NeoParacosm/Common/Assets/Textures/Misc/Empty100Tex";

    int AITimer = 0;
    ref float Distance => ref Projectile.ai[0];
    ref float StrengthDenominator => ref Projectile.ai[1];
    ref float Duration => ref Projectile.ai[2];

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 1;
    }

    public override void SetDefaults()
    {
        Projectile.width = 120;
        Projectile.height = 120;
        Projectile.hostile = false;
        Projectile.friendly = false;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 600;
    }

    public override void AI()
    {
        if (AITimer == 0)
        {
            for (int i = 0; i < Distance / 20f; i++)
            {
                Vector2 dustPos = Projectile.Center + Main.rand.NextVector2CircularEdge(Distance, Distance);
                Vector2 dir = dustPos.DirectionTo(Projectile.Center);
                Dust.NewDustPerfect(dustPos, DustType<StreakDust>(), dir * Main.rand.NextFloat(Distance / 20f, Distance / 15f)).noGravity = true;
            }
            SoundEngine.PlaySound(SoundID.DD2_EtherianPortalOpen with { PitchRange = (-0.9f, -0.8f), Volume = 0.7f, MaxInstances = 3 }, Projectile.Center);
            SoundEngine.PlaySound(SoundID.DD2_EtherianPortalOpen with { PitchRange = (0.8f, 0.9f), Volume = 0.7f, MaxInstances = 3 }, Projectile.Center);
        }

        if (AITimer <= Duration)
        {
            foreach (var npc in Main.ActiveNPCs)
            {
                if (npc.CanBeChasedBy() && npc.knockBackResist > 0f && npc.Distance(Projectile.Center) < Distance)
                {
                    float force = (npc.Distance(Projectile.Center) / StrengthDenominator) * npc.knockBackResist;
                    Vector2 dirToProjectile = (Projectile.Center - npc.Center).SafeNormalize(Vector2.Zero);
                    npc.velocity += dirToProjectile * force;
                }
            }
        }
        if (AITimer / cycleDuration >= 1)
        {
            Projectile.Kill();
        }
        Projectile.velocity = Vector2.Zero;
        AITimer++;
    }

    float speed = -2f;
    float cycleDuration = 100f;
    Color color = new Color(0.7f, 0.0f, 1f, 1f);

    public void DrawProjectile()
    {
        if (AITimer > cycleDuration / Math.Abs(speed))
        {
            return;
        }
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        Vector2 drawPos = Projectile.Center - Main.screenPosition;
        Main.instance.GraphicsDevice.Textures[1] = ParacosmTextures.NoiseTexture.Value;
        ShaderData.Shader.Parameters["time"].SetValue(AITimer / cycleDuration);
        ShaderData.Shader.Parameters["alwaysVisible"].SetValue(false);
        ShaderData.Shader.Parameters["speed"].SetValue(speed);
        ShaderData.Shader.Parameters["colorMultiplier"].SetValue(5f);
        ShaderData.Shader.Parameters["color"].SetValue(color.ToVector4());
        Main.EntitySpriteDraw(texture, drawPos, null, Color.White, Projectile.rotation, texture.Size() * 0.5f, Distance / 50f, SpriteEffects.None, 0);
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
