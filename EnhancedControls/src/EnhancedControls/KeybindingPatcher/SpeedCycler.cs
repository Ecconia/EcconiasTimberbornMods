using EnhancedControls.DynamicKeybindings;
using Timberborn.InputSystem;

namespace EnhancedControls.KeybindingPatcher
{
	public class SpeedCycler : DynamicSpeedKeybinding
	{
		private readonly DynamicKeybinding keybinding;

		public SpeedCycler(DynamicKeybinding keybinding)
		{
			this.keybinding = keybinding;
		}

		public int? resolve(InputService service, KeyboardController keyboard, MouseController mouse)
		{
			if(!keybinding.resolve(service, keyboard, mouse))
			{
				return new int?();
			}
			int currentSpeed = StaticSpeed.instance.getCurrentSpeed();
			if(currentSpeed >= 3)
			{
				return 1;
			}
			return currentSpeed + 1;
		}
	}
}
