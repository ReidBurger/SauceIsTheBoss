using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject shieldMeter;
    [SerializeField]
    private GameObject[] ammoSprites;

    private void Start()
    {

    }

    public void updateAmmo(int ammo)
    {
        foreach (GameObject ammo_sprite in ammoSprites)
        {
            ammo_sprite.SetActive(false);
        }
        for (int i = 0; i < ammo; i++)
        {
            ammoSprites[i].SetActive(true);
        }
    }

    public void updateShield(int time)
    {
        // stop animation

        if (time > 0)
        {
            // play animation for time seconds
        }
    }

    public void updateKillsCounter(int kills, int quota)
    {
        Text killCounter = transform.Find("Kills").GetComponent<Text>();
        killCounter.text = "Kills: " + kills + " / " + quota;
    }
}
