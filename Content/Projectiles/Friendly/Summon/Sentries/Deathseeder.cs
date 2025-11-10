using NeoParacosm.Common.Utils;
using NeoParacosm.Core.Globals.GlobalNPCs.Evil;
using System.Linq;

namespace NeoParacosm.Content.Projectiles.Friendly.Summon.Sentries;

public class Deathseeder : ModProjectile
{
    ref float AITimer => ref Projectile.ai[0];
    ref float AttackTimer => ref Projectile.ai[1];
    NPC closestEnemy;
    public override void SetStaticDefaults()
    {
        Main.projFrames[Projectile.type] = 4;
    }

    public override void SetDefaults()
    {
        Projectile.width = 54;
        Projectile.height = 56;
        Projectile.penetrate = -1;
        Projectile.DamageType = DamageClass.Summon;
        Projectile.tileCollide = true;
        Projectile.timeLeft = Projectile.SentryLifeTime;
        Projectile.friendly = false;
        Projectile.sentry = true;
    }

    public override void AI()
    {
        closestEnemy = GetClosestNPC(1000);

        Projectile.velocity.Y = 10f;
        if (AITimer % 300 == 0 && AITimer > 0)
        {
            LemonUtils.DustCircle(Projectile.Top, 16, 8, DustID.Shadowflame, 2f);
            if (LemonUtils.NotClient())
            {
                NPC npc = NPC.NewNPCDirect(Projectile.GetSource_FromAI("DeathseederSpawn"), Projectile.Top, Main.rand.NextFromCollection(DeathseederNPC.PossibleNPCs.ToList()));
                npc.damage = Projectile.damage;
                npc.lifeMax = Main.player[Projectile.owner].statLifeMax2 / 2;
                npc.defense = Main.player[Projectile.owner].statDefense / 2;
                NetMessage.SendData(MessageID.SyncNPC);
            }
        }

        /* if (closestEnemy != null)
         {
             if (AttackTimer == 90)
             {
                 if (LemonUtils.NotClient())
                 {
                     LemonUtils.QuickProj(Projectile, closestEnemy.Center + Vector2.UnitY * 100, -Vector2.UnitY * 20, ProjectileType<VilethornFriendly>());
                 }
                 SoundEngine.PlaySound(SoundID.Item43 with { Volume = 0.5f, PitchRange = (-0.2f, 0.2f) }, Projectile.Center);

                 AttackTimer = 0;
             }
             AttackTimer++;
         }
         else
         {
             AttackTimer = 0;
         }*/

        /*if (AITimer % 60 == 0)
        {
            LemonUtils.QuickProj(Projectile, Projectile.RandomPos(16, 16), Main.rand.NextVector2Circular(2, 2), ProjectileType<RotGas>());
        }*/

        Projectile.frameCounter++;
        if (Projectile.frameCounter == 20)
        {
            Projectile.frameCounter = 0;
            Projectile.frame++;
            if (Projectile.frame >= 4)
            {
                Projectile.frame = 0;
            }
        }
        Lighting.AddLight(Projectile.Center, 3, 0, 5);
        AITimer++;
    }

    public NPC GetClosestNPC(int distance)
    {
        NPC closestEnemy = null;
        foreach (var npc in Main.ActiveNPCs)
        {
            if (npc.CanBeChasedBy() && Projectile.Center.Distance(npc.Center) < distance)
            {
                if (closestEnemy == null)
                {
                    closestEnemy = npc;
                }
                float distanceToNPC = Projectile.Center.DistanceSQ(npc.Center);
                if (distanceToNPC < Projectile.Center.DistanceSQ(closestEnemy.Center))
                {
                    closestEnemy = npc;
                }
            }
        }
        return closestEnemy;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        Projectile.velocity.Y = 0;
        return false;
    }

    public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
    {
        fallThrough = false;
        return true;
    }
}
