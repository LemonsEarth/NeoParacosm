using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Common.Utils.Prim;
using NeoParacosm.Content.Items.Weapons.Magic.Spells;
using NeoParacosm.Core.Systems.Assets;
using Terraria.Audio;
using Terraria.GameContent;

namespace NeoParacosm.Content.Projectiles.Friendly.Magic;

public class LightningTrident : ModProjectile
{
    public override string Texture => ParacosmTextures.Empty100TexPath;

    ref float AITimer => ref Projectile.ai[0];
    ref float Scaling => ref Projectile.ai[1];
    bool released = false;
    Vector2 savedVelocity = Vector2.Zero;

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 42;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
    }

    public override void SetDefaults()
    {
        Projectile.width = 24;
        Projectile.height = 24;
        Projectile.friendly = true;
        Projectile.timeLeft = 600;
        Projectile.penetrate = 1;
        Projectile.Opacity = 1f;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 30;
        Projectile.tileCollide = false;
        Projectile.extraUpdates = 0;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {

    }

    int releasedTimer = 0;
    public override void AI()
    {
        Player player = Projectile.GetOwner();
        if (AITimer == 0)
        {
            SoundEngine.PlaySound(ParacosmSFX.ElectricBurst with { Volume = 0.8f, PitchRange = (0.4f, 0.8f) }, Projectile.Center);
            savedVelocity = Projectile.velocity;
            Projectile.velocity = Vector2.Zero;
        }

        if (!player.IsAlive() && !released)
        {
            Projectile.Kill();
            return;
        }

        if (!player.channel)
        {
            if (!released)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    float speed = 20 * player.GetElementalExpertiseBoostMultiplied(SpellElement.Lightning, 1);
                    Projectile.velocity = player.Center.DirectionTo(Main.MouseWorld) * speed;
                }
                SoundEngine.PlaySound(SoundID.DD2_EtherianPortalDryadTouch, Projectile.Center);
                SoundEngine.PlaySound(ParacosmSFX.ElectricBurst with { Volume = 0.8f, PitchRange = (0.4f, 0.8f) }, Projectile.Center);

                Projectile.netUpdate = true;
            }
            released = true;
        }

        if (!released)
        {
            Projectile.timeLeft = 600;
            player.manaRegenDelay = player.maxRegenDelay;
            player.SetDummyItemTime(player.NPCatalystPlayer().SelectedSpell.AttackCooldown);
            int chargeDuration = ((int)(60f / player.GetElementalExpertiseBoostMultiplied(SpellElement.Lightning, 2)));
            int aiTimerClamped = (int)MathHelper.Clamp(AITimer, 0, chargeDuration);
            Scaling = MathHelper.Lerp(0, 1, aiTimerClamped / (float)chargeDuration);
            Projectile.damage = ((int)(Projectile.originalDamage * Scaling));
            player.heldProj = Projectile.whoAmI;
            Vector2 dirToMouse = player.Center.DirectionTo(Main.MouseWorld);
            if (Main.myPlayer == Projectile.owner)
            {
                dirToMouse = player.Center.DirectionTo(Main.MouseWorld);
                Projectile.velocity = dirToMouse;
            }
            if (AITimer % 5 == 0)
            {
                Projectile.netUpdate = true;
            }
            Projectile.velocity = Vector2.Zero;
            Projectile.Center = player.Center;
            Vector2 leftPos = player.Center - dirToMouse * (Scaling * 32);
            Vector2 rightPos = player.Center + dirToMouse * (Scaling * 32);
            if (AITimer % 2 == 0)
            {
                LemonUtils.DustLine(leftPos, Projectile.Center, DustID.GemTopaz, 4, 1.5f);
                LemonUtils.DustLine(leftPos, Projectile.Center, DustID.GemDiamond, 4, 0.7f);
                for (int i = -1; i <= 1; i++)
                {
                    LemonUtils.DustLine(Projectile.Center, 
                        Projectile.Center + Projectile.Center.DirectionTo(rightPos).RotatedBy(i * MathHelper.Pi / 6) * 32 * Scaling, DustID.GemTopaz, 4, 1f);
                    LemonUtils.DustLine(Projectile.Center,
                        Projectile.Center + Projectile.Center.DirectionTo(rightPos).RotatedBy(i * MathHelper.Pi / 6) * 32 * Scaling, DustID.GemDiamond, 4, 0.4f);

            }
            }
            player.ChangeDir(LemonUtils.Sign(dirToMouse.X, 1));
        }
        else
        {
            Vector2 velocityDir = Projectile.velocity.SafeNormalize(Vector2.Zero);
            Vector2 leftPos = Projectile.Center - velocityDir * Scaling * 32;
            Vector2 rightPos = Projectile.Center + velocityDir * Scaling * 32;
            if (AITimer % 2 == 0)
            {
                LemonUtils.DustLine(leftPos, rightPos, DustID.GemTopaz, 4, 1.5f);
                LemonUtils.DustLine(leftPos, rightPos, DustID.GemDiamond, 4, 0.7f);
            }
            releasedTimer++;
            if (releasedTimer % (int)(20 / player.GetElementalExpertiseBoostMultiplied(SpellElement.Lightning, 2)) == 0)
            {
                Projectile.Kill();
            }
        }

        AITimer++;
    }

    void Split()
    {
        
    }

    public override bool? CanHitNPC(NPC target)
    {
        if (!released) return false;
        else return null;
    }

    public override void OnKill(int timeLeft)
    {
        LemonUtils.DustCircle(Projectile.Center, 8, 8, DustID.GemTopaz, 2.5f);
        LemonUtils.DustCircle(Projectile.Center, 6, 6, DustID.GemDiamond, 2f);
        SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);

        if (Scaling >= 1)
        {
            if (LemonUtils.NotClient())
            {
                for (int i = -1; i <= 1; i++)
                {
                    Projectile.NewProjectileDirect(
                        Projectile.GetSource_FromAI(),
                        Projectile.Center,
                        Projectile.velocity.RotatedBy(i * MathHelper.Pi / 6),
                        ProjectileType<LightningSpearSplit>(),
                        Projectile.damage / 2,
                        Projectile.knockBack,
                        Projectile.owner,
                        ai1: Scaling
                        );
                }
            }
        }
        SoundEngine.PlaySound(ParacosmSFX.Thunder, Projectile.Center);

    }

    public override bool PreDraw(ref Color lightColor)
    {
        return false;
    }
}
