using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEditor;
using HenryDev;
using System.IO;
using HenryDev.Utilities;

namespace AFK.Idle.EditorTools
{
    public class HeroSPUMExporter : MonoBehaviour
    {
        private static string SPUM_UNIT_PATH = "Units/";
        private static string LINKER_PATH = "Assets/_Game/HeroData/Json/heroLinker.json";
        private static string EXPORT_PATH = "Assets/_Game/HeroData/Heroes/";
        private static string SPUM_CONTROLLER = "Assets/SPUM/Basic_Resources/Animator/Unit/SPUMController.controller";
        [SerializeField] Transform heroContainer;
        [SerializeField] CanvasScaler scaler;
        [SerializeField] TMP_Text heroSPUMCode; 
        [SerializeField] TMP_InputField heroName;
        [SerializeField] TMP_InputField heroCode;
        [SerializeField] Button exportButton;
        [SerializeField] Button nextButton;
        [SerializeField] Button prevButton;
        private List<SPUM_Prefabs> heroList = new List<SPUM_Prefabs>();
        private int index = 0;
        private HeroLinkerList heroLinkerList = new HeroLinkerList();
        void Awake()
        {
            exportButton.onClick.AddListener(ExportHero);
            nextButton.onClick.AddListener(NextHero);
            prevButton.onClick.AddListener(PrevHero);
        }
        void Start()
        {
            Init();
            LoadLinker();
            LoadHeroes();

            UpdateHero();
        }
        void NextHero()
        {
            if (index < heroList.Count - 1)
            {
                index++;
                UpdateHero();
            }
        }
        void PrevHero()
        {
            if (index > 0)
            {
                index--;
                UpdateHero();
            }
        }
        void Init()
        {
            int childCount = heroContainer.childCount;
            for (int i = childCount - 1; i >= 0; i--)
            {
                Destroy(heroContainer.GetChild(i).gameObject);
            }
        }
        void UpdateHero()
        {
            Init();
            var curHero = this.heroList[this.index];
            var curHeroInstance = Instantiate(curHero.gameObject, this.heroContainer);
            var curHeroRect = curHeroInstance.GetComponent<RectTransform>();
            curHeroRect
                .SetAnchorPosX(-this.scaler.referenceResolution.x / 2)
                .SetAnchorPosY(-this.scaler.referenceResolution.y / 2);
            curHeroInstance.transform.localScale = Vector3.one * 2;
            var curHeroPrefab = curHeroInstance.GetComponent<SPUM_Prefabs>();
            this.heroSPUMCode.text = curHeroPrefab._code;

            var unitPrefab = GetUnitPrefabFromLinker(curHeroPrefab._code);
            if (unitPrefab != null)
            {
                this.heroCode.text = unitPrefab.Data.HeroCode;
                this.heroName.text = unitPrefab.Data.HeroName;
            }
            else
            {
                this.heroCode.text = "";
                this.heroName.text = "";
            }
        }
        UnitPrefab GetUnitPrefabFromLinker(string SPUMCode)
        {
            foreach (var linker in heroLinkerList.HeroLinkers)
            {
                if (linker.SPUMCode == SPUMCode)
                {
                    var heroPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(string.Format("{0}{1}/{2}.prefab", EXPORT_PATH, linker.HeroCode, linker.HeroCode));
                    return heroPrefab?.GetComponent<UnitPrefab>();
                }
            }
            return null;
        }
        public void LoadHeroes()
        {
            heroList.Clear();
            var heroesObject = Resources.LoadAll(SPUM_UNIT_PATH, typeof(GameObject));
            foreach (var heroObject in heroesObject)
            {
                SPUM_Prefabs prefab = (heroObject as GameObject).GetComponent<SPUM_Prefabs>();
                if (prefab == null)
                {
                    continue;
                }
                LoadHero(prefab);
            }
        }
        void LoadHero(SPUM_Prefabs prefab)
        {
            heroList.Add(prefab);
        }
        void LoadLinker()
        {
            if (!System.IO.File.Exists(LINKER_PATH))
            {
                return;
            }
            string json = System.IO.File.ReadAllText(LINKER_PATH);
            this.heroLinkerList = JsonUtility.FromJson<HeroLinkerList>(json) ?? new HeroLinkerList();
        }
        void SaveLinker()
        {
            string json = JsonUtility.ToJson(this.heroLinkerList);
            System.IO.File.WriteAllText(LINKER_PATH, json);
        }
        void ExportHero()
        {
            string heroPrefabPath = CreateHeroPrefab();
        }
        string CreateHeroPrefab()
        {
            var curHero = this.heroList[this.index];
            string heroCode = this.heroCode.text;
            string heroName = this.heroName.text;
            string spumCode = this.heroSPUMCode.text;

            string exportFolder = CreateHeroFolder(heroCode);
            string exportPath = exportFolder + heroCode + ".prefab";
            string clonedAnimator = exportFolder + heroCode + "_Controller.controller";

            GameObject unitPrefab = curHero.transform.GetChild(0).gameObject;
            GameObject heroPrefab = Instantiate(unitPrefab);
            heroPrefab.name = heroCode;

            UnitPrefab unit = heroPrefab.AddComponent<UnitPrefab>();
            unit.CopySpumPackage(curHero.spumPackages);
            unit.CopyUnitType(curHero.UnitType);

            AssetDatabase.CopyAsset(SPUM_CONTROLLER, clonedAnimator);
            RuntimeAnimatorController animator = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(clonedAnimator);
            unit.Anim = unit.gameObject.SafeAddComponent<Animator>();
            unit.Anim.runtimeAnimatorController = animator;
            unit.Data = new HeroData()
            {
                HeroCode = heroCode,
                HeroName = heroName,
                SPUMCode = spumCode
            };
            unit.QuickSetup();
            this.heroLinkerList.AddNewLinker(spumCode, heroCode);

            PrefabUtility.SaveAsPrefabAsset(heroPrefab, exportPath);
            AssetDatabase.SaveAssets();
            SaveLinker();
            LoadLinker();

            Destroy(heroPrefab);

            return exportPath;
        }
        string CreateHeroFolder(string heroCode)
        {
            string path = EXPORT_PATH + heroCode;
            if (File.Exists(path))
            {
                return path + "/";
            }
            Directory.CreateDirectory(path);
            return path + "/";
        }
    }
    [System.Serializable]
    public class HeroLinkerList
    {
        public List<HeroLinker> HeroLinkers;
        public HeroLinkerList()
        {
            HeroLinkers = new List<HeroLinker>();
        }
        public void AddNewLinker(string spumCode, string heroCode)
        {
            var foundData = HeroLinkers.Find(x => x.SPUMCode == spumCode);
            if (foundData != null)
            {
                return;
            }
            HeroLinkers.Add(new HeroLinker(spumCode, heroCode));
        }
    }
    [System.Serializable]
    public class HeroLinker
    {
        public string SPUMCode;
        public string HeroCode;
        public HeroLinker(string spumCode, string heroCode)
        {
            SPUMCode = spumCode;
            HeroCode = heroCode;
        }
    }
}
