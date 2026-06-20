using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria.Audio;
using Terraria.GameContent;

namespace NeoParacosm.Content.Items.Weapons.Melee;

public class DragonClawHeldProj : ModProjectile
{
    int AITimer = 0;

    ref float HitNPCWhoAmI => ref Projectile.ai[0];
    ref float Direction => ref Projectile.ai[1];
    Vector2 hitNPCOffset;

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (HitNPCWhoAmI == -1)
        {
            Projectile.timeLeft = 300;
            HitNPCWhoAmI = target.whoAmI;
            hitNPCOffset = Projectile.Center - target.Center;
            Projectile.netUpdate = true;
        }
    }

    public override void SendExtraAI(BinaryWriter writer)
    {
        writer.WriteVector2(hitNPCOffset);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        hitNPCOffset = reader.ReadVector2();
    }

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 16;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 1;
    }

    public override void SetDefaults()
    {
        Projectile.width = 42;
        Projectile.height = 42;
        Projectile.hostile = false;
        Projectile.friendly = true;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = 1;
        Projectile.stopsDealingDamageAfterPenetrateHits = true;
        Projectile.timeLeft = 120;
        Projectile.DamageType = DamageClass.Melee;
        Projectile.Opacity = 1f;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 180;
    }

    float goalRotation = 270;
    float lerpSpeed = 1 / 10f;
    float rotValue = -60;
    public override void AI()
    {
        Player player = Projectile.GetOwner();
        if (!player.IsAlive())
        {
            Projectile.Kill();
        }
        if (AITimer == 0)
        {
            HitNPCWhoAmI = -1;
        }
        //Projectile.TrackPlayerOldPos(AITimer == 0, playerOldPos);

        Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter);

        Projectile.velocity = Vector2.Zero;

        if (HitNPCWhoAmI == -1)
        {
            Projectile.timeLeft = 3;
            player.heldProj = Projectile.whoAmI;
            player.SetDummyItemTime(2);
            if (AITimer == 0)
            {
                if (Direction == 1)
                {
                    rotValue = -60;
                    goalRotation = 270;
                }
                else
                {
                    rotValue = 360;
                    goalRotation = 0;
                }
            }
            rotValue = MathHelper.Lerp(rotValue, goalRotation, lerpSpeed * player.GetAttackSpeed(DamageClass.Melee));
            if (Direction == 1 && rotValue > goalRotation - 10) Projectile.Kill();
            else if (Direction == -1 && rotValue < goalRotation + 10) Projectile.Kill();
            SetPositionRotationDirection(player, MathHelper.ToRadians(rotValue));
        }
        else
        {
            NPC npc = Main.npc[((int)HitNPCWhoAmI)];
            if (!npc.active || npc.life == 0)
            {
                Projectile.Kill();
                return;
            }
            Projectile.Center = npc.Center + hitNPCOffset * 0.75f;
            npc.GetGlobalNPC<DragonClawNPC>().Active = true;
        }

        AITimer++;
    }

    public override void OnKill(int timeLeft)
    {
        if (HitNPCWhoAmI != -1)
        {
            LemonUtils.DustBurst(8, Projectile.Center, DustID.GemDiamond, 5, 5, 2.0f, 2.5f);
            LemonUtils.DustBurst(8, Projectile.Center, DustID.Crimson, 5, 5, 2.0f, 2.5f);
            SoundEngine.PlaySound(SoundID.Tink with { PitchRange = (-0.4f, -0.2f)}, Projectile.Center);
        }
    }

    const float ThreePiOverFour = MathHelper.Pi - MathHelper.PiOver4; // dumb rotation and sprite direction stuff
    void SetPositionRotationDirection(Player player, float movedRotation = 0)
    {
        Vector2 pos = player.Center + new Vector2(-player.direction * (Projectile.width / 1f), -Projectile.height / 1f).RotatedBy(movedRotation * player.direction) * Projectile.scale;
        player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, movedRotation * player.direction + player.direction * ThreePiOverFour);
        Projectile.Center = pos;
        if (Direction == -1)
        {
            movedRotation += MathHelper.PiOver2;
        }
        Projectile.rotation = movedRotation * player.direction + MathHelper.PiOver2 * -player.direction;
        Projectile.spriteDirection = player.direction * (int)Direction;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Player player = Projectile.GetOwner();
        Vector2 drawPos = Projectile.Center - Main.screenPosition;
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        Main.EntitySpriteDraw(texture, drawPos, null, Color.White, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, LemonUtils.SpriteDirectionToSpriteEffects(Projectile.spriteDirection));

        return false;
    }

    public override void PostDraw(Color lightColor)
    {
        //Main.spriteBatch.End();
        //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
    }
}
