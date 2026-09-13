using System.Collections.Generic;
using UnityEngine;

public class Stove : MonoBehaviour, IInteractable
{
    [SerializeField] private float _cookingTime = 6f; // Time in seconds to cook the meat

    [SerializeField] private List<CookingSlot> _cookingSlots = new(); // List of cooking slots on the stove


    private void Start()
    {
        //safer case handling if the references missed.
        if (_cookingSlots.Count == 0)
        {
            foreach (Transform t in transform)
                _cookingSlots.Add(t.GetComponent<CookingSlot>());
        }
    }

    public void Interact(PlayerController player)
    {
        // Try to collect a finished item first
        foreach (CookingSlot slot in _cookingSlots)
        {
            if (slot.State == CookingState.Finished)
            {
                if (player.TryGrabItem(slot.CookedItem))
                {
                    slot.ClearSlot();
                    return;
                }
            }
        }

        // Otherwise, try to place meat into an empty slot
        if (player.GetHeldItemType() == IngredientType.Meat)
        {
            foreach (CookingSlot slot in _cookingSlots)
            {
                if (slot.State == CookingState.Empty)
                {
                    player.PlaceItem(slot.ItemPlacementPoint);
                    slot.StartCooking(_cookingTime);
                    return;
                }
            }
        }
    }
}
