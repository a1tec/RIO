// <copyright file="Credential.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace RIO.Models
{
    public class Credential
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Credential"/> class.
        /// </summary>
        /// <param name="rawData">RawData.</param>
        /// <param name="bitCount">BitCount.</param>
        public Credential(string rawData, int bitCount)
        {
            this.RawData = rawData;
            this.BitCount = bitCount;
        }

        /// <summary>
        /// Gets or sets rawData.
        /// </summary>
        public string RawData { get; private set; }

        /// <summary>
        /// Gets or sets bitCount.
        /// </summary>
        public int BitCount { get; private set; }
    }
}
