using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TerrainTools.Patches
{
	[HarmonyPatch(typeof(Console), "InputText")]
	internal static class Commands
	{
		private static readonly AccessTools.FieldRef<TerrainModifier, ZNetView> m_nview = AccessTools.FieldRefAccess<TerrainModifier, ZNetView>("m_nview");
		internal static readonly AccessTools.FieldRef<Heightmap, Material> m_materialInstance = AccessTools.FieldRefAccess<Heightmap, Material>("m_materialInstance");
		internal static bool debugTerrain = false;

		private enum TerrainType
		{
			all,
			level,
			smooth,
			paint
		}

		public static void Postfix(ref Console __instance)
		{
			string text = __instance.m_input.text;
			string[] part = text.Split(' ');
			if (part.Length <= 0)
			{
				return;
			}
			if (part[0].StartsWith("/"))
			{
				part[0] = part[0].Substring(1);
			}
			if (part[0] == "help")
			{
				__instance.Print("countterrain [radius=max] - count nearby terrain modifications");
				__instance.Print("resetterrain [radius=5] [type] - remove nearby terrain modifications");
				__instance.Print("debugterrain - visualize terrain modifications");
				__instance.Print("debugstrength [new strength] - visualization strength");
				__instance.Print("debugdistance [new distance] - visualization distance");
			}
			else if (part[0] == "countterrain")
			{
				float radius = float.PositiveInfinity;
				if (part.Length > 1)
				{
					if (!float.TryParse(part[1], out radius))
					{
						__instance.Print("Could not parse radius: " + part[1]);
						return;
					}
				}
				Vector3 playerPos = Player.m_localPlayer.transform.position;
				TerrainModifier[] mods = Resources.FindObjectsOfTypeAll<TerrainModifier>().Where(x =>
					x != null &&
					x.m_playerModifiction &&
					Vector3.Distance(playerPos, x.transform.position) <= radius
				).ToArray();
				int levelCount = mods.Where(x => x != null && x.m_level).Count();
				int smoothCount = mods.Where(x => x != null && x.m_smooth).Count();
				int paintCount = mods.Where(x => x != null && x.m_paintCleared && !x.m_level && !x.m_smooth).Count();
				__instance.Print("Counted " + mods.Length + " terrain modifications");
				__instance.Print(levelCount + " level modifications");
				__instance.Print(smoothCount + " smooth modifications");
				__instance.Print(paintCount + " paint modifications");
			}
			else if (part[0] == "resetterrain")
			{
				float radius = 5f;
				TerrainType type = TerrainType.all;
				if (part.Length > 1)
				{
					if (!float.TryParse(part[1], out radius))
					{
						__instance.Print("Could not parse radius: " + part[1]);
						return;
					}
				}
				if (part.Length > 2)
				{
					if (!Enum.TryParse(part[2].ToLowerInvariant(), out type))
					{
						__instance.Print("Could not parse type: " + part[2]);
						return;
					}
				}
				Vector3 playerPos = Player.m_localPlayer.transform.position;
				TerrainModifier[] mods = Resources.FindObjectsOfTypeAll<TerrainModifier>().Where(x =>
					x != null &&
					x.m_playerModifiction &&
					Vector3.Distance(playerPos, x.transform.position) <= radius &&
					(type != TerrainType.level || x.m_level) &&
					(type != TerrainType.smooth || x.m_smooth) &&
					(type != TerrainType.paint || (x.m_paintCleared && !x.m_level && !x.m_smooth))
				).ToArray();
				TerrainModifier[] mods2 = mods.Where(x => x != null && m_nview(x) != null && m_nview(x).IsValid() && m_nview(x).IsOwner()).ToArray();
				foreach (TerrainModifier mod in mods2)
				{
					ZNetScene.instance.Destroy(mod.gameObject);
				}
				__instance.Print("Removed " + mods2.Length + " terrain modifications");
				if (mods2.Length < mods.Length)
				{
					__instance.Print("Could not remove " + (mods.Length - mods2.Length) + " terrain mods due to ownership");
				}
			}
			else if (part[0] == "debugterrain")
			{
				int count = DebugToggle();
				__instance.Print((debugTerrain ? "Enabled" : "Disabled") + " lights on " + count + " terrain modifications");
			}
			else if (part[0] == "debugstrength")
			{
				if (part.Length > 1)
				{
					if (float.TryParse(part[1], out float intensity))
					{
						Settings.lightStrength.Value = intensity;
						__instance.Print("Light strength set to " + intensity);
					}
					else
					{
						__instance.Print("Could not parse intensity: " + part[1]);
					}
				}
				else
				{
					__instance.Print("Current light strength is " + Settings.lightStrength.Value);
				}
			}
			else if (part[0] == "debugdistance")
			{
				if (part.Length > 1)
				{
					if (float.TryParse(part[1], out float distance))
					{
						Settings.lightDistance.Value = distance;
						__instance.Print("Light distance set to " + distance);
					}
					else
					{
						__instance.Print("Could not parse distance: " + part[1]);
					}
				}
				else
				{
					__instance.Print("Current light distance is " + Settings.lightDistance.Value);
				}
			}
		}

		internal static int DebugToggle()
		{
			int count = 0;
			if (EnvMan.instance != null)
			{
				debugTerrain = !debugTerrain;
				foreach (TerrainModifier mod in Resources.FindObjectsOfTypeAll<TerrainModifier>().Where(x => x != null && x.m_playerModifiction))
				{
					Light light = SetupLight(mod);
					count++;
				}
				EnvMan.instance.m_dirLight.enabled = !debugTerrain;
				foreach (Heightmap map in Resources.FindObjectsOfTypeAll<Heightmap>().Where(x => x != null))
				{
					Material mat = m_materialInstance(map);
					if (mat != null)
					{
						if (debugTerrain)
						{
							mat.SetTexture("_DiffuseTex0", Texture2D.whiteTexture);
						}
						else
						{
							mat.SetTexture("_DiffuseTex0", map.m_material.GetTexture("_DiffuseTex0"));
						}
					}
				}
			}
			return count;
		}

		internal static Light SetupLight(TerrainModifier mod)
		{
			Light light = mod.GetComponent<Light>();
			if (light == null)
			{
				light = mod.gameObject.AddComponent<Light>();
				mod.gameObject.AddComponent<LightManager>();
			}
			light.color = new Color(mod.m_level ? 1f : 0f, mod.m_smooth ? 1f : 0f, (mod.m_paintCleared && !mod.m_level && !mod.m_smooth) ? 1f : 0f, 1f);
			light.intensity = Settings.lightStrength.Value;
			light.enabled = debugTerrain;
			return light;
		}
	}
}
