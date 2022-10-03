using RoR2;
using UnityEngine.Assertions;

namespace ExamplePlugin;

internal static class SurvivorTable {
    public static readonly Survivor Commando = new(
        name: "Commando",
        survivorIndex: 2,
        bodyIndex: 32
    );
    public static readonly Survivor Huntress = new(
        name: "Huntress",
        survivorIndex: 6,
        bodyIndex: 64
    );
    public static readonly Survivor Bandit = new(
        name: "Bandit",
        survivorIndex: 0,
        bodyIndex: 10
    );
    public static readonly Survivor MulT = new(
        name: "MUL-T",
        survivorIndex: 10,
        bodyIndex: 125
    );
    public static readonly Survivor Engineer = new(
        name: "Engineer",
        survivorIndex: 10,
        bodyIndex: 125
    );
    public static readonly Survivor Artificer = new(
        name: "Artificer",
        survivorIndex: 8,
        bodyIndex: 75
    );
    public static readonly Survivor Mercenary = new(
        name: "Mercenary",
        survivorIndex: 9,
        bodyIndex: 80
    );
    public static readonly Survivor Rex = new(
        name: "Rex",
        survivorIndex: 11,
        bodyIndex: 126
    );
    public static readonly Survivor Loader = new(
        name: "Loader",
        survivorIndex: 7,
        bodyIndex: 76
    );
    public static readonly Survivor Acrid = new(
        name: "Acrid",
        survivorIndex: 3,
        bodyIndex: 34
    );
    public static readonly Survivor Captain = new(
        name: "Captain",
        survivorIndex: 1,
        bodyIndex: 27
    );
    public static readonly Survivor Railgunner = new(
        name: "Railgunner",
        survivorIndex: 12,
        bodyIndex: 98
    );
    public static readonly Survivor VoidFiend = new(
        name: "V??oid Fiend",
        survivorIndex: 13,
        bodyIndex: 141
    );

    public static readonly Survivor[] All = new Survivor[] {
        Commando,
        Huntress,
        Bandit,
        MulT,
        Engineer,
        Artificer,
        Mercenary,
        Rex,
        Loader,
        Acrid,
        Captain,
        Railgunner,
        VoidFiend
    };
}

internal class Survivor {
    public Survivor(
        string name,
        int survivorIndex,
        int bodyIndex
    ) {
        this._name = name;
        this._survivorId = survivorIndex;
        this._bodyIndex = bodyIndex;
    }

    public string Name => _name;
    public BodyIndex BodyIndex => (BodyIndex)_bodyIndex;
    public int BodyIndexRaw => _bodyIndex;
    public SurvivorIndex SurvivorIndex => (SurvivorIndex)_survivorId;

    /// <summary>
    /// Do not use before the game has initialized (that is, before the 
    /// main menu is visible).
    /// </summary>
    public int GetSkillSlotCount() {
        if (_skillCount == null) {
            // Contains information 
            var skillCount = RoR2.Loadout
                .BodyLoadoutManager
                .GetSkillSlotCountForBody(BodyIndex);

            _skillCount = skillCount;
            _skillVariantCounts = new int[skillCount];
        }
        return (int)_skillCount;
    }

    /// <summary>
    /// Do not use before the game has initialized (that is, before the 
    /// main menu is visible).
    /// </summary>
    public int? GetSkillVariantCount(int slot) {
        var skillCount = GetSkillSlotCount();

        Assert.IsNotNull(_skillVariantCounts);

        // Cache results if they haven't been already
        if (_skillVariantCounts![0] == 0) {
            var loadout = RoR2.Loadout.RequestInstance();
            var blo = loadout.bodyLoadoutManager.GetReadOnlyBodyLoadout(BodyIndex);

            for (int i = 0; i < skillCount; i++) {
                _skillVariantCounts[i] = blo.LookUpMaxSkillVariants(i);
            }
            RoR2.Loadout.ReturnInstance(loadout);
        }

        if (slot >= skillCount || slot < 0) {
            return null;
        }
        return _skillVariantCounts[slot];
    }

    private readonly string _name;
    private readonly int _survivorId;
    private readonly int _bodyIndex;
    private int? _skillCount;
    private int[]? _skillVariantCounts;
}
