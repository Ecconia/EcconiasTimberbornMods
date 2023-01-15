using BepInEx;
using BepInEx.Logging;
using EnhancedControls.DynamicKeybindings;
using EnhancedControls.KeybindingPatcher;
using EnhancedControls.Tests;
using HarmonyLib;
using UnityEngine.InputSystem;

namespace EnhancedControls
{
	[BepInPlugin("ecconia.timberborn.enhancedcontrols", "Enhanced Controls", "1.0.0")]
	public class Plugin : BaseUnityPlugin
	{
		public static ManualLogSource? logger;
		public static Harmony harmony;

		private void Awake()
		{
			logger = Logger;
			print("Plugin Enhanced Controls is starting!");

			harmony = new Harmony("ecconia.timberborn.enhancedcontrols");
			StaticSpeed.init(harmony);
			InputServiceHijacker.showStockpileOverlay = new CheckKeyHeld(Key.K);
			InputServiceHijacker.changeGameSpeed = new SpeedCycler(new CheckKeyDown(Key.Tab));
			InputServiceHijacker.init(harmony);

			print("Plugin Enhanced Controls is loaded!");
		}

		public static void print(string message)
		{
			logger!.LogMessage(message);
		}
	}
}
