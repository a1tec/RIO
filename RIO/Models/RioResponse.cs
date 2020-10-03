// <copyright file="RioResponse.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Xml.Serialization;

namespace RIO.Classes
{


    //[XmlRoot(ElementName = "Response")]
    //[Serializable]
    [XmlRoot("Response")]
    //[XmlElement("Response")]
    public class RioResponse
    {
        [XmlText]
        public string Response { get; set; }
    }

    public interface IRioResponse
    {
        string Message { get; set; }
    }

    public class Response : IRioResponse
    {
        
        public string Message { get; set; }
    }
}
