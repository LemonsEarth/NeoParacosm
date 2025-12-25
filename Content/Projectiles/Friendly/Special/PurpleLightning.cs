using Microsoft.Xna.Framework.Graphics;
using NeoParacosm.Core.Systems.Assets;
using System.Collections.Generic;
using System.Linq;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.CameraModifiers;

namespace NeoParacosm.Content.Projectiles.Friendly.Special;

public class PurpleLightning : TargetedLightning
{
    protected override Color ShineColor => Color.White;
    protected override Color DarkColor => Color.DarkBlue;
    protected override float HorizontalOffsetMin => 10;
    protected override float HorizontalOffsetMax => 16;
    protected override float BaseSpacingDenominator => 20;

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.DrawScreenCheckFluff[Type] = 6000;
    }

    public override void SetDefaults()
    {
        Projectile.width = 64;
        Projectile.height = 64;
        Projectile.friendly = true;
        Projectile.hostile = false;
        Projectile.timeLeft = 30;
        Projectile.penetrate = -1;
        Projectile.Opacity = 1f;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 30;
        Projectile.tileCollide = false;
    }
}