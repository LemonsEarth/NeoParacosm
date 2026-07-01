using NeoParacosm.Content.Items.Weapons.Magic.Spells;
using System.Collections.Generic;
using Terraria.Audio;
namespace NeoParacosm.Content.Projectiles.Friendly.Magic;

public class RoarOfDreadlordProjectile : ModProjectile
{
    int AITimer = 0;
    ref float TimeLeft => ref Projectile.ai[0];

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {

    }

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 2;
        ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
    }

    public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
    {
        overPlayers.Add(index);
    }

    public override void SetDefaults()
    {
        Projectile.width = 128;
        Projectile.height = 118;
        Projectile.hostile = false;
        Projectile.friendly = false;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 600;
        Projectile.DamageType = DamageClass.Magic;
        Projectile.Opacity = 0f;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 20;
        Projectile.hide = true;
        Projectile.scale = 1.2f;
    }

    void PlayRoar(float bonusPitch = 0f)
    {
        SoundEngine.PlaySound(SoundID.Roar with { Pitch = -1f + bonusPitch }, Projectile.Center);
        SoundEngine.PlaySound(SoundID.NPCDeath62 with { Pitch = -0.5f + bonusPitch }, Projectile.Center);
    }

    public override void AI()
    {
        Player player = Projectile.GetOwner();
        if (!player.IsAlive())
        {
            Projectile.Kill();
            return;
        }

        if (AITimer == 0)
        {
            LemonUtils.DustBurst(12, Projectile.Center, DustID.Corruption, 16, 16, 3.5f, 4.5f);
            LemonUtils.DustBurst(12, Projectile.Center, DustID.GemEmerald, 16, 16, 3.5f, 4.5f);
            SoundEngine.PlaySound(SoundID.Item92, Projectile.Center);
        }

        player.heldProj = Projectile.whoAmI;
        player.SetDummyItemTime(2);

        if (AITimer < 90)
        {
            Projectile.frame = 0;
            Projectile.Opacity = MathHelper.Lerp(0, 1, AITimer / 30f);
            Projectile.Center = player.Center;
        }
        else if (AITimer == 90)
        {
            Projectile.Opacity = 1f;
            Projectile.friendly = true;
            Projectile.frame = 1;
            PlayRoar(-0.3f);
            if (LemonUtils.NotClient())
            {
                LemonUtils.QuickPulse(
                        Projectile,
                        Projectile.Center,
                        3, 20, 5,
                        Color.Lime
                        );
                LemonUtils.QuickPulse(
                        Projectile,
                        Projectile.Center,
                        3, 10, 3,
                        Color.Lime
                        );
            }
            LemonUtils.QuickScreenShake(Projectile.Center, 10f, 8f, 120, 2000);
            LemonUtils.DustBurst(12, Projectile.Center, DustID.CursedTorch, 25, 25, 0.5f, 1.2f);


            foreach (var npc in Main.ActiveNPCs)
            {
                if (npc.CanBeChasedBy())
                {
                    npc.AddBuff(BuffID.CursedInferno, (int)(300 * player.GetElementalExpertiseBoostMultiplied(SpellElement.Fire, 2)));
                    //npc.SimpleStrikeNPC(Projectile.damage, 1, false, Projectile.knockBack, Projectile.DamageType);
                }
            }
            Projectile.Resize(128 * 10, 118 * 10);
        }
        else
        {
            Projectile.Center = player.Center + Main.rand.NextVector2Circular(16, 16);
            if (AITimer % 20 == 0)
            {
                LemonUtils.DustBurst(12, Projectile.Center, DustID.GemEmerald, 25, 25, 1.5f, 3.2f);
                LemonUtils.QuickScreenShake(Projectile.Center, 10f, 8f, 120, 2000);
                if (LemonUtils.NotClient())
                {
                    LemonUtils.QuickPulse(
                            Projectile,
                            Projectile.Center,
                            3, 20, 5,
                            Color.Lime
                            );
                    LemonUtils.QuickPulse(
                            Projectile,
                            Projectile.Center,
                            3, 10, 3,
                            Color.Lime
                            );
                }

                foreach (var npc in Main.ActiveNPCs)
                {
                    if (npc.CanBeChasedBy())
                    {
                        npc.AddBuff(BuffID.CursedInferno, (int)(300 * player.GetElementalExpertiseBoostMultiplied(SpellElement.Fire, 2)));
                        //npc.SimpleStrikeNPC(Projectile.damage, 1, false, Projectile.knockBack, Projectile.DamageType);
                    }
                }
            }
        }

        if (TimeLeft - AITimer < 30)
        {
            Projectile.Opacity = MathHelper.Lerp(0, 1, (TimeLeft - AITimer) / 30f);
        }

        if (AITimer >= TimeLeft)
        {
            Projectile.Kill();
            return;
        }

        AITimer++;
    }

    public override void OnKill(int timeLeft)
    {

    }

    public override bool PreDraw(ref Color lightColor)
    {
        Projectile.DrawProjectile(Color.White * Projectile.Opacity);
        return false;
    }

    public override void PostDraw(Color lightColor)
    {

    }
}
