using NeoParacosm.Core.Systems;
using Terraria.Graphics.Capture;

namespace NeoParacosm.Content.Biomes.DeadForest;

public class DeadForestBiome : ModBiome
{
    public override int Music => MusicID.Graveyard;
    public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => GetInstance<DeadForestSurfaceBackgroundStyle>();
    //public override CaptureBiome.TileColorStyle TileColorStyle => CaptureBiome.TileColorStyle.Crimson;
    public override string BestiaryIcon => base.BestiaryIcon;
    public override string BackgroundPath => base.BackgroundPath;
    public override Color? BackgroundColor => Color.DarkSlateGray;
    public override string MapBackground => BackgroundPath;
    public override SceneEffectPriority Priority => SceneEffectPriority.Environment;


    public override bool IsBiomeActive(Player player)
    {
        return GetInstance<BiomeSystem>().deadDirtTileCount >= 1000 && player.ZoneOverworldHeight;
    }
}

public class DeadForestSurfaceBackgroundStyle : ModSurfaceBackgroundStyle
{
    public override void ModifyFarFades(float[] fades, float transitionSpeed)
    {
        for (int i = 0; i < fades.Length; i++)
        {
            if (i == Slot)
            {
                fades[i] += transitionSpeed;
                if (fades[i] > 1f)
                {
                    fades[i] = 1f;
                }
            }
            else
            {
                fades[i] -= transitionSpeed;
                if (fades[i] < 0f)
                {
                    fades[i] = 0f;
                }
            }
        }
    }

    public override int ChooseFarTexture()
    {
        return BackgroundTextureLoader.GetBackgroundSlot(Mod, "Common/Assets/Textures/Backgrounds/DeadForestBackgroundFar");
    }

    public override int ChooseMiddleTexture()
    {
        return BackgroundTextureLoader.GetBackgroundSlot(Mod, "Common/Assets/Textures/Backgrounds/DeadForestBackgroundMid");
    }

    public override int ChooseCloseTexture(ref float scale, ref double parallax, ref float a, ref float b)
    {
        return BackgroundTextureLoader.GetBackgroundSlot(Mod, "Common/Assets/Textures/Backgrounds/DeadForestBackgroundClose");
    }
}
