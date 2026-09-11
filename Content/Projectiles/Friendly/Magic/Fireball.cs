using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Items.Weapons.Magic.Spells;
using NeoParacosm.Core.Systems.Assets;
using NeoParacosm.Core.Systems.Drawing;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using static Terraria.GameContent.Animations.Actions.Sprites;

namespace NeoParacosm.Content.Projectiles.Friendly.Magic;

public class Fireball : ModProjectile, IShaderProjectile
{
    ref float AITimer => ref Projectile.ai[0];
    ref float Mode => ref Projectile.ai[1];

    public MiscShaderData ShaderData => ProjectileShaderRenderer.GetMiscShader("FireballShader");
    public override string Texture => ParacosmTextures.GlowBallTexturePath;

    bool released = false;
    Vector2 savedVelocity = Vector2.Zero;



    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
    }

    public override void SetDefaults()
    {
        Projectile.width = 24;
        Projectile.height = 24;
        Projectile.friendly = true;
        Projectile.timeLeft = 600;
        Projectile.penetrate = 1;
        Projectile.Opacity = 0f;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 30;
        Projectile.extraUpdates = 2;
        Projectile.Opacity = 0f;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.AddBuff(BuffID.OnFire, (int)(180 * Main.player[Projectile.owner].GetElementalExpertiseBoostMultiplied(SpellElement.Fire, 2)));
    }

    public override void AI()
    {
        if (AITimer == 0)
        {
            SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);
            savedVelocity = Projectile.velocity;
            Projectile.velocity = Vector2.Zero;
        }
        Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.OrangeStainedGlass, Scale: 2f, newColor: Color.OrangeRed).noGravity = true;
        Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.GemTopaz, Scale: 1f, newColor: Color.Yellow).noGravity = true;

        Player player = Projectile.GetOwner();

        if (!player.IsAlive() && !released)
        {
            Projectile.Kill();
            return;
        }
        int baseTimeToFire = 60;
        float fireSpeedBoost = player.NPCatalystPlayer().ElementalExpertiseBoosts[SpellElement.Fire];
        int minTimeToFire = 20;
        int timeAdjusted = Math.Max((int)(baseTimeToFire - (baseTimeToFire * (fireSpeedBoost - 1))), minTimeToFire);
        if ((!player.channel || AITimer >= timeAdjusted || Mode == 1) && !released)
        {
            released = true;
            if (Main.myPlayer == Projectile.owner)
            {
                if (Mode == 1) // used by flameforged battle axe
                {
                    Projectile.velocity = player.DirectionTo(Main.MouseWorld) * savedVelocity.Length();
                }
                else
                {
                    Projectile.velocity = player.DirectionTo(Main.MouseWorld) * savedVelocity.Length() * (Math.Clamp(AITimer, 0, timeAdjusted) / (float)timeAdjusted);
                }

            }
            Projectile.netUpdate = true;
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        if (!released)
        {
            Projectile.velocity = Vector2.Zero;
            Projectile.Opacity += 0.05f;
            Projectile.Center = player.Center;
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
        LemonUtils.QuickProj(Projectile, Projectile.Center, Vector2.Zero, ProjectileType<FireballExplosion>());
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
        Main.EntitySpriteDraw(texture, drawPos, null, Color.White, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale * 1.25f, SpriteEffects.None, 0);
        ShaderData.UseColor(Color.Yellow);
        ShaderData.Apply();
        Main.EntitySpriteDraw(texture, drawPos, null, Color.White, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale * 0.5f, SpriteEffects.None, 0);
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
