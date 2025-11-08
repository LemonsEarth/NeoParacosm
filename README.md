# NeoParacosm
NeoParacosm/NextParacosm is a content mod, more specifically a remake of another content mod called Paracosm. The mod features new biomes, bosses, items and more.
You cannot use the mod's assets in your own mods or projects without asking for permission first. This includes (our) sprites, music, SFX...
You are free to use the mod's assets for thumbnails, video background music etc, provided that you give proper credit (not necessary for thumbnails)
You are free to look at the code for inspiration and/or examples. Copying code directly is frowned upon.

Credits for external assets like sound effects can be found in ***credits.txt***.

If you wish to see how/where something was implemented, such as shaders, primitive drawing etc, here are some pointers:

**Shaders** are located in *Common/Assets/Shaders*
	- Shaders are loaded in the main mod file
	- Screen shaders/Filters are activated in *NPPlayer.PostUpdateMiscEffects()*
	- Item shader usage can be found in any ascended weapon item
	- Projectile shader usage can be found in in any ascended weapon projectile (held projectile)

**Primitive Drawing** methods can be found in *PrimHelper.cs*
	- Rectangular primitive trail example(s): *CorruptCarrierProj.cs*
	- Sword slash primitive trail example(s): *GraveswordHeldProj.cs*

**Saving and loading world data** can be found in *WorldDataSystem.cs*

**World Gen** can be found in *WorldGenSystem.cs*

**Custom item slot** implementation can be found in *ItemSlotWrapper.cs, InItemPanel.cs, OutItemPanel.cs*

**Custom dialogue** implementation can be found in *ResearcherDialogueUIState.cs*
