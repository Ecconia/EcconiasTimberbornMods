using Timberborn.InputSystem;
using UnityEngine.InputSystem;

namespace EnhancedControls.DynamicKeybindings
{
	public class CheckKeyDown : DynamicKeybinding
	{
		private readonly Key key;

		public CheckKeyDown(Key key)
		{
			this.key = key;
		}

		public bool resolve(InputService service, KeyboardController keyboard, MouseController mouse)
		{
			return keyboard.IsKeyDown(key);
		}
	}
}
