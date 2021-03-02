using HarmonyLib;

namespace TerrainTools.Patches
{
	[HarmonyPatch(typeof(TerrainModifier), "Awake")]
	internal static class TerrainLight
	{
		public static void Prefix(ref TerrainModifier __instance)
		{
			if (Commands.debugTerrain)
			{
				Commands.SetupLight(__instance);
			}
		}
	}
}
