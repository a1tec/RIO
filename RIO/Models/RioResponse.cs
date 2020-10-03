// <copyright file="RioResponse.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Xml.Serialization;

namespace RIO.Models
{
    [XmlRoot("Response")]
    public class RioResponse
    {
        [XmlText]
        public string Response { get; set; }
    }
}
