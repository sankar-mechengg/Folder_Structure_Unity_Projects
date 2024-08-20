using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HTTP_WebServer_Receiver_ESP32 : MonoBehaviour
{
    public GameObject targetObject; // Assign this in the Inspector
    private string baseUrl = "192.168.4.1/";

    public float xAngle;
    public float yAngle;
    public float zAngle;

    void Start()
    {
        if (targetObject == null)
        {
            Debug.LogError("Target object not assigned.");
            return;
        }

        // Start the coroutine to fetch Euler angles
        StartCoroutine(FetchEulerAngles());
    }

    IEnumerator FetchEulerAngles()
    {
        while (true) // Use a condition to stop this loop when needed
        {
            float x = 0,
                y = 0,
                z = 0;

            // Fetch each Euler angle individually and apply it
            yield return StartCoroutine(FetchEulerAngle("x", angle => x = angle));
            yield return StartCoroutine(FetchEulerAngle("y", angle => y = angle));
            yield return StartCoroutine(FetchEulerAngle("z", angle => z = angle));

            // Debug the Euler angles
            Debug.Log("Euler Angles: x=" + x + ", y=" + y + ", z=" + z);

            //Use the Euler angles to rotate the target object
            targetObject.transform.eulerAngles = new Vector3(x, y, z);

            // Wait a bit before fetching again
            yield return new WaitForSeconds(0.5f); // Adjust the wait time as needed
        }
    }

    IEnumerator FetchEulerAngle(string axis, System.Action<float> yield)
    {
        UnityWebRequest request = UnityWebRequest.Get(baseUrl + axis);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            // Parse the response text and return the angle
            if (float.TryParse(request.downloadHandler.text, out float angle))
            {
                yield return angle;
                Debug.Log("Fetched angle for axis " + axis + ": " + angle);
            }
            else
            {
                Debug.LogError("Failed to parse angle for axis " + axis);
                yield return 0f;
            }
        }
        else
        {
            Debug.LogError("Error fetching angle for axis " + axis + ": " + request.error);
            yield return 0f;
        }
    }
}
