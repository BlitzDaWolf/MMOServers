using System;

namespace NetCommen.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class SyncCommandAttribute : Attribute
    {
        public int Command { get; set; }
        public Type AttributeType { get; set; }

        public SyncCommandAttribute(int command, Type attributeType)
        {
            Command = command;
            AttributeType = attributeType;
        }
    }
}
