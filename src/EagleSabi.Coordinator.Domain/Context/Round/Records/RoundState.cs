using System.Collections.Immutable;
using EagleSabi.Coordinator.Domain.Context.Round.Enums;
using EagleSabi.Infrastructure.Common.Abstractions.EventSourcing.Models;
using NBitcoin;
using WalletWasabi.Crypto;
using WalletWasabi.WabiSabi.Crypto;
using WalletWasabi.WabiSabi.Models.MultipartyTransaction;

namespace EagleSabi.Coordinator.Domain.Context.Round.Records;

public record RoundState : IState
{
    public RoundParameters? RoundParameters { get; init; } = default;
    public ImmutableList<InputState> Inputs { get; init; } = ImmutableList<InputState>.Empty;
    public ImmutableList<OutputState> Outputs { get; init; } = ImmutableList<OutputState>.Empty;
    public PhaseEnum Phase { get; init; } = PhaseEnum.New;
    public uint256 Id => RoundParameters?.Id ?? uint256.Zero;
    public bool Succeeded { get; init; } = false;
}

public record InputState(
    Coin Coin,
    OwnershipProof OwnershipProof,
    Guid AliceSecret = default,
    bool ConnectionConfirmed = false,
    bool ReadyToSign = false,
    WitScript? WitScript = null);

public record OutputState(
    Script Script,
    long Value);

public record RoundParameters(
    FeeRate FeeRate,
    CredentialIssuerParameters AmountCredentialIssuerParameters,
    CredentialIssuerParameters VsizeCredentialIssuerParameters,
    DateTimeOffset InputRegistrationStart,
    TimeSpan InputRegistrationTimeout,
    TimeSpan ConnectionConfirmationTimeout,
    TimeSpan OutputRegistrationTimeout,
    TimeSpan TransactionSigningTimeout,
    long MaxAmountCredentialValue,
    long MaxVsizeCredentialValue,
    long MaxVsizeAllocationPerAlice,
    MultipartyTransactionParameters MultipartyTransactionParameters
)
{
    public uint256 BlameOf { get; init; } = uint256.Zero;

    private uint256? _id;
    public uint256 Id => _id ??= CalculateHash();
    public DateTimeOffset InputRegistrationEnd => InputRegistrationStart + InputRegistrationTimeout;

    private uint256 CalculateHash() =>
        RoundHasher.CalculateHash(
            InputRegistrationStart,
            InputRegistrationTimeout,
            ConnectionConfirmationTimeout,
            OutputRegistrationTimeout,
            TransactionSigningTimeout,
            MultipartyTransactionParameters.AllowedInputAmounts,
            MultipartyTransactionParameters.AllowedInputTypes,
            MultipartyTransactionParameters.AllowedOutputAmounts,
            MultipartyTransactionParameters.AllowedOutputTypes,
            MultipartyTransactionParameters.Network,
            MultipartyTransactionParameters.FeeRate.FeePerK,
            MultipartyTransactionParameters.MaxTransactionSize,
            MultipartyTransactionParameters.MinRelayTxFee.FeePerK,
            MaxAmountCredentialValue,
            MaxVsizeCredentialValue,
            MaxVsizeAllocationPerAlice,
            AmountCredentialIssuerParameters,
            VsizeCredentialIssuerParameters);
}