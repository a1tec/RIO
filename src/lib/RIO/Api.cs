// <copyright file="Api.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace RIO
{
    using System;
    using System.IO;
    using System.Net;
    using System.Xml.Serialization;
    using RestSharp;
    using RestSharp.Deserializers;
    using RestSharp.Serialization.Xml;
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
        public bool LogOn()
        {
            var request = new RestRequest("/Login", Method.POST);
            request.AddHeader("content-type", "application/xml");
            request.AddHeader("cache-control", "no-cache");
            request.AddParameter("application/xml", $"username={this.username}&password={this.password}", ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);

            if (response.StatusCode == HttpStatusCode.OK && response.Content == "Login successful")
            {
                IsConnected = true;

                return true;
            }

            throw new Exception(response.Content);

            return false;
        }

        public T Execute<T>(RestRequest request) where T : new()
        {
            var response = client.Execute<T>(request);

            if (response.StatusCode == HttpStatusCode.Found)
            {
                this.IsConnected = false;
                throw new Exception("Must login to synergis first.");
            }

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return response.Data;
            }

            throw new Exception(response.Content);
        }

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
            var message = $"<Request><BusUpdate><CardSwipe><Interface>{door.Interface}</Interface>" +
                   $"<Reader>{door.Reader}</Reader><BitCount>{credential.BitCount}</BitCount>" +
                   $"<CardCode>{credential.RawData}</CardCode></CardSwipe></BusUpdate></Request>";

            var request = new RestRequest($"/ExternalIntegrations/{door.Channel}/Update", Method.POST);
            request.AddHeader("content-type", "application/xml");
            request.AddHeader("cache-control", "no-cache");
            request.AddParameter("application/xml", message, ParameterType.RequestBody);

            var response = Execute<RioResponse>(request);

            if (response.Response == "OK")
            {
                return true;
            }

            return false;
        }

        public bool ReportOfflineAccess(Door door, Credential credential, DateTime timestampUTC, bool isGranted)
        {
            string message = $"<Request><BusUpdate><OfflineDecision><Interface>{door.Interface}</Interface>" +
                $"<Reader>{door.Reader}</Reader>" +
                $"<Timestamp>{timestampUTC.ToString("yyyy-MM-ddTHH:mm:ss")}</Timestamp>" +
                $"<Card><Some><BitCount>{credential.BitCount}</BitCount><CardCode>{credential.RawData}</CardCode>" +
                $"</Some></Card><Granted>{isGranted}</Granted></OfflineDecision></BusUpdate></Request>";

            var request = new RestRequest($"/ExternalIntegrations/{door.Channel}/Update", Method.POST);
            request.AddHeader("content-type", "application/xml");
            request.AddHeader("cache-control", "no-cache");
            request.AddParameter("application/xml", message, ParameterType.RequestBody);

            var response = Execute<RioResponse>(request);

            if (response.Response == "OK")
            {
                return true;
            }

            return false;
        }

        public bool SetInterfaceOnline(string channel, string @interface)
        {
            var message = $"<Request><BusUpdate><SetConnected><Interface>{@interface}</Interface>" +
                "<IsConnected>True</IsConnected><SpecificDevices><None /></SpecificDevices>" +
                "</SetConnected></BusUpdate></Request>";

            var request = new RestRequest($"/ExternalIntegrations/{channel}/Update", Method.POST);
            request.AddHeader("content-type", "application/xml");
            request.AddHeader("cache-control", "no-cache");
            request.AddParameter("application/xml", message, ParameterType.RequestBody);

            var response = Execute<RioResponse>(request);

            if (response.Response == "OK")
            {
                return true;
            }

            return false;
        }

        public bool SetInterfaceOffline(string channel, string @interface)
        {
            var message = $"<Request><BusUpdate><SetConnected><Interface>{@interface}</Interface>" +
                   $"<IsConnected>False</IsConnected><SpecificDevices><None /></SpecificDevices>" +
                   $"</SetConnected></BusUpdate></Request>";

            var request = new RestRequest($"/ExternalIntegrations/{channel}/Update", Method.POST);
            request.AddHeader("content-type", "application/xml");
            request.AddHeader("cache-control", "no-cache");
            request.AddParameter("application/xml", message, ParameterType.RequestBody);

            var response = Execute<RioResponse>(request);

            if (response.Response == "OK")
            {
                return true;
            }

            return false;
        }

        public bool SendKeepAlive(string channel, string duration)
        {
            var message = $"<Request><BusUpdate><StatusKeepalive>" +
                    $"<Duration>{duration}</Duration></StatusKeepalive></BusUpdate></Request>";

            var request = new RestRequest($"/ExternalIntegrations/{channel}/Update", Method.POST);
            request.AddHeader("content-type", "application/xml");
            request.AddHeader("cache-control", "no-cache");
            request.AddParameter("application/xml", message, ParameterType.RequestBody);

            var response = Execute<RioResponse>(request);

            if (response.Response == "OK")
            {
                return true;
            }

            return false;
        }

        private RestClient GetClient(string server)
        {
            ServicePointManager.ServerCertificateValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;

            RestClient client = new RestClient(server);
            client.CookieContainer = new CookieContainer();
            client.FollowRedirects = false;
            client.Timeout = this.ClientTimeout;
            client.ClearHandlers();
            client.UseDotNetXmlSerializer();

            return client;
        }
    }
}
