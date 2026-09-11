using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Items.Weapons.Magic.Spells;
using NeoParacosm.Core.Systems.Assets;
using NeoParacosm.Core.Systems.Drawing;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;

namespace NeoParacosm.Content.Projectiles.Friendly.Magic;

public class GreatFireball : ModProjectile, IShaderProjectile
{
    ref float AITimer => ref Projectile.ai[0];
    bool released = false;
    Vector2 savedVelocity = Vector2.Zero;
    public override string Texture => ParacosmTextures.GlowBallTexturePath;


    public MiscShaderData ShaderData => ProjectileShaderRenderer.GetMiscShader("FireballShader");

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
    }

    public override void SetDefaults()
    {
        Projectile.width = 48;
        Projectile.height = 48;
        Projectile.friendly = true;
        Projectile.timeLeft = 720;
        Projectile.penetrate = 1;
        Projectile.Opacity = 0f;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 30;
        Projectile.extraUpdates = 2;
        Projectile.Opacity = 0f;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.AddBuff(BuffID.OnFire3, (int)(240 * Main.player[Projectile.owner].GetElementalExpertiseBoostMultiplied(SpellElement.Fire, 2)));
    }

    int releasedTimer = 0;
    public override void AI()
    {
        if (AITimer == 0)
        {
            SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);
            savedVelocity = Projectile.velocity;
            Projectile.velocity = Vector2.Zero;
        }
        if ((releasedTimer < 30 && released) || !released)
        {
            Projectile.tileCollide = false;
        }
        else
        {
            Projectile.tileCollide = true;
        }

        float dustScaleOR = 0.75f;
        float dustScaleYel = 0.5f;
        if (released)
        {
            dustScaleOR = 3f;
            dustScaleYel = 2f;
            releasedTimer++;
        }
        Dust.NewDustDirect(Projectile.RandomPos(), 2, 2, DustID.OrangeStainedGlass, Scale: dustScaleOR, newColor: Color.OrangeRed).noGravity = true;
        Dust.NewDustDirect(Projectile.RandomPos(-24, -24), 2, 2, DustID.GemTopaz, Scale: dustScaleYel, newColor: Color.Yellow).noGravity = true;

        Player player = Projectile.GetOwner();

        if (!player.IsAlive() && !released)
        {
            Projectile.Kill();
            return;
        }

        int baseTimeToFire = 300;
        float fireSpeedBoost = player.NPCatalystPlayer().ElementalExpertiseBoosts[SpellElement.Fire];
        int minTimeToFire = 60;
        int timeAdjusted = Math.Max((int)(baseTimeToFire - (baseTimeToFire * (fireSpeedBoost - 1))), minTimeToFire);
        if ((!player.channel || AITimer >= timeAdjusted) && !released)
        {
            released = true;
            if (Main.myPlayer == Projectile.owner)
            {
                Projectile.velocity = player.DirectionTo(Main.MouseWorld) * savedVelocity.Length() * (Math.Clamp(AITimer, 0, timeAdjusted) / (float)timeAdjusted);
            }
            Projectile.netUpdate = true;
        }

        if (!released)
        {
            Projectile.velocity = Vector2.Zero;
            Projectile.Center = player.Center;
            Projectile.Opacity += 0.05f;
            player.SetDummyItemTime(player.NPCatalystPlayer().SelectedSpell.AttackCooldown);
        }
        else
        {
            Projectile.velocity.Y += 0.1f;
        }

        AITimer++;
    }

    public override bool? CanHitNPC(NPC target)
    {
        if (!released) return false;
        else return null;
    }

    public override void OnKill(int timeLeft)
    {
        LemonUtils.QuickProj(Projectile, Projectile.Center, Vector2.Zero, ProjectileType<GreatFireballExplosion>());
    }

    public void DrawProjectile()
    {
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        Vector2 drawPos = Projectile.Center - Main.screenPosition;
        ShaderData.Shader.Parameters["velocity"].SetValue(-Projectile.velocity.SafeNormalize(Vector2.Zero));
        ShaderData.UseColor(Color.Red);
        ShaderData.UseImage1(ParacosmTextures.NoiseTexture);
        ShaderData.UseOpacity(Projectile.Opacity);
        ShaderData.Apply();
        Main.EntitySpriteDraw(texture, drawPos, null, Color.White, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale * 2f, SpriteEffects.None, 0);
        ShaderData.UseColor(Color.Yellow);
        ShaderData.Apply();
        Main.EntitySpriteDraw(texture, drawPos, null, Color.White, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale * 1f, SpriteEffects.None, 0);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        this.QueueToShaderRenderer();
        return false;
    }

    public override void PostDraw(Color lightColor)
    {

    }
}
