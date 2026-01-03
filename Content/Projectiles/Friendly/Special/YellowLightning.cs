namespace NeoParacosm.Content.Projectiles.Friendly.Special;

public class YellowLightning : TargetedLightning
{
    protected override Color ShineColor => Color.White;
    protected override Color DarkColor => Color.Yellow;
    protected override float HorizontalOffsetMin => 20;
    protected override float HorizontalOffsetMax => 36;
    protected override float BaseSpacingDenominator => 10;
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