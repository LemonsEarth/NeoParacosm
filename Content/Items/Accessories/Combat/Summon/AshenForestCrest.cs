using NeoParacosm.Content.Items.Pickups;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat.Summon;

public class AshenForestCrest : ModItem
{
    static readonly int sentryBoost = 2;
    static readonly float moveSpeedBoost = 10;

    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(sentryBoost, moveSpeedBoost);

    public override void SetDefaults()
    {
        Item.width = 28;
        Item.height = 28;
        Item.accessory = true;
        Item.value = Item.sellPrice(0, 2);
        Item.rare = ItemRarityID.Green;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.maxTurrets += sentryBoost;
        player.moveSpeed += moveSpeedBoost / 100;
        player.GetModPlayer<AshenForestCrestPlayer>().forestCrest = true;
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemType<ForestCrest>(), 1);
        recipe.AddIngredient(ItemID.Ruby, 3);
        recipe.AddIngredient(ItemID.Fireblossom, 8);
        recipe.AddIngredient(ItemID.AshBlock, 50);
        recipe.AddIngredient(ItemID.HellstoneBrick, 10);
        recipe.AddTile(TileID.WorkBenches);
        recipe.Register();
    }
}

public class AshenForestCrestPlayer : ModPlayer
{
    public bool forestCrest { get; set; } = false;
    int forestCrestPickupCooldown = 0;

    public override void ResetEffects()
    {
        forestCrest = false;
    }

    public override void PostUpdateEquips()
    {
        if (forestCrestPickupCooldown > 0) forestCrestPickupCooldown--;
    }

    public override void OnHitAnything(float x, float y, Entity victim)
    {
        if (victim is NPC npc && forestCrest)
        {
            if (Main.rand.NextBool(10) && forestCrestPickupCooldown == 0)
            {
                Item item = Player.QuickSpawnItemDirect(Player.GetSource_OnHit(victim, "Ashen Forest Crest Hit"), ItemType<AshenSmallFlowerPickup>(), 1);
                item.position = new Vector2(x, y);
                NetMessage.SendData(MessageID.SyncItem, number: item.whoAmI, number2: 0);
                forestCrestPickupCooldown = 30;
            }
        }
    }
}

public class AshenForestCrestBuff : ModBuff
{
    readonly float speedBoost = 15f;
    readonly float damageBoost = 12f;
    public override LocalizedText Description => base.Description.WithFormatArgs(speedBoost, damageBoost);

    public override void SetStaticDefaults()
    {
        Main.debuff[Type] = false;
    }

    public override void Update(Player player, ref int buffIndex)
    {
        player.moveSpeed += speedBoost / 100f;
        player.GetDamage(DamageClass.Summon) += damageBoost / 100f;
        player.maxTurrets += 2;
    }
}
