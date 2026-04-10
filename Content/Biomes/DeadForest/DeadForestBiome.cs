using NeoParacosm.Content.NPCs.Bosses.Deathbird;
using NeoParacosm.Content.NPCs.Bosses.Deathbird.DeathbirdMini;
using NeoParacosm.Content.NPCs.Hostile.DeadForest;
using NeoParacosm.Core.Systems.Data;
using NeoParacosm.Core.Systems.World;
using System.Collections.Generic;

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

public class DeadForestBiomeNPC : GlobalNPC
{
    public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
    {
        if (spawnInfo.Player.InModBiome<DeadForestBiome>())
        {
            pool.Clear();
            if (!DownedBossSystem.downedDeathbirdMini)
            {
                pool.Add(NPCType<DeathbirdMini>(), 1f);
            }
            else
            {
                pool.Add(NPCType<SoulfulSludge>(), 0.9f);
                pool.Add(NPCType<WingedEyeball>(), 0.8f);
                pool.Add(NPCType<TaintedSteed>(), 0.7f);
                pool.Add(NPCType<SpearKnight>(), 0.4f);
                pool.Add(NPCType<ShieldKnight>(), 0.2f);
                pool.Add(NPCType<StaffKnight>(), 0.2f);
                pool.Add(NPCType<BombKnight>(), 0.1f);
                pool.Add(NPCType<InsatiableDolour>(), 0.1f);
            }
        }
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
