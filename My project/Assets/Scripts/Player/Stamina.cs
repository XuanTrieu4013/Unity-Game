using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stamina : Singleton<Stamina>
{
    public int CurrentStamina {get; private set;}
    

    [SerializeField] private Sprite fullStaminaImage, emptyStanimaImage;
    [SerializeField] private int timeBetweenStanimaRefresh = 3;

    private Transform stanimaContainer;
    private int startingStanima = 3;
    private int maxStanima;
    const string STAMINA_CONTAINER_TEXT = "Stamina Container";

    protected override void Awake()
    {
        base.Awake();

        maxStanima = startingStanima;
        CurrentStamina = startingStanima;
    }

    private void Start()
    {
        stanimaContainer = GameObject.Find(STAMINA_CONTAINER_TEXT).transform;
    }

    public void UseStamina()
    {
        CurrentStamina -- ;
        UpdateStaminaImages();
    }

    public void RefreshStamina()
    {
        if(CurrentStamina < maxStanima)
        {
            CurrentStamina++;
        }
        UpdateStaminaImages();
    }

    private IEnumerator RefreshStaminaRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeBetweenStanimaRefresh);
            RefreshStamina();
        }
        
    }
    private void UpdateStaminaImages()
    {
        for(int i = 0; i <maxStanima; i++)
        {
            if(i <= CurrentStamina - 1)
            {
                stanimaContainer.GetChild(i).GetComponent<Image>().sprite = fullStaminaImage;
            }
            else
            {
                stanimaContainer.GetChild(i).GetComponent<Image>().sprite = emptyStanimaImage;
            }
        }
    }
}
