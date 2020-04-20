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
        public string[] powers = new string[3];
        public static bool centric = false;
        Thread listenerThread = null;

        public bool stopper = false;
        private bool odoisRequested = false;
        private bool gyroisRequested = false;
        public string runningstring;


        public void StartListener()
        {
            if(listenerThread == null)
            {
                listenerThread = new Thread(StartListeningThread);
            }
            listenerThread.Start();
        }

        public void StartListeningThread()
        {
            stopper = false;
            string recievedMessage;
            byte[] bytes = new Byte[1024];
           

            // Establish the local endpoint for the socket.  
            // Dns.GetHostName returns the name of the
            // host running the application.  
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[1];
            Debug.Log(ipAddress);
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 8719);

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

                Socket handler = null;

                // Start listening for connections.  
                do
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


                    if (odoisRequested && gyroisRequested)
                    {
                        runningstring = "O" + "," + BlockController.encoderCountLeft.ToString() + "," + BlockController.encoderCountRight.ToString() + "," + BlockController.encoderCountStrafe.ToString() + "," + BlockController.heading.ToString();

                        send(runningstring, handler);
                        runningstring = "";
                    }else if (odoisRequested)
                    {
                        runningstring = "O" + "," + BlockController.encoderCountLeft.ToString() + "," + BlockController.encoderCountRight.ToString() + "," + BlockController.encoderCountStrafe.ToString();
                        send(runningstring, handler);
                        runningstring = "";
                    }

                    if (stopper)
                    {
                        stop(handler);
                    }
                } while (!stopper);
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }

        public void stop(Socket handler)
        {
            handler.Shutdown(SocketShutdown.Both);
            handler.Close();
        }

        public void send(string message, Socket handler)
        {
            // Echo the data back to the client.  
            byte[] msg = Encoding.ASCII.GetBytes(message);

            handler.Send(msg);
        }

        private static char[] message;
        private static string messageFull;
        string fullpowers;

        private void Parse(string messageFull)
        {
            Debug.Log(messageFull);
            message = messageFull.ToCharArray();

            fullpowers = "";

            for(int i = 2; i < (message.Length - 1); i++)
            {
                fullpowers += message[i];
            }

            if (message[1] == 'p')
            {
                powers = fullpowers.Split('|');

                BlockController.signalForce.x = float.Parse(powers[0]);
                BlockController.signalForce.y = float.Parse(powers[1]);
                BlockController.signaltorque = float.Parse(powers[2]);
                BlockController.signalForce.Scale(new Vector3(BlockController.newsignalScale, BlockController.newsignalScale));
                if(message[0] == 'c')
                {
                    centric = true;
                }
                Debug.Log(BlockController.signalForce);
            } 
            else if (message[0] == 'O')
            {
                odoisRequested = true;
            }
            else if (message[0] == 'G')
            {
                gyroisRequested = true;
            }
            else if (messageFull.Contains("stop"))
            {
                Debug.Log("Stopped");
                BlockController.signalForce = new Vector3(0, 0);
                BlockController.signaltorque = 0;
                //stopper = true;
                //StartListener();
            }
        }

        private Vector3 rotatedpowers(Vector3 input, double angle)
        {
            double newX = input.x * Math.Cos(angle) - input.y * Math.Sin(angle);
            double newY = input.x * Math.Sin(angle) + input.y * Math.Cos(angle);
            return new Vector3((float)(newX), (float)(newY));
        }
    }
}
