using System.Reflection;
using HarmonyLib;
using Timberborn.CursorToolSystem;
using Timberborn.InputSystem;
using UnityEngine.InputSystem;

namespace CopyStorageFilter
{
	public static class CursorToolHook
	{
		public static string rightButton;
		public static string leftButton;
		
		public static void init(Harmony harmony)
		{
			var field = typeof(InputService).GetField(nameof(InputService.MouseLeftKey), BindingFlags.Static | BindingFlags.NonPublic);
			checkNotNull(field, "Could not find field \"MouseLeftKey\" in \"InputService\"");
			leftButton = (string) field.GetValue(null);
			
			field = typeof(InputService).GetField(nameof(InputService.MouseRightKey), BindingFlags.Static | BindingFlags.NonPublic);
			checkNotNull(field, "Could not find field \"MouseRightKey\" in \"InputService\"");
			rightButton = (string) field.GetValue(null);
			
			var stuff = typeof(CursorTool).GetMethod(nameof(CursorTool.ProcessSelectObject), BindingFlags.NonPublic | BindingFlags.Instance);
			checkNotNull(stuff, "Could not find method \"ProcessSelectObject\" in \"CursorTool\"");
			var hook = typeof(CursorToolHook).GetMethod(nameof(cursorToolSelectHook), BindingFlags.Public | BindingFlags.Static);
			harmony.Patch(stuff, new HarmonyMethod(hook));
		}
		
		private static void checkNotNull(object value, string message)
		{
			if (value == null)
			{
				throw new Exception(message);
			}
		}
		
		public static bool cursorToolSelectHook(ref bool __result)
		{
			var instance = CopyTool.instance;
			var isShift = Keyboard.current.leftShiftKey.isPressed;
			if (!isShift || instance == null)
			{
				//The copy tool must be initialized and shift must be pressed, for the copy/paste tool to function.
				return true;
			}
			
			instance.triggerCopyPaste(out bool consume);
			if (consume)
			{
				__result = true;
			}
			//False skips original
			//True executes original
			return !consume;
		}
	}
}
