using System.Reflection;
using BepInEx;
using Bindito.Core;
using HarmonyLib;
using Timberborn.SelectionSystem;

namespace CopyStorageFilter
{
	[BepInPlugin("ecconia.timberborn.copystoragefilter", "Copy Storage Filter", "2.0.0")]
	public class Plugin : BaseUnityPlugin
	{
		private void Awake()
		{
			var harmony = new Harmony("ecconia.timberborn.copystoragefilter");
			
			//Hook a service (for easy access into the dependency system):
			var stuff = typeof(SelectionSystemConfigurator).GetMethod("Configure", BindingFlags.Public | BindingFlags.Instance);
			var hook = typeof(Plugin).GetMethod(nameof(serviceHook), BindingFlags.Public | BindingFlags.Static);
			harmony.Patch(stuff, null, new HarmonyMethod(hook));
			
			//Setup all the reflection/harmony stuff for the CursorTool hooks: 
			CursorToolHook.init(harmony);
		}
		
		public static void serviceHook(IContainerDefinition containerDefinition)
		{
			containerDefinition.Bind<CopyTool>().AsSingleton();
		}
	}
}
