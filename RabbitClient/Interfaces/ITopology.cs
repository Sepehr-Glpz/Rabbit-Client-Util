
namespace SGSX.RabbitClient.Interfaces;
public interface ITopology
{
    void CommitQueues(bool noWait);
    void CommitExchanges(bool noWait);
    void CommitBindings(bool noWait);
    void CommitAll(bool noWait);
}
