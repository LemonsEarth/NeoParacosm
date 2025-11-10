using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Common.Utils;
using NeoParacosm.Content.Gores;
using NeoParacosm.Content.Projectiles.Friendly.Special;
using NeoParacosm.Core.Systems.Assets;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.CameraModifiers;
using Terraria.Graphics.Shaders;

namespace NeoParacosm.Content.Projectiles.Friendly.Melee;

public class SupremeBloodButchererHeldProj : ModProjectile
{
    int AITimer = 0;
    float releasedTimer = 0;
    ref float chargeAmount => ref Projectile.ai[0];
    bool released = false;
    int hitTimer = 0;

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (hitTimer != 0) return;
        for (int i = 0; i < 2 * Projectile.scale; i++)
        {
            LemonUtils.QuickProj(Projectile, Projectile.RandomPos(), Vector2.UnitY.RotatedByRandom(6.28f) * 8, ProjectileType<CrimsonThornFriendly>(), Projectile.damage / 3);
        }
        hitTimer = 20;
    }

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 1;
    }

    public override void SetDefaults()
    {
        Projectile.width = 64;
        Projectile.height = 64;
        Projectile.hostile = false;
        Projectile.friendly = true;
        Projectile.ignoreWater = false;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 60;
        Projectile.DamageType = DamageClass.Melee;
        Projectile.Opacity = 0f;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 300;
    }

    float goalRotation = 270;
    float lerpT = 8f / 60f;
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
        if (AITimer < 30) Projectile.Opacity = MathHelper.Lerp(0, 1, AITimer / 30f);
        if (Projectile.timeLeft < 30) Projectile.Opacity = MathHelper.Lerp(0, 1, Projectile.timeLeft / 30f);
        if (player.channel && !released)
        {
            Projectile.damage = 0;
            Projectile.timeLeft = 180;
            Projectile.velocity = Vector2.Zero;

            SetPositionRotationDirection(player, 0);
            int aiTimerClamped = Math.Clamp(AITimer, 0, 180);
            chargeAmount = MathHelper.Lerp(0, 1, aiTimerClamped / 180f);
            if (AITimer > 0 && AITimer <= 270 && AITimer % 45 == 0)
            {
                Projectile.scale = 1 + (AITimer / 60);
                Projectile.Resize((int)(64 * Projectile.scale), (int)(64 * Projectile.scale));
                PunchCameraModifier mod1 = new PunchCameraModifier(Projectile.Center, (Main.rand.NextFloat() * ((float)Math.PI * 2f)).ToRotationVector2(), 15f, 6f, 20, 1000f, FullName);
                Main.instance.CameraModifiers.Add(mod1);
                SoundEngine.PlaySound(SoundID.DD2_DrakinShot, Projectile.Center);
                for (int i = 0; i < 3 * Projectile.scale; i++)
                {
                    Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.RandomPos(), Vector2.UnitY.RotatedByRandom(6.28f) * Main.rand.NextFloat(4, 8), GoreType<RedGore>());
                    Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.RandomPos(), Vector2.UnitY.RotatedByRandom(6.28f) * Main.rand.NextFloat(4, 8), GoreType<RedSmokeGore>());
                }
            }
        }
        else
        {
            released = true;
            Projectile.damage = (int)(Projectile.originalDamage * Projectile.scale);
        }

        if (released)
        {
            releasedTimer = MathHelper.Lerp(releasedTimer, goalRotation, lerpT * player.GetAttackSpeed(DamageClass.Melee));
            if (releasedTimer > goalRotation - 2) Projectile.Kill();
            SetPositionRotationDirection(player, MathHelper.ToRadians(releasedTimer));
        }
        if (hitTimer > 0) hitTimer--;
        AITimer++;
    }

    const float ThreePiOverFour = MathHelper.Pi - MathHelper.PiOver4; // dumb rotation and sprite direction stuff
    void SetPositionRotationDirection(Player player, float movedRotation = 0)
    {
        Vector2 pos = player.Center + (new Vector2(-player.direction * 28, -28) * Projectile.scale).RotatedBy(movedRotation * player.direction);
        player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, movedRotation * player.direction + player.direction * ThreePiOverFour);
        Projectile.Center = pos;
        Projectile.rotation = movedRotation * player.direction + MathHelper.PiOver2 * -player.direction;
        Projectile.spriteDirection = player.direction;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D glowTexture = TextureAssets.Projectile[Type].Value;

        Texture2D originalTexture = TextureAssets.Item[ItemID.BloodButcherer].Value;
        Vector2 drawPos = Projectile.Center - Main.screenPosition;

        if (released) //afterimages
        {
            for (int k = Projectile.oldPos.Length - 1; k > 0; k--)
            {
                Vector2 afterimagePos = Projectile.oldPos[k] - Main.screenPosition + glowTexture.Size() * 0.5f + new Vector2(0, Projectile.gfxOffY);
                Color color = new Color(70, 0, 0, 255) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                color.A /= 2;
                SpriteEffects spriteEffects = SpriteEffects.None;
                if (Projectile.oldSpriteDirection[k] == -1)
                {
                    spriteEffects = SpriteEffects.FlipHorizontally;
                }
                Main.EntitySpriteDraw(glowTexture, drawPos, null, color, Projectile.oldRot[k], glowTexture.Size() * 0.5f, Projectile.scale, spriteEffects, 0);
            }
        }

        Color glowColor = new Color(1, 1 - chargeAmount, 0, 1);
        Main.EntitySpriteDraw(originalTexture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, originalTexture.Size() * 0.5f, Projectile.scale, LemonUtils.SpriteDirectionToSpriteEffects(Projectile.spriteDirection));
        var shader = GameShaders.Misc["NeoParacosm:AscendedWeaponGlow"];
        shader.Shader.Parameters["uTime"].SetValue(AITimer);
        shader.Shader.Parameters["color"].SetValue(glowColor.ToVector4());
        shader.Shader.Parameters["moveSpeed"].SetValue(0.5f);
        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, default, Main.Rasterizer, shader.Shader, Main.GameViewMatrix.TransformationMatrix);
        Main.instance.GraphicsDevice.Textures[1] = ParacosmTextures.NoiseTexture.Value;
        shader.Apply();
        Main.EntitySpriteDraw(glowTexture, drawPos, null, Color.White, Projectile.rotation, glowTexture.Size() * 0.5f, Projectile.scale, LemonUtils.SpriteDirectionToSpriteEffects(Projectile.spriteDirection), 0);
        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        return false;

    }

    public override void PostDraw(Color lightColor)
    {
        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
    }
}
