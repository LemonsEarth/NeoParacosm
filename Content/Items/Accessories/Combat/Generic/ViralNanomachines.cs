using NeoParacosm.Content.Items.Accessories.Combat.Ranged;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat.Generic;

public class ViralNanomachines : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 64;
        Item.height = 92;
        Item.accessory = true;
        Item.value = Item.buyPrice(0, 5);
        Item.rare = ItemRarityID.Pink;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<ViralNanomachinesPlayer>().Active = true;

    }

    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddIngredient(ItemID.Stinger, 6);
        recipe.AddIngredient(ItemID.SpiderFang, 8);
        recipe.AddIngredient(ItemID.HallowedBar, 10);
        recipe.AddTile(TileID.MythrilAnvil);
        recipe.Register();
    }
}

public class ViralNanomachinesPlayer : ModPlayer
{
    public bool Active { get; set; } = false;
    public override void ResetEffects()
    {
        Active = false;
    }

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        if (target.HasBuff(BuffID.Poisoned))
        {
            modifiers.CritDamage += 300f / 100f;
            target.RequestBuffRemoval(BuffID.Poisoned);
        }

        if (target.HasBuff(BuffID.Venom))
        {
            modifiers.CritDamage += 150f / 100f;
            target.RequestBuffRemoval(BuffID.Venom);
        }
    }
}
