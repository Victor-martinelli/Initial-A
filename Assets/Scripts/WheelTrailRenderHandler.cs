using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelTrailRenderHandler : MonoBehaviour
{
    //Object controllers
    CarController carController;
    TrailRenderer trailRenderer;

    //This function is called as the object is being loader
    void Awake()
    {

        //Get the car controller
        carController = GetComponentInParent<CarController>();

        //Get the TrailRender component
        trailRenderer = GetComponent<TrailRenderer>();

        //Stop the marks at the very beginning
        trailRenderer.emitting = false;

    }

    // Update is called once per frame
    void Update()
    {

        //Are we drifting, show the trail
        if(carController.isDrifting(out float lateralVelocity, out bool isBraking))
        {
            trailRenderer.emitting = true;
        } else
        {
            trailRenderer.emitting = false;
        }
        
    }
}
