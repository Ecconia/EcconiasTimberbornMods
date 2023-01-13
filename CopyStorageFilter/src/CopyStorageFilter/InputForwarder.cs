using Timberborn.InputSystem;
using UnityEngine;

namespace CopyStorageFilter
{
	//Dirty hack to get things working. No clue how InputProcessors are meant to be used without a GameObject... This wraps it.
	public class InputForwarder : MonoBehaviour, IInputProcessor
	{
		private IInputProcessor? target;

		public static void instantiate(InputService inputService, IInputProcessor target)
		{
			GameObject go = new GameObject();
			DontDestroyOnLoad(go);
			InputForwarder sis = go.AddComponent<InputForwarder>();
			sis.set(target);
			go.SetActive(true);
			inputService.AddInputProcessor(sis);
		}

		private void set(IInputProcessor target)
		{
			this.target = target;
		}

		public bool ProcessInput()
		{
			return target!.ProcessInput();
		}
	}
}
