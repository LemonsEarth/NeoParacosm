using NeoParacosm.Content.Items.Materials;
using NeoParacosm.Content.Items.Weapons.Magic.Spells;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat.Magic;

public class MysticCrystal : ModItem
{
    readonly float damageBoost = 10f;
    readonly float pureDamageBoost = 15f;
    readonly float pureExpertiseBoost = 15f;
    readonly float iceDamageBoost = 15f;
    readonly float iceExpertiseBoost = 15f;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(damageBoost, pureDamageBoost, pureExpertiseBoost, iceDamageBoost, iceExpertiseBoost);
    public override void SetDefaults()
    {
        Item.width = 52;
        Item.height = 70;
        Item.accessory = true;
        Item.value = Item.sellPrice(0, 5);
        Item.rare = ItemRarityID.LightRed;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetDamage(DamageClass.Magic) += damageBoost / 100f;
        player.AddElementalDamageBoost(SpellElement.Pure, pureDamageBoost / 100f);
        player.AddElementalDamageBoost(SpellElement.Ice, iceDamageBoost / 100f);
        player.AddElementalExpertiseBoost(SpellElement.Pure, pureExpertiseBoost / 100f);
        player.AddElementalExpertiseBoost(SpellElement.Ice, iceExpertiseBoost / 100f);
        player.GetModPlayer<MysticCrystalPlayer>().Active = true;
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemType<CondensedCrystal>())
            .AddIngredient(ItemType<FrigidFossil>(), 4)
            .AddIngredient(ItemID.FrostCore, 1)
            .AddIngredient(ItemID.CrystalShard, 15)
            .AddIngredient(ItemID.SoulofLight, 4)
            .AddIngredient(ItemID.SoulofNight, 4)
            .AddTile(TileID.CrystalBall)
            .Register();
    }
}

public class MysticCrystalPlayer : ModPlayer
{
    public bool Active { get; set; } = false;
    public override void ResetEffects()
    {
        Active = false;
    }

    public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
    {
        if (Active && proj.CountsAsClass(DamageClass.Magic))
        {
            if ((int)Player.GetCritChance(DamageClass.Magic) > 0 && Main.rand.NextBool(100 / (int)Player.GetCritChance(DamageClass.Magic))) // if crit
            {
                Player.AddBuff(BuffID.ManaRegeneration, 600);
                Player.AddBuff(BuffID.MagicPower, 600);
            }
            modifiers.DisableCrit();
        }
    }
}
