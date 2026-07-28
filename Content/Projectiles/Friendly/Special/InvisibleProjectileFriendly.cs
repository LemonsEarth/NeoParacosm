namespace NeoParacosm.Content.Projectiles.Friendly.Special;

public class InvisibleProjectileFriendly : ModProjectile
{
    int AITimer = 0;
    ref float FollowNPCID => ref Projectile.ai[0];
    ref float Width => ref Projectile.ai[1];
    ref float Height => ref Projectile.ai[2];

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 1;
    }

    public override void SetDefaults()
    {
        Projectile.width = 32;
        Projectile.height = 32;
        Projectile.hostile = false;
        Projectile.friendly = true;
        Projectile.ignoreWater = false;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.localNPCHitCooldown = 30;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.timeLeft = 1000;
        Projectile.scale = 1f;
    }

    int timeLeft = 60;

    public override void AI()
    {
        if (AITimer == 0)
        {
            // using velocity X to store timeleft since other ai values are used for other things
            if (Projectile.velocity != Vector2.Zero)
            {
                timeLeft = (int)Projectile.velocity.X;
                Projectile.timeLeft = timeLeft;
            }
            Projectile.velocity = Vector2.Zero;
        }

        if (FollowNPCID >= 0)
        {
            if (Main.npc[(int)FollowNPCID] == null || !Main.npc[(int)FollowNPCID].active)
            {
                Projectile.Kill();
            }
            Projectile.position = Main.npc[(int)FollowNPCID].position;

        }

        Projectile.width = (int)Width;
        Projectile.height = (int)Height;

        Projectile.velocity = Vector2.Zero;

        AITimer++;
    }

    public override bool? CanHitNPC(NPC target)
    {
        if (target.whoAmI == FollowNPCID)
        {
            return false;
        }
        return null;
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {

    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {

    }
}
