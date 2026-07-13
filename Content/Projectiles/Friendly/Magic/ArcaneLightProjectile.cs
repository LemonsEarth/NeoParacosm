using NeoParacosm.Core.Systems.Assets;
using System.Linq;
using Terraria.Audio;

namespace NeoParacosm.Content.Projectiles.Friendly.Magic;

public class ArcaneLightProjectile : ModProjectile
{
    int AITimer = 0;
    ref float BezierDistance => ref Projectile.ai[0];
    ref float TargetPosX => ref Projectile.ai[1];
    ref float TargetPosY => ref Projectile.ai[2];

    public override string Texture => ParacosmTextures.Empty100TexPath;

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
        Projectile.timeLeft = 30;
        Projectile.penetrate = 3;
        Projectile.Opacity = 1f;
        Projectile.tileCollide = false;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 30;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        //target.AddBuff(BuffID.OnFire3, 90);
        if (Main.rand.NextBool(10))
        {
            Player lowestHealthTeammate = Main.player
                .Where(p => p.active && p.team == Projectile.GetOwner().team)
                .MinBy(p => p.statLife);
            if (lowestHealthTeammate != null)
            {
                lowestHealthTeammate.Heal(3);
            }
        }
    }

    Vector2 startPos;
    Vector2 targetPos;
    Vector2 dirToTarget;
    Vector2 controlPoint;
    Vector2 controlDir;
    float savedDistance;
    public override void AI()
    {
        if (AITimer == 0)
        {
            startPos = Projectile.Center;
            targetPos = new Vector2(TargetPosX, TargetPosY);
            dirToTarget = startPos.DirectionTo(targetPos);
            controlDir = dirToTarget.RotatedBy(MathHelper.PiOver2);
            savedDistance = startPos.Distance(targetPos);
            controlPoint = startPos + (dirToTarget * savedDistance * 0.5f) + (controlDir * BezierDistance);
        }
        for (int i = 0; i < 2; i++)
        {
            Dust.NewDustDirect(Projectile.RandomPos(-16, -16), 2, 2, DustID.GemRuby, Scale: 1.5f).noGravity = true;
            Dust.NewDustDirect(Projectile.RandomPos(-16, -16), 2, 2, DustID.Granite, Scale: 1.5f, newColor: Color.Black).noGravity = true;
        }
        Projectile.Center = LemonUtils.BezierCurve(startPos, targetPos, controlPoint, AITimer / 30f);

        AITimer++;
    }

    public override void OnKill(int timeLeft)
    {
        LemonUtils.DustBurst(5, Projectile.Center, DustID.GemRuby, 4, 4, 1f, 2f);
        LemonUtils.DustBurst(5, Projectile.Center, DustID.Granite, 4, 4, 1f, 2f, Color.Black);
    }
}
