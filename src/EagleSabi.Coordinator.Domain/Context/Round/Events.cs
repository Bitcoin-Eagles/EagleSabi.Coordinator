using EagleSabi.Coordinator.Domain.Context.Round.Records;
using EagleSabi.Infrastructure.Common.Abstractions.EventSourcing.Models;
using NBitcoin;
using WalletWasabi.Crypto;

namespace EagleSabi.Coordinator.Domain.Context.Round;

public record RoundStartedEvent(RoundParameters RoundParameters) : IEvent, IRoundClientEvent;
public record InputRegisteredEvent(Guid AliceSecret, Coin Coin, OwnershipProof OwnershipProof) : IEvent;
public record InputUnregistered(OutPoint AliceOutPoint) : IEvent;
public record InputsConnectionConfirmationStartedEvent() : IEvent, IRoundClientEvent;
public record InputConnectionConfirmedEvent(Coin Coin, OwnershipProof OwnershipProof) : IEvent, IRoundClientEvent;
public record OutputRegistrationStartedEvent() : IEvent, IRoundClientEvent;
public record OutputRegisteredEvent(Script Script, long CredentialAmount) : IEvent, IRoundClientEvent;
public record SigningStartedEvent() : IEvent, IRoundClientEvent;
public record InputReadyToSignEvent(OutPoint AliceOutPoint) : IEvent;
public record SignatureAddedEvent(OutPoint AliceOutPoint, WitScript WitScript) : IEvent, IRoundClientEvent;
public record RoundSucceedEvent() : IEvent, IRoundClientEvent;
public record RoundEndedEvent() : IEvent, IRoundClientEvent;