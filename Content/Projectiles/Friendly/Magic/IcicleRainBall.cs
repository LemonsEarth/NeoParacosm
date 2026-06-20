using NeoParacosm.Content.Items.Weapons.Magic.Spells;
using NeoParacosm.Core.Systems.Assets;
using Terraria.Audio;

namespace NeoParacosm.Content.Projectiles.Friendly.Magic;

public class IcicleRainBall : ModProjectile
{
    ref float AITimer => ref Projectile.ai[0];
    ref float Mode => ref Projectile.ai[1];
    bool released = false;

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
    }

    public override void SetDefaults()
    {
        Projectile.width = 32;
        Projectile.height = 32;
        Projectile.friendly = true;
        Projectile.timeLeft = 60;
        Projectile.penetrate = 1;
        Projectile.Opacity = 1f;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 30;
    }


    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        //target.AddBuff(BuffID.OnFire, (int)(180 * Main.player[Projectile.owner].GetElementalExpertiseBoostMultiplied(SpellElement.Fire, 2)));
    }

    public override void AI()
    {
        if (AITimer == 0)
        {
            SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);
            Projectile.velocity = Vector2.Zero;
            LemonUtils.DustBurst(8, Projectile.Center, DustID.IceTorch, 5, 5, 1f, 1.5f);
        }
        //Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.OrangeStainedGlass, Scale: 2f, newColor: Color.OrangeRed).noGravity = true;
        //Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.GemTopaz, Scale: 1f, newColor: Color.Yellow).noGravity = true;

        Player player = Projectile.GetOwner();

        if (!player.IsAlive())
        {
            Projectile.Kill();
            return;
        }
        Lighting.AddLight(Projectile.Center, 1, 1, 1);
        if (!player.channel && !released)
        {
            released = true;
        }

        if (!released)
        {
            Projectile.timeLeft = 30;
            player.heldProj = Projectile.whoAmI;
            player.SetDummyItemTime(2);
            Projectile.Center = player.Center - Vector2.UnitY * 64f;
            if (AITimer % 20 == 0 && player.CheckManaButGood(10, true, false))
            {
                SoundEngine.PlaySound(SFX.IceBlockStrike, Projectile.Center);
                int projCount = (int)(3 * player.GetElementalExpertiseBoostMultiplied(SpellElement.Ice, 2));
                if (Main.myPlayer == Projectile.owner)
                {
                    for (int i = 0; i < projCount; i++)
                    {
                        Projectile.NewProjectileDirect(
                            player.GetSource_FromThis(),
                            Projectile.Center,
                            Vector2.Zero,
                            ProjectileID.NorthPoleSnowflake,
                            Projectile.damage,
                            Projectile.knockBack / 2,
                            player.whoAmI,
                            ai1: Main.rand.Next(0, 3)
                        );
                    }
                }
            }
        }
        else
        {
            Projectile.Opacity -= 1 / 60f;
        }
        Projectile.rotation = MathHelper.ToRadians(AITimer * 1);

        AITimer++;
    }

    public override bool? CanHitNPC(NPC target)
    {
        return false;
    }

    public override void OnKill(int timeLeft)
    {

    }
}
