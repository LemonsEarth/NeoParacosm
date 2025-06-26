using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;

namespace NeoParacosm.Content.Projectiles.Friendly.Melee;

public class SawProjectile : ModProjectile
{
    ref float AITimer => ref Projectile.ai[0];
    public bool Released
    {
        get
        {
            return Projectile.ai[1] == 1;
        }
        set
        {
            Projectile.ai[1] = value == true ? 1 : 0;
        }
    }

    ref float ParentIndex => ref Projectile.ai[2];

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        Main.projFrames[Type] = 1;
    }

    public override void SetDefaults()
    {
        Projectile.width = 64;
        Projectile.height = 64;
        Projectile.hostile = false;
        Projectile.friendly = true;
        Projectile.ignoreWater = false;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 300;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 10;
    }

    public override void AI()
    {
        if (AITimer == 0)
        {
            Projectile.scale = 0.2f;
        }
        Projectile parent = Main.projectile[(int)ParentIndex];

        if (Released && AITimer < 30) Projectile.Kill();

        if (!Main.player[Projectile.owner].channel && !Released)
        {
            Released = true;
            if (Main.myPlayer == Projectile.owner)
            {
                Projectile.velocity = Main.player[Projectile.owner].Center.DirectionTo(Main.MouseWorld) * 20f;
            }
            Projectile.penetrate = 7;
            Projectile.netUpdate = true;
        }
        if (!Released)
        {
            Projectile.velocity = Vector2.UnitX.RotatedByRandom(6.28f) * 1.5f;
            if (Main.myPlayer == Projectile.owner)
            {
                Projectile.Center = parent.Center + Main.player[Projectile.owner].Center.DirectionTo(Main.MouseWorld) * 32;
            }
            if (AITimer < 60)
            {
                Projectile.scale = MathHelper.Lerp(0f, 1f, AITimer / 60f);
                Projectile.damage = (int)MathHelper.Lerp(1, Projectile.originalDamage, AITimer / 60f);
            }
            Projectile.timeLeft = 300;
        }

        var dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch);
        dust.noGravity = true;
        Projectile.rotation = MathHelper.ToRadians(AITimer * 36);
        AITimer++;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Type].Value;

        Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
        for (int k = Projectile.oldPos.Length - 1; k >= 0; k--)
        {
            Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
            Color color = Color.Black;
            if (k == 0)
            {
                color = Projectile.GetAlpha(lightColor);
            }
            Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
        }

        return false;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {

    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {

    }
}
