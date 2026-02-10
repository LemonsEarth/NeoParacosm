using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Core.Systems.Assets;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;

namespace NeoParacosm.Content.Projectiles.Friendly.Magic;

public class CrossedWireHeldProj : PrimProjectile
{
    int AITimer = 0;
    Color electricColor = Color.White;

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 1;
    }

    public override void SetDefaults()
    {
        Projectile.width = 48;
        Projectile.height = 48;
        Projectile.hostile = false;
        Projectile.friendly = true;
        Projectile.ignoreWater = false;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 60;
        Projectile.DamageType = DamageClass.Magic;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 10;
        Projectile.Opacity = 1f;
        Projectile.ArmorPenetration = 20;
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        float distance = MathHelper.Clamp(target.Distance(Main.MouseWorld), 0, 300);
        float damageMod = (300 - distance) / 300;
        float clampedDamageMod = MathHelper.Clamp(damageMod, 0.5f, 1f);
        modifiers.FinalDamage *= clampedDamageMod;
    }

    void SetPositionRotationDirection(Player player, float movedRotation = 0)
    {
        Vector2 dir = player.Center.DirectionTo(Main.MouseWorld);
        float armRotValue = player.direction == 1 ? -MathHelper.PiOver2 : -MathHelper.PiOver2;
        player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, movedRotation + armRotValue);
        Projectile.Center = player.Center + dir * 28;
        Projectile.rotation = movedRotation + MathHelper.PiOver4;
        Projectile.spriteDirection = 1;
        if (!dir.HasNaNs())
        {
            player.ChangeDir(Math.Sign(dir.X));
        }
    }

    float lightningLength = 1f;

    public override void AI()
    {
        Player player = Main.player[Projectile.owner];
        if (!player.Alive())
        {
            Projectile.Kill();
        }
        Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter);
        player.heldProj = Projectile.whoAmI;
        player.SetDummyItemTime(2);

        if (AITimer % 2 == 0 && !player.CheckMana(player.HeldItem.mana, true, false))
        {
            Projectile.Kill();
            return;
        }


        Projectile.timeLeft = 2;

        if (AITimer == 0)
        {

        }

        if (Main.myPlayer == Projectile.owner)
        {
            Vector2 mouseDir = player.Center.DirectionTo(Main.MouseWorld);
            float mouseDistance = player.Center.Distance(Main.MouseWorld);
            lightningLength = mouseDistance / 100f;
            SetPositionRotationDirection(player, mouseDir.ToRotation());

        }
        if (!player.channel)
        {
            Projectile.Kill();
            return;
        }

        SoundEngine.PlaySound(SoundID.DD2_LightningBugZap with { PitchRange = (1f, 1.2f), Volume = 0.5f }, Projectile.Center);

        AITimer++;
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        float _ = float.NaN;
        Vector2 endPos = Main.MouseWorld;
        return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, endPos, 32, ref _);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D tex = TextureAssets.Projectile[Type].Value;
        Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, tex.Size() * 0.5f, Projectile.scale, LemonUtils.SpriteDirectionToSpriteEffects(Projectile.spriteDirection));

        var shader = GameShaders.Misc["NeoParacosm:LightningShader"];
        shader.Shader.Parameters["lightningLength"].SetValue(lightningLength);
        shader.Shader.Parameters["segmentCount"].SetValue(10);
        shader.Apply();
        Main.spriteBatch.End();
        LemonUtils.BeginSpriteBatchProjectile(effect: shader.Shader);

        Vector2 lightningScale = new(lightningLength, 1);
        Main.EntitySpriteDraw(
            ParacosmTextures.Empty100Tex.Value, 
            Projectile.Center - Main.screenPosition, 
            null, 
            Color.White, 
            Projectile.rotation - MathHelper.PiOver4, 
            Vector2.UnitY * ParacosmTextures.Empty100Tex.Height() * 0.5f, 
            lightningScale, 
            LemonUtils.SpriteDirectionToSpriteEffects(Projectile.spriteDirection));

        Main.spriteBatch.End();
        LemonUtils.BeginSpriteBatchProjectile();

        return false;
    }

    public override void PostDraw(Color lightColor)
    {
        //Main.spriteBatch.End();
        //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
    }
}
