using HarmonyLib;

namespace TerrainTools.Patches
{
	[HarmonyPatch(typeof(Game), "Logout")]
	internal static class HandleLogout
	{
		public static void Prefix()
		{
			if (Commands.debugTerrain)
			{
				Commands.DebugToggle(out _);
			}
		}
	}
}
