using JudoPayDotNet.Enums;
using System;


namespace JudoDotNetXamarin
{
    [AttributeUsage (AttributeTargets.Field, AllowMultiple = false)]
    public class EnumDescriptionAttribute :Attribute
    {
        private readonly string description;

        public string Description { get { return description; } }

        public EnumDescriptionAttribute (string description)
        {
            this.description = description;
        }
    }

    public enum BillingCountryOptions
    {
        [EnumDescription ("UK")]
        BillingCountryOptionUK,
        [EnumDescription ("USA")]
        BillingCountryOptionUSA,
        [EnumDescription ("Can")]
        BillingCountryOptionCanada,
        [EnumDescription ("other")]
        BillingCountryOptionOther
    }
		

}

