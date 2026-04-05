using NeoParacosm.Content.Dusts;
using NeoParacosm.Content.Items.Weapons.Magic.Spells;
using NeoParacosm.Core.Players;
using Terraria.DataStructures;

namespace NeoParacosm.Content.Buffs.GoodBuffs;

public class MinersBlessingBuff : ModBuff
{
    public override void SetStaticDefaults()
    {
        Main.debuff[Type] = false;
    }
}

public class MinersBlessingBuffPlayer : ModPlayer
{
    public override void UpdateEquips()
    {
        if (Player.HasBuff(BuffType<MinersBlessingBuff>()))
        {
            Player.pickSpeed -= 0.25f * Player.GetElementalExpertiseBoostMultiplied(SpellElement.Earth, 2);
            Player.tileSpeed += 0.15f * Player.GetElementalExpertiseBoostMultiplied(SpellElement.Earth, 2);
        }
    }
}

public class MinersBlessingTile : GlobalTile
{
    public override void Drop(int i, int j, int type)
    {
        /*if (Main.LocalPlayer.HasBuff(BuffType<MinersBlessingBuff>()) && type == TileID.Stone && Main.rand.NextBool(20))
        {
            Item.NewItem(new EntitySource_TileBreak(i, j), new Vector2(i, j) * 16,
                Main.rand.NextFromList(
                    ItemID.Amethyst, ItemID.Topaz, ItemID.Sapphire, ItemID.Emerald, ItemID.Ruby, ItemID.Diamond),
                Main.rand.Next(1, 4));
        }*/
    }
}
