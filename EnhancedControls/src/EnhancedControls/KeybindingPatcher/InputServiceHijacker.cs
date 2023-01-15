using System.Reflection;
using EnhancedControls.DynamicKeybindings;
using HarmonyLib;
using Timberborn.InputSystem;

namespace EnhancedControls.KeybindingPatcher
{
	public static class InputServiceHijacker
	{
		public static void init(Harmony harmony)
		{
			var self = typeof(InputServiceHijacker);
			foreach(var method in self.GetMethods(BindingFlags.NonPublic | BindingFlags.Static))
			{
				var propertyName = method.Name;
				//Check if patch has to be applied:
				var fieldName = ((char) (propertyName[0] - 'A' + 'a')) + propertyName[1..];
				var field = self.GetField(fieldName, BindingFlags.Public | BindingFlags.Static);
				if(field == null)
				{
					throw new Exception("Dear developer, you messed up. If you add a private static method for patching a property in this class, you should also add a public static field for it.");
				}
				if(field.GetValue(null) == null)
				{
					continue;
				}
				//Continue patching:
				var property = typeof(InputService).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
				if(property == null)
				{
					throw new Exception("The property '" + propertyName + "' does not exist in object InputService");
				}
				var propertyGetter = property.GetMethod;
				harmony.Patch(propertyGetter, new HarmonyMethod(method));
			}
		}

		public static DynamicKeybinding showStockpileOverlay;
		
		private static bool ShowStockpileOverlay(ref bool __result, InputService __instance, KeyboardController ____keyboard, MouseController ____mouse)
		{
			__result = showStockpileOverlay.resolve(__instance, ____keyboard, ____mouse);
			return false;
		}
		
		public static DynamicSpeedKeybinding changeGameSpeed;
		
		private static bool ChangeGameSpeed(ref int? __result, InputService __instance, KeyboardController ____keyboard, MouseController ____mouse)
		{
			__result = changeGameSpeed.resolve(__instance, ____keyboard, ____mouse);
			return false;
		}
	}
}
