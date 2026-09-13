using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Items.Weapons.Magic.Spells;
using NeoParacosm.Core.Systems.Assets;
using NeoParacosm.Core.Systems.Drawing;
using NeoParacosm.Core.Systems.Particles;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;

namespace NeoParacosm.Content.Projectiles.Friendly.Magic;

public class Hailfireball : ModProjectile, IShaderProjectile
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
        Projectile.width = 20;
        Projectile.height = 20;
        Projectile.friendly = true;
        Projectile.timeLeft = 600;
        Projectile.penetrate = 1;
        Projectile.Opacity = 0f;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 30;
        Projectile.extraUpdates = 2;
        Projectile.ArmorPenetration = 10;
        Projectile.Opacity = 0f;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.AddBuff(BuffID.Frostburn, (int)(180 * Main.player[Projectile.owner].GetElementalExpertiseBoostMultiplied(SpellElement.Ice, 3)));
    }

    public override void AI()
    {
        if (AITimer == 0)
        {
            SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);
            savedVelocity = Projectile.velocity;
            Projectile.velocity = Vector2.Zero;
        }
        Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.IceTorch, Scale: 2f).noGravity = true;
        Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.GemSapphire, Scale: 1f).noGravity = true;

        ParticleSystem.SpawnParticle(
                ParticleID.Glowy,
                Projectile.RandomPos(8, 8),
                Main.rand.NextVector2Circular(2, 2),
                Main.rand.NextFromList(Color.LightBlue),
                0f,
                scale: 0.5f,
                data0: 30,
                data1: 5,
                data2: 5,
                data3: 0.93f);

        Player player = Projectile.GetOwner();

        if (!player.IsAlive() && !released)
        {
            Projectile.Kill();
            return;
        }
        int baseTimeToFire = 60;
        float iceSpeedBoost = player.NPCatalystPlayer().ElementalExpertiseBoosts[SpellElement.Ice];
        int minTimeToFire = 20;
        int timeAdjusted = Math.Max((int)(baseTimeToFire - (baseTimeToFire * (iceSpeedBoost - 1))), minTimeToFire);
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
            Projectile.velocity.Y += 0.06f;
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
        LemonUtils.QuickProj(Projectile, Projectile.Center, Vector2.Zero, ProjectileType<HailfireballExplosion>());
    }

    public void DrawProjectile()
    {
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        Vector2 drawPos = Projectile.Center - Main.screenPosition;
        ShaderData.Shader.Parameters["velocity"].SetValue(-Projectile.velocity.SafeNormalize(Vector2.Zero));
        ShaderData.UseColor(Color.CornflowerBlue);
        ShaderData.UseImage1(ParacosmTextures.NoiseTexture);
        ShaderData.UseOpacity(Projectile.Opacity);
        ShaderData.Apply();
        Main.EntitySpriteDraw(texture, drawPos, null, Color.White, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale * 0.75f, SpriteEffects.None, 0);
        ShaderData.UseColor(Color.AliceBlue);
        ShaderData.Apply();
        Main.EntitySpriteDraw(texture, drawPos, null, Color.White, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale * 0.2f, SpriteEffects.None, 0);
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
