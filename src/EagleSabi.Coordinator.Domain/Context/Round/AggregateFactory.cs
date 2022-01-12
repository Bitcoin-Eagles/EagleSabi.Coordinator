using EagleSabi.Infrastructure.Common.Abstractions.EventSourcing.Dependencies;
using EagleSabi.Infrastructure.Common.Abstractions.EventSourcing.Models;
using EagleSabi.Infrastructure.Common.Exceptions;

namespace EagleSabi.Coordinator.Domain.Context.Round;

public class AggregateFactory : IAggregateFactory
{
    public IAggregate Create(string aggregateType)
    {
        if (TryCreate(aggregateType, out var aggregate))
        {
            return aggregate
                ?? throw new AssertionFailedException($"AggregateFactory returned null for aggregateType: '{aggregateType}'");
        }
        else
        {
            throw new InvalidOperationException($"AggregateFactory is missing for aggregate type '{aggregateType}'.");
        }
    }

    public bool TryCreate(string aggregateType, out IAggregate aggregate)
    {
        switch (aggregateType)
        {
            case nameof(RoundAggregate):
                aggregate = new RoundAggregate();
                return true;

            default:
                aggregate = null!;
                return false;
        }
    }
}