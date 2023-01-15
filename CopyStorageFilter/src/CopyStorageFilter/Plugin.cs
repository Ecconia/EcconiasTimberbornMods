using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using Bindito.Core;
using HarmonyLib;
using Timberborn.SelectionSystem;

namespace CopyStorageFilter
{
	[BepInPlugin("ecconia.timberborn.copystoragefilter", "Copy Storage Filter", "1.0.1")]
	public class Plugin : BaseUnityPlugin
	{
		public static ManualLogSource? logger;

		private void Awake()
		{
			logger = Logger;

			var harmony = new Harmony("ecconia.timberborn.copystoragefilter");
			//Modify select keybinding:
			SelectKeybindChanger.init(harmony);
			//Hook service:
			//Has to be hooked after 'SelectionSystem' as that provides the required ray-caster.
			//TODO: Find a better point to hook services, that is after Timberborn is done or dynamically sorted.
			var stuff = typeof(SelectionSystemConfigurator).GetMethod("Configure", BindingFlags.Public | BindingFlags.Instance);
			var hook = typeof(Plugin).GetMethod(nameof(Plugin.serviceHook), BindingFlags.Public | BindingFlags.Static);
			harmony.Patch(stuff, null, new HarmonyMethod(hook));
		}

		public static void serviceHook(IContainerDefinition containerDefinition)
		{
			containerDefinition.Bind<CopyTool>().AsSingleton();
		}
	}
}
