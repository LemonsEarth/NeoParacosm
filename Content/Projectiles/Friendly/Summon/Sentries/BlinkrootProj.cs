using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Common.Utils;
using NeoParacosm.Core.Systems;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace NeoParacosm.Content.Projectiles.Friendly.Summon.Sentries;

public class BlinkrootProj : ModProjectile
{
    float AITimer = 0;

    public override void SetStaticDefaults()
    {
        Main.projFrames[Projectile.type] = 4;
    }

    public override void SetDefaults()
    {
        Projectile.width = 50;
        Projectile.height = 50;
        Projectile.hostile = false;
        Projectile.friendly = true;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 60;
        Projectile.alpha = 255;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
    }


    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(SoundID.Item29 with { PitchRange = (-0.3f, 0.3f) }, Projectile.Center);
        for (int i = 0; i < 2; i++)
        {
            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GemTopaz);
            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GemDiamond);
            Gore gore = Gore.NewGoreDirect(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(Main.rand.NextFloat(-5, 5)), Main.rand.Next(61, 64), Main.rand.NextFloat(0.5f, 1f));
        }
    }

    public override void AI()
    {
        if (Projectile.alpha > 0)
        {
            Projectile.alpha -= 8;
        }
        Projectile.velocity = Vector2.Zero;
        Projectile.scale = (float)Math.Sin(MathHelper.ToRadians(AITimer * 4)) / 5 + 1; // 1/5 * sin(4x) + 1 ranges from 0.8 to 1.2
        Lighting.AddLight(Projectile.Center, 10, 10, 0);
        Projectile.frameCounter++;
        if (Projectile.frameCounter == 6)
        {
            Projectile.frameCounter = 0;
            Projectile.frame++;
            if (Projectile.frame >= 4)
            {
                Projectile.frame = 0;
            }
        }
        Projectile.friendly = false;
        if (Projectile.timeLeft < 2)
        {
            Projectile.friendly = true;
        }
        AITimer++;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, texture.Frame(1, 4, 0, Projectile.frame), Color.White, 0f, texture.Frame(1, 4, 0, Projectile.frame).Size() * 0.5f, Projectile.scale, SpriteEffects.None);
        return false;
    }

    public override void PostDraw(Color lightColor)
    {
        LemonUtils.DrawGlow(Projectile.Center, Color.Yellow, Projectile.Opacity, 1f);
    }
}
