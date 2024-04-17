
namespace SGSX.RabbitClient.Attributes;
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public class HandleQueueAttribute(string queue) : Attribute
{
    public string Queue { get; } = queue;
}
