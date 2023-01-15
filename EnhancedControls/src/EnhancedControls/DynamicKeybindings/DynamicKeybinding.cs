using Timberborn.InputSystem;

namespace EnhancedControls.DynamicKeybindings
{
	public interface DynamicKeybinding
	{
		bool resolve(InputService service, KeyboardController keyboard, MouseController mouse);
	}
}
