
using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Projectiles;
using NeoParacosm.Core.Systems.Assets;
using NeoParacosm.Core.Systems.Drawing;
using System.IO;
using System.Linq;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;

namespace NeoParacosm.Content.Projectiles.EffectProjectiles;

public class PulseEffect : ModProjectile, IShaderProjectile
{
    public MiscShaderData ShaderData => ProjectileShaderRenderer.GetMiscShader("ShieldPulseShader");

    public override string Texture => "NeoParacosm/Common/Assets/Textures/Misc/Empty100Tex";

    int AITimer = 0;
    ref float Speed => ref Projectile.ai[0];
    ref float Scale => ref Projectile.ai[1];
    ref float ColorMult => ref Projectile.ai[2];

    public Color PulseColor { get; set; } = Color.White;
    public Entity EntityToFollow { get; set; } = null;
    int entityType = -1;
    int entityID = -1;

    public override void SendExtraAI(BinaryWriter writer)
    {
        writer.Write(PulseColor.R);
        writer.Write(PulseColor.G);
        writer.Write(PulseColor.B);
        writer.Write(PulseColor.A);
        writer.Write(entityType);
        writer.Write(entityID);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        PulseColor = new Color(reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
        entityType = reader.ReadInt32();
        entityID = reader.ReadInt32();
        if (entityID < 0) return;
        switch (entityType)
        {
            case 1:
                EntityToFollow = Main.player[entityID];
                break;
            case 2:
                EntityToFollow = Main.projectile.FirstOrDefault(p => p.identity == entityID, null);
                break;
            case 3:
                EntityToFollow = Main.npc[entityID];
                break;
        }
    }

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
            if (EntityToFollow != null)
            {
                if (EntityToFollow is Player)
                {
                    entityType = 1;
                    entityID = EntityToFollow.whoAmI;
                }
                else if (EntityToFollow is Projectile proj)
                {
                    entityType = 2;
                    entityID = proj.identity;
                }
                else if (EntityToFollow is NPC)
                {
                    entityType = 3;
                    entityID = EntityToFollow.whoAmI;
                }
            }
            Projectile.netUpdate = true;
        }
        if (EntityToFollow != null)
        {
            Projectile.Center = EntityToFollow.Center;
        }
        if (Scale == 0) Scale = 1;
        if (Speed == 0) Speed = 1;
        Projectile.velocity = Vector2.Zero;
        if (AITimer > 60 / Speed) Projectile.Kill();
        AITimer++;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        this.QueueToShaderRenderer();
        return false;
    }

    public void DrawProjectile()
    {
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        Vector2 drawPos = Projectile.Center - Main.screenPosition;
        Main.instance.GraphicsDevice.Textures[1] = ParacosmTextures.NoiseTexture.Value;
        ShaderData.Shader.Parameters["time"].SetValue(AITimer / 60f);
        ShaderData.Shader.Parameters["alwaysVisible"].SetValue(false);
        ShaderData.Shader.Parameters["speed"].SetValue(Speed);
        ShaderData.Shader.Parameters["colorMultiplier"].SetValue(ColorMult);
        ShaderData.Shader.Parameters["color"].SetValue(PulseColor.ToVector4());
        Main.EntitySpriteDraw(texture, drawPos, null, Color.White, Projectile.rotation, texture.Size() * 0.5f, Scale, SpriteEffects.None, 0);
    }

    public override void PostDraw(Color lightColor)
    {
    }
}
