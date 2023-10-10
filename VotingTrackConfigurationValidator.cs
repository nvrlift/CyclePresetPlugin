using FluentValidation;
using JetBrains.Annotations;

namespace VotingTrackPlugin;

[UsedImplicitly]
public class VotingTrackConfigurationValidator : AbstractValidator<VotingTrackConfiguration>
{
    public VotingTrackConfigurationValidator()
    {
        RuleFor(cfg => cfg.NumChoices).GreaterThanOrEqualTo(2);
        RuleFor(cfg => cfg.VotingIntervalMinutes).GreaterThanOrEqualTo(5);
        RuleFor(cfg => cfg.VotingDurationSeconds).GreaterThanOrEqualTo(30);
        RuleFor(cfg => cfg.TransitionDurationMinutes).GreaterThanOrEqualTo(1);
    }
}
