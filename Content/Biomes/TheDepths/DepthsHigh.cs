using NeoParacosm.Core.Systems;

namespace NeoParacosm.Content.Biomes.TheDepths;

public class DepthsHigh : ModBiome
{
    public override int Music => MusicID.Boss2;
    public override string BestiaryIcon => base.BestiaryIcon;
    public override string BackgroundPath => base.BackgroundPath;
    public override Color? BackgroundColor => Color.Blue;
    public override string MapBackground => BackgroundPath;
    public override SceneEffectPriority Priority => SceneEffectPriority.Event;

    public override bool IsBiomeActive(Player player)
    {
        return ModContent.GetInstance<BiomeTileCounts>().depthStoneTileCount >= 2000 && player.ZoneDirtLayerHeight;
    }
}

public class DepthsHighUndergroundBackgroundStyle : ModUndergroundBackgroundStyle
{
    string path = "Content/Biomes/TheDepths/DepthsHigh_Background";
    public override void FillTextureArray(int[] textureSlots)
    {
        textureSlots[0] = BackgroundTextureLoader.GetBackgroundSlot(Mod, path + "0");
        textureSlots[1] = BackgroundTextureLoader.GetBackgroundSlot(Mod, path + "1");
        textureSlots[2] = BackgroundTextureLoader.GetBackgroundSlot(Mod, path + "2");
        textureSlots[3] = BackgroundTextureLoader.GetBackgroundSlot(Mod, path + "3");
    }
}
