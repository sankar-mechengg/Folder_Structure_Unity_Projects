using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Maths = System.Math;

public class RotateObject : MonoBehaviour
{
    //Create a reference to the UDP_Receiver_IMU_Pi script
    public UDP_Receiver_IMU_ESP32 udpReceiverIMU;

    //Get the transform of a GameObject
    public Transform objectTransform;

    float previous_i_value = 0.0f;
    float previous_j_value = 0.0f;
    float previous_k_value = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        previous_i_value = 0.0f;
        previous_j_value = 0.0f;
        previous_k_value = 0.0f;
    }

    // Update is called once per frame
    void Update() { }

    void LateUpdate()
    {
        //Get the values of i,j,k from the queue
        //Check if the queue is not empty
        if (
            udpReceiverIMU.IQueue.Count > 0
            && udpReceiverIMU.JQueue.Count > 0
            && udpReceiverIMU.KQueue.Count > 0
        )
        {
            //Get the values from the queue
            float i_value = udpReceiverIMU.IQueue.Dequeue();
            float j_value = udpReceiverIMU.JQueue.Dequeue();
            float k_value = udpReceiverIMU.KQueue.Dequeue();
            //Debug.Log("i Value: " + i_value);
            //Debug.Log("j Value: " + j_value);
            //Debug.Log("k Value: " + k_value);

            // The i,j,k values constantly change as the IMU sensor moves hence the object will rotate continuously
            // calculate the difference between the current value and the previous value and only if it is greater than a certain threshold, rotate the object
            // This will prevent the object from rotating when the sensor is stationary
            // The threshold value can be adjusted based on the sensitivity of the sensor

            // calculate the difference between the current value and the previous value and only if it is greater than a certain threshold, rotate the object
            float current_i_value = i_value;
            float current_j_value = j_value;
            float current_k_value = k_value;

            float threshold = 3.0f;
            if (Maths.Abs(current_i_value - previous_i_value) > threshold)
            {
                // Rotate the object around the x-axis
                objectTransform.Rotate(Vector3.right, current_i_value - previous_i_value);
            }
            if (Maths.Abs(current_j_value - previous_j_value) > threshold)
            {
                // Rotate the object around the y-axis
                objectTransform.Rotate(Vector3.up, current_j_value - previous_j_value);
            }
            if (Maths.Abs(current_k_value - previous_k_value) > threshold)
            {
                // Rotate the object around the z-axis
                objectTransform.Rotate(Vector3.forward, current_k_value - previous_k_value);
            }

            // Update the previous values
            previous_i_value = current_i_value;
            previous_j_value = current_j_value;
            previous_k_value = current_k_value;
        }
    }
}
