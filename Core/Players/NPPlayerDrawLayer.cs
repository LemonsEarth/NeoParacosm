using NeoParacosm.Content.Buffs.Debuffs.Cooldowns;
using NeoParacosm.Content.Buffs.GoodBuffs;
using NeoParacosm.Content.Items.Accessories.Combat;
using NeoParacosm.Content.Items.Pickups;
using NeoParacosm.Content.Projectiles.Friendly.Special;
using System.Collections.Generic;
using Terraria.DataStructures;

namespace NeoParacosm.Core.Players;

public class NPPlayerDrawLayer : PlayerDrawLayer
{
    protected override void Draw(ref PlayerDrawSet drawInfo)
    {
       // DrawData data = new DrawData();
    }

    public override Position GetDefaultPosition()
    {
        return new AfterParent(PlayerDrawLayers.Head);
    }
}
