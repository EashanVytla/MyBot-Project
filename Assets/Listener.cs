using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace Assets
{
    class Listener
    {
        public string[] powers = new string[4];
        public static bool centric = false;

        public bool stopper = false;
        private bool odoisRequested = false;
        private bool gyroisRequested = false;
        private bool poseisRequested = false;
        public string runningstring;
        public double input_ul = 0;
        public double input_ur = 0;
        public double input_bl = 0;
        public double input_br = 0;
        public bool clientDisconnecting = true;
        //public bool init = false;
        public bool wrongversion = false;
        private bool firstp = true;
        string versionnum;
        int currentThread = 0;
        List<Thread> listenerThread = new List<Thread>();

        public Listener()
        {
            versionnum = "v1.5,";
        }


        Socket listener = null;
        Socket handler = null;


        public void StartListener()
        {
            listenerThread.Add(new Thread(StartListeningThread));
            listenerThread[0].Start();
        }

        public void RestartListener()
        {
            if (listener != null)
            {
                stop(listener);
            }

            if (handler != null)
            {
                stop(handler);
            }
            stopper = true;
            stopListener();
            currentThread++;
            listenerThread.Add(new Thread(StartListeningThread));
            listenerThread[currentThread].Start();
        }

        public void stopListener()
        {
            listenerThread[currentThread].Abort();
        }

        private Vector3 pose;

        public void setPose(Vector3 RobotPose)
        {
            pose = RobotPose;
        }

        public static string[] GetAllLocalIPv4(NetworkInterfaceType _type)
        {
            List<string> ipAddrList = new List<string>();
            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
            {
                Debug.Log(item.Name);
                if (item.NetworkInterfaceType == _type && item.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            ipAddrList.Add(ip.Address.ToString());
                        }
                    }
                }
            }
            return ipAddrList.ToArray();
        }

        public void StartListeningThread()
        {
            stopper = false;
            string recievedMessage;
            byte[] bytes = new Byte[1024];

            IPAddress ipAddress = null;

            IPAddress.TryParse("127.0.0.1", out ipAddress);


            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 8719);

            // Create a TCP/IP socket.  
            listener = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            int i = 0;

            // Bind the socket to the local endpoint and
            // listen for incoming connections.  
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(10);

                handler = null;

                // Start listening for connections.  
                do
                {
                    if (!wrongversion)
                    {
                        if (clientDisconnecting)
                        {
                            handler = listener.Accept();
                        }
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

                        if (gyroisRequested && poseisRequested)
                        {
                            runningstring = "P,G" + "," + pose.x.ToString() + "," + pose.z.ToString() + "," + WheelController.heading.ToString();
                        }

                        if (poseisRequested && odoisRequested)
                        {
                            runningstring = "O,P" + "," + WheelController.encoderCountLeft.ToString() + "," + WheelController.encoderCountRight.ToString() + "," + WheelController.encoderCountStrafe.ToString() + "," + pose.x.ToString() + "," + pose.z.ToString();
                        }

                        if (gyroisRequested && poseisRequested && odoisRequested)
                        {
                            runningstring = "O,G,P" + "," + WheelController.encoderCountLeft.ToString() + "," + WheelController.encoderCountRight.ToString() + "," + WheelController.encoderCountStrafe.ToString() + "," + pose.x.ToString() + "," + pose.z.ToString() + "," + WheelController.heading.ToString();
                        }

                        if (gyroisRequested || poseisRequested || odoisRequested)
                        {
                            send(runningstring, handler);
                            runningstring = "";
                        }
                        else
                        {
                            send("check", handler);
                            runningstring = " ";
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
            if (handler.Connected)
            {
                handler.Shutdown(SocketShutdown.Both);
            }
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
            if (messageFull == versionnum)
            {
                wrongversion = false;
                clientDisconnecting = false;
            }
            else if (clientDisconnecting && messageFull != versionnum || firstp && messageFull != versionnum)
            {
                wrongversion = true;
            }
            firstp = false;

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

            if (message[0] == 'P')
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
                //init = true;
                //while (init) { }
            }
        }
    }
}
