using Robust.Shared.Audio;

namespace Content.Shared.Actions.Events;

/// <summary>
/// Raised on the melee weapon used to push something (which will be the disarmer in 99% of cases).
/// </summary>
public sealed partial class PushEvent : HandledEntityEventArgs
{
    public readonly EntityUid Pusher;

    public readonly EntityUid PushedEntity;

    public readonly float StaminaDamage;

    // In case you want something to use a specific sound for pushing... I guess.
    public SoundSpecifier? PushSoundOverride;

    public PushEvent(EntityUid pusher, EntityUid pushedEnt, float damage)
    {
        Pusher = pusher;
        PushedEntity = pushedEnt;
        StaminaDamage = damage;
    }
}
