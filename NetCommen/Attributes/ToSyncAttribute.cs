using System;

namespace NetCommen.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ToSyncAttribute : Attribute
    {
        public string AttributeName { get; set; }

        public ToSyncAttribute(string attributeName)
        {
            AttributeName = attributeName;
        }
    }
}
