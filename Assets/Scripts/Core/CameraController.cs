using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float panSpeed = 30f;
    public float panBorderThickness = 10f;

    public float zoomMin = 10f;
    public float zoomMax = 80f;

    Vector3 touchStart;
    bool cameraIsMove = false;

    RaycastHit HitInfo;

    void Update()
    {
        transform.position = new(transform.position.x, 50, transform.position.z);

        if (Input.GetKey("w") && !Input.GetKey("q") && !Input.GetKey("e"))
        {
            transform.Translate(Vector3.up * panSpeed * Time.deltaTime);
        }
        if (Input.GetKey("s") && !Input.GetKey("q") && !Input.GetKey("e"))
        {
            transform.Translate(Vector3.down * panSpeed * Time.deltaTime);
        }
        if (Input.GetKey("d") && !Input.GetKey("q") && !Input.GetKey("e"))
        {
            transform.Translate(Vector3.right * panSpeed * Time.deltaTime);
        }
        if (Input.GetKey("a") && !Input.GetKey("q") && !Input.GetKey("e"))
        {
            transform.Translate(Vector3.left * panSpeed * Time.deltaTime);
        }
        if (Input.GetKey("q"))
        {
            transform.RotateAround(HitInfo.point, Vector3.up, 90 * Time.deltaTime);
        }
        if (Input.GetKey("e"))
        {
            transform.RotateAround(HitInfo.point, -Vector3.up, 90 * Time.deltaTime);
        }

        if (!Input.GetKey("w") && !Input.GetKey("s") && !Input.GetKey("d") && !Input.GetKey("a") && !Input.GetKey("q") && !Input.GetKey("e"))
        {
            Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out HitInfo, 100.0f);
        }

        // Hacer zoom con el ratón
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        float size = Camera.main.orthographicSize;

        if (Input.GetMouseButtonDown(0))
        {
            touchStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            cameraIsMove = true;
        }
        if (Input.GetMouseButton(0) && cameraIsMove)
        {
            Vector3 direction = touchStart - Camera.main.ScreenToWorldPoint(Input.mousePosition);
            direction = new Vector3(direction.x, 0, direction.z);
            Camera.main.transform.position += direction;
        }

        if (Input.GetMouseButtonUp(0))
        {
            cameraIsMove = false;
        }

        Zoom(scroll, size);
    }

    public void Zoom(float increment, float size)
    {
        if (increment < 0)
        {
            size += 5;
        }
        else if (increment > 0)
        {
            size -= 5;
        }

        size = Mathf.Clamp(size, zoomMin, zoomMax);
        Camera.main.orthographicSize = size;
    }
}
