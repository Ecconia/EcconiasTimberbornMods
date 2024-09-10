using Timberborn.BaseComponentSystem;
using Timberborn.Gathering;
using Timberborn.Goods;
using Timberborn.InputSystem;
using Timberborn.InventorySystem;
using Timberborn.Planting;
using Timberborn.SelectionSystem;
using Timberborn.Stockpiles;
using Timberborn.WaterBuildings;
using Timberborn.Workshops;
using Timberborn.Yielding;
using UnityEngine;

namespace CopyStorageFilter
{
	public class CopyTool
	{
		public static CopyTool instance;
		
		private readonly Dictionary<string, string> copiedFilters = new();
		private readonly Dictionary<int, RecipeSpecification> copiedRecipes = new();
		private readonly Dictionary<string, Plantable> copiedPlantables = new();
		private readonly Dictionary<string, Gatherable> copiedGatherables = new();
		private (bool clean, bool contamined) copiedWaterTypeProperty = (true, true);
		
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
			
			attemptCopyPasteAction(isLeft, out bool successful);
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
			
			successful = tryStockpile(gameObject, isPaste)
				|| tryManufacturer(gameObject, isPaste)
				|| tryPlanter(gameObject, isPaste)
				|| tryWaterMover(gameObject, isPaste)
				|| tryGatherer(gameObject, isPaste);
			
			// if (!successful)
			// {
			// 	debugObject(gameObject);
			// }
		}
		
		// private void debugObject(GameObject obj)
		// {
		// 	var sb = new StringBuilder();
		// 	sb.Append("Debugging clicked object: '").Append(obj.name).AppendLine("'");
		// 	foreach(var component in obj.GetComponents<Component>())
		// 	{
		// 		sb.Append("- ").AppendLine(component.GetType().Name);
		// 	}
		// 	Debug.Log(sb);
		// }
		
		private bool tryStockpile(GameObject gameObject, bool isPaste)
		{
			var stockpile = gameObject.GetComponent<Stockpile>();
			var singleGoodAllower = gameObject.GetComponent<SingleGoodAllower>();
			if (stockpile == null || singleGoodAllower == null)
			{
				//The inventory type must be "single good allower".
				//The building must be of stockpile type.
				return false;
			}
			
			if (isPaste)
			{
				if (copiedFilters.TryGetValue(stockpile.WhitelistedGoodType, out string filter))
				{
					if (filter == null)
					{
						singleGoodAllower.Disallow();
					}
					else
					{
						singleGoodAllower.Allow(filter);
					}
				}
			}
			else //isCopy
			{
				copiedFilters[stockpile.WhitelistedGoodType] = singleGoodAllower.AllowedGood;
			}
			
			return true;
		}
		
		private bool tryManufacturer(GameObject gameObject, bool isPaste)
		{
			var manufacturer = gameObject.GetComponent<Manufactory>();
			if (manufacturer == null)
			{
				return false;
			}
			
			var recipesHash = hash(manufacturer.ProductionRecipes);
			if (isPaste)
			{
				if (copiedRecipes.TryGetValue(recipesHash, out RecipeSpecification recipe))
				{
					manufacturer.SetRecipe(recipe);
				}
			}
			else //isCopy
			{
				copiedRecipes[recipesHash] = manufacturer.CurrentRecipe;
			}
			
			return true;
			
			int hash(IEnumerable<RecipeSpecification> values) => values
				.Select(e => e.Id)
				.Aggregate(19, (current, value) => current * 31 + value.GetHashCode());
		}
		
		private bool tryPlanter(GameObject gameObject, bool isPaste)
		{
			var planterBuilding = gameObject.GetComponent<PlanterBuilding>();
			var planterBuildingSpecs = gameObject.GetComponent<PlanterBuildingSpec>();
			var planterPriority = gameObject.GetComponent<PlantablePrioritizer>();
			if (planterBuilding == null || planterBuildingSpecs == null || planterPriority == null)
			{
				return false;
			}
			
			var key = planterBuildingSpecs.PlantableResourceGroup;
			if (isPaste)
			{
				if (copiedPlantables.TryGetValue(key, out Plantable plantable))
				{
					//Paste setting:
					planterPriority.PrioritizePlantable(plantable);
				}
			}
			else //isCopy
			{
				copiedPlantables[key] = planterPriority.PrioritizedPlantable;
			}
			
			return true;
		}
		
		private bool tryWaterMover(GameObject gameObject, bool isPaste)
		{
			var waterMover = gameObject.GetComponent<WaterMover>();
			if (waterMover == null)
			{
				return false;
			}
			
			if (isPaste)
			{
				(waterMover.CleanWaterMovement, waterMover.ContaminatedWaterMovement) = copiedWaterTypeProperty;
			}
			else //isCopy
			{
				copiedWaterTypeProperty = (waterMover.CleanWaterMovement, waterMover.ContaminatedWaterMovement);
			}
			
			return true;
		}
		
		private bool tryGatherer(GameObject gameObject, bool isPaste)
		{
			var building = gameObject.GetComponent<YieldRemovingBuilding>();
			var prioritizer = gameObject.GetComponent<GatherablePrioritizer>();
			if (building == null || prioritizer == null)
			{
				return false;
			}
			
			var key = building._yieldRemovingBuildingSpec.ResourceGroup;
			if (isPaste)
			{
				if (copiedGatherables.TryGetValue(key, out Gatherable gatherable))
				{
					prioritizer.PrioritizeGatherable(gatherable);
				}
			}
			else //isCopy
			{
				copiedGatherables[key] = prioritizer.PrioritizedGatherable;
			}
			
			return true;
		}
	}
}
