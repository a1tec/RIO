// <copyright file="RioResponse.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace RIO.Classes
{
    public class RioResponse
    {
        public bool Status { get; set; }

        public string ErrorMessage { get; set; }
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
