using Timberborn.InputSystem;

namespace EnhancedControls.DynamicKeybindings
{
	public interface DynamicSpeedKeybinding
	{
		int? resolve(InputService service, KeyboardController keyboard, MouseController mouse);
	}
}
