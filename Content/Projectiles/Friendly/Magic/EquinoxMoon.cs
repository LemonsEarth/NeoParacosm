using System.Linq;
using Terraria.Audio;
using Terraria.DataStructures;

namespace NeoParacosm.Content.Projectiles.Friendly.Magic;

public class EquinoxMoon : ModProjectile
{
    int AITimer = 0;

    ref float RotSpeed => ref Projectile.ai[0];
    ref float PosX => ref Projectile.ai[1];
    ref float PosY => ref Projectile.ai[2];
    public override void SetStaticDefaults()
    {
        Main.projFrames[Projectile.type] = 1;
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
    }

    public override void SetDefaults()
    {
        Projectile.width = 24;
        Projectile.height = 24;
        Projectile.hostile = false;
        Projectile.friendly = false;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 20;
        Projectile.alpha = 255;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 60;
        Projectile.DamageType = DamageClass.Magic;
    }

    public override void OnSpawn(IEntitySource source)
    {
        PosX = Main.rand.NextBool().ToDirectionInt();
        PosY = Main.rand.NextBool().ToDirectionInt();
    }

    public override void AI()
    {
        Player player = Main.player[Projectile.owner];
        player.SetDummyItemTime(2);
        Projectile sun = Main.projectile.FirstOrDefault(proj => proj.active && proj.type == ProjectileType<EquinoxSun>() && proj.owner == Projectile.owner, null);
        if (sun is null)
        {
            Projectile.Kill();
            return;
        }
        Projectile.timeLeft = 2;
        if (Projectile.alpha > 0)
        {
            Projectile.alpha -= 255 / 60;
        }
        if (AITimer == 0)
        {
            SoundEngine.PlaySound(SoundID.Item92 with { MaxInstances = 2 }, Projectile.Center);
        }

        if (AITimer == 30)
        {
            Projectile.damage *= 2;
            Projectile.netUpdate = true;
        }

        if (AITimer % 30 == 0)
        {
            if (Main.myPlayer == Projectile.owner)
            {
                player.CheckMana(30, true, true);
                Vector2 mouseDir = player.Center.DirectionTo(Main.MouseWorld);
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, mouseDir * 4, ModContent.ProjectileType<EquinoxMoonProj>(), Projectile.damage, 4f, ai0: 30f);
            }
        }

        if (player.channel)
        {
            float rotSpeedDeg = AITimer * 4;
            Projectile.rotation = MathHelper.ToRadians(rotSpeedDeg);
            float distance = 33.94f; //(float)Math.Sqrt(2 * (Projectile.width * Projectile.width))
            Vector2 offset = new Vector2(PosX, PosY).SafeNormalize(Vector2.Zero);
            Projectile.Center = player.MountedCenter + (offset * distance * 2).RotatedBy(MathHelper.ToRadians(rotSpeedDeg));
        }
        else
        {
            if (Main.myPlayer == Projectile.owner)
            {
                if (Projectile.Colliding(Projectile.Hitbox, sun.Hitbox))
                {
                    player.CheckMana(80, true, true);
                    for (int i = 0; i < 8; i++)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, (Vector2.UnitY * 16).RotatedBy(MathHelper.PiOver4 * i), ModContent.ProjectileType<EquinoxMoonProj>(), Projectile.damage, 4f, Projectile.owner, ai0: 30f);
                    }
                    for (int i = 0; i < 8; i++)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, (Vector2.UnitY * 16).RotatedBy(MathHelper.PiOver4 * i + MathHelper.PiOver4 / 2), ModContent.ProjectileType<EquinoxSunProj>(), Projectile.damage, 4f, Projectile.owner, ai0: 30f);
                    }

                    LemonUtils.QuickPulse(Projectile, Projectile.Center, 2f, 10f, 5f, Color.Gold);
                }
                else
                {
                    player.CheckMana(30, true, true);
                    Vector2 mouseDir = player.Center.DirectionTo(Main.MouseWorld);
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, mouseDir * 12, ModContent.ProjectileType<EquinoxMoonProj>(), Projectile.damage, 4f, Projectile.owner, ai0: 30f);
                }
            }
            Projectile.Kill();
        }

        Lighting.AddLight(Projectile.Center, 2, 2, 2);
        Dust dust = Dust.NewDustDirect(Projectile.RandomPos(), 2, 2, DustID.GemDiamond);
        dust.noGravity = true;

        AITimer++;
    }

    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(SoundID.Item90 with { PitchRange = (0.2f, 0.5f) }, Projectile.Center);
        //LemonUtils.DustCircle(Projectile.Center, 16, 10, DustID.GemDiamond, 1.5f);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        LemonUtils.DrawGlow(Projectile.Center, Color.Black, Projectile.Opacity, Projectile.scale * 1.5f);
        LemonUtils.DrawProjectile(Projectile, Color.White);
        return false;
    }
}
