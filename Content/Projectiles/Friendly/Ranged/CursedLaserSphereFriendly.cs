using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Common.Utils.Prim;
using NeoParacosm.Content.Items.Weapons.Ranged;
using NeoParacosm.Core.Systems.Assets;
using NeoParacosm.Core.Systems.Particles;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.Graphics.Shaders;

namespace NeoParacosm.Content.Projectiles.Friendly.Ranged;

public class CursedLaserSphereFriendly : ModProjectile
{
    int AITimer = 0;
    ref float TimeLeft => ref Projectile.ai[0];

    public override string Texture => ParacosmTextures.Empty100TexPath;

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 6;
    }

    public override void SetDefaults()
    {
        Projectile.width = 64;
        Projectile.height = 64;
        Projectile.hostile = false;
        Projectile.friendly = true;
        Projectile.ignoreWater = false;
        Projectile.tileCollide = true;
        Projectile.penetrate = 3;
        Projectile.timeLeft = 120;
        Projectile.scale = 0.3f;
        Projectile.Opacity = 1f;
        Projectile.hide = true;
    }

    float savedSpeed = 1f;
    public override void AI()
    {
        if (AITimer == 0)
        {
            Projectile.NewProjectileDirect(
                Projectile.GetSource_FromAI(),
                Projectile.Center,
                Vector2.Zero,
                ProjectileType<CursedLaserFriendly>(),
                Projectile.damage,
                Projectile.knockBack,
                Projectile.owner,
                TimeLeft
                );
            savedSpeed = Projectile.velocity.Length();
            SoundEngine.PlaySound(SoundID.Zombie104 with { PitchRange = (-0.2f, 0.2f), MaxInstances = 0 }, Projectile.Center);
            SoundEngine.PlaySound(SoundID.Zombie103 with { PitchRange = (-0.2f, 0.2f), MaxInstances = 0 }, Projectile.Center);
            SoundEngine.PlaySound(SoundID.NPCHit52 with { PitchRange = (-0.2f, 0.2f), MaxInstances = 0 }, Projectile.Center);
        }

        Player player = Projectile.GetOwner();
        if (player.HeldItem.type != ItemType<DualDragonfire>() || !player.ItemAnimationActive)
        {
            Projectile.Kill();
            return;
        }

        if (TimeLeft == 0)
        {
            TimeLeft = 60;
        }

        if (TimeLeft - AITimer < 5)
        {
            Projectile.scale *= 0.9f;
        }

        if (AITimer > TimeLeft)
        {
            Projectile.Kill();
        }

        if (Main.myPlayer == Projectile.owner)
        {
            Vector2 dirToMouse = player.DirectionTo(Main.MouseWorld);
            Projectile.Center = player.Center + dirToMouse * 90;
        }

        Projectile.Opacity = AITimer / 15f;
        Lighting.AddLight(Projectile.Center, 0.5f, 0.8f, 1f);
        //Dust.NewDustDirect(Projectile.RandomPos(32, 32), 2, 2, DustID.GemEmerald, 0, Main.rand.NextFloat(-10, -5), Scale: Main.rand.NextFloat(2f, 4f)).noGravity = true;
        //Projectile.rotation = MathHelper.ToRadians(AITimer * 12);
        Projectile.StandardAnimation(6, 6);
        AITimer++;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = ParacosmTextures.NoiseTexture.Value;
        Vector2 drawOrigin = texture.Size() * 0.5f;
        Color color = Color.White;

        var shader = GameShaders.Misc["NeoParacosm:SphereShader"];
        shader.Shader.Parameters["moveSpeed"].SetValue(-2f);
        shader.Shader.Parameters["velocity"].SetValue(Projectile.GetOwner().DirectionTo(Main.MouseWorld));
        shader.Shader.Parameters["centerColor"].SetValue(Color.White.ToVector4());
        shader.Shader.Parameters["endColor"].SetValue(Color.Lime.ToVector4());
        Main.spriteBatch.End();
        Main.instance.GraphicsDevice.Textures[1] = ParacosmTextures.NoiseTexture.Value;
        LemonUtils.BeginSpriteBatchProjectile(effect: shader.Shader);
        shader.Apply();
        Main.EntitySpriteDraw(
            texture,
            Projectile.Center - Main.screenPosition,
            null,
            Color.White,
            Projectile.rotation,
            texture.Size() * 0.5f,
            Projectile.scale,
            SpriteEffects.None
            );
        Main.spriteBatch.End();
        LemonUtils.BeginSpriteBatchProjectile();
        //LemonUtils.DrawGlow(Projectile.Center, Color.White, Projectile.Opacity, Projectile.scale);
        return false;
    }

    public override void PostDraw(Color lightColor)
    {
        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {

    }

    public override void OnKill(int timeLeft)
    {
        if (!Main.dedServ)
        {
            Vector2 movedPos = Vector2.Lerp(Projectile.Center, Main.LocalPlayer.Center, 0.8f);
            SoundEngine.PlaySound(SoundID.Zombie103 with { PitchRange = (-0.2f, 0.2f) }, movedPos);
            SoundEngine.PlaySound(SoundID.NPCHit52 with { PitchRange = (-0.2f, 0.2f) }, movedPos);
            SoundEngine.PlaySound(SoundID.Item14 with { PitchRange = (-0.2f, 0.2f) }, movedPos);
        }
        /*if (LemonUtils.NotClient())
        {
            LemonUtils.QuickPulse(Projectile, Projectile.Center, 3, 30, 5, Color.LightGreen);
            for (int i = 0; i < 16; i++)
            {
                LemonUtils.QuickProj(Projectile, Projectile.Center, Vector2.UnitY.RotatedBy(i * Angle) * 2, ProjectileType<CursedFlameSphere>(), ai1: SpeedUP);
            }
        }*/
        LemonUtils.DustCircle(Projectile.Center, 8, 8, DustID.CursedTorch, 2f);
    }

    public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
    {
        overPlayers.Add(index);
    }
}