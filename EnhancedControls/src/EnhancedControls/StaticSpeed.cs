using System.Reflection;
using Bindito.Core;
using HarmonyLib;
using Timberborn.Debugging;
using Timberborn.TimeSpeedButtonSystem;
using Timberborn.TimeSystem;
using Timberborn.TimeSystemUI;

namespace EnhancedControls
{
	public class StaticSpeed
	{
		public static void init(Harmony harmony)
		{
			ConfigureHook.hookGameScene(harmony, typeof(StaticSpeed), nameof(hook));
		}

		public static void hook(IContainerDefinition containerDefinition)
		{
			containerDefinition.Bind<StaticSpeed>().AsSingleton();
		}

		//### Instance code: #############

		public static StaticSpeed instance;
		private readonly SpeedManager speedManager;
		private readonly SpeedControlPanel speedControlPanel;

		//The first three speed levels, the last one is unused though.
		private readonly int[] speeds = new int[3];

		public StaticSpeed(IEnumerable<IConsoleModule> consoleModules, SpeedManager speedManager)
		{
			instance = this;
			this.speedManager = speedManager;
			var tmp = consoleModules.First(module => module.GetType() == typeof(SpeedControlPanel));
			if(tmp == null)
			{
				throw new Exception("Could not find SpeedControlPanel in the ConsoleModules...");
			}
			this.speedControlPanel = (SpeedControlPanel) tmp;

			var loadMethod = typeof(SpeedControlPanel).GetMethod(nameof(SpeedControlPanel.Load), BindingFlags.Instance | BindingFlags.Public);
			var patch = typeof(StaticSpeed).GetMethod(nameof(initSpeeds), BindingFlags.NonPublic | BindingFlags.Static);
			Plugin.harmony.Patch(loadMethod, null, new HarmonyMethod(patch));
		}

		private static void initSpeeds()
		{
			var buttonsField = instance.speedControlPanel.GetType().GetField("_buttons", BindingFlags.Instance | BindingFlags.NonPublic);
			if(buttonsField == null)
			{
				throw new Exception("The '_buttons' field in 'SpeedControlPanel' was probably renamed or removed - notify dev to update this!");
			}
			var list = (List<TimeSpeedButton>) buttonsField.GetValue(instance.speedControlPanel);
			if(list == null)
			{
				throw new Exception("The buttons in the SpeedControlPanel are not set.");
			}
			if(list.Count < 4)
			{
				throw new Exception("The buttons in the SpeedControlPanel are not enough: " + list.Count);
			}
			if(list[0].TimeSpeed != 0)
			{
				throw new Exception("First speed button was expected to be 0 ticks (pause), but its: " + list[0].TimeSpeed);
			}
			for(int i = 1; i < 4; i++)
			{
				instance.speeds[i - 1] = list[i].TimeSpeed;
			}
		}

		public int getCurrentSpeed()
		{
			int realSpeed = speedManager.CurrentSpeed;
			if(realSpeed == 0)
			{
				return 0;
			}
			if(realSpeed <= speeds[0])
			{
				return 1;
			}
			if(realSpeed <= speeds[1])
			{
				return 2;
			}
			return 3;
		}
	}
}
