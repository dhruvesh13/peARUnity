using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class touchEvents : MonoBehaviour
{

    public float perspectiveZoomSpeed=0.005f;        // The rate of change of the field of view in perspective mode.
    public float orthoZoomSpeed = 0.5f;        // The rate of change of the orthographic size in orthographic mode.

    public Vector3 maxScale, minScale;
    float initialFingersDistance;
    Vector3 initialScale;


    // Use this for initialization
    void Start()
    {


        maxScale = 2 * transform.localScale;
        minScale = 0.1f * transform.localScale;
        transform.Rotate(Vector3.forward * 17.0f * Time.deltaTime);

    }
    // Update is called once per frame
    void Update()
    {
        //transform.Rotate(Vector3.forward * 17.0f * Time.deltaTime);

        if (Input.touchCount == 1)
        {
            Touch touch0 = Input.GetTouch(0);
            //Debug.Log(Input.touchCount);
            if (touch0.phase == TouchPhase.Moved)
            {

                transform.Rotate(0f, 0f, -touch0.deltaPosition.x);
            }
            
            if (touch0.phase == TouchPhase.Began)
            {
                
                transform.Rotate(0f, 0f, 0);
            }
            if (touch0.phase == TouchPhase.Ended)
            {
               
               // transform.Rotate(Vector3.forward * 17.0f * Time.deltaTime);
            }

        }       
        else if (Input.touchCount == 2)
        {
            Touch t1 = Input.touches[0];
            Touch t2 = Input.touches[1];
            Debug.Log("2 Touch");
            if (t1.phase == TouchPhase.Began || t2.phase == TouchPhase.Began)
            {
                initialFingersDistance = Vector2.Distance(t1.position, t2.position);
                initialScale = transform.localScale;
            }
            else if (t1.phase == TouchPhase.Moved || t2.phase == TouchPhase.Moved)
            {
                var currentFingersDistance = Vector2.Distance(t1.position, t2.position);
                var scaleFactor = currentFingersDistance / initialFingersDistance;
                transform.localScale = initialScale * scaleFactor;
            }
        }
        else
        {
            transform.Rotate(Vector3.forward * 17.0f * Time.deltaTime);
        }
        
    }
}




