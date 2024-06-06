using Timberborn.BaseComponentSystem;
using Timberborn.InputSystem;
using Timberborn.InventorySystem;
using Timberborn.SelectionSystem;
using Timberborn.Stockpiles;

namespace CopyStorageFilter
{
	public class CopyTool
	{
		public static CopyTool instance;
		
		private readonly SelectableObjectRaycaster raycaster;
		private readonly InputService inputService;
		
		public CopyTool(InputService inputService, SelectableObjectRaycaster raycaster)
		{
			instance = this;
			this.inputService = inputService;
			this.raycaster = raycaster;
		}
		
		public void triggerCopyPaste(out bool consume)
		{
			consume = false;
			if (inputService.MouseOverUI)
			{
				//Nothing to do, when mouse is over UI. Need to be able to select a building or so.
				return;
			}
			
			var isRight = inputService.IsKeyDown(CursorToolHook.rightButton);
			var isLeft = inputService.IsKeyDown(CursorToolHook.leftButton);
			if (isRight == isLeft)
			{
				//Left or right must be pressed for this tool to function. None or both is not supported. 
				return;
			}
			
			attemptCopyPasteAction(isRight, out bool successful);
			consume = successful;
		}
		
		private void attemptCopyPasteAction(bool isPaste, out bool successful)
		{
			successful = false;
			
			raycaster.TryHitSelectableObject(out BaseComponent hitGameObject);
			if (hitGameObject == null)
			{
				//Not looking at an object? Reject.
				return;
			}
			var gameObject = hitGameObject.GameObjectFast;
			var stockpile = gameObject.GetComponent<Stockpile>();
			var singleGoodAllower = gameObject.GetComponent<SingleGoodAllower>();
			if (stockpile == null || singleGoodAllower == null)
			{
				//The inventory type must be "single good allower".
				//The building must be of stockpile type.
				return;
			}
			successful = true;
			
			performCopyPasteAction(singleGoodAllower, stockpile, isPaste);
		}
		
		private readonly Dictionary<string, string> copiedFilters = new Dictionary<string, string>();
		
		private void performCopyPasteAction(SingleGoodAllower singleGoodAllower, Stockpile stockpile, bool isPaste)
		{
			//At this point, always consume input.
			if (isPaste)
			{
				//Get setting:
				if (copiedFilters.TryGetValue(stockpile.WhitelistedGoodType, out string filter))
				{
					//Paste setting:
					if (filter == null)
					{
						singleGoodAllower.Disallow();
					}
					else
					{
						singleGoodAllower.Allow(filter);
					}
				}
				//else Nothing copied of that type so far... (so do nothing).
			}
			else
			{
				//Copy setting:
				copiedFilters[stockpile.WhitelistedGoodType] = singleGoodAllower.AllowedGood;
			}
		}
	}
}
