using Terraria;
using Terraria.ModLoader;

namespace NeoParacosm.Content.Buffs.GoodBuffs;

public class ReducedSpawns : ModBuff
{
    public override void SetStaticDefaults()
    {
        Main.buffNoTimeDisplay[Type] = true;
    }
    public override void Update(Player player, ref int buffIndex)
    {
        //player.GetModPlayer<ParacosmPlayer>().branchedOfLifed = true;
    }
}
