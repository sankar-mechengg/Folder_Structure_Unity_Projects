using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class UDP_Receiver_IMU_Pi : MonoBehaviour
{
    public int Port; // Port number to listen on
    private UdpClient _ReceiveClientIMU; // Receiving UdpClient
    private Thread _ReceiveThreadIMU; // Thread to receive data. Thread is a class in System.Threading namespace

    //Create a Queue to store the values
    public Queue<float> IQueue = new Queue<float>();
    public Queue<float> JQueue = new Queue<float>();
    public Queue<float> KQueue = new Queue<float>();
    public Queue<float> WQueue = new Queue<float>();

    void Start()
    {
        Initialize();
    }

    /// <summary>
    /// Initialize objects.
    /// </summary>
    public void Initialize()
    {
        // Receive
        _ReceiveThreadIMU = new Thread(new ThreadStart(ReceiveDataIMU));
        _ReceiveThreadIMU.IsBackground = true;
        _ReceiveThreadIMU.Start();
    }

    /// <summary>
    /// Receive data with pooling.
    private void ReceiveDataIMU()
    {
        _ReceiveClientIMU = new UdpClient(Port);

        while (true)
        {
            try
            {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = _ReceiveClientIMU.Receive(ref anyIP);
                // Convert the byte array to a string
                string message = System.Text.Encoding.UTF8.GetString(data);
                //Debug.Log("Received: " + message);

                //message is of the format value1&value2&value3&value4 so we need to split the message into four parts
                string[] splitMessage = message.Split('&');
                string i_msg = splitMessage[0];
                string j_msg = splitMessage[1];
                string k_msg = splitMessage[2];
                string w_msg = splitMessage[3];

                //If there is no data in the value, then take the value as 0
                if (i_msg == null)
                {
                    //store string as 0
                    i_msg = "0";
                }
                if (j_msg == null)
                {
                    //store string as 0
                    j_msg = "0";
                }
                if (k_msg == null)
                {
                    //store string as 0
                    k_msg = "0";
                }
                if (w_msg == null)
                {
                    //store string as 0
                    w_msg = "0";
                }

                //Convert the message to a float and store it in a queue
                float i_value = float.Parse(i_msg);
                float j_value = float.Parse(j_msg);
                float k_value = float.Parse(k_msg);
                float w_value = float.Parse(w_msg);

                //Store the value in a queue
                IQueue.Enqueue(i_value);
                JQueue.Enqueue(j_value);
                KQueue.Enqueue(k_value);
                WQueue.Enqueue(w_value);

                //Debug.Log("Queue: " + IQueue.Count);
                //Debug.Log("Queue: " + JQueue.Count);
                //Debug.Log("Queue: " + KQueue.Count);
                //Debug.Log("Queue: " + WQueue.Count);
            }
            catch (Exception err)
            {
                Debug.Log("<color=red>" + err.Message + "</color>");
            }
        }
    }

    private void OnApplicationQuit()
    {
        try
        {
            _ReceiveThreadIMU.Abort();
            _ReceiveThreadIMU = null;
            _ReceiveClientIMU.Close();
        }
        catch (Exception err)
        {
            Debug.Log("<color=red>" + err.Message + "</color>");
        }
    }
}
