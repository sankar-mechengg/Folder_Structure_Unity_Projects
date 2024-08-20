using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Maths = System.Math;

public class ScaleObject : MonoBehaviour
{
    //Create a reference to the UDP_Receiver_IMU_Pi script
    public UDP_Receiver_FORCE_ESP8266 udpReceiverForce;

    //Get the transform of a GameObject
    public Transform objectTransform;

    int previous_f_value;

    // Start is called before the first frame update
    void Start()
    {
        previous_f_value = 0;
    }

    // Update is called once per frame
    void Update() { }

    void LateUpdate()
    {
        //Get the values of i,j,k from the queue
        //Check if the queue is not empty
        if (udpReceiverForce.FQueue.Count > 0)
        {
            //Get the values from the queue
            int f_value = udpReceiverForce.FQueue.Dequeue();

            // The i,j,k values constantly change as the IMU sensor moves hence the object will rotate continuously
            // calculate the difference between the current value and the previous value and only if it is greater than a certain threshold, rotate the object
            // This will prevent the object from rotating when the sensor is stationary
            // The threshold value can be adjusted based on the sensitivity of the sensor

            // calculate the difference between the current value and the previous value and only if it is greater than a certain threshold, rotate the object
            int current_f_value = f_value;

            int threshold = 20;

            //Scale the object based on the value received from the force sensor
            if (Maths.Abs(current_f_value - previous_f_value) > threshold)
            {
                //Scale the object based on the value received from the force sensor
                objectTransform.localScale = new Vector3(1, 1, 1) * (current_f_value / 500.0f);
            }

            // Update the previous values
            previous_f_value = current_f_value;
        }
    }
}
