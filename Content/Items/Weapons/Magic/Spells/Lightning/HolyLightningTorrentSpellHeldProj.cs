using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Dusts;
using NeoParacosm.Content.Items.Weapons.Magic.Spells;
using NeoParacosm.Core.Systems.Assets;
using Terraria.Audio;
using Terraria.Graphics.Shaders;

namespace NeoParacosm.Content.Projectiles.Friendly.Magic;

public class HolyLightningTorrentSpellHeldProj : ModProjectile
{
    public override string Texture => ParacosmTextures.Empty100TexPath;
    int AITimer = 0;
    ref float ChargeAmount => ref Projectile.ai[0];
    ref float DirectionX => ref Projectile.ai[1];
    ref float DirectionY => ref Projectile.ai[2];
    float Length = 120f;
    int releasedTimer = 0;
    bool released = false;

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
    }

    public override void SetDefaults()
    {
        Projectile.width = 32;
        Projectile.height = 32;
        Projectile.friendly = true;
        Projectile.hostile = false;
        Projectile.timeLeft = 600;
        Projectile.penetrate = 1;
        Projectile.Opacity = 1f;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 30;
        Projectile.tileCollide = false;
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        //target.AddBuff(BuffID.Ichor, 300);
    }

    Color color = Color.LightYellow;
    float random = 0;
    public override void AI()
    {
        if (AITimer == 0)
        {
            LemonUtils.DustBurst(10, Projectile.Center, DustType<StreakDust>(), 5, 5, 0.5f, 2f, Color.LightYellow);

            SoundEngine.PlaySound(ParacosmSFX.Thunder with { PitchRange = (0.5f, 0.8f), MaxInstances = 10, Volume = 0.6f }, Projectile.Center);
            SoundEngine.PlaySound(SoundID.DD2_LightningBugZap with { PitchRange = (1f, 1.2f), Volume = 0.5f }, Projectile.Center);
            random = Main.rand.Next(1, 100);
            Projectile.velocity = Vector2.Zero;
        }

        Player player = Projectile.GetOwner();

        if (!player.channel && !released)
        {
            released = true;
            Vector2 direction = Vector2.UnitX.RotatedBy(Projectile.rotation);
            DirectionX = direction.X;
            DirectionY = direction.Y;
            Projectile.velocity = direction * 60;

            for (int i = 0; i < (ChargeAmount / 10) * player.GetElementalExpertiseBoost(SpellElement.Holy); i++)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    LemonUtils.QuickProj(
                            Projectile,
                            player.Center - direction * 800 * Main.rand.NextFloat(1f, 1.4f) + direction.RotatedBy(MathHelper.PiOver2) * Main.rand.NextFloat(-128, 128),
                            direction * 80 * player.GetElementalExpertiseBoost(SpellElement.Lightning),
                            ProjectileType<HolyLightningSpearFriendly>(),
                            Projectile.damage,
                            Projectile.knockBack,
                            Projectile.owner,
                            ai0: 180,
                            ai1: 60 + i * 5
                        );
                }
            }
        }

        if (!released)
        {
            Projectile.Center = player.Top;
            player.SetDummyItemTime(2);
            player.heldProj = Projectile.whoAmI;
            if (Main.myPlayer == Projectile.owner)
            {
                Projectile.rotation = player.DirectionTo(Main.MouseWorld).ToRotation();
            }
            if (ChargeAmount < 120)
            {
                ChargeAmount++;
            }

            if (ChargeAmount == 119)
            {
                SoundEngine.PlaySound(SFX.CrystalBall, player.Center);
                LemonUtils.DustBurst(20, Projectile.Center, DustType<StreakDust>(), 10, 10, 0.5f, 2f, Color.LightYellow);
            }

            if (AITimer % 5 == 0)
            {
                Projectile.netUpdate = true;
            }
        }

        if (released)
        {
            releasedTimer++;
        }

        if (releasedTimer > 90)
        {
            Projectile.Kill();
            return;
        }

        AITimer++;
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        // float _ = float.NaN;
        // return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, targetPos, Projectile.width, ref _);
        return null;
    }

    public override void OnKill(int timeLeft)
    {
        LemonUtils.DustBurst(10, Projectile.Center, DustType<StreakDust>(), 5, 5, 0.5f, 2f, Color.LightYellow);

        //LemonUtils.DustCircle(Projectile.Center, 8, 10, DustID.TintableDustLighted, 5f, color: Color.Orange);
    }

    float lightningLength => Length / 100f;

    void DrawLightning(float randomMul = 1, float segCountMul = 1)
    {
        var shader = GameShaders.Misc["NeoParacosm:BigLightningShader"];
        shader.Shader.Parameters["lightningLength"].SetValue(lightningLength);
        shader.Shader.Parameters["segmentCount"].SetValue(6);
        shader.Shader.Parameters["time"].SetValue(random * randomMul + AITimer * 10);
        shader.Shader.Parameters["tolerance"].SetValue(0.02f);
        shader.Shader.Parameters["amplitudeMult"].SetValue(0.15f); // empty texture is much larger than weapon sprite, so we're making the lightning smaller
        shader.UseOpacity(Projectile.Opacity);
        shader.UseColor(color * Projectile.Opacity);
        shader.Apply();

        Vector2 lightningScale = new(lightningLength, 1);
        Main.spriteBatch.End();
        LemonUtils.BeginSpriteBatchProjectile(effect: shader.Shader);
        for (int k = Projectile.oldPos.Length - 1; k > 0; k--)
        {
            Vector2 drawPos = Projectile.oldPos[k] + new Vector2(Projectile.width, Projectile.height) * 0.5f - Main.screenPosition;
            Color color = (Projectile.GetAlpha(Color.White) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length));
            Main.EntitySpriteDraw(ParacosmTextures.Empty100Tex.Value, drawPos, null, color, Projectile.oldRot[k], ParacosmTextures.Empty100Tex.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
        }
        Main.EntitySpriteDraw(
            ParacosmTextures.Empty100Tex.Value,
            Projectile.Center - Main.screenPosition,
            null,
            Color.White,
            Projectile.rotation,
            ParacosmTextures.Empty100Tex.Size() * 0.5f,
            lightningScale,
            LemonUtils.SpriteDirectionToSpriteEffects(Projectile.spriteDirection));

        Main.spriteBatch.End();
        LemonUtils.BeginSpriteBatchProjectile();
    }

    public override bool PreDraw(ref Color lightColor)
    {
        if (AITimer <= 2)
        {
            return false;
        }
        LemonUtils.DrawGlow(Projectile.Center, Color.White, Projectile.Opacity * 0.5f, Projectile.scale);
        DrawLightning(1f, 1);
        return false;
    }

    public override void PostDraw(Color lightColor)
    {
        Main.spriteBatch.End();
        LemonUtils.BeginSpriteBatchProjectile();
    }
}