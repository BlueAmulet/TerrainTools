using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;
using TerrainTools.Patches;

namespace TerrainTools
{
	[BepInPlugin(GUID, PluginName, Version)]
	public class TerrainTools : BaseUnityPlugin
	{
		public const string GUID = "TerrainTools";
		public const string PluginName = "TerrainTools";
		public const string Version = "1.0.3";

		internal static new ManualLogSource Logger;
		internal static readonly Harmony harmony = new Harmony(GUID);

		public void Awake()
		{
			Logger = base.Logger;
			Settings.InitConfig(Config);
			harmony.PatchAll(Assembly.GetExecutingAssembly());
			int patchedMethods = 0;
			foreach (MethodBase method in harmony.GetPatchedMethods())
			{
				Logger.LogInfo("Patched " + method.DeclaringType.Name + "." + method.Name);
				patchedMethods++;
			}
			Logger.LogInfo(patchedMethods + " patches applied\n");
		}
	}
}
