﻿using System.Threading;

namespace SGSX.RabbitClient.Interfaces;
public interface IClient
{
    Task ConnectAsync(CancellationToken ct);
    Task DisconnectAsync(CancellationToken ct);

    IConsumer Consumer { get; }

    IPublisher Publisher { get; }

    IRabbitConnection Connection { get; }

    ITopology Topology { get; }
}