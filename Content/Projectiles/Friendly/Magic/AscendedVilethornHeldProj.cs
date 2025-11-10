using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Common.Utils;
using NeoParacosm.Core.Systems.Assets;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;

namespace NeoParacosm.Content.Projectiles.Friendly.Magic;

public class AscendedVilethornHeldProj : ModProjectile
{
    int AITimer = 0;
    bool released = false;
    Vector2 trackingPos;

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {

    }

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 1;
    }

    public override void SetDefaults()
    {
        Projectile.width = 36;
        Projectile.height = 36;
        Projectile.hostile = false;
        Projectile.friendly = false;
        Projectile.ignoreWater = false;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 60;
        Projectile.DamageType = DamageClass.Magic;
        Projectile.Opacity = 0f;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 300;
    }

    public override void AI()
    {
        Player player = Main.player[Projectile.owner];
        if (!player.Alive())
        {
            Projectile.Kill();
        }
        Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter);
        player.heldProj = Projectile.whoAmI;
        if (AITimer == 0)
        {
            trackingPos = Projectile.Center;
        }
        if (player.channel)
        {
            Projectile.timeLeft = 2;
            SetPositionRotationDirection(player, player.Center.DirectionTo(Main.MouseWorld).ToRotation());
            if (AITimer % 30 == 0)
            {
                if (!player.CheckMana(player.HeldItem.mana, true))
                {
                    Projectile.Kill();
                }
                if (Main.myPlayer == Projectile.owner)
                {
                    if (Main.rand.NextBool(5))
                    {
                        LemonUtils.DustCircle(trackingPos, 8, 5, DustID.ScourgeOfTheCorruptor, 3f);
                        SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, trackingPos);
                        for (int i = 0; i < 8; i++)
                        {
                            LemonUtils.QuickProj(Projectile, trackingPos, -Vector2.UnitY.RotatedBy(i * MathHelper.PiOver4) * 5, ProjectileType<VilethornFriendly>(), Projectile.damage / 2);
                        }
                    }
                }
            }
            trackingPos += trackingPos.DirectionTo(Main.MouseWorld) * 5;
            Dust.NewDustPerfect(trackingPos, DustID.GemEmerald, Scale: 2f).noGravity = true;
            if (trackingPos.Distance(Main.MouseWorld) < 12)
            {
                Projectile.Kill();
            }
        }
        else
        {
            Projectile.Kill();
        }
        AITimer++;
    }

    public override void OnKill(int timeLeft)
    {
        LemonUtils.QuickProj(Projectile, trackingPos + Vector2.UnitY * 200, -Vector2.UnitY * 40, ProjectileType<VilethornFriendly>());
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

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D glowTexture = TextureAssets.Projectile[Type].Value;

        Texture2D originalTexture = TextureAssets.Item[ItemID.Vilethorn].Value;
        Vector2 drawPos = Projectile.Center - Main.screenPosition;

        Main.EntitySpriteDraw(originalTexture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, originalTexture.Size() * 0.5f, Projectile.scale, LemonUtils.SpriteDirectionToSpriteEffects(Projectile.spriteDirection));
        var shader = GameShaders.Misc["NeoParacosm:AscendedWeaponGlow"];
        shader.Shader.Parameters["uTime"].SetValue(AITimer);
        shader.Shader.Parameters["color"].SetValue(Color.YellowGreen.ToVector4());
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

    public override void PostDraw(Color lightColor)
    {
        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
    }
}
