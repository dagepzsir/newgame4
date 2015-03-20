using UnityEngine;
using UnityEngine.Rendering;

using System.Collections;
public class CameraController : MonoBehaviour
{

    public Canvas pauseMenu;
    public Canvas openFileDialog;

    public bool EnableMovement = true;
    public float MaxSpeed = 2;
    public float Acceleration = 1;
    public float MaxZoom = 4;
    public float MinZoom = 1;
    public float ZoomBy = 0.5f;
    private float currentSpeedX;
    private float targetSpeedX;
    private float currentSpeedY;
    private float targetSpeedY;

    void Update()
    {
        if (EnableMovement)
        {
            targetSpeedX = Input.GetAxisRaw("Horizontal") * MaxSpeed;
            currentSpeedX = IncrementTowards(currentSpeedX, targetSpeedX, Acceleration);

            targetSpeedY = Input.GetAxisRaw("Vertical") * MaxSpeed;
            currentSpeedY = IncrementTowards(currentSpeedY, targetSpeedY, Acceleration);

            if (Input.GetAxis("Fire1") != 0)
            {

                currentSpeedX = MaxSpeed * -Input.GetAxis("Mouse X");
                currentSpeedY = MaxSpeed * -Input.GetAxis("Mouse Y");
            }

            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                if (GetComponent<Camera>().orthographicSize >= MinZoom)
                    GetComponent<Camera>().orthographicSize -= ZoomBy;
            }
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                if (GetComponent<Camera>().orthographicSize <= MaxZoom)
                    GetComponent<Camera>().orthographicSize += ZoomBy;
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {

            if (pauseMenu.enabled == false)
            {
                Time.timeScale = 0;
                pauseMenu.enabled = true;
                currentSpeedX = 0;
                currentSpeedY = 0;
                //manager.isPaused = true;
                GetComponent<BlurEffect>().enabled = true;
            }
            else
            {
                if (openFileDialog != null)
                {
                    if (openFileDialog.enabled)
                    {
                        openFileDialog.enabled = false;
                    }
                    else
                    {
                        Time.timeScale = 1;
                        pauseMenu.enabled = false;
                        //manager.isPaused = false;
                        GetComponent<BlurEffect>().enabled = false;
                    }
                }
                else
                {
                    Time.timeScale = 1;
                    pauseMenu.enabled = false;
                    //manager.isPaused = false;
                    GetComponent<BlurEffect>().enabled = false;
                }
            }
        }

        transform.Translate(currentSpeedX, currentSpeedY, 0);

    }
    void Start()
    {
        pauseMenu.enabled = false;
    }
    private float IncrementTowards(float n, float target, float a)
    {
        if (n == target)
        {
            return n;
        }
        else
        {
            float dir = Mathf.Sign(target - n); // must n be increased or decreased to get closer to target
            n += a * Time.deltaTime * dir;
            return (dir == Mathf.Sign(target - n)) ? n : target; // if n has now passed target then return target, otherwise return n
        }
    }
}