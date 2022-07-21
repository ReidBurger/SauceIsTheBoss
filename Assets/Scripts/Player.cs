using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private Vector2 startingPosition = new Vector2(0, 0);
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
    private GameObject UI_Manager;
    private UIManager UIManager; 

    // Start is called before the first frame update
    void Start()
    {
        transform.position = startingPosition;
        UIManager = UI_Manager.GetComponent<UIManager>();
        UIManager.updateAmmo(ammo);
    }

    private void movePlayer()
    {
        // Player movement
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

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
            UIManager.updateAmmo(ammo);
            Instantiate(pasta, transform.position, transform.rotation);
        }
    }

    private void collectItem(GameObject item)
    {
        // if the item is pasta, increase ammo by 1
        if (item.transform.name == "Pasta_Pickup(Clone)" && ammo < maxAmmo)
        {
            ammo++;
            UIManager.updateAmmo(ammo);
            Destroy(item);
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
        if (Input.GetAxis("Jump") > 0 && ammo < maxAmmo && kitchenReloadTimeRemaining > 0 && isInKitchen)
        {
            if (kitchenReloadTimeRemaining % spp < 0.01)
            {
                ammo++;
                UIManager.updateAmmo(ammo);
            }

            yield return new WaitForSeconds(0.1f);
            kitchenReloadTimeRemaining -= 0.1f;
            StartCoroutine(kitchenLoadup(spp));
        }
        else
        {
            Debug.Log("Reloading Kitchen...");
            StartCoroutine(kitchenReload());
        }
    }

    private IEnumerator kitchenReload()
    {
        yield return new WaitForSeconds(kitchenReloadTime);
        Debug.Log("Kitchen Ready!");
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
                deactivateForceField();
            }
            else
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
                deactivateForceField();
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
        if (forcefieldActive == true)
        {
            // update UI
            yield return new WaitForSeconds(1);
            forcefieldTimeRemaining--;
            //Debug.Log("Seconds Remaining: " + forcefieldTimeRemaining);

            if (forcefieldTimeRemaining <= 0 || forcefieldActive == false)
            {
                deactivateForceField();
            }

            StartCoroutine(forceFieldRoutine());
        }
        else
        {
            deactivateForceField();
        }
    }

    private void activateForceField()
    {
        if (forcefieldActive == false)
        {
            forcefieldActive = true;
            forcefield.SetActive(true);
            forcefieldTimeRemaining = forcefieldTime;
            UIManager.updateShield(forcefieldTimeRemaining);
            StartCoroutine(forceFieldRoutine());
        }
        else
        {
            // force field is already active, collected another one
            forcefieldTimeRemaining = forcefieldTime;
            UIManager.updateShield(forcefieldTimeRemaining);
        }
    }

    private void deactivateForceField()
    {
        forcefieldActive = false;
        forcefield.SetActive(false);
        UIManager.updateShield(0);
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
    }
}