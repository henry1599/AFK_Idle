#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using HenryDev;
using System.IO;
using HenryDev.Utilities;
using UnityEditor;
using HenryDev.Math;

namespace AFK.Idle.EditorTools
{
    public class HeroSPUMExporter : MonoBehaviour
    {
        private static string SPUM_UNIT_PATH = "Units/";
        private static string LINKER_PATH = "Assets/_Game/HeroData/Json/heroLinker.json";
        private static string EXPORT_PATH = "Assets/_Game/HeroData/Heroes/";
        private static string SPUM_CONTROLLER = "Assets/SPUM/Basic_Resources/Animator/Unit/SPUMController.controller";
        private static string UNIT_DATA_PATH = "Assets/_Game/HeroData/Resources/UnitData/";
        [SerializeField] Transform heroContainer;
        [SerializeField] CanvasScaler scaler;
        [SerializeField] TMP_Text heroSPUMCode; 
        [SerializeField] TMP_InputField heroName;
        [SerializeField] TMP_InputField heroCode;
        [SerializeField] Button exportButton;
        [SerializeField] Button nextButton;
        [SerializeField] Button prevButton;
        [SerializeField] Camera captureCamera;
        [SerializeField] LayerMask captureLayer;
        private List<SPUM_Prefabs> heroList = new List<SPUM_Prefabs>();
        private int index = 0;
        private RenderTexture rt;
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
            curHeroInstance.StripCloneName();
            curHeroInstance.SetLayerRecursively("Capture");
            var curHeroRect = curHeroInstance.GetComponent<RectTransform>();
            curHeroRect
                .SetAnchorPosX(-this.scaler.referenceResolution.x / 2)
                .SetAnchorPosY(-this.scaler.referenceResolution.y / 2 - 1);
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
            CreateHeroIcon(heroPrefabPath);

            CreateData(heroPrefabPath);
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
        void CreateData(string prefabPath)
        {
            string heroCode = this.heroCode.text;
            string heroName = this.heroName.text;
            string spumCode = this.heroSPUMCode.text;
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            Sprite icon = AssetDatabase.LoadAssetAtPath<Sprite>(string.Format("{0}{1}/{2}.png", EXPORT_PATH, heroCode, heroCode));

            UnitData data = ScriptableObject.CreateInstance<UnitData>();
            data.UnitName = heroName;
            data.UnitCode = heroCode;
            data.SPUMCode = spumCode;
            data.UnitPrefab = prefab;
            data.UnitIcon = icon;

            string dataPath = UNIT_DATA_PATH + heroCode + ".asset";
            AssetDatabase.CreateAsset(data, dataPath);
            AssetDatabase.SaveAssets();
        }
        void CreateHeroIcon(string prefabPath)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            UnitPrefab unit = prefab.GetComponent<UnitPrefab>();
            CaptureSingleTake(unit.Data.HeroCode, 2048);
        }
        public void CaptureSingleTake(string id, int captureSize = 2048)
        {
            this.captureCamera.clearFlags = CameraClearFlags.Depth;
            this.captureCamera.backgroundColor = Color.clear;
            this.captureCamera.cullingMask = this.captureLayer;
            var tex = new Texture2D(captureSize, captureSize, TextureFormat.ARGB32, false, true);
            tex.filterMode = FilterMode.Trilinear;
            tex.anisoLevel = 16;

            var grab_area = new Rect(0, 0, captureSize, captureSize);
            rt = new RenderTexture(captureSize, captureSize, 12, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
            rt.filterMode = FilterMode.Trilinear;
            rt.anisoLevel = 16;

            RenderTexture.active = rt;
            this.captureCamera.targetTexture = rt;

            this.captureCamera.Render();
            tex.ReadPixels(grab_area, 0, 0);

            this.captureCamera.targetTexture = null;

            byte[] bytes = ImageConversion.EncodeToPNG(tex);

            string finalPath = string.Format("{0}{1}/{2}", EXPORT_PATH, id, id);

            File.WriteAllBytes(finalPath + ".png", bytes);
            DestroyImmediate(tex);
            rt.Release();

            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
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


        [MenuItem("Tools/Remove All Unit Data")]
        public static void RemoveAllUnitData()
        {
            // * Hero Prefab
            if (Directory.Exists(EXPORT_PATH)) 
            { 
                Directory.Delete(EXPORT_PATH, true); 
            }
            Directory.CreateDirectory(EXPORT_PATH);

            // * Hero Data
            if (Directory.Exists(UNIT_DATA_PATH)) 
            { 
                Directory.Delete(UNIT_DATA_PATH, true); 
            }
            Directory.CreateDirectory(UNIT_DATA_PATH);

            // * Json Linker
            CommonUtilities.ClearTextFileContent(LINKER_PATH);

            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
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
#endif