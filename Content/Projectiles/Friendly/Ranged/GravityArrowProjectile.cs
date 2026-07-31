using NeoParacosm.Content.Buffs.Debuffs;
using NeoParacosm.Content.Projectiles.Friendly.Special;
using Terraria.Audio;

namespace NeoParacosm.Content.Projectiles.Friendly.Ranged;

public class GravityArrowProjectile : ModProjectile
{
    int AITimer = 0;
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 1;
    }

    public override void SetDefaults()
    {
        Projectile.width = 10;
        Projectile.height = 10;
        Projectile.arrow = true;
        Projectile.friendly = true;
        Projectile.penetrate = 1;
        Projectile.DamageType = DamageClass.Ranged;
        Projectile.timeLeft = 1200;
    }

    public override void AI()
    {
        // The code below was adapted from the ProjAIStyleID.Arrow behavior. Rather than copy an existing aiStyle using Projectile.aiStyle and AIType,
        // like some examples do, this example has custom AI code that is better suited for modifying directly.
        // See https://github.com/tModLoader/tModLoader/wiki/Basic-Projectile#what-is-ai for more information on custom projectile AI.

        // Apply gravity after a quarter of a second
        Projectile.ai[0] += 1f;
        if (Projectile.ai[0] >= 15f)
        {
            Projectile.ai[0] = 15f;
            Projectile.velocity.Y += 0.1f;
        }

        // The projectile is rotated to face the direction of travel
        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

        // Cap downward velocity
        if (Projectile.velocity.Y > 16f)
        {
            Projectile.velocity.Y = 16f;
        }

        Lighting.AddLight(Projectile.Center, 1, 0, 1);

        AITimer++;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {

    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {

    }

    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(SoundID.Dig, Projectile.position); // Plays the basic sound most projectiles make when hitting blocks.
        if (Main.myPlayer == Projectile.owner)
        {
            LemonUtils.QuickProj(
                Projectile,
                Projectile.Center,
                Vector2.Zero,
                ProjectileType<GravitySuckyProjFriendly>(),
                ai0: 150,
                ai1: 20,
                ai2: 3
                );
        }
    }
}
