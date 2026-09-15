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

    public void Interact(PlayerController m_player)
    {
        // Try to collect a finished item first
        foreach (CookingSlot slot in _cookingSlots)
        {
            if (slot.CookingState == CookingState.Finished)
            {
                if (m_player.TryGrabItem(slot.CookedItem))
                {
                    slot.ClearSlot();
                    return;
                }
            }
        }

        // Otherwise, try to place meat into an empty slot
        if (m_player.GetHeldItemType() == IngredientType.Meat)
        {
            foreach (CookingSlot slot in _cookingSlots)
            {
                if (slot.CookingState == CookingState.Empty)
                {
                    m_player.PlaceItem(slot.ItemPlacementPoint);
                    slot.StartCooking(_cookingTime);
                    return;
                }
            }
        }
    }

    public void ResetStoves()
    {
        foreach (CookingSlot slot in _cookingSlots)
        {
            slot.ResetStove();
        }
    }

}
