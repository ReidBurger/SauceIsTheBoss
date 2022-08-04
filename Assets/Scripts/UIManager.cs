using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class UIManager : MonoBehaviour
{
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
    [SerializeField]
    private GameObject[] kitchenMeterSprites;
    [SerializeField]
    private GameObject[] shieldSprites;
    [SerializeField]
    private GameObject gameOverScreen;

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

        for (float t = 0; t < duration; t += Time.deltaTime)
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

        StartCoroutine(Fade(round_num_txt, 2));
        StartCoroutine(Fade(round_txt, 2));
    }

    public void updateShield(int time, int total)
    {
        int ratioLeft = time * 8 / total;
        if (ratioLeft > total) ratioLeft = total;

        foreach (GameObject sprite in shieldSprites)
        {
            sprite.SetActive(false);
        }

        shieldSprites[ratioLeft].SetActive(true);
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

    public void startReload(int plates, float time)
    {
        StartCoroutine(reloadingPlatesUI(plates, time));
    }

    public IEnumerator reloadingPlatesUI(int plates, float time)
    {
        float spp = time / plates;
        int platesReady = 0;

        updateKitchenMeter(platesReady, true);

        while (platesReady < plates)
        {
            yield return new WaitForSeconds(spp);
            platesReady++;
            updateKitchenMeter(platesReady, true);
        }
    }

    public void updateKitchenMeter(int plates, bool filling)
    {
        foreach (GameObject meterSprite in kitchenMeterSprites)
        {
            meterSprite.SetActive(false);
        }

        if (filling)
        {
            kitchenMeterSprites[plates].SetActive(true);
        }
        else
        {
            if (plates == 6) plates = -6;
            kitchenMeterSprites[plates + 6].SetActive(true);
        }
    }

    public void displayGameOver(bool display, int kills = 0, int thrown = 1)
    {
        Transform _scoreInfo = gameOverScreen.transform.Find("ScoreInfo");
        if (_scoreInfo != null)
        {
            int accuracy = 0;
            if (thrown != 0)
            {
                accuracy = kills * 100 / thrown;
            }
            float accuracyMultiplier = (float)accuracy * 4.0f / 100.0f;
            int finalScore = (int)(kills * 10 * accuracyMultiplier);

            Text text = _scoreInfo.GetComponent<Text>();
            text.text = "Enemies Splatted: " + kills + " (+" + kills * 10 + ")\nSauce Slung: " +
                thrown + "\nAccuracy: " + accuracy + "% (x" + accuracyMultiplier +
                ")\n______________________\n\nFinal Score: " + finalScore;
        }

        gameOverScreen.SetActive(display);
    }
}
