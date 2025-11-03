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
        Projectile.tileCollide = true;
        Projectile.penetrate = -1;
        Projectile.localNPCHitCooldown = 30;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.timeLeft = 1000;
        Projectile.scale = 1f;
    }

    public override void AI()
    {
        if (AITimer == 0)
        {

        }
        if (FollowNPCID <=-1 || Main.npc[(int)FollowNPCID] == null || !Main.npc[(int)FollowNPCID].active)
        {
            Projectile.Kill();
        }
        Projectile.width = (int)Width;
        Projectile.height = (int)Height;
        Projectile.position = Main.npc[(int)FollowNPCID].position;
        Projectile.velocity = Vector2.Zero;

        AITimer++;
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        if (target.whoAmI == FollowNPCID)
        {
            modifiers.FinalDamage *= 0f;
        }
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {

    }
}
