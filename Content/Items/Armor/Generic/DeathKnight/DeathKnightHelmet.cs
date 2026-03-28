using NeoParacosm.Content.Projectiles.Friendly.Special;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Armor.Generic.DeathKnight;

[AutoloadEquip(EquipType.Head)]
public class DeathKnightHelmet : ModItem
{
    static readonly float damageBoost = 3;
    static readonly float critChanceBoost = 3;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(damageBoost, critChanceBoost);

    public static LocalizedText setBonusText;

    public override void SetStaticDefaults()
    {
        setBonusText = this.GetLocalization("SetBonus");
    }

    public override void SetDefaults()
    {
        Item.width = 26;
        Item.height = 26;
        Item.defense = 9;
        Item.rare = ItemRarityID.Yellow;
        Item.value = Item.sellPrice(0, 3, 0, 0);
    }

    public override void UpdateEquip(Player player)
    {
        player.GetDamage(DamageClass.Generic) += damageBoost / 100f;
        player.GetCritChance(DamageClass.Generic) += critChanceBoost;
    }

    public override void UpdateInventory(Player player)
    {

    }

    public override bool IsArmorSet(Item head, Item body, Item legs)
    {
        return body.type == ItemType<DeathKnightChestplate>() && legs.type == ItemType<DeathKnightGreaves>();
    }

    public override void UpdateArmorSet(Player player)
    {
        player.setBonus = setBonusText.Value;
        player.statLifeMax2 /= 2;
        player.GetModPlayer<DeathKnightArmorPlayer>().Active = true;
    }

    public override void AddRecipes()
    {

    }
}

public class DeathKnightArmorPlayer : ModPlayer
{
    public bool Active { get; set; } = false;
    public int ReviveCount { get; set; } = 0;
    public int MaxReviveCount { get; set; } = 2;
    public int ReviveCooldownMax { get; set; } = 3 * 60 * 60;
    public int ReviveCooldown { get; set; } = 0;

    float ReviveDamageBoost1 = 10;
    float ReviveCritBoost1 = 6;
    float ReviveMeleeSpeedBoost1 = 6;
    float ReviveManaCostReduction1 = 6;
    float ReviveRangedCritChance1 = 5;
    float ReviveDRBoost1 = 10;
    int ReviveAggroBoost1 = 300;

    float ReviveDamageBoost2 = 25;
    float ReviveCritBoost2 = 12;
    float ReviveMeleeSpeedBoost2 = 12;
    float ReviveManaCostReduction2 = 18;
    float ReviveRangedCritChance2 = 12;
    float ReviveDRBoost2 = 20;
    int ReviveAggroBoost2 = 1000;

    public override void ResetEffects()
    {
        Active = false;
    }

    public override void PostUpdateEquips()
    {
        if (!Active)
        {
            return;
        }
        switch (ReviveCount)
        {
            case 1:
                Player.GetDamage(DamageClass.Generic) += ReviveDamageBoost1 / 100f;
                Player.GetCritChance(DamageClass.Generic) += ReviveCritBoost1;
                Player.GetAttackSpeed(DamageClass.Melee) += ReviveMeleeSpeedBoost1 / 100f;
                Player.GetCritChance(DamageClass.Ranged) += ReviveRangedCritChance1;
                Player.manaCost -= ReviveManaCostReduction1 / 100f;
                Player.endurance += ReviveDRBoost1 / 100f;
                Player.aggro += ReviveAggroBoost1;
                break;
            case 2:
                Player.GetDamage(DamageClass.Generic) += ReviveDamageBoost2 / 100f;
                Player.GetCritChance(DamageClass.Generic) += ReviveCritBoost2;
                Player.GetAttackSpeed(DamageClass.Melee) += ReviveMeleeSpeedBoost2 / 100f;
                Player.GetCritChance(DamageClass.Ranged) += ReviveRangedCritChance2;
                Player.manaCost -= ReviveManaCostReduction2 / 100f;
                Player.endurance += ReviveDRBoost2 / 100f;
                Player.aggro += ReviveAggroBoost2;
                break;
        }

        if (ReviveCooldown > 0)
        {
            ReviveCooldown--;
        }

        if (ReviveCooldown == 0)
        {
            if (ReviveCount > 0)
            {
                Player.AddBuff(BuffType<DeathKnightReviveCooldown>(), ReviveCooldownMax);
                ReviveCooldown = ReviveCooldownMax;
                ReviveCount--;
            }
        }
    }

    public override void UpdateEquips()
    {

    }

    public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genDust, ref PlayerDeathReason damageSource)
    {
        if (!Active)
        {
            ReviveCount = 0;
            return true;
        }

        if (ReviveCount < MaxReviveCount)
        {
            SoundEngine.PlaySound(SoundID.DD2_BetsyWindAttack with { PitchRange = (0.2f, 0.4f) }, Player.Center);
            SoundEngine.PlaySound(SoundID.DD2_DarkMageAttack with { PitchRange = (0.2f, 0.4f) }, Player.Center);
            SoundEngine.PlaySound(SoundID.DD2_BetsyFlameBreath with { PitchRange = (-0.3f, 0f), Volume = 0.6f }, Player.Center);
            LemonUtils.DustBurst(5, Player.Center, DustID.Granite, 6, 10, 2f, 3.5f, Color.Black);
            LemonUtils.DustBurst(5, Player.Center, DustID.GemDiamond, 6, 10, 2f, 3.5f);
            if (Main.myPlayer == Player.whoAmI)
            {
                for (int i = 0; i < 4; i++)
                {
                    Projectile.NewProjectileDirect(
                        Player.GetSource_FromThis(),
                        Player.Center,
                        -Vector2.UnitY.RotatedBy(Main.rand.NextFloat(-MathHelper.Pi / 4, MathHelper.Pi / 4)) * Main.rand.NextFloat(4f, 8f),
                        ProjectileType<LingeringDeathflameFriendly>(),
                        40, 2f,
                        Player.whoAmI,
                        ai0: 1, ai1: 360, ai2: Main.rand.NextFloat(1, 2));
                }
            }
            Player.Heal(Player.statLifeMax2);
            ReviveCount++;

            Player.AddBuff(BuffType<DeathKnightReviveCooldown>(), ReviveCooldownMax);
            ReviveCooldown = ReviveCooldownMax;

            playSound = false;
            genDust = false;
            return false;
        }
        ReviveCount = 0;
        return true;
    }
}

public class DeathKnightReviveCooldown : ModBuff
{
    public override void SetStaticDefaults()
    {
        Main.debuff[Type] = true;
        BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
    }
}
