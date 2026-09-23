using Terraria.GameContent.Bestiary;
using Terraria.Localization;

namespace NeoParacosm.Core.Globals.GlobalDusts;

//shitpost
/*public class NPGlobalDust : ModSystem
{
    public override void Load()
    {
        On_Main.DrawDust += Main_DrawDust;
    }

    private static void Main_DrawDust(On_Main.orig_DrawDust orig, Main self)
    {
        orig(self);
        LemonUtils.BeginSpriteBatchProjectile();
        foreach (var dust in Main.dust)
        {
            if (dust.active && DustID.Search.TryGetName(dust.type, out string name))
            {
                LemonUtils.DrawText(name, dust.position - Vector2.UnitY * 32 - Main.screenPosition, Color.White, 0, Vector2.Zero, dust.scale * 0.5f);
            }
        }
        Main.spriteBatch.End();
    }
}*/
