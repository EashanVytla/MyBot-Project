using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets
{
    class Listener
    {
        public string[] powers = new string[4];
        public static bool centric = false;
        Thread listenerThread = null;

        public bool stopper = false;
        private bool odoisRequested = false;
        private bool gyroisRequested = false;
        private bool poseisRequested = false;
        public string runningstring;
        public double input_ul = 0;
        public double input_ur = 0;
        public double input_bl = 0;
        public double input_br = 0;
        public bool clientDisconnecting = false;
        public bool wrongversion = false;
        private bool first = true;
       

        public void StartListener()
        {
            if (listenerThread == null)
            {
                listenerThread = new Thread(StartListeningThread);
            }
            listenerThread.Start();
        }

        public void stopListener()
        {
            listenerThread.Abort();
        }

        private Vector3 pose;

        public void setPose(Vector3 RobotPose)
        {
            pose = RobotPose;
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

            var address = NetworkInterface
                .GetAllNetworkInterfaces()
                .Where(c => c.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
                .SelectMany(c => c.GetIPProperties().UnicastAddresses)
                .Where(c => c.Address.AddressFamily == AddressFamily.InterNetwork)
                .Select(c => c.Address.ToString())
                .ToList();

            foreach (var myaddress in address)
            {
                if (myaddress.StartsWith("192."))
                {
                     IPAddress.TryParse(myaddress, out ipAddress);
                }
            }
            
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
                    if (!wrongversion)
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

                       // Debug.Log(pose);

                        if (odoisRequested)
                        {
                            runningstring = "O" + "," + WheelController.encoderCountLeft.ToString() + "," + WheelController.encoderCountRight.ToString() + "," + WheelController.encoderCountStrafe.ToString();
                        }

                        if (gyroisRequested)
                        {
                            runningstring = "G" + WheelController.heading.ToString();
                        }

                        if (poseisRequested)
                        {
                            runningstring = "P" + "," + pose.x.ToString() + "," + pose.z.ToString();
                        }

                        if (odoisRequested && gyroisRequested)
                        {
                            runningstring = "O,G" + "," + WheelController.encoderCountLeft.ToString() + "," + WheelController.encoderCountRight.ToString() + "," + WheelController.encoderCountStrafe.ToString() + "," + WheelController.heading.ToString();
                        }
                        
                        if (gyroisRequested && poseisRequested) {
                            runningstring = "P,G" + "," + pose.x.ToString() + "," + pose.z.ToString() + "," + WheelController.heading.ToString();
                        }
                        
                        if (poseisRequested && odoisRequested) {
                            runningstring = "O,P" + "," + WheelController.encoderCountLeft.ToString() + "," + WheelController.encoderCountRight.ToString() + "," + WheelController.encoderCountStrafe.ToString() + "," + pose.x.ToString() + "," + pose.z.ToString();
                        }
                        
                        if (gyroisRequested && poseisRequested && odoisRequested) {
                            runningstring = "O,G,P" + "," + WheelController.encoderCountLeft.ToString() + "," + WheelController.encoderCountRight.ToString() + "," + WheelController.encoderCountStrafe.ToString() + "," + pose.x.ToString() + "," + pose.z.ToString() + "," + WheelController.heading.ToString();
                        }

                        if(gyroisRequested || poseisRequested || odoisRequested)
                        {
                            send(runningstring, handler);
                            runningstring = "";
                        }

                        if (stopper)
                        {
                            stop(handler);
                        }
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
            if (first)
            {
                if (messageFull.Contains("v1.1"))
                {
                    wrongversion = false;
                }
                else
                {
                    wrongversion = true;
                }
                first = false;
            }
            message = messageFull.ToCharArray();

            fullpowers = "";

            for (int i = 2; i < (message.Length - 1); i++)
            {
                fullpowers += message[i];
            }

            if (message[1] == 'p')
            {
                powers = fullpowers.Split('|');

                input_ul = float.Parse(powers[0]);
                input_ur = float.Parse(powers[1]);
                input_bl = float.Parse(powers[2]);
                input_br = float.Parse(powers[3]);
                if (message[0] == 'c')
                {
                    centric = true;
                }
                else if (message[0] == 'r')
                {
                    centric = false;
                }
            }
            
            if (message[0] == 'O')
            {
                odoisRequested = true;
            }
            
            if (message[0] == 'G')
            {
                gyroisRequested = true;
            }
            
            if(message[0] == 'P')
            {
                poseisRequested = true;
            }
            
            else if (messageFull.Contains("stop"))
            {
                Debug.Log("Stopped");
                clientDisconnecting = true;
            }
            else if (messageFull.Contains("start"))
            {
                Debug.Log("Start");
                WheelController.encoderCountLeft = 0;
                WheelController.encoderCountRight = 0;
                WheelController.encoderCountStrafe = 0;
                clientDisconnecting = false;
            }
        }
    }
}
