namespace Content.Shared.CombatMode;

[ByRefEvent]
public record struct ShovedEvent(EntityUid Target, EntityUid Source)
{
    /// <summary>
    /// The entity being disarmed.
    /// </summary>
    public readonly EntityUid Target = Target;

    /// <summary>
    /// The entity performing the disarm.
    /// </summary>
    public readonly EntityUid Source = Source;

    public bool Handled;
}
