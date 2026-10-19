namespace NeoParacosm.Content.Items.Accessories.Combat.Melee;

public class PocketGrease : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 40;
        Item.height = 42;
        Item.accessory = true;
        Item.value = Item.sellPrice(0, 3);
        Item.rare = ItemRarityID.Green;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<PocketGreasePlayer>().Active = true;
    }
}

public class PocketGreasePlayer : ModPlayer
{
    public bool Active { get; set; } = false;
    int currentFlask = -1;
    public override void ResetEffects()
    {
        Active = false;
    }

    public override void UpdateEquips()
    {
        if (Active
            && Player.HeldItem.DamageType == DamageClass.Melee
            && Player.HeldItem.damage > 0
            && Player.HeldItem.useStyle > ItemUseStyleID.None
            && Player.grapCount > 0
            && Player.ItemAnimationActive)
        {
            if (currentFlask == -1)
            {
                currentFlask = Main.rand.NextFromList(
                    BuffID.WeaponImbueCursedFlames,
                    BuffID.WeaponImbueFire,
                    BuffID.WeaponImbuePoison,
                    BuffID.WeaponImbueNanites,
                    BuffID.WeaponImbueGold,
                    BuffID.WeaponImbueConfetti
                    );
            }

            Player.AddBuffWithStackingTime(currentFlask, 10, 600);
        }
        else
        {
            currentFlask = -1;
        }
    }
}