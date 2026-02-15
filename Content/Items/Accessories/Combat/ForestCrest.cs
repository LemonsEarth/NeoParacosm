using NeoParacosm.Content.Items.Pickups;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat;

public class ForestCrest : ModItem
{
    static readonly int sentryBoost = 1;
    static readonly float moveSpeedBoost = 5;

    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(sentryBoost, moveSpeedBoost);

    public override void SetDefaults()
    {
        Item.width = 28;
        Item.height = 28;
        Item.accessory = true;
        Item.value = Item.buyPrice(0, 1);
        Item.rare = ItemRarityID.Green;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.maxTurrets += sentryBoost;
        player.moveSpeed += moveSpeedBoost / 100;
        player.GetModPlayer<ForestCrestPlayer>().forestCrest = true;
    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemID.Wood, 20);
        recipe.AddIngredient(ItemID.Daybloom, 3);
        recipe.AddIngredient(ItemID.Sunflower, 1);
        recipe.AddTile(TileID.WorkBenches);
        recipe.Register();
    }
}

public class ForestCrestPlayer : ModPlayer
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
                Item item = Player.QuickSpawnItemDirect(Player.GetSource_OnHit(victim, "Forest Crest Hit"), ItemType<SmallFlowerPickup>(), 1);
                item.position = new Vector2(x, y);
                NetMessage.SendData(MessageID.SyncItem, number: item.whoAmI, number2: 0);
                forestCrestPickupCooldown = 30;
            }
        }
    }
}

public class ForestCrestBuff : ModBuff
{
    readonly float speedBoost = 10f;
    readonly float damageBoost = 10f;
    public override LocalizedText Description => base.Description.WithFormatArgs(speedBoost, damageBoost);

    public override void SetStaticDefaults()
    {
        Main.debuff[Type] = false;
    }

    public override void Update(Player player, ref int buffIndex)
    {
        player.moveSpeed += 10f / 100f;
        player.GetDamage(DamageClass.Summon) += 10f / 100f;
    }
}
