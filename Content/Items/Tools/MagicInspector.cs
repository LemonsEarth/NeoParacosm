using NeoParacosm.Content.Items.Weapons.Magic.Spells;

namespace NeoParacosm.Content.Items.Tools;

public class MagicInspector : ModItem
{
    public override void SetStaticDefaults()
    {

    }

    public override void SetDefaults()
    {
        Item.width = 68;
        Item.height = 68;
        Item.value = Item.sellPrice(0, 1, 0, 0);
        Item.useTime = 30;
        Item.useAnimation = 30;
        Item.useStyle = ItemUseStyleID.HoldUp;
        Item.UseSound = SoundID.Item29;
        Item.rare = ItemRarityID.Blue;
    }

    public override bool? UseItem(Player player)
    {
        if (player.whoAmI == Main.myPlayer)
        {
            if (player.altFunctionUse != 2)
            {
                Main.NewText("Elemental Damage Boosts:");
                foreach (SpellElement element in Enum.GetValues<SpellElement>())
                {
                    Main.NewText($"{element}: {(int)((player.GetElementalDamageBoost(element) - 1) * 100)}%");
                }
            }
            else
            {
                Main.NewText("Elemental Expertise Boosts:");
                foreach (SpellElement element in Enum.GetValues<SpellElement>())
                {
                    Main.NewText($"{element}: {(int)((player.GetElementalExpertiseBoost(element) - 1) * 100)}%");
                }
            }
        }
        return true;
    }

    public override bool AltFunctionUse(Player player)
    {
        return true;
    }
}
