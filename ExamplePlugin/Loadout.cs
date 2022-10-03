using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExamplePlugin;

internal class Loadout {
    public Loadout(uint[] loadout) {
        this._skillVariants = loadout;
    }

    /// <summary>
    /// Check if a <see cref="Survivor"/> has enough skill slots and variants for 
    /// this loadout to be applied to them.
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public bool IsValidForSurvivor(Survivor s) {
        if (_skillVariants.Length != s.GetSkillSlotCount()) {
            return false;
        }
        for (int i = 0; i < _skillVariants.Length; i++) {
            if (_skillVariants[i] < 0 || _skillVariants[i] >= s.GetSkillVariantCount(i)) {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Set a loadout for a given <c>bodyIndex</c>.
    /// </summary>
    /// <param name="loadout"></param>
    /// <param name="bodyIndex"></param>
    public void Force(RoR2.Loadout loadout, BodyIndex bodyIndex) {
        var blm = loadout.bodyLoadoutManager;

        for (int i = 0; i < _skillVariants.Length; i++) {
            blm.SetSkillVariant(bodyIndex, i, _skillVariants[i]);
        }
    }

    /// <summary>
    /// Print the internal names of this loadout.
    /// </summary>
    /// <param name="loadout"></param>
    /// <param name="bodyIndex"></param>
    /// <returns></returns>
    public IEnumerable<string> Names(RoR2.Loadout loadout, BodyIndex bodyIndex) {
        var names = new List<string>();
        var blo = loadout.bodyLoadoutManager.GetReadOnlyBodyLoadout(bodyIndex);
        
        for (int i = 0; i < _skillVariants.Length; i++) {
            var skill = blo.GetSkillFamily(i);
            var variantIdx = (int)_skillVariants[i];
            names.Add(skill.GetVariantName(variantIdx));
        }

        return names;
    }

    /// <summary>
    /// The internal representation of the loadout. The number of elements must
    /// match the number of skills for a given <see cref="BodyIndex"/>, and the
    /// elements themselves are the variant indeces.
    /// </summary>
    private readonly uint[] _skillVariants;
}
