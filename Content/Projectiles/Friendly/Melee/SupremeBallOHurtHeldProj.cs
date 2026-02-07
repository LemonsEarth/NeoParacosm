using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Projectiles.Friendly.Special;
using NeoParacosm.Core.Systems.Assets;
using ReLogic.Content;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;

namespace NeoParacosm.Content.Projectiles.Friendly.Melee;

public class SupremeBallOHurtHeldProj : ModProjectile
{
    int AITimer = 0;
    int releasedTimer = 0;
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

    int chargeCount = 0;
    static Asset<Texture2D> chainTexture;

    public override void Load()
    {
        chainTexture = Request<Texture2D>("NeoParacosm/Content/Projectiles/Friendly/Melee/AscendedBallOHurtChain");
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (chargeCount >= 1 && released)
        {
            for (int i = 0; i < chargeCount * 2; i++)
            {
                Vector2 pos = target.Center + new Vector2(Main.rand.NextFloat(-200, 200), -450);
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), pos, Vector2.Zero, ProjectileType<PurpleLightning>(), Projectile.damage / 4, 1f, Projectile.owner, ai1: target.Center.X, ai2: target.Center.Y);
            }
        }

        if (released)
        {
            Projectile.damage /= 4;
        }
    }

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 1;
    }

    public override void SetDefaults()
    {
        Projectile.width = 38;
        Projectile.height = 38;
        Projectile.hostile = false;
        Projectile.friendly = true;
        Projectile.ignoreWater = false;
        Projectile.tileCollide = false;
        Projectile.penetrate = 6;
        Projectile.stopsDealingDamageAfterPenetrateHits = true;
        Projectile.timeLeft = 60;
        Projectile.DamageType = DamageClass.Melee;
        Projectile.Opacity = 0f;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 10;
    }

    int releasedDuration = 45;
    float lerpValue = 0;
    public override void AI()
    {
        Player player = Main.player[Projectile.owner];
        if (!player.Alive())
        {
            Projectile.Kill();
        }
        Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter);
        if (AITimer < 30) Projectile.Opacity = MathHelper.Lerp(0, 1, AITimer / 30f);
        if (Projectile.timeLeft < 30) Projectile.Opacity = MathHelper.Lerp(0, 1, Projectile.timeLeft / 30f);
        player.heldProj = Projectile.whoAmI;
        player.SetDummyItemTime(2);
        Projectile.timeLeft = 180;

        if (chargeCount >= 1)
        {
            Dust.NewDustDirect(Projectile.RandomPos(), 2, 2, DustID.Electric, Scale: chargeCount / 2f).noGravity = true;
        }

        if (player.channel && !released)
        {
            //Dust.NewDustPerfect(Projectile.Center + new Vector2(-1, -1).RotatedBy(Projectile.rotation) * Projectile.height * 0.5f, DustID.Crimson).noGravity = true;
            Projectile.velocity = Vector2.Zero;
            float rotSpeed = Math.Clamp(AITimer / 60f, 0f, 2f);
            Projectile.Center = playerCenter - Vector2.UnitY.RotatedBy(MathHelper.ToRadians(AITimer * 24 * player.GetAttackSpeed(DamageClass.Melee)) * player.direction) * 32 * Projectile.scale;
            Projectile.rotation = player.DirectionTo(Projectile.Center).ToRotation();
            Projectile.penetrate = 6;
            //SoundEngine.PlaySound(SoundID.Item1, Projectile.Center);
            if (Main.myPlayer == Projectile.owner)
            {
                savedMousePos = Main.MouseWorld;
            }

            if (chargeCount < 6 && AITimer % 45 == 0 && AITimer > 0)
            {
                chargeCount++;
                LemonUtils.DustCircle(playerCenter, 8, 8, DustID.Corruption, chargeCount / 2);
                SoundEngine.PlaySound(SoundID.DD2_DarkMageSummonSkeleton with { PitchRange = (-0.2f, 0.2f) }, Projectile.Center);
                Projectile.scale += Projectile.scale / 4;
                Projectile.Resize(Projectile.width + Projectile.width / 4, Projectile.height + Projectile.height / 4);
            }
        }
        else
        {
            released = true;
        }

        if (released)
        {
            releasedTimer++;

            int controlPointOffset = 150;
            if (lerpValue == 0)
            {
                SoundEngine.PlaySound(SoundID.Item1 with { PitchRange = (-0.2f, 0.2f) });
            }

            if (releasedTimer <= releasedDuration / 2)
            {
                lerpValue++;
                controlPointOffset = 150;
            }
            else
            {
                lerpValue -= 1.5f;
                controlPointOffset = -150;
                if (lerpValue <= 0)
                {
                    Projectile.Kill();
                }
            }

            Projectile.rotation = MathHelper.ToRadians(AITimer * 12);
            Projectile.damage = (Projectile.originalDamage / 2) * (chargeCount + 1);

            Vector2 controlPos = playerCenter + (savedMousePos - playerCenter) / 2 - playerCenter.DirectionTo(savedMousePos).RotatedBy(MathHelper.PiOver2 * player.direction) * controlPointOffset;
            Projectile.Center = LemonUtils.BezierCurve(playerCenter, savedMousePos, controlPos, lerpValue * player.GetAttackSpeed(DamageClass.Melee) / (releasedDuration / 2));

            if (releasedTimer > releasedDuration || (Projectile.Center.Distance(playerCenter) < 16))
            {
                Projectile.Kill();
            }
        }

        AITimer++;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D glowTexture = TextureAssets.Projectile[Type].Value;
        Texture2D originalTexture = TextureAssets.Projectile[ProjectileID.BallOHurt].Value;
        Vector2 drawPos = Projectile.Center - Main.screenPosition;
        Main.EntitySpriteDraw(originalTexture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, originalTexture.Size() * 0.5f, Projectile.scale, LemonUtils.SpriteDirectionToSpriteEffects(Projectile.spriteDirection));
        var shader = GameShaders.Misc["NeoParacosm:AscendedWeaponGlow"];
        shader.Shader.Parameters["uTime"].SetValue(AITimer);
        shader.Shader.Parameters["color"].SetValue(Color.Magenta.ToVector4());
        shader.Shader.Parameters["moveSpeed"].SetValue(0.5f);
        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, default, Main.Rasterizer, shader.Shader, Main.GameViewMatrix.TransformationMatrix);
        Main.instance.GraphicsDevice.Textures[1] = ParacosmTextures.NoiseTexture.Value;
        shader.Apply();

        Vector2 startPos = Main.player[Projectile.owner].Center;
        Vector2 chainDrawPos = startPos;
        int segmentCount = (int)(Projectile.Center.Distance(startPos) / (chainTexture.Value.Height * Projectile.scale));
        int segmentsDrawn = 0;

        Vector2 StartToProj = startPos.DirectionTo(Projectile.Center);

        while (segmentsDrawn < segmentCount)
        {
            Main.EntitySpriteDraw(chainTexture.Value, chainDrawPos - Main.screenPosition, null, Color.White * Projectile.Opacity, StartToProj.ToRotation() + -MathHelper.PiOver2, new Vector2(8, 8), Projectile.scale, SpriteEffects.None);
            chainDrawPos += StartToProj * 16 * Projectile.scale;
            segmentsDrawn++;
        }

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
