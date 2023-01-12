using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Timberborn.CameraSystem;
using UnityEngine;

namespace FixNearPlane
{
	[BepInPlugin("ecconia.timberborn.fixnearplane", "Fix Near Plane", "1.0.0")]
	public class Plugin : BaseUnityPlugin
	{
		private static ManualLogSource? logger;

		private void Awake()
		{
			logger = Logger;

			var harmony = new Harmony("ecconia.timberborn.fixnearplane");
			var property = typeof(CameraComponent).GetMethod("Awake", BindingFlags.Public | BindingFlags.Instance);
			var hook = typeof(Plugin).GetMethod(nameof(patch), BindingFlags.Public | BindingFlags.Static);
			harmony.Patch(property, null, new HarmonyMethod(hook));
		}

		public static void patch(Camera ____camera)
		{
			const float target = 0.1f;
			float before = ____camera.nearClipPlane;
			if(Math.Abs(before - target) > 0.00001f)
			{
				____camera.nearClipPlane = target;
				logger!.LogInfo("Changed camera near plane from " + before + " to " + target);
			}
		}
	}
}
