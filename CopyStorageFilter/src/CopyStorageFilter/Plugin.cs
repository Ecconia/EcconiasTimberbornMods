using Bindito.Core;
using HarmonyLib;
using Timberborn.ModManagerScene;

namespace CopyStorageFilter
{
	public class Plugin : IModStarter
	{
		public void StartMod()
		{
			var harmony = new Harmony("ecconia.timberborn.copystoragefilter");
			
			//Setup all the reflection/harmony stuff for the CursorTool hooks: 
			CursorToolHook.init(harmony);
		}
		
		[Context("Game")]
		public class CopyStorageFilterConfigurator : IConfigurator
		{
			public void Configure(IContainerDefinition containerDefinition)
			{
				containerDefinition.Bind<CopyTool>().AsSingleton();
			}
		}
	}
}
