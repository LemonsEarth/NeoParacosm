using System.Collections.Generic;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Accessories.Master;

public class FrightOfTheScourge : ModItem
{
    public static float DamageBoost { get; private set; } = 12f;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DamageBoost);

    public override void SetDefaults()
    {
        Item.width = 48;
        Item.height = 48;
        Item.accessory = true;
        Item.value = Item.sellPrice(0, 3);
        Item.rare = ItemRarityID.Master;
        Item.master = true;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<FrightOfTheScourgePlayer>().Active = true;
    }
}

public class FrightOfTheScourgePlayer : ModPlayer
{
    public bool Active { get; set; } = false;
    int selectedClass = 0;
    DamageClass[] classes = [DamageClass.Melee, DamageClass.Ranged, DamageClass.Magic, DamageClass.Summon];
    int timer = 0;

    public override void SetStaticDefaults()
    {

    }

    public override void ResetEffects()
    {
        Active = false;
    }

    public override void UpdateEquips()
    {
        if (!Active)
        {
            return;
        }

        if (timer >= 600)
        {
            selectedClass++;
            if (selectedClass > 3)
            {
                selectedClass = 0;
            }
            timer = 0;
        }

        int dustType = DustID.SolarFlare;
        switch (selectedClass)
        {
            case 0:
                dustType = DustID.SolarFlare;
                break;
            case 1:
                dustType = DustID.GemEmerald;
                break;
            case 2:
                dustType = DustID.GemAmethyst;
                break;
            case 3:
                dustType = DustID.BlueFlare;
                break;
        }

        Dust.NewDustPerfect(Player.RandomPos(), dustType, -Vector2.UnitY * Main.rand.NextFloat(1, 3), Scale: Main.rand.NextFloat(0.5f, 1)).noGravity = true;

        Player.GetDamage(classes[selectedClass]) += FrightOfTheScourge.DamageBoost / 100f;

        timer++;
    }
}

public class FrightOfTheScourgeDropNPC : GlobalNPC
{
    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return entity.type == NPCID.SkeletronPrime;
    }

    public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.ByCondition(Condition.InMasterMode.ToDropCondition(ShowItemDropInUI.Always), ItemType<NightOfTheWatcher>()));
    }
}
