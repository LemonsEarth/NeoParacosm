using Terraria.Audio;

namespace NeoParacosm.Content.Projectiles.Interactable;

public class ImmortalEnergyProj : ModProjectile
{
    ref float AITimer => ref Projectile.ai[0];
    ref float TimeLeft => ref Projectile.ai[1];
    ref float RotDirection => ref Projectile.ai[2];

    public override void SetStaticDefaults()
    {
        Main.projFrames[Type] = 4;
    }

    public override void SetDefaults()
    {
        Projectile.width = 64;
        Projectile.height = 64;
        Projectile.hostile = false;
        Projectile.friendly = false;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 600;
        Projectile.penetrate = -1;
        Projectile.Opacity = 1f;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        if (Main.myPlayer != Projectile.owner)
        {
            return false;
        }
        LemonUtils.DrawGlow(Projectile.Center, Color.White, Projectile.Opacity, Projectile.scale);
        Projectile.DrawProjectile(Color.White);
        return false;
    }

    public override void AI()
    {
        if (AITimer == 0)
        {
            if (Main.myPlayer == Projectile.owner)
            {
                SoundEngine.PlaySound(SoundID.NPCHit36 with { PitchRange = (0.5f, 0.8f), Volume = 0.5f }, Projectile.position);
                LemonUtils.DustBurst(16, Projectile.Center, DustID.GemSapphire, 8, 8, 2f, 3f);
            }
        }

        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        Projectile.StandardAnimation(6, 4);
        if (Projectile.GetOwner().IsAlive() || AITimer >= TimeLeft)
        {
            Projectile.Kill();
            return;
        }

        if (Main.myPlayer == Projectile.owner)
        {
            Dust.NewDustPerfect(
                Projectile.RandomPos(),
                DustID.GemSapphire
                ).noGravity = true;

            float projMouseDistanceSQ = Projectile.DistanceSQ(Main.MouseWorld);
            if (projMouseDistanceSQ < 300 * 300)
            {
                Vector2 mouseToProj = Main.MouseWorld.DirectionTo(Projectile.Center);
                Vector2 targetPosition = Main.MouseWorld + (mouseToProj * projMouseDistanceSQ * 2).RotatedBy(RotDirection * MathHelper.PiOver4);
                Projectile.TurningMoveToPos(targetPosition, 30, 8);
            }

            if (Projectile.Hitbox.Contains(Main.MouseWorld.ToPoint()) && Main.mouseLeft && Main.mouseLeftRelease)
            {
                Projectile.GetOwner().respawnTimer -= 2 * 60;
                Projectile.Kill();
                return;
            }
        }

        AITimer++;
    }


    public override void OnKill(int timeLeft)
    {
        if (Main.myPlayer == Projectile.owner)
        {
            SoundEngine.PlaySound(SoundID.NPCHit36 with { PitchRange = (-0.6f, -0.3f), Volume = 0.5f }, Projectile.position);
            LemonUtils.DustBurst(16, Projectile.Center, DustID.GemSapphire, 8, 8, 2f, 3f);
        }
    }
}
