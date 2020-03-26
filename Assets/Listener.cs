using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets
{
    class Listener
    {
        private string recievedMessage;
        private Socket handler;
        private byte[] bytes = new Byte[1024];
        private bool odoisRequested = false;
        private string runningstring;
        public static bool stopper = false;

        public void StartListener()
        {
            Thread listenerThread = new Thread(StartListeningThread);
            listenerThread.Start();
        }

        public void StartListeningThread()
        {
            // Data buffer for incoming data.  
            byte[] bytes = new Byte[1024];

            // Establish the local endpoint for the socket.  
            // Dns.GetHostName returns the name of the
            // host running the application.  
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

            // Create a TCP/IP socket.  
            Socket listener = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            int i = 0;

            // Bind the socket to the local endpoint and
            // listen for incoming connections.  
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(10);

                // Start listening for connections.  
                while (!stopper)
                {
                    handler = listener.Accept();
                    recievedMessage = null;

                    // An incoming connection needs to be processed.  
                    while (true)
                    {
                        int bytesRec = handler.Receive(bytes);
                        recievedMessage = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                        
                        if (recievedMessage.IndexOf(",") > -1)
                        {
                            break;
                        }
                    }
                    i++;

                    try
                    {
                        Parse(recievedMessage);
                    }
                    catch
                    {
                        Debug.Log("None");
                    }

                    if (odoisRequested)
                    {
                        runningstring = BlockController.encoderCountLeft.ToString() + "," + BlockController.encoderCountRight.ToString() + "," + BlockController.encoderCountStrafe.ToString();
                    }

                    if(odoisRequested)
                    {
                        send(runningstring);
                        runningstring = "";
                    }
                    
                    stop();
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }

        public void stop()
        {
            handler.Shutdown(SocketShutdown.Both);
            handler.Close();
        }

        public void send(string message)
        {
            // Echo the data back to the client.  
            byte[] msg = Encoding.ASCII.GetBytes(message);

            handler.Send(msg);
        }

        private static char[] message;
        private static string messageFull;

        private void Parse(string messageFull)
        {
            Debug.Log(messageFull);
            message = messageFull.ToCharArray();

            if (message[0] == 'p')
            {
                BlockController.signalForce.x = float.Parse(message[1].ToString() + message[2].ToString() + message[3].ToString() + message[4].ToString());
                BlockController.signalForce.y = float.Parse(message[5].ToString() + message[6].ToString() + message[7].ToString() + message[8].ToString());
                BlockController.signaltorque = float.Parse(message[9].ToString() + message[10].ToString() + message[11].ToString() + message[12].ToString());
                BlockController.signalForce.Scale(new Vector3(BlockController.signalScale, BlockController.signalScale));
                Debug.Log(BlockController.signalForce);
                //Debug.DrawLine(rb.position, signalForce);
            } 
            else if (message[0] == 'O')
            {
                odoisRequested = true;
            }
            else if (messageFull == "stop")
            {
                BlockController.signalForce = new Vector3(0, 0);
                BlockController.signaltorque = 0;
                stopper = true;
                stop();
            }
        }
    }
}
