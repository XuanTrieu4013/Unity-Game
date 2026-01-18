using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PickupSpawner : MonoBehaviour
{
   [SerializeField] private GameObject goinCoin, healthGlobe, staminaGlobe;

   public void DropItem()
    {
        int randomNum = Random.Range(1, 5);

        if(randomNum == 1)
        {
            Instantiate(healthGlobe, transform.position,Quaternion.identity);
        }
        if(randomNum == 2)
        {
            Instantiate(staminaGlobe, transform.position,Quaternion.identity);
        }
        if(randomNum == 3)
        {
            int ramdomAmountOfGold = Random.Range(1, 4);

            for(int i = 0; i < ramdomAmountOfGold; i++)
            {
                Instantiate(goinCoin, transform.position, Quaternion.identity);
            }
        }
    }
}
