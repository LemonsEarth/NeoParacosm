using NeoParacosm.Common.Utils;
using NeoParacosm.Content.Items.Weapons.Magic.Spells.Fire;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;

namespace NeoParacosm.Content.Projectiles.Friendly.Magic;

public class Fireball : ModProjectile
{
    ref float AITimer => ref Projectile.ai[0];
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
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.AddBuff(BuffID.OnFire, 180);
    }

    public override void AI()
    {
        if (AITimer == 0)
        {
            SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);
            savedVelocity = Projectile.velocity;
            Projectile.velocity = Vector2.Zero;
        }
        Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.OrangeStainedGlass, Scale: 2f, newColor:Color.OrangeRed).noGravity = true;
        Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.GemTopaz, Scale: 1f, newColor:Color.Yellow).noGravity = true;

        Player player = Main.player[Projectile.owner];

        if (!player.Alive() && !released)
        {
            Projectile.Kill();
            return;
        }

        if ((!player.channel || AITimer >= 60) && !released)
        {
            released = true;
            Projectile.velocity = savedVelocity * (Math.Clamp(AITimer, 0, 60) / 60f);
        }

        if (!released)
        {
            Projectile.velocity = Vector2.Zero;
            Projectile.Center = player.Center;
            player.SetDummyItemTime(player.NPCatalystPlayer().SelectedSpell.AttackCooldown);
        }
        else
        {
            Projectile.velocity.Y += 0.06f;
        }

        AITimer++;
    }

    public override void OnKill(int timeLeft)
    {
        LemonUtils.QuickProj(Projectile, Projectile.Center, Vector2.Zero, ProjectileType<FireballExplosion>());
    }
}
