using System;
using System.Collections.Generic;
using System.Linq;
using RoR2;
using UnityEngine.Assertions;

namespace ExamplePlugin;

// TODO could also use BepInEx's Config manager thing, but maybe that should
//      only be used for actual configuration (like the server URL)

/// <summary>
/// Represents the internal state of the mod.
/// </summary>
internal class State {
    public static readonly State instance = new();

    public string PlayerName => _playerName ?? "";
    public bool IsSignedIn => _playerName != null;

    // TODO
    public RaceTiming? raceTiming;
    public RaceLoadout? raceLoadout;

    public bool IsClockedIn => true;
    public bool IsRacing => IsSignedIn && IsClockedIn && raceTiming != null;

    public void SignIn(string playerName) {
        _playerName = playerName;
    }

    public void SignOut() {
        _playerName = null;
    }

    private State() { }
    private string? _playerName;

    // TODO add save/load
    // public string Serialize()
    // public static State Deserialize()
}

/// <summary>
/// Describes a race's timing.
/// </summary>
struct RaceTiming {
    public DateTime localStart;
    public DateTime localEnd;
    public long timeInc;
    public string overall;
}

/// <summary>
/// Contains a race's loadouts.
/// </summary>
struct RaceLoadout {
    public RaceLoadout(IEnumerable<Loadout> loadouts) {
        Assert.IsTrue(loadouts.Any());

        _loadouts = new List<Loadout>(loadouts);
        _currentLoadoutIdx = 0;
    }

    public Loadout CurrentLoadout => _loadouts[_currentLoadoutIdx];
    public int RemainingLoadouts => Math.Max(_loadouts.Count - 1, 0);

    /// <summary>
    /// Discard the current loadout and set the one at <paramref name="newIdx"/>
    /// as the current.
    /// </summary>
    public void RotateCurrentLoadout(int newIdx) {
        Assert.IsTrue(RemainingLoadouts > 0);
        Assert.IsTrue(_loadouts.Count > newIdx);

        _loadouts.RemoveAt(_currentLoadoutIdx);
        _currentLoadoutIdx = newIdx;
    }

    private List<Loadout> _loadouts;
    private int _currentLoadoutIdx;
}
