// <copyright file="Heartbeat.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace RIO.Classes
{
    using System;
    using System.Threading;

    public class Heartbeat
    {
        private string channel;

        private string @interface;

        private string duration;

        private int timeInterval;

        private Api api;

        private bool heartbeatFlag = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="Heartbeat"/> class.
        /// </summary>
        /// <param name="channel">Channel.</param>
        /// <param name="interface">Interface.</param>
        /// <param name="duration">Duration.</param>
        /// <param name="timeInterval">TimeInterval.</param>
        /// <param name="api">Api.</param>
        public Heartbeat(string channel, string @interface, string duration, int timeInterval, Api api)
        {
            this.channel = channel;
            this.@interface = @interface;
            this.duration = duration;
            this.timeInterval = timeInterval;
            this.api = api;
        }

        public void Start()
        {
            //this.api.LogOn();

            while (this.heartbeatFlag)
            {
                while (this.api.IsConnected)
                {
                    var response = this.api.SendKeepAlive(this.channel, this.duration);

                    if (response)
                    {
                        response = this.api.SetInterfaceOnline(this.channel, this.@interface);

                        if (response)
                        {
                            Console.WriteLine("Delivered Online");
                        }
                        else
                        {
                            //Console.WriteLine(response.ErrorMessage);
                        }
                    }
                    else
                    {
                        //Console.WriteLine(response.ErrorMessage);
                    }

                    Thread.Sleep(this.timeInterval);
                }

                //this.api.LogOn();
            }

            Console.WriteLine("Heartbeat Stopped.");
        }

        public void Stop()
        {
            this.heartbeatFlag = false;
        }
    }
}
