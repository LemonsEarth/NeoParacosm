using NeoParacosm.Content.NPCs.Bosses.ResearcherBoss;
using Terraria.Audio;

namespace NeoParacosm.Content.Projectiles.Effect;

public class DreadlordJumpscareProj : ModProjectile
{
    int AITimer = 0;
    ref float Timeleft => ref Projectile.ai[0];
    ref float Scale => ref Projectile.ai[1];
    ref float ResearcherBossWhoAmI => ref Projectile.ai[2];

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 1;
    }

    public override void SetDefaults()
    {
        Projectile.width = 1108;
        Projectile.height = 488;
        Projectile.hostile = false;
        Projectile.friendly = false;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 600;
    }

    public override void AI()
    {
        if (AITimer == 0)
        {
            SoundEngine.PlaySound(SoundID.Roar with { Pitch = -1f });
            SoundEngine.PlaySound(SoundID.NPCDeath62 with { Pitch = -0.5f });
            SoundEngine.PlaySound(SoundID.Roar with { Pitch = -0.7f });
            SoundEngine.PlaySound(SoundID.NPCDeath62 with { Pitch = -0.2f });
        }

        if (AITimer > Timeleft)
        {
            Projectile.Kill();
        }

        if (!Main.npc[(int)ResearcherBossWhoAmI].active || Main.npc[(int)ResearcherBossWhoAmI].life == 0 || Main.npc[(int)ResearcherBossWhoAmI].type != NPCType<ResearcherBoss>())
        {
            return;
        }

        if (Main.npc[(int)ResearcherBossWhoAmI].Hitbox.Intersects(Projectile.Hitbox))
        {
            Main.npc[(int)ResearcherBossWhoAmI].SimpleStrikeNPC(999999, 1, true);
        }

        AITimer++;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Projectile.DrawAfterimages(Color.White, 0.5f);
        Projectile.DrawProjectile(Color.White);
        return false;
    }

    public override void PostDraw(Color lightColor)
    {

    }
}
