using Content.Shared.Body.Systems;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Examine;
using Content.Shared.Verbs;

namespace Content.Shared._Cathedral.ChemVaporizer;

public abstract class SharedChemVaporizerSystem : EntitySystem
{
    [Dependency] private readonly SharedSolutionContainerSystem _solution = default!;
    [Dependency] private readonly ItemSlotsSystem _itemSlots = default!;
    [Dependency] private readonly SharedBloodstreamSystem _bloodstream = default!;
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<ChemVaporizerComponent, GetVerbsEvent<AlternativeVerb>> (OnGetAltVerbs);
        SubscribeLocalEvent<ChemVaporizerComponent, ExaminedEvent>(OnExamined);
    }

    private void OnGetAltVerbs(Entity<ChemVaporizerComponent> ent, ref GetVerbsEvent<AlternativeVerb> args)
    {
        if (!args.CanAccess || !args.CanInteract || args.Hands is null)
            return;

        var priority = 0;
        foreach (var value in ent.Comp.TransferValues)
        {
            AlternativeVerb verb = new AlternativeVerb
            {
                Text = value + "u",
                Category = VerbCategory.SetTransferAmount,
                Priority = priority,
                Act = () =>
                {
                    ent.Comp.VaporizeRate = value;
                },
            };
            priority--;
            args.Verbs.Add(verb);
        }
    }

    private void OnExamined(Entity<ChemVaporizerComponent> ent, ref ExaminedEvent args)
    {
        using var _ = args.PushGroup(nameof(ChemVaporizerComponent));
        if (args.IsInDetailsRange)
        {
            args.PushMarkup("Vaporization rate: " + ent.Comp.VaporizeRate.ToString() + "u");
        }
    }

}
