// <copyright file="Api.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace RIO
{
    using System;
    using System.Net;
    using RestSharp;
    using RIO.Classes;
    using RIO.Models;

    /// <summary>
    /// Class used to communicate with Genetec Synergis Softwire.
    /// </summary>
    public class Api
    {
        private readonly string username;

        private readonly string password;

        private readonly IRestClient client;

        /// <summary>
        /// Initializes a new instance of the <see cref="Api"/> class.
        /// </summary>
        /// <param name="server">Server.</param>
        /// <param name="username">Username.</param>
        /// <param name="password">Password.</param>
        public Api(string server, string username, string password)
        {
            this.client = GetClient(server);
            this.username = username;
            this.password = password;
        }

        /// <summary>
        /// Gets or Sets ClientTimeout.
        /// </summary>
        public int ClientTimeout { get; set; } = 10000;

        /// <summary>
        /// Gets a value indicating whether Api is connected to the server.
        /// </summary>
        public bool IsConnected { get; private set; } = false;

        /// <summary>
        /// Returns created Heartbeat.
        /// </summary>
        /// <param name="channel">Channel.</param>
        /// <param name="interface">Interface.</param>
        /// <param name="duration">Duration.</param>
        /// <param name="timeInterval">Time Interval.</param>
        /// <returns>The created Heartbeat.</returns>
        public Heartbeat Heartbeat(string channel, string @interface, string duration, int timeInterval)
        {
            return new Heartbeat(channel, @interface, duration, timeInterval, this);
        }

        /// <summary>
        /// Synchronously log on to Genetec Synergis Softwire.
        /// </summary>
        public void LogOn()
        {
            var request = new RestRequest("/Login", Method.POST);
            request.AddHeader("content-type", "application/xml");
            request.AddHeader("cache-control", "no-cache");
            request.AddParameter("application/xml", $"username={this.username}&password={this.password}", ParameterType.RequestBody);

            //return Execute<Response>(request);

            IRestResponse response = client.Execute(request);

            if (response.ErrorException != null)
            {
                const string message = "Error retrieving response.  Check inner details for more info.";
                throw new Exception(message, response.ErrorException);
            }

            if (response.Content == RioMessage.LoginSuccessful)
            {
                IsConnected = true;
            }
        }

        //public T Execute<T>(RestRequest request) where T : new()
        //{
        //    var response = client.Execute<T>(request);

        //    if (response.ErrorException != null)
        //    {
        //        const string message = "Error retrieving response.  Check inner details for more info.";
        //        throw new Exception(message, response.ErrorException);
        //    }

        //    return response.Data;
        //}

        /// <summary>
        /// Reports a card swipe.
        /// </summary>
        /// <param name="door">Door for card swipe.</param>
        /// <param name="credential">Credential for card swipe.</param>
        /// <returns>RioResponse.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="door"/> is null.</exception>
        /// /// <exception cref="ArgumentNullException"><paramref name="credential"/> is null.</exception>
        public bool ReportCardSwipe(Door door, Credential credential)
        {
            if (door == null)
            {
                throw new ArgumentNullException(nameof(door));
            }

            if (credential == null)
            {
                throw new ArgumentNullException(nameof(credential));
            }

            var request = new RestRequest($"/ExternalIntegrations/{door.Channel}/Update", Method.POST);
            request.AddHeader("content-type", "application/xml");
            request.AddHeader("cache-control", "no-cache");

            var message = $"<Request><BusUpdate><CardSwipe><Interface>{door.Interface}</Interface>" +
                $"<Reader>{door.Reader}</Reader><BitCount>{credential.BitCount}</BitCount>" +
                $"<CardCode>{credential.RawData}</CardCode></CardSwipe></BusUpdate></Request>";

            request.AddParameter("application/xml", message, ParameterType.RequestBody);

            var response = this.client.Execute(request);

            if (response.StatusCode == HttpStatusCode.OK && response.Content == RioMessage.OK)
            {
                return true;
            }

            if (response.StatusCode == HttpStatusCode.Found)
            {
                IsConnected = false;
                throw new Exception("Must log in");
                //LogOn();
                //ReportCardSwipe(door, credential);
            }

            throw new Exception(response.Content);
        }

        public bool ReportOfflineAccess(Door door, Credential credential, DateTime timestamp, bool isGranted)
        {
            string message = $"<Request><BusUpdate><OfflineDecision><Interface>{door.Interface}</Interface>" +
                $"<Reader>{door.Reader}</Reader>" +
                $"<Timestamp>{timestamp.ToString("yyyy-MM-ddTHH:mm:ss")}</Timestamp>" +
                $"<Card><Some><BitCount>{credential.BitCount}</BitCount><CardCode>{credential.RawData}</CardCode>" +
                $"</Some></Card><Granted>{isGranted}</Granted></OfflineDecision></BusUpdate></Request>";

            var request = new RestRequest($"/ExternalIntegrations/{door.Channel}/Update", Method.POST);
            request.AddHeader("content-type", "application/xml");
            request.AddHeader("cache-control", "no-cache");
            request.AddParameter("application/xml", message, ParameterType.RequestBody);
            var response = this.client.Execute(request);

            if (response.StatusCode == HttpStatusCode.OK && response.Content == RioMessage.OK)
            {
                return true;
            }

            if (response.StatusCode == HttpStatusCode.Found)
            {
                this.IsConnected = false;
            }

            return false;
        }

        public bool SetInterfaceOnline(string channel, string @interface)
        {
            var request = new RestRequest($"/ExternalIntegrations/{channel}/Update", Method.POST);
            request.AddHeader("content-type", "application/xml");
            request.AddHeader("cache-control", "no-cache");
            var message = $"<Request><BusUpdate><SetConnected><Interface>{@interface}</Interface>" +
                "<IsConnected>True</IsConnected><SpecificDevices><None /></SpecificDevices>" +
                "</SetConnected></BusUpdate></Request>";
            request.AddParameter("application/xml", message, ParameterType.RequestBody);
            var response = this.client.Execute(request);

            if (response.StatusCode == HttpStatusCode.OK && response.Content == RioMessage.OK)
            {
                return true;
            }

            if (response.StatusCode == HttpStatusCode.Found)
            {
                this.IsConnected = false;
            }

            return false;
        }

        public bool SetInterfaceOffline(string channel, string @interface)
        {
            var request = new RestRequest($"/ExternalIntegrations/{channel}/Update", Method.POST);
            request.AddHeader("content-type", "application/xml");
            request.AddHeader("cache-control", "no-cache");
            var message = "<Request><BusUpdate><SetConnected><Interface>{@interface}</Interface>" +
                "<IsConnected>False</IsConnected><SpecificDevices><None /></SpecificDevices>" +
                "</SetConnected></BusUpdate></Request>";
            request.AddParameter("application/xml", message, ParameterType.RequestBody);
            var response = this.client.Execute(request);

            if (response.StatusCode == HttpStatusCode.OK && response.Content == RioMessage.OK)
            {
                return true;
            }

            if (response.StatusCode == HttpStatusCode.Found)
            {
                this.IsConnected = false;
            }

            return false;
        }

        public bool SendKeepAlive(string channel, string duration)
        {
            var request = new RestRequest("/ExternalIntegrations/" + channel + "/Update", Method.POST);
            request.AddHeader("content-type", "application/xml");
            request.AddHeader("cache-control", "no-cache");

            var message = $"<Request><BusUpdate><StatusKeepalive>" +
                    $"<Duration>{duration}</Duration></StatusKeepalive></BusUpdate></Request>";

            request.AddParameter("application/xml", message, ParameterType.RequestBody);
            var response = this.client.Execute(request);

            if (response.StatusCode == HttpStatusCode.OK && response.Content == RioMessage.OK)
            {
                return true;
            }

            if (response.StatusCode == HttpStatusCode.Found)
            {
                this.IsConnected = false;
            }

            return false;
        }

        private RestClient GetClient(string server)
        {
            ServicePointManager.ServerCertificateValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;

            return new RestClient(server)
            {
                CookieContainer = new CookieContainer(),
                FollowRedirects = false,
                //Timeout = this.ClientTimeout,
            };
        }
    }
}
