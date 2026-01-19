using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Common.Utils.Prim;
using NeoParacosm.Content.Items.Weapons.Magic.Spells;
using Terraria.Audio;
using Terraria.GameContent;

namespace NeoParacosm.Content.Projectiles.Friendly.Magic;

public class DarkSpear : PrimProjectile
{
    ref float AITimer => ref Projectile.ai[0];
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
        Projectile.penetrate = 3;
        Projectile.Opacity = 1f;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 30;
        Projectile.tileCollide = false;
        Projectile.extraUpdates = 2;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {

    }

    public override void AI()
    {
        Player player = Main.player[Projectile.owner];
        if (AITimer == 0)
        {
            SoundEngine.PlaySound(SoundID.DD2_EtherianPortalSpawnEnemy, Projectile.Center);
            savedVelocity = Projectile.velocity;
            Projectile.velocity = Vector2.Zero;
        }

        if (!player.Alive() && !released)
        {
            Projectile.Kill();
            return;
        }
        int baseTimeToFire = 540;
        float darkSpeedBoost = player.NPCatalystPlayer().ElementalExpertiseBoosts[BaseSpell.SpellElement.Dark];
        int minTimeToFire = 180;
        int timeAdjusted = Math.Max((int)(baseTimeToFire - (baseTimeToFire * (darkSpeedBoost - 1))), minTimeToFire);
        if ((!player.channel || AITimer >= timeAdjusted) && !released)
        {
            float scaling = Math.Clamp(AITimer, timeAdjusted / 4f, timeAdjusted) / (float)timeAdjusted;
            Vector2 toMouse = player.DirectionTo(Main.MouseWorld);
            float boost = 1;
            if (scaling > 0.99f)
            {
                Vector2 dustStartPos = player.Center - toMouse * 1200;
                Vector2 dustEndPos = player.Center + toMouse * 1200;
                LemonUtils.DustLine(dustStartPos, dustEndPos, DustID.Granite, 8, 3f, Color.Black);
                Projectile.penetrate = 15;
                boost = 2;
                Projectile.Center = dustStartPos;
            }
            Projectile.damage = (int)(Projectile.damage * (1 + scaling) * boost);
            Projectile.velocity = toMouse * savedVelocity.Length() * scaling * boost;
            released = true;
            SoundEngine.PlaySound(SoundID.DD2_EtherianPortalDryadTouch, Projectile.Center);
        }

        if (!released)
        {
            Projectile.velocity = Vector2.Zero;
            Projectile.Center = player.Center;
            player.SetDummyItemTime(player.NPCatalystPlayer().SelectedSpell.AttackCooldown);
            if (AITimer % 180 == 0)
            {
                SoundEngine.PlaySound(SoundID.DD2_EtherianPortalSpawnEnemy, Projectile.Center);
            }
            if (Main.myPlayer == player.whoAmI)
            {
                float lengthPercent = AITimer / timeAdjusted;
                float maxDistance = 64;
                Vector2 direction = player.DirectionTo(Main.MouseWorld);
                Vector2 dustStartPos = player.Center + -direction * (lengthPercent * maxDistance);
                Vector2 dustEndPos = player.Center + direction * (lengthPercent * maxDistance);
                float distance = dustStartPos.Distance(dustEndPos);
                for (int i = 0; i < 8; i++)
                {
                    Dust.NewDustDirect(dustStartPos + i * direction * (distance / 8f), 2, 2, DustID.Granite, newColor: Color.Black).noGravity = true;
                }
            }
        }
        else
        {
            Dust.NewDustDirect(Projectile.RandomPos(), 2, 2, DustID.Granite, newColor: Color.Black, Scale: 2f).noGravity = true;
        }
        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        AITimer++;
    }

    public override bool? CanHitNPC(NPC target)
    {
        if (!released) return false;
        else return null;
    }

    public override void OnKill(int timeLeft)
    {

    }

    public override bool PreDraw(ref Color lightColor)
    {
        if (!released) return false;

        if (Main.dedServ) return true;
        PrimHelper.DrawBasicProjectilePrimTrailRectangular(Projectile, Color.Black, Color.Transparent, BasicEffect);

        Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

        Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
        for (int k = Projectile.oldPos.Length - 1; k > 0; k--)
        {
            Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
            Color color = Projectile.GetAlpha(lightColor) * (((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length) + 0.3f);
            if (k < 3)
            {
                color = Color.White;
            }
            Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
        }
        return true;
    }
}
