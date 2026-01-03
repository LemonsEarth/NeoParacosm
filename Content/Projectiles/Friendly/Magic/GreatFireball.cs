using NeoParacosm.Content.Items.Weapons.Magic.Spells;
using Terraria.Audio;

namespace NeoParacosm.Content.Projectiles.Friendly.Magic;

public class GreatFireball : ModProjectile
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
        Projectile.width = 48;
        Projectile.height = 48;
        Projectile.friendly = true;
        Projectile.timeLeft = 720;
        Projectile.penetrate = 1;
        Projectile.Opacity = 0f;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 30;
        Projectile.extraUpdates = 2;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.AddBuff(BuffID.OnFire3, 180);
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

        Player player = Main.player[Projectile.owner];

        if (!player.Alive() && !released)
        {
            Projectile.Kill();
            return;
        }

        int baseTimeToFire = 300;
        float fireSpeedBoost = player.NPCatalystPlayer().ElementalExpertiseBoosts[BaseSpell.SpellElement.Fire];
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
}
