using Terraria.DataStructures;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Combat;

public class RuneOfExtinction : ModItem
{
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs();

    public override void SetStaticDefaults()
    {
        Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(6, 9, true));
        ItemID.Sets.AnimatesAsSoul[Type] = true;
    }

    public override void SetDefaults()
    {
        Item.width = 54;
        Item.height = 64;
        Item.accessory = true;
        Item.value = Item.buyPrice(0, 3, 0, 0);
        Item.rare = ItemRarityID.Expert;
        Item.expert = true;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<RuneOfExtinctionPlayer>().runeOfExtinction = true;
    }
}

public class RuneOfExtinctionPlayer : ModPlayer
{
    public bool runeOfExtinction { get; set; } = false;
    int runeOfExtinctionTimer = 0;

    public override void ResetEffects()
    {
        runeOfExtinction = false;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (runeOfExtinction)
        {
            if (target.life <= 0 && !target.friendly && !target.SpawnedFromStatue)
            {
                if (runeOfExtinctionTimer <= 0)
                {
                    Player.Heal(20);
                    runeOfExtinctionTimer = 15;
                }

                NPC.killCount[Item.NPCtoBanner(target.type)] += 1;
            }
        }
    }

    public override void PostUpdateEquips()
    {
        if (runeOfExtinction && runeOfExtinctionTimer > 0)
        {
            runeOfExtinctionTimer--;
        }
    }
}
