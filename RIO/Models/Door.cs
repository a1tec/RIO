// <copyright file="Door.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace RIO.Models
{
    public class Door
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Door"/> class.
        /// </summary>
        /// <param name="channel">Channel.</param>
        /// <param name="interface">Interface.</param>
        /// <param name="reader">Reader.</param>
        public Door(string channel, string @interface, string reader)
        {
            this.Channel = channel;
            this.Interface = @interface;
            this.Reader = reader;
        }

        public string Channel { get; set; }

        public string Interface { get; set; }

        public string Reader { get; set; }
    }
}
