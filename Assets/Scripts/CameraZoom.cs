using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    [Range(0f, 60f)]
    public float zoomSpeed = 50;
    private float touchDistance;

    // Update is called once per frame
    void Update()
    {
        float ScrollWheelChange = Input.GetAxis("Mouse ScrollWheel");
        if (ScrollWheelChange != 0)
        {
            zoom(ScrollWheelChange);
        } else if (Input.touchCount == 2)
        {
            Vector2 touch0, touch1;
            
            touch0 = Input.GetTouch(0).position;
            touch1 = Input.GetTouch(1).position;
            float newTouchDistance = Vector2.Distance(touch0, touch1);

            if(touchDistance == 0)
                touchDistance = newTouchDistance;
            else{
                float deltaTouchDistance = newTouchDistance - touchDistance;
                touchDistance = newTouchDistance;
                /*while(Mathf.Abs(deltaTouchDistance) >= 1)
                    deltaTouchDistance /= 10f;
                zoom(deltaTouchDistance);*/
                if(deltaTouchDistance != 0)
                    zoom(deltaTouchDistance > 0 ? 0.1f : -0.1f);
            }
        } else if(Input.touchCount < 2){
            touchDistance = 0; // Reset
        }
    }

    private void zoom(float value)
    {
        Debug.Log("Zooming with value: "+value);

        Vector3 tmp = Camera.main.transform.position;
        tmp += Camera.main.transform.forward * value * zoomSpeed;
        if (tmp.z < -5)
            Camera.main.transform.position = tmp;
    }
}
