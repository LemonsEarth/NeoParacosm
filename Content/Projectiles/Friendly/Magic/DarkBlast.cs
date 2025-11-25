using NeoParacosm.Common.Utils;
using NeoParacosm.Content.Items.Weapons.Magic.Spells;
using Terraria;
using Terraria.Audio;

namespace NeoParacosm.Content.Projectiles.Friendly.Magic;

public class DarkBlast : ModProjectile
{
    ref float AITimer => ref Projectile.ai[0];
    ref float NewPenetrate => ref Projectile.ai[1];
    bool released = false;
    Vector2 savedVelocity = Vector2.Zero;
    int releasedTimer = 0;

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
    }

    public override void SetDefaults()
    {
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.friendly = true;
        Projectile.timeLeft = 3600;
        Projectile.penetrate = 1;
        Projectile.Opacity = 0f;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
        Projectile.extraUpdates = 50;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        
    }

    public override void AI()
    {
        if (AITimer == 0)
        {
            savedVelocity = Projectile.velocity;
            Projectile.velocity = Vector2.Zero;
            SoundEngine.PlaySound(SoundID.Item20 with { PitchRange = (-0.5f, -0.3f)}, Projectile.Center);
        }
        float dustScaleBlack = 0.5f;
        float dustScaleWhite = 0.2f;
        if (released)
        {
            dustScaleBlack = 1.2f;
            dustScaleWhite = 0.5f;
        }

        Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Granite, Scale:dustScaleBlack, newColor: Color.Black).noGravity = true;
        Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.GemDiamond, Scale: dustScaleWhite).noGravity = true;

        Player player = Main.player[Projectile.owner];

        if (releasedTimer == 600)
        {
            Projectile.velocity = savedVelocity;
        }
        if (releasedTimer == 1200)
        {
            Projectile.velocity *= 10 * player.NPCatalystPlayer().ElementalExpertiseBoosts[BaseSpell.SpellElement.Dark];
            SoundEngine.PlaySound(SoundID.Zombie53 with { Volume = 0.2f, PitchRange = (-1f, -0.8f)}, Projectile.Center);
        }


        if (!player.Alive() && !released)
        {
            Projectile.Kill();
            return;
        }

        if ((!player.channel || AITimer >= 2100) && !released)
        {
            released = true;
            if (AITimer >= 2100)
            {
                Projectile.penetrate = (int)NewPenetrate;
            }
        }

        if (!released)
        {
            Projectile.velocity = Vector2.Zero;
            Projectile.Center = player.Center;
            player.SetDummyItemTime(player.NPCatalystPlayer().SelectedSpell.AttackCooldown);
        }
        else
        {
            releasedTimer++;
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
        for (int i = 0; i < 10; i++)
        {
            Vector2 randCircle = Main.rand.NextVector2Circular(5, 5);
            Dust.NewDustDirect(Projectile.Center, 2, 2, DustID.Granite, randCircle.X, randCircle.Y, Scale: 1.2f, newColor: Color.Black).noGravity = true;

        }
    }
}

