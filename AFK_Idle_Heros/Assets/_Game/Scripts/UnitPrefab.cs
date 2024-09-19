using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;

namespace AFK.Idle 
{
    public enum eAnimationDetail
    {
        NO_DETAIL,
        NORMAL_ATTACK_MELEE,
        NORMAL_ATTACK_RANGE,
        NORMAL_ATTACK_SKILL,
        SPECIAL_ATTACK_MELEE,
        SPECIAL_ATTACK_RANGE,
        SPECIAL_ATTACK_SKILL,
        CONCENTRATE,
        BUFF,
        SIT
    }
    public class HeroData
    {
        public string SPUMCode;
        public string HeroCode;
        public string HeroName;
    }
    public class UnitPrefab : MonoBehaviour
    {
        private Dictionary<eAnimationDetail, string> animationDetailDict = new Dictionary<eAnimationDetail, string>
        {
            {eAnimationDetail.NORMAL_ATTACK_MELEE, "0_attack_normal"},
            {eAnimationDetail.NORMAL_ATTACK_RANGE, "0_attack_bow"},
            {eAnimationDetail.NORMAL_ATTACK_SKILL, "0_attack_magic"},
            {eAnimationDetail.SPECIAL_ATTACK_MELEE, "1_skill_normal"},
            {eAnimationDetail.SPECIAL_ATTACK_RANGE, "1_skill_bow"},
            {eAnimationDetail.SPECIAL_ATTACK_SKILL, "1_skill_magic"},
            {eAnimationDetail.CONCENTRATE, "0_conecntrate"},
            {eAnimationDetail.BUFF, "0_buff"},
            {eAnimationDetail.SIT, "sit"}
        };
        public Animator Anim;
        public HeroData Data;
        public bool showData;
        [ShowIf(nameof(showData)), SerializeField] private string unitType;
        [ShowIf(nameof(showData)), SerializeField] private List<SpumPackage> spumPackages = new List<SpumPackage>();
        [ShowIf(nameof(showData)), SerializeField] private AnimatorOverrideController overrideController;    
        [ShowIf(nameof(showData)), SerializeField] private Dictionary<string, Dictionary<eAnimationDetail, AnimationClip>> stateAnimationPairs = new();
        [ShowIf(nameof(showData)), SerializeField] private Dictionary<eAnimationDetail, AnimationClip> idleList = new();
        [ShowIf(nameof(showData)), SerializeField] private Dictionary<eAnimationDetail, AnimationClip> moveList = new();
        [ShowIf(nameof(showData)), SerializeField] private Dictionary<eAnimationDetail, AnimationClip> attackList = new();
        [ShowIf(nameof(showData)), SerializeField] private Dictionary<eAnimationDetail, AnimationClip> damagedList = new();
        [ShowIf(nameof(showData)), SerializeField] private Dictionary<eAnimationDetail, AnimationClip> debuffList = new();
        [ShowIf(nameof(showData)), SerializeField] private Dictionary<eAnimationDetail, AnimationClip> deathList = new();
        [ShowIf(nameof(showData)), SerializeField] private Dictionary<eAnimationDetail, AnimationClip> otherList = new();
#if UNITY_EDITOR
        public void CopySpumPackage(List<SpumPackage> packages)
        {
            spumPackages = packages;
        }
        public void CopyUnitType(string type)
        {
            unitType = type;
        }
        [Button]
        public void QuickSetup()
        {
            PopulateAnimationLists();
            OverrideControllerInit();
        }
        public void OverrideControllerInit()
        {
            Animator animator = this.Anim;
            this.overrideController = new AnimatorOverrideController();
            this.overrideController.runtimeAnimatorController = animator.runtimeAnimatorController;

            AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;

            foreach (AnimationClip clip in clips)
            {
                this.overrideController[clip.name] = clip;
            }

            animator.runtimeAnimatorController= this.overrideController;
            foreach (PlayerState state in Enum.GetValues(typeof(PlayerState)))
            {
                var stateText = state.ToString();
                stateAnimationPairs[stateText] = new Dictionary<eAnimationDetail, AnimationClip>();
                switch (stateText)
                {
                    case "IDLE":
                        stateAnimationPairs[stateText] = idleList;
                        break;
                    case "MOVE":
                        stateAnimationPairs[stateText] = moveList;
                        break;
                    case "ATTACK":
                        stateAnimationPairs[stateText] = attackList;
                        break;
                    case "DAMAGED":
                        stateAnimationPairs[stateText] = damagedList;
                        break;
                    case "DEBUFF":
                        stateAnimationPairs[stateText] = debuffList;
                        break;
                    case "DEATH":
                        stateAnimationPairs[stateText] = deathList;
                        break;
                    case "OTHER":
                        stateAnimationPairs[stateText] = otherList;
                        break;
                }
            }
        }
        public void PopulateAnimationLists()
        {
            idleList = new();
            moveList = new();
            attackList = new();
            damagedList = new();
            debuffList = new();
            deathList = new();
            otherList = new();
            
            var groupedClips = spumPackages
                .SelectMany(package => package.SpumAnimationData)
                .Where(
                    spumClip => 
                        spumClip.HasData && 
                        spumClip.UnitType.Equals(unitType) && 
                        spumClip.index > -1 
                )
                .GroupBy(spumClip => spumClip.StateType)
                .ToDictionary(
                        group => group.Key, 
                        group => group.OrderBy(clip => clip.index).ToList()
                );
            foreach (var kvp in groupedClips)
            {
                var stateType = kvp.Key;
                var orderedClips = kvp.Value;
                switch (stateType)
                {
                    case "IDLE":
                        this.idleList.TryAdd(eAnimationDetail.NO_DETAIL, orderedClips.Select(clip => LoadAnimationClip(clip.ClipPath)).FirstOrDefault());
                        //StateAnimationPairs[stateType] = IDLE_List;
                        break;
                    case "MOVE":
                        this.moveList.TryAdd(eAnimationDetail.NO_DETAIL, orderedClips.Select(clip => LoadAnimationClip(clip.ClipPath)).FirstOrDefault());
                        //StateAnimationPairs[stateType] = MOVE_List;
                        break;
                    case "ATTACK":
                        var attackClips = orderedClips.Select(clip => LoadAnimationClip(clip.ClipPath));
                        foreach (var clip in attackClips)
                        {
                            foreach (var detail in animationDetailDict)
                            {
                                if (clip.name.ToLower().Contains(detail.Value))
                                {
                                    this.attackList.TryAdd(detail.Key, clip);
                                }
                            }
                        }
                        break;
                    case "DAMAGED":
                        this.damagedList.TryAdd(eAnimationDetail.NO_DETAIL, orderedClips.Select(clip => LoadAnimationClip(clip.ClipPath)).FirstOrDefault());
                        //StateAnimationPairs[stateType] = DAMAGED_List;
                        break;
                    case "DEBUFF":
                        this.deathList.TryAdd(eAnimationDetail.NO_DETAIL, orderedClips.Select(clip => LoadAnimationClip(clip.ClipPath)).FirstOrDefault());
                        //StateAnimationPairs[stateType] = DEBUFF_List;
                        break;
                    case "DEATH":
                        this.deathList.TryAdd(eAnimationDetail.NO_DETAIL, orderedClips.Select(clip => LoadAnimationClip(clip.ClipPath)).FirstOrDefault());
                        //StateAnimationPairs[stateType] = DEATH_List;
                        break;
                    case "OTHER":
                        var otherClips = orderedClips.Select(clip => LoadAnimationClip(clip.ClipPath));
                        foreach (var clip in otherClips)
                        {
                            foreach (var detail in animationDetailDict)
                            {
                                if (clip.name.ToLower().Contains(detail.Value))
                                {
                                    this.otherList.TryAdd(detail.Key, clip);
                                }
                            }
                        }
                        break;
                }
            }
        }
        AnimationClip LoadAnimationClip(string clipPath)
        {
            AnimationClip clip = Resources.Load<AnimationClip>(clipPath.Replace(".anim", ""));
            if (clip == null)
            {
                Debug.LogWarning($"Failed to load animation clip '{clipPath}'.");
            }
            
            return clip;
        }
#endif
        public void PlayAnimation(PlayerState PlayState, eAnimationDetail detail){
            Animator animator = this.Anim;
            //Debug.Log(PlayState.ToString());
            var animations =  stateAnimationPairs[PlayState.ToString()];
            //Debug.Log(OverrideController[PlayState.ToString()].name);
            this.overrideController[PlayState.ToString()] = animations[detail];
            //Debug.Log( OverrideController[PlayState.ToString()].name);
            var StateStr = PlayState.ToString();
    
            bool isMove = StateStr.Contains("MOVE");
            bool isDebuff = StateStr.Contains("DEBUFF");
            bool isDeath = StateStr.Contains("DEATH");
            animator.SetBool("1_Move", isMove);
            animator.SetBool("5_Debuff", isDebuff);
            animator.SetBool("isDeath", isDeath);
            if(!isMove && !isDebuff)
            {
                AnimatorControllerParameter[] parameters = animator.parameters;
                foreach (AnimatorControllerParameter parameter in parameters)
                {
                    if(parameter.type == AnimatorControllerParameterType.Trigger)
                    {
                        bool isTrigger = parameter.name.ToUpper().Contains(StateStr.ToUpper());
                        if(isTrigger){
                            Debug.Log($"Parameter: {parameter.name}, Type: {parameter.type}");
                            animator.SetTrigger(parameter.name);
                        }
                    }
                }
            }
        }
    }
}
