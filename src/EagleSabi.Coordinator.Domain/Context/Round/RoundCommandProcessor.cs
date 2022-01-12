using System.Collections.Immutable;
using EagleSabi.Coordinator.Domain.Context.Round.Enums;
using EagleSabi.Coordinator.Domain.Context.Round.Records;
using EagleSabi.Infrastructure.Common.Abstractions.EventSourcing.Dependencies;
using EagleSabi.Infrastructure.Common.Abstractions.EventSourcing.Models;
using EagleSabi.Infrastructure.Common.Records.EventSourcing;

namespace EagleSabi.Coordinator.Domain.Context.Round;

public class RoundCommandProcessor : ICommandProcessor
{
    public Result Process(StartRoundCommand command, RoundState state)
    {
        var errors = PrepareErrors();
        if (!IsStateValid(PhaseEnum.New, state, command.GetType().Name, out var errorResult))
            return errorResult;
        return errors.Count > 0 ?
            Result.Fail(errors) :
            Result.Succeed(
                new[] { new RoundStartedEvent(command.RoundParameters) });
    }

    public Result Process(RegisterInputCommand command, RoundState state)
    {
        var errors = PrepareErrors();
        if (!IsStateValid(PhaseEnum.InputRegistration, state, command.GetType().Name, out var errorResult))
            return errorResult;
        return errors.Count > 0 ?
            Result.Fail(errors) :
            Result.Succeed(
                new[] { new InputRegisteredEvent(command.AliceSecret, command.Coin, command.OwnershipProof) });
    }

    public Result Process(EndRoundCommand command, RoundState state)
    {
        return Result.Succeed(new RoundEndedEvent());
    }

    public Result Process(ConfirmInputConnectionCommand command, RoundState state)
    {
        return Result.Succeed(new InputConnectionConfirmedEvent(command.Coin, command.OwnershipProof));
    }

    public Result Process(RemoveInputCommand command, RoundState state)
    {
        return Result.Succeed(new InputUnregistered(command.AliceOutPoint));
    }

    public Result Process(RegisterOutputCommand command, RoundState state)
    {
        return Result.Succeed(new OutputRegisteredEvent(command.Script, command.Value));
    }

    public Result Process(StartOutputRegistrationCommand command, RoundState state)
    {
        return Result.Succeed(new OutputRegistrationStartedEvent());
    }

    public Result Process(StartConnectionConfirmationCommand command, RoundState state)
    {
        return Result.Succeed(new InputsConnectionConfirmationStartedEvent());
    }

    public Result Process(StartTransactionSigningCommand command, RoundState state)
    {
        return Result.Succeed(new SigningStartedEvent());
    }

    public Result Process(SucceedRoundCommand command, RoundState state)
    {
        return Result.Succeed(new IEvent[] { new RoundSucceedEvent(), new RoundEndedEvent() });
    }

    public Result Process(NotifyInputReadyToSignCommand command, RoundState state)
    {
        return Result.Succeed(new InputReadyToSignEvent(command.AliceOutPoint));
    }

    public Result Process(AddSignatureCommand command, RoundState state)
    {
        return Result.Succeed(new SignatureAddedEvent(command.AliceOutPoint, command.WitScript));
    }

    public Result Process(ICommand command, IState state)
    {
        if (state is not RoundState roundState)
            throw new ArgumentException($"State should be type of {nameof(RoundState)}.", nameof(state));
        return ProcessDynamic(command, roundState);
    }

    protected Result ProcessDynamic(dynamic command, RoundState state)
    {
        return Process(command, state);
    }

    private static ImmutableArray<IError>.Builder PrepareErrors()
    {
        return ImmutableArray.CreateBuilder<IError>();
    }

    private bool IsStateValid(PhaseEnum expected, RoundState state, string commandName, out Result errorResult)
    {
        var isStateValid = expected == state.Phase;
        errorResult = null!;
        if (!isStateValid)
        {
            errorResult = Result.Fail(
                new Error(
                    $"Unexpected State for '{commandName}'. expected: '{expected}', actual: '{state.Phase}'"));
        }
        return isStateValid;
    }
}