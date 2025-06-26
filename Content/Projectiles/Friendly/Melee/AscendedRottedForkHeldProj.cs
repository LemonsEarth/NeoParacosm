using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Common.Utils;
using NeoParacosm.Core.Systems;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;

namespace NeoParacosm.Content.Projectiles.Friendly.Melee;

public class AscendedRottedForkHeldProj : ModProjectile
{
    int AITimer = 0;
    int releasedTimer = 0;
    ref float addsLeft => ref Projectile.ai[0];
    bool released = false;
    Vector2 savedMousePos
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

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (released)
        {
            Projectile.damage = (int)(Projectile.damage * 0.9f);
        }
        else
        {
            if (Main.rand.NextBool(10))
            {
                target.AddBuff(BuffID.Ichor, 120);
            }
        }
    }

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 1;
    }

    public override void SetDefaults()
    {
        Projectile.width = 88;
        Projectile.height = 88;
        Projectile.hostile = false;
        Projectile.friendly = true;
        Projectile.ignoreWater = false;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 60;
        Projectile.DamageType = DamageClass.Melee;
        Projectile.Opacity = 0f;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 10;
    }

    int releasedDuration = 90;
    public override void AI()
    {
        Player player = Main.player[Projectile.owner];
        Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter);
        if (player == null || player.dead || !player.active) Projectile.Kill();
        if (AITimer < 30) Projectile.Opacity = MathHelper.Lerp(0, 1, AITimer / 30f);
        if (Projectile.timeLeft < 30) Projectile.Opacity = MathHelper.Lerp(0, 1, Projectile.timeLeft / 30f);

        if (player.channel && !released)
        {
            Dust.NewDustPerfect(Projectile.Center + new Vector2(-1, -1).RotatedBy(Projectile.rotation) * Projectile.height * 0.5f, DustID.Crimson).noGravity = true;
            Projectile.timeLeft = 180;
            player.heldProj = Projectile.whoAmI;
            player.SetDummyItemTime(2);
            Projectile.velocity = Vector2.Zero;
            Projectile.Center = playerCenter;
            float rotSpeed = addsLeft >= 0 ? Math.Clamp(AITimer / 180f, 0f, 1f) : 15;
            Projectile.rotation = MathHelper.ToRadians(AITimer * 15 * rotSpeed * player.direction);
            Projectile.spriteDirection = -player.direction;
            SoundEngine.PlaySound(SoundID.Item1, Projectile.Center);

            if (AITimer % 60 == 0 && AITimer > 0)
            {
                if (addsLeft > 0)
                {
                    if (Main.myPlayer == Projectile.owner)
                    {
                        SoundEngine.PlaySound(SoundID.DD2_DarkMageSummonSkeleton with { PitchRange = (-0.2f, 0.2f) });
                        LemonUtils.QuickProj(Projectile, Projectile.Center, Vector2.Zero, Type, Projectile.damage / ((addsLeft + 1) * 0.5f), ai0: -addsLeft);
                    }
                    addsLeft--;
                }
            }
        }
        else
        {
            released = true;
        }

        if (released)
        {
            releasedTimer++;
            Projectile.localNPCHitCooldown = 30;
            if (AITimer % 3 == 0 && addsLeft >= 0)
            {
                Vector2 pos = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * Projectile.height * 0.5f;
                Dust.NewDustPerfect(pos, DustID.Crimson, Projectile.velocity.RotatedBy(MathHelper.PiOver2).SafeNormalize(Vector2.Zero) * 5, Scale: 2f).noGravity = true;
                Dust.NewDustPerfect(pos, DustID.Crimson, Projectile.velocity.RotatedBy(-MathHelper.PiOver2).SafeNormalize(Vector2.Zero) * 5, Scale: 2f).noGravity = true;
            }
            if (savedMousePos == Vector2.Zero)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    savedMousePos = Main.MouseWorld;
                }
                Projectile.netUpdate = true;
                Vector2 dirToMouse = player.Center.DirectionTo(savedMousePos);
                if (addsLeft >= 0)
                {
                    Projectile.velocity = dirToMouse * (10 * (5 - addsLeft));
                    Projectile.damage -= (int)(addsLeft * (0.2f * Projectile.damage));
                }
                else
                {
                    Projectile.velocity = dirToMouse * (10 * -addsLeft);
                }
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
                Projectile.spriteDirection = -1;
                Projectile.timeLeft = 120;
            }

            if (releasedTimer > releasedDuration)
            {
                Projectile.velocity = Projectile.DirectionTo(playerCenter) * (releasedTimer - 90);
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4 + MathHelper.Pi;
                if (Projectile.Center.Distance(playerCenter) < 30)
                {
                    Projectile.Kill();
                }
            }
            else
            {
                Projectile.velocity *= 0.9f;
            }

        }

        AITimer++;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D glowTexture = TextureAssets.Projectile[Type].Value;
        if (addsLeft >= 0)
        {
            Texture2D originalTexture = TextureAssets.Projectile[ProjectileID.TheRottedFork].Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            float colorChangeRate = (addsLeft + 1) * 4;
            float greenSin = ((float)Math.Sin(AITimer / colorChangeRate) + 1) * 0.5f;
            Color glowColor = new Color(1, greenSin, 0, 1);
            Main.EntitySpriteDraw(originalTexture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, originalTexture.Size() * 0.5f, Projectile.scale, LemonUtils.SpriteDirectionToSpriteEffects(Projectile.spriteDirection));
            var shader = GameShaders.Misc["NeoParacosm:AscendedWeaponGlow"];
            shader.Shader.Parameters["uTime"].SetValue(AITimer);
            shader.Shader.Parameters["color"].SetValue(glowColor.ToVector4());
            shader.Shader.Parameters["moveSpeed"].SetValue(0.5f);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, default, Main.Rasterizer, shader.Shader, Main.GameViewMatrix.TransformationMatrix);
            Main.instance.GraphicsDevice.Textures[1] = ParacosmTextures.NoiseTexture.Value;
            shader.Apply();
            Main.EntitySpriteDraw(glowTexture, drawPos, null, Color.White, Projectile.rotation, glowTexture.Size() * 0.5f, Projectile.scale, LemonUtils.SpriteDirectionToSpriteEffects(Projectile.spriteDirection), 0);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }

        Color color = Color.Red * (-addsLeft * 0.25f);
        color.A = 255;

        if (released)
        {
            Main.EntitySpriteDraw(glowTexture, Projectile.Center - Main.screenPosition, null, color * Projectile.Opacity, Projectile.rotation, glowTexture.Size() * 0.5f, Projectile.scale, LemonUtils.SpriteDirectionToSpriteEffects(Projectile.spriteDirection));
        }
        return false;
    }
}
