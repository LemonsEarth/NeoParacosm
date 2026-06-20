using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Common.Utils.Prim;
using NeoParacosm.Content.Dusts;
using NeoParacosm.Content.Projectiles;
using NeoParacosm.Content.Projectiles.Friendly.Special;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.GameContent;

namespace NeoParacosm.Content.Items.Weapons.Melee;

public class CurseflameGreatswordHeldProj : PrimProjectile
{
    int AITimer = 0;

    ref float special => ref Projectile.ai[0];
    ref float direction => ref Projectile.ai[1];
    List<Vector2> playerOldPos = new List<Vector2>();

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.AddBuff(BuffID.CursedInferno, 120);
    }

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 16;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 1;
    }

    public override void SetDefaults()
    {
        Projectile.width = 76;
        Projectile.height = 76;
        Projectile.hostile = false;
        Projectile.friendly = true;
        Projectile.ignoreWater = false;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 120;
        Projectile.DamageType = DamageClass.Melee;
        Projectile.Opacity = 1f;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 180;
    }

    float goalRotation = 270;
    float lerpSpeed = 1 / 30f;
    float rotValue = -60;
    public override void AI()
    {
        Player player = Projectile.GetOwner();
        if (!player.IsAlive())
        {
            Projectile.Kill();
        }

        Projectile.TrackPlayerOldPos(AITimer == 0, playerOldPos);

        Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter);
        player.heldProj = Projectile.whoAmI;
        player.SetDummyItemTime(2);

        Projectile.velocity = Vector2.Zero;
        if (AITimer % 5 == 0)
        {
            Dust.NewDustPerfect(Projectile.RandomPos(-16, -16), DustType<FireDust>(), Vector2.Zero, newColor: Color.Lime, Scale: Main.rand.NextFloat(0.5f, 1f));
        }
        if (special != 0)
        {
            player.StopExtraJumpInProgress();
            if (player.IsGrounded())
            {
                SoundEngine.PlaySound(SoundID.Item62, playerCenter);
                if (Main.myPlayer == Projectile.owner)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        LemonUtils.QuickProj(Projectile, player.Center, Vector2.UnitY.RotatedByRandom(6.28f) * 5, ProjectileID.CursedFlameFriendly, Projectile.damage / 2);
                    }

                    for (int i = -2; i <= 2; i++)
                    {
                        LemonUtils.QuickProj(Projectile, player.Center + Vector2.UnitX * 48 * i, Vector2.UnitY * 2, ProjectileType<LingeringCursedFlameFriendly>(), Projectile.damage / 2, ai0: 1, ai1: 90 + AITimer * 2, ai2: 1 + Math.Abs(i) / 3f);
                    }
                }
                for (int i = -4; i <= 4; i++)
                {
                    Dust.NewDustPerfect(player.Center + Vector2.UnitX * Main.rand.NextFloat(20, 28) * i, DustID.GemDiamond, -Vector2.UnitY * Main.rand.NextFloat(6, 10), Scale: Main.rand.NextFloat(2f, 4f)).noGravity = true;
                    Dust.NewDustPerfect(player.Center + Vector2.UnitX * Main.rand.NextFloat(20, 28) * i, DustID.GemEmerald, -Vector2.UnitY * Main.rand.NextFloat(6, 10), Scale: Main.rand.NextFloat(2f, 4f)).noGravity = true;
                }
                Projectile.Kill();
            }
            player.NPPlayer().FastFall = true;
            Projectile.Center = player.Center + Vector2.UnitY * (Projectile.height / 2);
            Projectile.spriteDirection = player.direction;
            Projectile.rotation = player.direction * ThreePiOverFour;

            AITimer++;
            return;
        }

        Projectile.timeLeft = 3;
        if (AITimer == 0)
        {
            if (direction == 1)
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
        Projectile.extraUpdates = 3;
        rotValue = MathHelper.Lerp(rotValue, goalRotation, lerpSpeed * player.GetAttackSpeed(DamageClass.Melee));
        if (direction == 1 && rotValue > goalRotation - 10) Projectile.Kill();
        else if (direction == -1 && rotValue < goalRotation + 10) Projectile.Kill();
        SetPositionRotationDirection(player, MathHelper.ToRadians(rotValue));
        AITimer++;
    }

    public override void OnKill(int timeLeft)
    {

    }

    const float ThreePiOverFour = MathHelper.Pi - MathHelper.PiOver4; // dumb rotation and sprite direction stuff
    void SetPositionRotationDirection(Player player, float movedRotation = 0)
    {
        Vector2 pos = player.Center + new Vector2(-player.direction * (Projectile.width / 2), -Projectile.height / 2).RotatedBy(movedRotation * player.direction) * Projectile.scale;
        player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, movedRotation * player.direction + player.direction * ThreePiOverFour);
        Projectile.Center = pos;
        if (direction == -1)
        {
            movedRotation += MathHelper.PiOver2;
        }
        Projectile.rotation = movedRotation * player.direction + MathHelper.PiOver2 * -player.direction;
        Projectile.spriteDirection = player.direction * (int)direction;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Player player = Projectile.GetOwner();
        Vector2 drawPos = Projectile.Center - Main.screenPosition;
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        float topRotOffset = player.direction == 1 ? -MathHelper.PiOver4 : -3 * MathHelper.PiOver4;
        float botRotOffset = player.direction == 1 ? 3 * MathHelper.PiOver4 : MathHelper.PiOver4;
        if (direction == -1)
        {
            topRotOffset += MathHelper.PiOver2 * player.direction;
            botRotOffset += MathHelper.PiOver2 * player.direction;
        }

        if (special == 0)
        {
            Main.spriteBatch.End(); // Restarting spritebatch around Primitive Drawing to fix some layering issues
            PrimHelper.DrawHeldProjectilePrimTrailRectangular(Projectile, Color.Lime, Color.Transparent, BasicEffect, playerOldPos, topRotOffset, botRotOffset, (int)(Projectile.height * 0.75f), (int)(Projectile.height * 0.75f));
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        }

        for (int k = Projectile.oldPos.Length - 1; k > 0; k--)
        {
            Vector2 afterimagePos = Projectile.oldPos[k] + texture.Size() * 0.5f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
            Color color = Color.White * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length) * 0.2f;
            Main.EntitySpriteDraw(texture, afterimagePos, null, color, Projectile.oldRot[k], texture.Size() * 0.5f, Projectile.scale, LemonUtils.SpriteDirectionToSpriteEffects(Projectile.oldSpriteDirection[k]), 0);
        }
        Main.EntitySpriteDraw(texture, drawPos, null, Color.White, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, LemonUtils.SpriteDirectionToSpriteEffects(Projectile.spriteDirection));

        return false;
    }

    public override void PostDraw(Color lightColor)
    {
        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
    }
}
