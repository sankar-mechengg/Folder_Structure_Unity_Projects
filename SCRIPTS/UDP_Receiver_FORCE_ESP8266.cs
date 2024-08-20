using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class UDP_Receiver_FORCE_ESP8266 : MonoBehaviour
{
    public int Port; // Port number to listen on
    private UdpClient _ReceiveClientIMU; // Receiving UdpClient
    private Thread _ReceiveThreadIMU; // Thread to receive data. Thread is a class in System.Threading namespace

    //Create a Queue to store the values
    public Queue<int> FQueue = new Queue<int>();

    void Start()
    {
        Initialize();
    }

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
                string message = System.Text.Encoding.ASCII.GetString(data);
                //Debug.Log("Received: " + message);

                string f_msg = message;

                //If there is no data in the value, then take the value as 0
                if (f_msg == "")
                {
                    f_msg = "0";
                }

                Debug.Log("Force: " + f_msg);

                //Convert the message to a float and store it in a queue
                int f_value = int.Parse(f_msg);

                //Store the value in a queue
                FQueue.Enqueue(f_value);

                //Debug.Log("Queue: " + FQueue.Count);
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
