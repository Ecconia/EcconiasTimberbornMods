using System.Reflection;
using HarmonyLib;
using Timberborn.MasterScene;

namespace EnhancedControls
{
	public static class ConfigureHook
	{
		public static void hookGameScene(Harmony harmony, Type type, string methodName)
		{
			var configMethod = typeof(MasterSceneConfigurator).GetMethod("Configure", BindingFlags.Instance | BindingFlags.NonPublic);
			if(configMethod == null)
			{
				throw new Exception("Could not find Configure method in MasterSceneConfigurator");
			}
			var patch = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static);
			harmony.Patch(configMethod, null, new HarmonyMethod(patch));
		}
	}
}
