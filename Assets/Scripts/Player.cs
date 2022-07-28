using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private Vector2 startingPosition = new Vector2(3.5f, -4f);
    [SerializeField]
    private float speed = 5.0f;
    private int ammo = 0;
    private int maxAmmo = 6;
    [SerializeField]
    private GameObject pasta;
    private bool forcefieldActive = false;
    [SerializeField]
    private GameObject forcefield;
    private int forcefieldTime = 10;
    private int forcefieldTimeRemaining;
    private float kitchenLoadupTime = 3f;
    private float kitchenReloadTime = 10f;
    private float kitchenReloadTimeRemaining;
    private int kitchenPlates = 6;
    private bool kitchenReady = true;
    private bool isInKitchen = false;
    [SerializeField]
    private GameObject kitchenObj;
    private Kitchen kitchen;
    [SerializeField]
    private GameObject UI_Manager;
    private UIManager uiManager;
    [SerializeField]
    private AudioClip throw_sfx;
    [SerializeField]
    private AudioClip shield_sfx;
    [SerializeField]
    private AudioClip shield_powerdown_sfx;
    [SerializeField]
    private AudioClip pickup_sfx;
    [SerializeField]
    private AudioClip fail_sfx;
    [SerializeField]
    private AudioClip kitchen_ready_sfx;
    private AudioSource source;
    private bool playingFailSFX = false;
    [SerializeField]
    private bool isInvincible = false;
    [SerializeField]
    private bool instantAcceleration = false;
    private float oldHorizontalAcceleration = 0;
    private float oldVerticalAcceleration = 0;


    // Start is called before the first frame update
    void Start()
    {
        transform.position = startingPosition;
        uiManager = UI_Manager.GetComponent<UIManager>();
        source = transform.GetComponent<AudioSource>();
        kitchen = kitchenObj.GetComponent<Kitchen>();
        uiManager.updateAmmo(ammo);
    }

    private void movePlayer()
    {
        // Player movement
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        if (instantAcceleration)
        {
            if (horizontalInput > oldHorizontalAcceleration)
            {
                if (horizontalInput > 0) horizontalInput = 0.99f;
                if (horizontalInput < 0) horizontalInput = 0;
            }
            else if (horizontalInput < oldHorizontalAcceleration)
            {
                if (horizontalInput > 0) horizontalInput = 0;
                if (horizontalInput < 0) horizontalInput = -0.99f;
            }

            if (verticalInput > oldVerticalAcceleration)
            {
                if (verticalInput > 0) verticalInput = 0.99f;
                if (verticalInput < 0) verticalInput = 0;
            }
            else if (verticalInput < oldVerticalAcceleration)
            {
                if (verticalInput > 0) verticalInput = 0;
                if (verticalInput < 0) verticalInput = -0.99f;
            }

            oldHorizontalAcceleration = Input.GetAxis("Horizontal");
            oldVerticalAcceleration = Input.GetAxis("Vertical");
        }

        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);

        transform.Translate(direction * speed * Time.deltaTime, Space.World);

        // Change where player looks
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        ray.origin = new Vector3(ray.origin.x, ray.origin.y, 0);
        transform.up = ray.origin - transform.position;
    }

    private void shootPasta()
    {
        if (Input.GetMouseButtonDown(0) && ammo > 0)
        {
            ammo--;
            source.PlayOneShot(throw_sfx, 1);
            uiManager.updateAmmo(ammo);
            Instantiate(pasta, transform.position, transform.rotation);
        }
        else if (Input.GetMouseButtonDown(0))
        {
            source.PlayOneShot(fail_sfx, 1);
            uiManager.emptyWarning();
        }
    }

    private void collectItem(GameObject item)
    {
        // if the item is pasta, increase ammo by 1
        if (item.transform.name == "Pasta_Pickup(Clone)" && ammo < maxAmmo)
        {
            source.PlayOneShot(pickup_sfx, 0.6f);
            ammo++;
            uiManager.updateAmmo(ammo);
            Destroy(item);
        }
        else if (item.transform.name == "Pasta_Pickup(Clone)" && playingFailSFX != true)
        {
            playingFailSFX = true;
            uiManager.fullWarning();
            source.PlayOneShot(fail_sfx, 1);
        }
        else if (item.transform.name == "Force_Field_Pickup(Clone)")
        {
            // has force field
            activateForceField();
            Destroy(item);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Pickup") && Input.GetAxis("Jump") > 0)
        {
            collectItem(collision.gameObject);
        }
    }

    private void checkKitchen()
    {
        if (Input.GetAxis("Jump") > 0 && kitchenReady && isInKitchen)
        {
            kitchenReady = false;
            float secondsPerPlate = Mathf.Round((kitchenLoadupTime / kitchenPlates) * 10) / 10;
            kitchenReloadTimeRemaining = kitchenLoadupTime;
            StartCoroutine(kitchenLoadup(secondsPerPlate));
        }
    }

    private IEnumerator kitchenLoadup(float spp)
    {
        int platesGained = 0;

        while (Input.GetAxis("Jump") > 0 && kitchenReloadTimeRemaining > 0 && isInKitchen)
        {
            if (kitchenReloadTimeRemaining > (spp + 0.01f) && kitchenReloadTimeRemaining % spp < 0.01)
            {
                if (ammo >= maxAmmo)
                {
                    source.PlayOneShot(fail_sfx, 1);
                    uiManager.fullWarning();
                    break;
                }
                else
                {
                    ammo++;
                    platesGained++;
                    uiManager.updateKitchenMeter(platesGained, false);
                    source.PlayOneShot(pickup_sfx, 0.5f);
                    uiManager.updateAmmo(ammo);
                }
            }

            yield return new WaitForSeconds(0.01f);
            kitchenReloadTimeRemaining -= 0.01f;
        }
        StartCoroutine(kitchenReload());
    }

    private IEnumerator kitchenReload()
    {
        uiManager.startReload(kitchenPlates, kitchenReloadTime);
        kitchen.kitchenReady = false;
        yield return new WaitForSeconds(kitchenReloadTime);

        source.PlayOneShot(kitchen_ready_sfx, 1);
        kitchen.kitchenReady = true;
        kitchenReady = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Target"))
        {
            isInKitchen = true;
        }

        if (collision.CompareTag("bullet"))
        {
            if (forcefieldActive)
            {
                forcefieldActive = false;
            }
            else if (isInvincible != true)
            {
                // You died
                Destroy(collision.gameObject);
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Target"))
        {
            isInKitchen = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Enemy"))
        {
            if (forcefieldActive)
            {
                forcefieldActive = false;
            }
            else
            {
                // You died
                Destroy(collision.gameObject);
                Destroy(gameObject);
            }
        }
    }

    private IEnumerator forceFieldRoutine()
    {
        while (forcefieldActive)
        {
            // update UI
            yield return new WaitForSeconds(1);
            forcefieldTimeRemaining--;
            //Debug.Log("Seconds Remaining: " + forcefieldTimeRemaining);

            if (forcefieldTimeRemaining <= 0)
            {
                break;
            }
        }
        deactivateForceField();
    }

    private void activateForceField()
    {
        if (forcefieldActive == false)
        {
            source.PlayOneShot(shield_sfx, 0.4f);
            forcefieldActive = true;
            forcefield.SetActive(true);
            forcefieldTimeRemaining = forcefieldTime;
            uiManager.updateShield(forcefieldTimeRemaining);
            StartCoroutine(forceFieldRoutine());
        }
        else
        {
            // force field is already active, collected another one
            forcefieldTimeRemaining = forcefieldTime;
            uiManager.updateShield(forcefieldTimeRemaining);
        }
    }

    private void deactivateForceField()
    {
        source.Stop();
        source.PlayOneShot(shield_powerdown_sfx, 0.8f);
        forcefieldActive = false;
        forcefield.SetActive(false);
        uiManager.updateShield(0);
    }

    public void updateKitchen(float[] kitchenData)
    {
        kitchenReloadTime = kitchenData[0];
        kitchenLoadupTime = kitchenData[1];
    }

    private void FixedUpdate()
    {
        movePlayer();
    }

    // Update is called once per frame
    void Update()
    {
        shootPasta();
        checkKitchen();

        if (Input.GetKeyUp(KeyCode.Space)) playingFailSFX = false;
    }
}
