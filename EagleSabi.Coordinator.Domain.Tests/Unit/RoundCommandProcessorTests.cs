using System;
using EagleSabi.Coordinator.Domain.Context.Round;
using EagleSabi.Coordinator.Domain.Context.Round.Records;
using EagleSabi.Infrastructure.Common.Abstractions.EventSourcing.Dependencies;
using Shouldly;
using Xunit;

namespace EagleSabi.Coordinator.Domain.Tests.Unit;

public class RoundCommandProcessorTests
{
    protected ICommandProcessor RoundCommandProcessor { get; init; }

    public RoundCommandProcessorTests()
    {
        RoundCommandProcessor = new RoundCommandProcessor();
    }

    [Fact]
    public void StartRound_Success()
    {
        // Arrange
        var command = new StartRoundCommand(null!, Guid.NewGuid());
        var state = new RoundState();

        // Act
        var result = RoundCommandProcessor.Process(command, state);

        // Assert
        result.Success.ShouldBeTrue();
    }
}