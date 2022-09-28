using RoR2;
using UnityEngine.AddressableAssets;
using UnityEngine;

namespace ExamplePlugin {
    internal static class SurvivorTable {
        public static readonly Survivor Commando = new(
            assetName: "Commando",
            name: "Commando",
            survivorId: 2,
            bodyIndex: 32
        );
        public static readonly Survivor Huntress = new(
            assetName: "Huntress",
            name: "Huntress",
            survivorId: 6,
            bodyIndex: 64
        );
        public static readonly Survivor Bandit = new(
            assetName: "Bandit2",
            name: "Bandit",
            survivorId: 0,
            bodyIndex: 10
        );
        public static readonly Survivor MulT = new(
            assetName: "Toolbot",
            name: "MUL-T",
            survivorId: 10,
            bodyIndex: 125
        );
        public static readonly Survivor Engineer = new(
            assetName: "Engi",
            name: "Engineer",
            survivorId: 10,
            bodyIndex: 125
        );
        public static readonly Survivor Artificer = new(
            assetName: "Mage",
            name: "Artificer",
            survivorId: 8,
            bodyIndex: 75
        );
        public static readonly Survivor Mercenary = new(
            assetName: "Merc",
            name: "Mercenary",
            survivorId: 9,
            bodyIndex: 80
        );
        public static readonly Survivor Rex = new(
            assetName: "Treebot",
            name: "Rex",
            survivorId: 11,
            bodyIndex: 126
        );
        public static readonly Survivor Loader = new(
            assetName: "Loader",
            name: "Loader",
            survivorId: 7,
            bodyIndex: 76
        );
        public static readonly Survivor Acrid = new(
            assetName: "Croco",
            name: "Acrid",
            survivorId: 3,
            bodyIndex: 34
        );
        public static readonly Survivor Captain = new(
            assetName: "Captain",
            name: "Captain",
            survivorId: 1,
            bodyIndex: 27
        );
        public static readonly Survivor Railgunner = new(
            assetName: "Railgunner",
            name: "Railgunner",
            survivorId: 12,
            bodyIndex: 98
        );
        public static readonly Survivor VoidFiend = new(
            assetName: "VoidSurvivor",
            name: "Railgunner",
            survivorId: 13,
            bodyIndex: 141
        );
    }

    internal class Survivor {
        public Survivor(
            string assetName,
            string name,
            int survivorId,
            int bodyIndex
        ) {
            this._assetName = assetName;
            this._name = name;
            this._survivorId = survivorId;
            this._bodyIndex = (BodyIndex)(object)bodyIndex;
        }

        public string Name {
            get => _name;
        }

        public BodyIndex BodyIndex {
            get => _bodyIndex;
        }

        public int SurvivorId {
            get => _survivorId;
        }

        public SurvivorDef Load() {
            var defAssetPath = $"RoR2/Base/{_assetName}/{_assetName}.asset";
            var bodyPrefabPath = $"RoR2/Base/{_assetName}/{_assetName}Body.prefab";
            var displayPrefabPath = $"RoR2/Base/{_assetName}/{_assetName}Display.prefab";

            var s = Addressables.LoadAssetAsync<SurvivorDef>(defAssetPath).WaitForCompletion();
            s.bodyPrefab = Resources.Load<GameObject>(bodyPrefabPath);
            s.displayPrefab = Resources.Load<GameObject>(displayPrefabPath);
            return s;
        }

        private readonly string _assetName;
        private readonly string _name;
        private readonly int _survivorId;
        private readonly BodyIndex _bodyIndex;
    }
}
