using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Buffs.Debuffs;
using NeoParacosm.Core.Systems.Assets;
using NeoParacosm.Core.Systems.Drawing;
using System.Collections.Generic;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;

namespace NeoParacosm.Content.Projectiles.Friendly.Magic;

public class MushroomGas : ModProjectile, IShaderProjectile
{
    public MiscShaderData ShaderData => ProjectileShaderRenderer.GetMiscShader("GasShader");
    ref float AITimer => ref Projectile.ai[0];
    ref float RandomRot => ref Projectile.ai[1];
    int despawnTimer = 0;

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
        Projectile.timeLeft = 420;
        Projectile.penetrate = 6;
        Projectile.stopsDealingDamageAfterPenetrateHits = true;
        Projectile.Opacity = 0f;
        Projectile.usesIDStaticNPCImmunity = true;
        Projectile.idStaticNPCHitCooldown = 30;
        Projectile.hide = true;
        Projectile.scale = 8;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.AddBuff(BuffType<ShroomedDebuff>(), 60);
    }

    public override void AI()
    {
        /*if (AITimer % 10 == 0)
        {
            for (int i = 0; i < 2; i++)
            {
                Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.RandomPos(), Vector2.UnitX.RotatedByRandom(6.28f) * Main.rand.NextFloat(1, 2), GoreType<RedSmokeGore>(), Main.rand.NextFloat(0.8f, 1.2f));
            }
        }*/

        if (AITimer % 30 == 0)
        {
            if (LemonUtils.NotClient())
            {
                LemonUtils.QuickProj(Projectile, Projectile.RandomPos(-16, -16), Vector2.Zero, ProjectileID.Mushroom, Projectile.damage * 2);
            }
        }

        Dust.NewDustDirect(Projectile.RandomPos(-16, -16), 2, 2, DustID.GemDiamond).noGravity = true;

        if (AITimer == 0)
        {
            RandomRot = Main.rand.NextFloat(0.5f, 3);
        }
        Projectile.velocity *= 0.98f;
        if (Projectile.timeLeft < 30)
        {
            despawnTimer++;
            Projectile.Opacity = MathHelper.Lerp(1, 0, despawnTimer / 30f);
        }

        if (AITimer < 30)
        {
            Projectile.Opacity = MathHelper.Lerp(0, 1, AITimer / 30f);
        }

        //Projectile.rotation += MathHelper.ToRadians(RandomRot);
        AITimer++;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        Projectile.velocity *= 0.8f;
        return false;
    }

    public void DrawProjectile()
    {
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        Vector2 drawOrigin = texture.Size() / 2;
        Vector2 drawPos = Projectile.Center - Main.screenPosition;

        ShaderData.Shader.Parameters["distance"].SetValue(1);
        ShaderData.Shader.Parameters["color"].SetValue(new Vector4(0, 0, 1, Projectile.Opacity));
        ShaderData.Shader.Parameters["velocity"].SetValue(new Vector2(0, 0.5f));
        Main.instance.GraphicsDevice.Textures[1] = ParacosmTextures.NoiseTexture.Value;
        Main.EntitySpriteDraw(texture, drawPos, null, Color.White, Projectile.rotation, drawOrigin, new Vector2(Projectile.scale * 2, Projectile.scale), SpriteEffects.None, 0);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        this.QueueToShaderRenderer();
        return false;
    }

    public override void PostDraw(Color lightColor)
    {
    }

    public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
    {
        overPlayers.Add(index);
    }

    public override void OnKill(int timeLeft)
    {

    }
}
