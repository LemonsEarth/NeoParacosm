using NeoParacosm.Content.Items.Weapons.Magic.Spells;
using NeoParacosm.Content.Projectiles.Friendly.Magic;
using NeoParacosm.Core.Players;
using NeoParacosm.Core.Systems.Assets;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Weapons.Magic;

public class Candlestick : BaseCatalyst
{
    static readonly float fireDamageBoost = 15;
    static readonly float fireSpeedBoost = 10;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(fireDamageBoost, fireSpeedBoost);

    public override void SetStaticDefaults()
    {
        Item.staff[Type] = true;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 15;
        Item.width = 50;
        Item.height = 50;
        Item.useTime = 20;
        Item.useAnimation = 20;
        Item.knockBack = 3;
        Item.value = Item.buyPrice(gold: 3);
        Item.rare = ItemRarityID.Green;
        Item.autoReuse = true;
        Item.mana = 12;
    }

    public override void HoldItem(Player player)
    {
        player.NPCatalystPlayer().ElementalDamageBoosts[BaseSpell.SpellElement.Fire] += fireDamageBoost / 100f;
        player.NPCatalystPlayer().ElementalSpeedBoosts[BaseSpell.SpellElement.Fire] += fireSpeedBoost / 100f;
    }
}