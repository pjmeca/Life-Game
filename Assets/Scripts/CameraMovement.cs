using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public CellController controller;
    [Range(0, 30f)]
    public float speed;

    // Its important that this is a Start and not an Awake, because we want
    // this to run after the right size is set up in the controller
    void Start()
    {
        // Center the camera
        int pos = controller.getSize() / 2;
        transform.position = new Vector3(pos, pos, transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        float x, y;
        x = Input.GetAxisRaw("Mouse X");
        y = Input.GetAxisRaw("Mouse Y");

        if (Input.touchCount > 0)
        {
            Debug.Log("Touch movement");
            x = Input.touches[0].deltaPosition.x;
            y = Input.touches[0].deltaPosition.y;
        } else if(Input.touchCount > 1)
            return; // If more than one finger, do not move so we can make zoom
        
        float speedConverted = Input.touchCount == 0 ? speed : speed/50;

        if (Input.GetMouseButton(0))
            transform.position += new Vector3(-x * Time.deltaTime * speedConverted, -y * Time.deltaTime * speedConverted, 0f);
    }
}
