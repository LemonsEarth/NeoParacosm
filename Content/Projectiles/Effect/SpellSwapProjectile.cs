using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Core.Players;
using Terraria.GameContent;

namespace NeoParacosm.Content.Projectiles.Effect;

public class SpellSwapProjectile : ModProjectile
{
    ref float AITimer => ref Projectile.ai[0];

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
        Projectile.friendly = false;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 90;
    }

    public override void AI()
    {
        Player player = Projectile.GetOwner();
        Projectile.Center = player.Center;
        Projectile.velocity = Vector2.Zero;
        if (AITimer == 0)
        {
            foreach (var proj in Main.ActiveProjectiles)
            {
                if (proj.whoAmI != Projectile.whoAmI && proj.type == Projectile.type)
                {
                    proj.Kill();
                }
            }
        }

        if (Projectile.timeLeft <= 60)
        {
            Projectile.Opacity = MathHelper.Lerp(0, 1, Projectile.timeLeft / 60f);
        }
        else if (Projectile.timeLeft >= 85)
        {
            Projectile.Opacity = MathHelper.Lerp(0, 1, AITimer / 5f);
        }
        Projectile.velocity = -Vector2.UnitY * 2;
        AITimer++;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Player player = Projectile.GetOwner();
        NPCatalystPlayer cp = player.NPCatalystPlayer();
        if (cp.EquippedSpells.Count == 0)
        {
            return false;
        }
        Color color = Color.White * Projectile.Opacity;
        Vector2 centerPos = player.Top + new Vector2(0, -64);
        Vector2 leftPos = player.Top + new Vector2(-64, -48);
        Vector2 rightPos = player.Top + new Vector2(+64, -48);
        int centerSpellID = cp.SelectedSpell.Type;
        Texture2D centerSpellTexture = TextureAssets.Item[centerSpellID].Value;
        LemonUtils.DrawGlow(centerPos, Color.White, Projectile.Opacity, 2f);
        Main.EntitySpriteDraw(centerSpellTexture,
            centerPos - Main.screenPosition,
            null,
            color,
            0,
            centerSpellTexture.Size() * 0.5f,
            1.5f,
            SpriteEffects.None);

        if (cp.EquippedSpells.Count == 1)
        {
            return false;
        }

        int rightSpellIndex = cp.SelectedSpellIndex + 1;
        if (rightSpellIndex >= cp.EquippedSpells.Count)
        {
            rightSpellIndex = 0;
        }
        else if (rightSpellIndex < 0)
        {
            rightSpellIndex = cp.EquippedSpells.Count - 1;
        }
        int rightSpellID = cp.EquippedSpells[rightSpellIndex].Type;
        Texture2D rightSpellTexture = TextureAssets.Item[rightSpellID].Value;
        LemonUtils.DrawGlow(rightPos, Color.White, Projectile.Opacity, 1.5f);
        Main.EntitySpriteDraw(rightSpellTexture,
            rightPos - Main.screenPosition,
            null,
            color,
            0,
            rightSpellTexture.Size() * 0.5f,
            1f,
            SpriteEffects.None);


        int leftSpellIndex = cp.SelectedSpellIndex - 1;
        if (leftSpellIndex >= cp.EquippedSpells.Count)
        {
            leftSpellIndex = 0;
        }
        else if (leftSpellIndex < 0)
        {
            leftSpellIndex = cp.EquippedSpells.Count - 1;
        }
        int leftSpellID = cp.EquippedSpells[leftSpellIndex].Type;
        Texture2D leftSpellTexture = TextureAssets.Item[leftSpellID].Value;
        LemonUtils.DrawGlow(leftPos, Color.White, Projectile.Opacity, 1.5f);
        Main.EntitySpriteDraw(leftSpellTexture,
            leftPos - Main.screenPosition,
            null,
            color,
            0,
            leftSpellTexture.Size() * 0.5f,
            1f,
            SpriteEffects.None);
        return false;
    }
}
