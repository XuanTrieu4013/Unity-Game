using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PickupSpawner : MonoBehaviour
{
   [SerializeField] private GameObject goinCoinPrefab;

   public void DropItem()
    {
        Instantiate(goinCoinPrefab, transform.position, Quaternion.identity);
    }
}
