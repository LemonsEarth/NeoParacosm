
using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Common.Utils;
using NeoParacosm.Content.NPCs.Bosses.Deathbird;
using NeoParacosm.Core.Systems;
using System.Collections.Generic;

namespace NeoParacosm.Content.Projectiles.Hostile;

public class DeathbirdGrab : ModProjectile
{
    public override string Texture => "NeoParacosm/Common/Assets/Textures/Misc/GlowBall";

    int AITimer = 0;
    ref float DeathbirdID => ref Projectile.ai[0];
    ref float duration => ref Projectile.ai[1];

    HashSet<Player> hitPlayers = new HashSet<Player>();

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 1;
    }

    public override void SetDefaults()
    {
        Projectile.width = 64;
        Projectile.height = 64;
        Projectile.hostile = false;
        Projectile.friendly = false;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 999;
        Projectile.scale = 1f;
    }

    public override Color? GetAlpha(Color lightColor)
    {
        return Color.DarkSlateBlue;
    }

    public override void AI()
    {
        if (AITimer == 0)
        {

        }

        if (AITimer > duration)
        {
            Projectile.Kill();
        }

        Projectile.damage = 1;

        if (Main.npc[(int)DeathbirdID].type != NPCType<Deathbird>() || !Main.npc[(int)DeathbirdID].active || Main.npc[(int)DeathbirdID].life <= 0 || Main.npc[(int)DeathbirdID].dontTakeDamage)
        {
            Projectile.Kill();
            return;
        }

        Projectile.velocity = Vector2.Zero;
        Lighting.AddLight(Projectile.Center, 0.75f, 0, 1);

        if (hitPlayers.Count > 0)
        {
            foreach (var player in hitPlayers)
            {
                player.NPBuffPlayer().grabbed = true;
                player.Center = Projectile.Center;
            }

            if (AITimer % 10 == 0)
            {
                hitPlayers.RemoveWhere(p => p == null || !p.active || p.statLife <= 0);
            }
        }
        if (AITimer % 20 == 0)
        {
            LemonUtils.DustCircle(Projectile.Center, 8, 4, DustID.GemRuby, 2);
        }
        foreach (var player in Main.ActivePlayers)
        {
            if (hitPlayers.Contains(player)) return;
            if (Projectile.Colliding(Projectile.Hitbox, player.Hitbox))
            {
                hitPlayers.Add(player);
                Projectile.timeLeft = 180;
            }
        }

        AITimer++;
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {

    }

    public override bool PreDraw(ref Color lightColor)
    {
        return false;
    }
}
