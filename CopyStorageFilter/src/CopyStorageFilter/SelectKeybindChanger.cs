using System.Reflection;
using HarmonyLib;
using Timberborn.InputSystem;

namespace CopyStorageFilter
{
	//Will change the keybinding from 'left-click' to 'left-click without shift'.
	//Needed as the shift+click will be used for this mod.
	//TODO: Find better solution to hook shift+click without changing normal click behavior.
	public static class SelectKeybindChanger
	{
		public static void init(Harmony harmony)
		{
			var hook = typeof(SelectKeybindChanger).GetMethod(nameof(keybindingFixer), BindingFlags.Public | BindingFlags.Static);
			var property = typeof(InputService).GetProperty("SelectionStart", BindingFlags.Public | BindingFlags.Instance);
			if(property == null)
			{
				throw new Exception("Report to dev: Could not find property 'SelectionStart' in class 'InputService'. Plugin will not be functional.");
			}
			harmony.Patch(property.GetMethod, new HarmonyMethod(hook));
		}

		public static bool keybindingFixer(ref bool __result, InputService __instance, MouseController ____mouse)
		{
			__result = !__instance.IsShiftHeld && ____mouse.IsButtonDown(MouseButton.Left);
			return false;
		}
	}
}
