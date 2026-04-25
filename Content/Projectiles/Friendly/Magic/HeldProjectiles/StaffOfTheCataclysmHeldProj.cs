using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Content.Projectiles.Friendly.Special;
using NeoParacosm.Core.Players.NPEffectPlayers;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;

namespace NeoParacosm.Content.Projectiles.Friendly.Magic.HeldProjectiles;

public class StaffOfTheCataclysmHeldProj : ModProjectile
{
    int AITimer = 0;
    bool fadeOut = false;
    ref float Range => ref Projectile.ai[1];
    public Vector2 DomainPos { get; private set; }
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 1;
    }

    public override void SetDefaults()
    {
        Projectile.width = 80;
        Projectile.height = 80;
        Projectile.hostile = false;
        Projectile.friendly = false;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 600;
        Projectile.DamageType = DamageClass.Magic;
        Projectile.Opacity = 1f;
    }
    Player player;
    int windUpDuration = 120;
    public override void AI()
    {
        player = Projectile.GetOwner();
        if (!player.IsAlive())
        {
            Projectile.Kill();
        }
        Projectile.velocity = Vector2.Zero;
        if (fadeOut)
        {
            AITimer++;
            Projectile.Opacity = MathHelper.Lerp(0, 1, Projectile.timeLeft / 60f);
            return;
        }

        Projectile.timeLeft = 60;
        player.manaRegenDelay = player.maxRegenDelay;

        if (AITimer < windUpDuration)
        {
            WindUp();
            DomainPos = Projectile.Center;
        }
        else
        {
            if (AITimer == windUpDuration)
            {
                SoundEngine.PlaySound(SoundID.DD2_EtherianPortalOpen with { PitchRange = (-0.5f, -0.2f) }, Projectile.Center);
                LemonUtils.DustCircle(Projectile.Center, 8, 10, DustID.GemRuby, 4f);
                LemonUtils.DustCircle(Projectile.Center, 8, 15, DustID.Granite, 4f);
            }
            if (AITimer % 2 == 0)
            {
                int manaToConsume = ((int)MathF.Ceiling(AITimer / 300f));
                bool enoughMana = player.CheckManaButGood(manaToConsume, true, true);
                if (!enoughMana)
                {
                    fadeOut = true;
                    return;
                }
            }

            DomainEffect();
        }

        AITimer++;
    }

    int AITimerAdjusted => AITimer - windUpDuration;

    public void WindUp()
    {
        Vector2 basePos = player.Center + new Vector2(0, -Projectile.height * 2);
        Projectile.Center = basePos + new Vector2(0, MathF.Sin(AITimer / 20f) * 16);
        Projectile.rotation = -MathHelper.PiOver4;
        player.heldProj = Projectile.whoAmI;
    }

    public void DomainEffect()
    {
        DCDomainPlayer.DCDomainProjectile = Projectile;
        /*for (int i = 0; i < 32; i++)
        {
            Dust.NewDustDirect(Projectile.Center - Vector2.UnitY.RotatedBy(i * MathHelper.TwoPi / 32) * Range, 2, 2, DustID.GemDiamond);
        }*/
        Projectile.Center = DomainPos + new Vector2(0, MathF.Sin(AITimer / 20f) * 16);
        Vilethorns();
        RandomLightning();
        TargetedLightning();
    }

    void Vilethorns()
    {
        if (AITimerAdjusted % 300 == 0 && Main.myPlayer == Projectile.owner)
        {
            float baseGapBetweenVilethorns = 150;
            int vtCount = (int)MathF.Ceiling((2 * Range) / baseGapBetweenVilethorns);
            Vector2 startPos = Projectile.Center + new Vector2(-Range, Range);
            for (int i = 0; i < vtCount; i++)
            {
                float gapBetweenVilethorns = baseGapBetweenVilethorns + Main.rand.NextFloat(-25, 25);
                Vector2 pos = startPos + new Vector2(i * baseGapBetweenVilethorns, 0);
                Vector2 velocity = -Vector2.UnitY * Main.rand.Next(70, 100);
                LemonUtils.QuickProj(Projectile, pos, velocity, ProjectileType<VilethornFriendly>(), damage: ScaledProjDamage(player));
            }
        }
    }

    void RandomLightning()
    {
        if (AITimerAdjusted % 30 == 0 && Main.myPlayer == Projectile.owner)
        {
            Vector2 topPos = Projectile.Center + new Vector2(Main.rand.NextFloat(-Range, Range), -800);
            Vector2 botPos = topPos + Vector2.UnitY * 1600;
            Projectile.NewProjectile(Projectile.GetSource_FromAI(),
                topPos,
                Vector2.Zero,
                ProjectileType<YellowLightning>(),
                ScaledProjDamage(player),
                1f,
                Projectile.owner,
                ai1: botPos.X,
                ai2: botPos.Y);
        }
    }

    void TargetedLightning()
    {
        if (AITimerAdjusted % 60 == 0 && Main.myPlayer == Projectile.owner)
        {
            NPC closestNPC = LemonUtils.GetClosestNPC(Projectile.Center, Range / 2);
            if (closestNPC == null)
            {
                return;
            }
            Vector2 topPos = closestNPC.Center + new Vector2(0, -800);
            Vector2 botPos = closestNPC.Center + new Vector2(0, 800);
            Projectile.NewProjectile(Projectile.GetSource_FromAI(),
                topPos,
                Vector2.Zero,
                ProjectileType<YellowLightning>(),
                ScaledProjDamage(player) * 2,
                1f,
                Projectile.owner,
                ai1: botPos.X,
                ai2: botPos.Y);
        }
    }

    int ScaledProjDamage(Player player)
    {
        return ((int)(Projectile.damage * (1 - player.manaSickReduction)));
    }

    public override bool PreDraw(ref Color lightColor)
    {
        LemonUtils.DrawGlow(Projectile.Center, Color.White, Projectile.Opacity * 0.5f, Projectile.scale * 2);
        return true;
    }

    public override void PostDraw(Color lightColor)
    {
        LemonUtils.DrawGlow(Projectile.Top + Vector2.UnitY * 8, Color.Red, Projectile.Opacity * 1f, Projectile.scale * 1);
    }
}
