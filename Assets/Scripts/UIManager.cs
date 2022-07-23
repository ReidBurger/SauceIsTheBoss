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
    [SerializeField]
    private GameObject[] progressBarSprites;
    [SerializeField]
    private GameObject empty;
    [SerializeField]
    private GameObject full;
    [SerializeField]
    private GameObject round_txt;
    [SerializeField]
    private GameObject round_num_txt;
    [SerializeField]
    private Sprite[] round_num_sprites;

    public void emptyWarning()
    {
        StartCoroutine(Fade(empty, 0.2f));
    }

    public void fullWarning()
    {
        StartCoroutine(Fade(full, 0.2f));
    }

    private IEnumerator Fade(GameObject sprite, float duration)
    {
        Color start = new Color(1, 1, 1, 1.0f);
        Color end = new Color(1, 1, 1, 0f);

        sprite.SetActive(true);

        SpriteRenderer renderer = sprite.GetComponent<SpriteRenderer>();
        Image image = sprite.GetComponent<Image>();

        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            float normalizedTime = t / duration;

            if (renderer != null)
            {
                renderer.color = Color.Lerp(start, end, normalizedTime);
            }

            if (image != null)
            {
                image.color = Color.Lerp(start, end, normalizedTime);
            }

            yield return null;
        }
        sprite.SetActive(false);
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

    public void roundDisplay(int roundNum)
    {
        Image img = round_num_txt.GetComponent<Image>();
        img.sprite = round_num_sprites[roundNum];

        StartCoroutine(Fade(round_num_txt, 1.5f));
        StartCoroutine(Fade(round_txt, 1.5f));
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
        int roundedProgress = (int)(kills * 10 / quota);

        foreach (GameObject bar_sprite in progressBarSprites)
        {
            bar_sprite.SetActive(false);
        }
        progressBarSprites[roundedProgress].SetActive(true);
    }
}
