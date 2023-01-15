using Timberborn.InputSystem;
using UnityEngine.InputSystem;

namespace EnhancedControls.DynamicKeybindings
{
	public class CheckKeyHeld : DynamicKeybinding
	{
		private readonly Key key;

		public CheckKeyHeld(Key key)
		{
			this.key = key;
		}

		public bool resolve(InputService service, KeyboardController keyboard, MouseController mouse)
		{
			return keyboard.IsKeyHeld(key);
		}
	}
}
