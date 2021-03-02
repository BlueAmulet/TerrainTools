using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace TerrainTools
{
	internal static class Settings
	{
		private static ConfigFile configFile;
		private static Dictionary<Type, string> typeStr = new Dictionary<Type, string>();

		internal static ConfigEntry<float> lightDistance;
		internal static ConfigEntry<float> lightStrength;
		internal static ConfigEntry<KeyboardShortcut> debugToggle;

		static Settings()
		{
			typeStr.Add(typeof(bool), "bool");
			typeStr.Add(typeof(int), "int");
			typeStr.Add(typeof(float), "float");
			typeStr.Add(typeof(KeyboardShortcut), "keybind");
		}

		public static void InitConfig()
		{
			var mOriginal = AccessTools.Method(typeof(ConfigEntryBase), "WriteDescription");
			var mPrefix = AccessTools.Method(typeof(Settings), "MyWriteDescription");
			TerrainTools.harmony.Patch(mOriginal, new HarmonyMethod(mPrefix));

			configFile = new ConfigFile(Path.Combine(Paths.ConfigPath, TerrainTools.PluginName + ".cfg"), true);
			ConfigEntry<int> version = configFile.Bind("Tools", "Version", 1, "Configuration Version");

			lightDistance = configFile.Bind("Debug", "LightDistance", 25f, "Distance of visible debug lights");
			lightStrength = configFile.Bind("Debug", "LightStrength", 0.5f, "Strength of debug lights");
			debugToggle = configFile.Bind("Debug", "Toggle", new KeyboardShortcut(KeyCode.F4), "Terrain debug toggle keybind");
		}

		public static bool MyWriteDescription(ref ConfigEntryBase __instance, ref StreamWriter writer)
		{
			if (__instance.ConfigFile == configFile)
			{
				if (!typeStr.TryGetValue(__instance.SettingType, out string TypeName))
				{
					TypeName = __instance.SettingType.Name;
				}
				string TypeDescription = TypeName + ", default: " + TomlTypeConverter.ConvertToString(__instance.DefaultValue, __instance.SettingType);
				if (!string.IsNullOrEmpty(__instance.Description.Description))
				{
					writer.WriteLine("# " + __instance.Description.Description.Replace("\n", "\n# ") + " (" + TypeDescription + ")");
				}
				else
				{
					writer.WriteLine("# " + TypeDescription);
				}
				if (__instance.Description.AcceptableValues != null)
				{
					writer.WriteLine(__instance.Description.AcceptableValues.ToDescriptionString());
				}
				else if (__instance.SettingType.IsEnum)
				{
					writer.WriteLine("# Acceptable values: " + string.Join(", ", Enum.GetNames(__instance.SettingType)));
					if (__instance.SettingType.GetCustomAttributes(typeof(FlagsAttribute), inherit: true).Any())
					{
						writer.WriteLine("# Multiple values can be set at the same time by separating them with , (e.g. Debug, Warning)");
					}
				}
				return false;
			}
			else
			{
				return true;
			}
		}
	}
}
