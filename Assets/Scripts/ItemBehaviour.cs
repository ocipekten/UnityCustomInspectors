using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBehaviour : MonoBehaviour
{
    public Item item;

    private void OnTriggerEnter(Collider other)
    {
        //if (other.TryGetComponent(out PlayerMovement player))
        //{
        //    Destroy(gameObject);
        //}        
    }
}
