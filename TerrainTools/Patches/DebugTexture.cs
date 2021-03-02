using HarmonyLib;
using UnityEngine;

namespace TerrainTools.Patches
{
	[HarmonyPatch(typeof(Heightmap), "Initialize")]
	internal static class DebugTexture
	{
		public static void Postfix(ref Heightmap __instance)
		{
			if (Commands.debugTerrain)
			{
				Commands.m_materialInstance(__instance).SetTexture("_DiffuseTex0", Texture2D.whiteTexture);
			}
		}
	}
}
