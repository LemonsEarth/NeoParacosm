using NeoParacosm.Content.Buffs.GoodBuffs;
using NeoParacosm.Content.Projectiles.Friendly.Magic;
using Terraria.Audio;
using Terraria.WorldBuilding;

namespace NeoParacosm.Content.Items.Weapons.Magic.Spells.Fire;

public class FireblinkSpell : BaseSpell
{
    public override int AttackCooldown => 60;
    public override int ManaCost => 30;
    public override Vector2 GetTargetVector(Player player) => Main.MouseWorld;

    public override bool CanUseItem(Player player)
    {
        return !player.HasBuff(BuffType<FireblinkDebuff>());
    }

    public override void SpellAction(Player player)
    {
        Vector2 dashPos = Main.MouseWorld;
        float distanceLimit = 300 * player.GetElementalExpertiseBoostMultiplied(SpellElement.Fire, 1.5f);
        if (player.Center.DistanceSQ(Main.MouseWorld) > distanceLimit * distanceLimit)
        {
            Vector2 toMouse = player.Center.DirectionTo(Main.MouseWorld);
            dashPos = player.Center + toMouse * distanceLimit;
        }
        int duration = (int)(15 * player.GetElementalExpertiseBoostMultiplied(SpellElement.Fire, 2f));
        player.velocity = Vector2.Zero;
        FireblinkPlayer fbPlayer = player.GetModPlayer<FireblinkPlayer>();
        fbPlayer.SavedStartPos = player.Center;
        fbPlayer.SavedMousePos = dashPos;
        fbPlayer.Duration = duration;
        player.AddBuff(BuffType<FireblinkDebuff>(), duration);
        SoundEngine.PlaySound(SoundID.DD2_EtherianPortalDryadTouch with { PitchRange = (0f, 0.1f) }, player.Center);
        for (int j = 0; j < 6; j++)
        {
            Vector2 randVector = new Vector2(Main.rand.NextFloat(-8, 8), Main.rand.NextFloat(-8, 2));
            Vector2 randVector2 = new Vector2(Main.rand.NextFloat(-8, 8), Main.rand.NextFloat(-8, 2));
            Dust.NewDustDirect(player.RandomPos(), 2, 2, DustID.OrangeStainedGlass, randVector.X, randVector.Y, Scale: Main.rand.NextFloat(3.5f, 4.4f)).noGravity = true;
            Dust.NewDustDirect(player.RandomPos(), 2, 2, DustID.GemRuby, randVector2.X, randVector2.Y, Scale: Main.rand.NextFloat(3.5f, 4.4f)).noGravity = true;
        }
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 40;
        Item.width = 40;
        Item.height = 38;
        Item.value = Item.buyPrice(gold: 3);
        Item.rare = ItemRarityID.Green;
        SpellElements = [SpellElement.Fire];
    }
}

public class FireblinkDebuff : ModBuff
{
    public override void SetStaticDefaults()
    {
        Main.debuff[Type] = true;
        Main.buffNoSave[Type] = true;
        BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
    }

    public override void Update(Player player, ref int buffIndex)
    {
        player.controlLeft = false;
        player.controlRight = false;
        player.controlUp = false;
        player.controlDown = false;
        player.controlJump = false;
        player.controlHook = false;
        player.controlMount = false;
        player.controlUseItem = false;
        player.controlUseTile = false;
        LemonUtils.DustBurst(2, player.Center, DustID.Torch, 5, 5, 1.5f, 3f);
        LemonUtils.DustBurst(2, player.Center, DustID.GemRuby, 5, 5, 1.5f, 3f);
        //LemonUtils.DustBurst(2, player.Center, DustID.GemAmber, 5, 5, 1.5f, 3f);
    }
}

public class FireblinkPlayer : ModPlayer
{
    public Vector2 SavedStartPos { get; set; } = Vector2.Zero;
    public Vector2 SavedMousePos { get; set; } = Vector2.Zero;
    public int Duration { get; set; } = 0;
    int dashTimer = 0;

    public override void ResetEffects()
    {

    }

    public override void PostUpdateBuffs()
    {
        if (Player.HasBuff(BuffType<FireblinkDebuff>()))
        {
            if (SavedStartPos != Vector2.Zero && SavedMousePos != Vector2.Zero && Duration > 0)
            {
                Player.velocity = SavedStartPos.DirectionTo(SavedMousePos) * (SavedStartPos.Distance(SavedMousePos) / Duration);
                //Player.Center = Vector2.Lerp(SavedStartPos, SavedMousePos, dashTimer / (float)Duration);
                dashTimer++;
            }

        }
        else
        {
            Duration = 0;
            dashTimer = 0;
            SavedStartPos = Vector2.Zero;
            SavedMousePos = Vector2.Zero;
        }
    }

    public override void ModifyHurt(ref Player.HurtModifiers modifiers)
    {
        if (Player.HasBuff(BuffType<FireblinkDebuff>()) && modifiers.DamageSource.TryGetCausingEntity(out Entity entity) && entity is NPC)
        {
            modifiers.FinalDamage *= 0.75f;
        }
    }

    public override void OnHurt(Player.HurtInfo info)
    {
        if (Player.HasBuff(BuffType<FireblinkDebuff>()))
        {
            Projectile.NewProjectileDirect(
                Player.GetSource_OnHurt(info.DamageSource),
                Player.Center,
                Vector2.Zero,
                ProjectileType<GreatFireballExplosion>(),
                (int)(Player.GetTotalDamage(DamageClass.Magic).ApplyTo(40) * Player.GetElementalDamageBoost(SpellElement.Fire)),
                12f
                );
        }
    }
}