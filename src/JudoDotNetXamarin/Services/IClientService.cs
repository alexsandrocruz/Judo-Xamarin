using System;
using JudoDotNetXamarin;
using Newtonsoft.Json.Linq;

namespace JudoDotNetXamarin
{
    public interface IClientService
    {
        JObject GetClientDetails ();

        string GetSDKVersion ();
    }
}

