using NeoParacosm.Common.Utils;
using NeoParacosm.Content.Items.Weapons.Magic.Spells;
using NeoParacosm.Content.Items.Weapons.Magic.Spells.Fire;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;

namespace NeoParacosm.Content.Projectiles.Friendly.Magic;

public class Hailfireball : ModProjectile
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
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.AddBuff(BuffID.Frostburn, 180);
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

        Player player = Main.player[Projectile.owner];

        if (!player.Alive() && !released)
        {
            Projectile.Kill();
            return;
        }
        int baseTimeToFire = 60;
        float iceSpeedBoost = player.NPCatalystPlayer().ElementalExpertiseBoosts[BaseSpell.SpellElement.Ice];
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
}
