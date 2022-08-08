using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject[] selectionArrows;
    [SerializeField]
    private Text music_txt;
    [SerializeField]
    private Text sfx_txt;
    [SerializeField]
    private Text movement_txt;
    public int music_vol = 10;
    public int sfx_vol = 10;
    private string movement_opt = "SMOOTH";
    [SerializeField]
    private GameObject _gameManager;
    private GameManager gameManager;

    private int selected = 0;

    private void Start()
    {
        music_vol = MainManager.Instance.music_vol;
        sfx_vol = MainManager.Instance.sfx_vol;
        if (MainManager.Instance.instant_acceleration)
        {
            movement_opt = "SNAPPY";
        }
        else
        {
            movement_opt = "SMOOTH";
        }

        //Debug.Log(music_vol + "|" + sfx_vol + "|" + movement_opt);

        music_txt.text = "" + music_vol;
        sfx_txt.text = "" + sfx_vol;
        movement_txt.text = movement_opt;

        gameManager = _gameManager.GetComponent<GameManager>();
        updateSelected();
    }

    void updateSelected()
    {
        foreach (GameObject arrow in selectionArrows)
        {
            arrow.SetActive(false);
        }
        selectionArrows[selected].SetActive(true);
    }

    void updateSettings()
    {
        MainManager.Instance.music_vol = music_vol;
        MainManager.Instance.sfx_vol = sfx_vol;
        if (movement_opt == "SMOOTH")
        {
            MainManager.Instance.instant_acceleration = false;
        }
        else
        {
            MainManager.Instance.instant_acceleration = true;
        }

        gameManager.updateSettings();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            selected--;
            if (selected < 0) selected = selectionArrows.Length - 1;
            updateSelected();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            selected++;
            if (selected > selectionArrows.Length - 1) selected = 0;
            updateSelected();
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            switch (selected)
            {
                case 0:
                    music_vol--;
                    if (music_vol < 0) music_vol = 0;
                    music_txt.text = "" + music_vol;
                    break;
                case 1:
                    sfx_vol--;
                    if (sfx_vol < 0) sfx_vol = 0;
                    sfx_txt.text = "" + sfx_vol;
                    break;
                case 2:
                    if (movement_opt == "SMOOTH") movement_opt = "SNAPPY";
                    else movement_opt = "SMOOTH";
                    movement_txt.text = movement_opt;
                    break;
            }
            updateSettings();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            switch (selected)
            {
                case 0:
                    music_vol++;
                    if (music_vol > 10) music_vol = 10;
                    music_txt.text = "" + music_vol;
                    break;
                case 1:
                    sfx_vol++;
                    if (sfx_vol > 10) sfx_vol = 10;
                    sfx_txt.text = "" + sfx_vol;
                    break;
                case 2:
                    if (movement_opt == "SMOOTH") movement_opt = "SNAPPY";
                    else movement_opt = "SMOOTH";
                    movement_txt.text = movement_opt;
                    break;
            }
            updateSettings();
        }

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (selected == 3)
            {
                Application.Quit();
            }
        }
    }
}
