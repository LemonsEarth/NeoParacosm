using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;

namespace NeoParacosm.Content.Projectiles.Friendly.Magic;

public class EquinoxSun : ModProjectile
{
    int AITimer = 0;
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
        Projectile.width = 32;
        Projectile.height = 32;
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

    Projectile moon;

    public override void AI()
    {
        if (moon == null)
        {
            moon = Main.projectile.FirstOrDefault(proj => proj.active && proj.type == ProjectileType<EquinoxMoon>() && proj.owner == Projectile.owner, null);
        }
        Player player = Main.player[Projectile.owner];
        player.SetDummyItemTime(2);
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

        if (player.channel)
        {
            float rotSpeedDeg = AITimer * 2;
            Projectile.rotation = MathHelper.ToRadians(rotSpeedDeg);
            float distance = 33.94f; // Moon distance
            Vector2 offset = new Vector2(PosX, PosY).SafeNormalize(Vector2.Zero);
            Projectile.Center = player.MountedCenter + (offset * distance * 2).RotatedBy(MathHelper.ToRadians(rotSpeedDeg));
        }
        else
        {
            if (!moon.active)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    Vector2 mouseDir = player.Center.DirectionTo(Main.MouseWorld);
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, mouseDir * 12, ModContent.ProjectileType<EquinoxSunProj>(), Projectile.damage, 4f, Projectile.owner, ai0: 30f);
                }
                Projectile.Kill();
                return;
            }
        }

        Lighting.AddLight(Projectile.Center, 10, 10, 2);
        Dust dust = Dust.NewDustDirect(Projectile.RandomPos(), 2, 2, DustID.GemTopaz);
        dust.noGravity = true;

        AITimer++;
    }

    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(SoundID.Item90 with { PitchRange = (0.2f, 0.5f) }, Projectile.Center);
        LemonUtils.DustCircle(Projectile.Center, 16, 10, DustID.GemTopaz, 2f);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        LemonUtils.DrawProjectile(Projectile, Color.White);
        LemonUtils.DrawGlow(Projectile.Center, Color.LightYellow, Projectile.Opacity, Projectile.scale * 2f);
        return false;
    }
}
