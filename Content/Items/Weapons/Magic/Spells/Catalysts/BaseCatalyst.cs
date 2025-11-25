using NeoParacosm.Content.Projectiles.Friendly.Magic;
using NeoParacosm.Core.Players;
using NeoParacosm.Core.Systems.Assets;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Localization;

namespace NeoParacosm.Content.Items.Weapons.Magic.Spells.Catalysts;

public abstract class BaseCatalyst : ModItem
{
    public override void SetDefaults()
    {
        Item.DamageType = DamageClass.Magic;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.noMelee = true;
        Item.mana = 1;
        Item.UseSound = null;
        Item.shoot = ProjectileID.PurificationPowder;
        Item.useTime = 10;
        Item.useAnimation = 10;
        Item.channel = true;
    }

    public override bool CanUseItem(Player player)
    {
        return player.NPCatalystPlayer().SelectedSpell != null;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        position = player.Center;
        velocity = player.NPCatalystPlayer().SelectedSpell.TargetVector == Vector2.Zero ? 
                                                                           Main.MouseWorld : 
                                                                           player.DirectionTo(player.NPCatalystPlayer().SelectedSpell.TargetVector);
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        NPCatalystPlayer cp = player.NPCatalystPlayer();
        if (cp.SelectedSpell == null)
        {
            return false;
        }
        cp.SelectedSpell.ShootBehaviour(player);
        return false;
    }

    public override float UseSpeedMultiplier(Player player)
    {
        return (float)Item.useTime / (player.NPCatalystPlayer().SelectedSpell.AttackCooldown + Item.useTime);
    }

    public override void ModifyManaCost(Player player, ref float reduce, ref float mult)
    {
        if (player.NPCatalystPlayer().SelectedSpell != null)
        {
            mult = (player.NPCatalystPlayer().SelectedSpell.ManaCost + Item.mana) / Item.mana;
        }
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        TooltipLine catalystLine = new TooltipLine(Mod, "NeoParacosm:CatalystDescription", Language.GetTextValue("Mods.NeoParacosm.Items.CatalystTemplate.Description"));
        tooltips.Add(catalystLine);

        TooltipLine spellsLine = new TooltipLine(Mod, "NeoParacosm:Spells", Language.GetTextValue("Mods.NeoParacosm.Items.CatalystTemplate.EquippedSpells"));
        tooltips.Add(spellsLine);

        if (Main.LocalPlayer.NPCatalystPlayer().EquippedSpells.Count == 0)
        {
            TooltipLine noneSpellsLine = new TooltipLine(Mod, "NeoParacosm:NoSpells", Language.GetTextValue("Mods.NeoParacosm.Items.CatalystTemplate.None"));
            tooltips.Add(noneSpellsLine);
            return;
        }
        string equippedSpells = "";
        foreach (var spell in Main.LocalPlayer.NPCatalystPlayer().EquippedSpells)
        {
            equippedSpells += $"[i:NeoParacosm/{spell.GetType().Name}] ";
        }
        TooltipLine spellsLine2 = new TooltipLine(Mod, "NeoParacosm:EquippedSpells", equippedSpells);
        tooltips.Add(spellsLine2);
    }
}