using System.Reflection;
using Timberborn.BaseComponentSystem;
using Timberborn.InputSystem;
using Timberborn.InventorySystem;
using Timberborn.SelectionSystem;
using Timberborn.Stockpiles;
using UnityEngine;

namespace CopyStorageFilter
{
	//The tool that is not an actual game 'Tool' (class).
	public class CopyTool : MonoBehaviour, IInputProcessor
	{
		//Instance fields:
		private readonly SelectableObjectRaycaster raycaster;
		private readonly InputService inputService;
		private readonly MouseController mc;

		//Data fields:
		private string type = "";
		private string? good;

		public CopyTool(InputService inputService, SelectableObjectRaycaster raycaster)
		{
			//Save services to fields:
			this.inputService = inputService;
			this.raycaster = raycaster;
			//Get the value of the mouse controller from the input service:
			var tmp = inputService.GetType().GetField("_mouse", BindingFlags.Instance | BindingFlags.NonPublic);
			if(tmp == null)
			{
				throw new Exception("Complain to the dev: The 'InputService' class does not contain a private field '_mouse'. Whoops mod won't be functional!");
			}
			mc = (MouseController) tmp.GetValue(inputService);
			if(mc == null)
			{
				throw new Exception("Complain to the dev: The 'InputService's field '_mouse' has no value. Whoops mod won't be functional!");
			}
			//Receive input events from the input system:
			InputForwarder.instantiate(inputService, this);
		}

		public bool ProcessInput()
		{
			bool isLeft = mc.IsButtonUpAfterShortHold(MouseButton.Left);
			if(!inputService.IsShiftHeld || !(isLeft || mc.IsButtonUpAfterShortHold(MouseButton.Right)))
			{
				//Shift + Left/Right has to be pressed, else reject:
				return false;
			}

			raycaster.TryHitSelectableObject(out BaseComponent hitGameObject);
			if(hitGameObject == null)
			{
				//Not looking at an object? Reject:
				return false;
			}

			var stockpile = hitGameObject.GameObjectFast.GetComponent<Stockpile>();
			if(stockpile == null)
			{
				//The building must be of stockpile type:
				return false;
			}
			var singleGoodAllower = hitGameObject.GameObjectFast.GetComponent<SingleGoodAllower>();
			if(singleGoodAllower == null)
			{
				//The inventory type must be "single good allower":
				return false;
			}

			//At this point, always consume input.
			if(isLeft)
			{
				//Paste setting:
				if(!type.Equals(stockpile.WhitelistedGoodType))
				{
					//The type is not equal to what was last copied, so stop here.
					return true;
				}
				if(good == null)
				{
					singleGoodAllower.Disallow();
				}
				else
				{
					singleGoodAllower.Allow(good);
				}
			}
			else
			{
				//Copy setting:
				type = stockpile.WhitelistedGoodType;
				good = singleGoodAllower.AllowedGood;
			}
			//True means, consumed.
			return true;
		}
	}
}
