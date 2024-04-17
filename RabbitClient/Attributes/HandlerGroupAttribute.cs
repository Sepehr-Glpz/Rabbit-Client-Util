namespace SGSX.RabbitClient.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public class HandlerGroupAttribute(string group) : Attribute
{
    public string Group { get; } = group;
}
