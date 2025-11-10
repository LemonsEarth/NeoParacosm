using NeoParacosm.Common.Utils;
using Terraria.Audio;

namespace NeoParacosm.Content.Projectiles.Friendly.Summon.Sentries;

public class BranchOfLifeProj : ModProjectile
{
    float AITimer = 0;
    Color drawColor = new Color(0f, 0f, 0f, 1f);
    Color desiredColor = new Color(1f, 1f, 1f, 1f);

    public override void SetStaticDefaults()
    {
        Main.projFrames[Projectile.type] = 4;
    }

    public override void SetDefaults()
    {
        Projectile.width = 50;
        Projectile.height = 50;
        Projectile.hostile = false;
        Projectile.friendly = true;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 60;
        Projectile.alpha = 255;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
    }

    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(SoundID.Item29 with { PitchRange = (-0.3f, 0.3f), Volume = 0.5f }, Projectile.Center);

        Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GemTopaz);
        Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GemDiamond);
        Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GemAmethyst);
        Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GemSapphire);
        Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GemRuby);
        Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GemEmerald);
        Gore gore = Gore.NewGoreDirect(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(Main.rand.NextFloat(-5, 5)), Main.rand.Next(61, 64), Main.rand.NextFloat(0.5f, 1f));

    }

    public override void AI()
    {
        if (Projectile.alpha > 0)
        {
            Projectile.alpha -= 8;
        }

        if (AITimer == 0)
        {
            desiredColor = new Color(Main.rand.NextFloat(0f, 1f), Main.rand.NextFloat(0f, 1f), Main.rand.NextFloat(0f, 1f), 1f);
        }

        Projectile.velocity = Vector2.Zero;
        Lighting.AddLight(Projectile.Center, drawColor.ToVector3());
        Projectile.frameCounter++;
        if (Projectile.frameCounter == 6)
        {
            Projectile.frameCounter = 0;
            Projectile.frame++;
            if (Projectile.frame >= 4)
            {
                Projectile.frame = 0;
            }
        }

        drawColor = Color.Lerp(drawColor, desiredColor, AITimer / 60);
        Projectile.friendly = false;
        if (Projectile.timeLeft < 2)
        {
            Projectile.friendly = true;
        }
        AITimer++;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        return false;
    }

    public override void PostDraw(Color lightColor)
    {
        LemonUtils.DrawGlow(Projectile.Center, drawColor, Projectile.Opacity, 1f);
    }
}
