using BepInEx;

namespace EnhancedControls
{
	[BepInPlugin("de.ecconia.timberborn.keyboardcontrols", "Keyboard Controls", "1.0.0")]
	public class Plugin : BaseUnityPlugin
	{
		private void Awake()
		{
			Logger.LogInfo("Plugin Keyboard Controls is starting!");
			
			Logger.LogInfo("Plugin Keyboard Controls is loaded!");
		}
	}
}
