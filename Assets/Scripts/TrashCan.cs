using UnityEngine;

public class TrashCan : MonoBehaviour, IInteractable
{
    public void Interact(PlayerController m_playerController)
    {
        m_playerController.DropItem();
    }
}
