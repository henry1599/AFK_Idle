using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;
using RotaryHeart.Lib.SerializableDictionary;

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
    public enum eUnitType
    {
        HERO,
        HORSE
    }
    public enum eUnitAttackKind
    {
        MELEE,
        RANGE,
        SKILL
    }
    public enum eUnitAttackType
    {
        NORMAL,
        SPECIAL
    }
    [System.Serializable]
    public class AnimationDict : SerializableDictionaryBase<eAnimationDetail, AnimationClip> {}
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
        private UnitData data;
        [ShowInInspector] public UnitData Data => this.data;
        public bool showData;
        [ShowIf(nameof(showData)), SerializeField] private string unitType;
        [ShowIf(nameof(showData)), SerializeField] private List<SpumPackage> spumPackages = new List<SpumPackage>();
        [ShowIf(nameof(showData)), SerializeField] private AnimatorOverrideController overrideController;
        [ShowIf(nameof(showData)), SerializeField] private Dictionary<string, AnimationDict> stateAnimationPairs = new();
        [ShowIf(nameof(showData)), SerializeField] private AnimationDict idleList = new();
        [ShowIf(nameof(showData)), SerializeField] private AnimationDict moveList = new();
        [ShowIf(nameof(showData)), SerializeField] private AnimationDict attackList = new();
        [ShowIf(nameof(showData)), SerializeField] private AnimationDict damagedList = new();
        [ShowIf(nameof(showData)), SerializeField] private AnimationDict debuffList = new();
        [ShowIf(nameof(showData)), SerializeField] private AnimationDict deathList = new();
        [ShowIf(nameof(showData)), SerializeField] private AnimationDict otherList = new();
        public bool HasHorse => string.Compare(this.unitType, "horse", StringComparison.OrdinalIgnoreCase) == 0;
        void Start()
        {
            OverrideControllerInit();
        }
        public void Setup(UnitData data)
        {
            this.data = data;
        }
        public void OverrideControllerInit()
        {
            Animator animator = this.Anim;
            overrideController = new AnimatorOverrideController();
            overrideController.runtimeAnimatorController= animator.runtimeAnimatorController;

            AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;

            foreach (AnimationClip clip in clips)
            {
                overrideController[clip.name] = clip;
            }

        animator.runtimeAnimatorController= overrideController;
            foreach (PlayerState state in Enum.GetValues(typeof(PlayerState)))
            {
                var stateText = state.ToString();
                stateAnimationPairs[stateText] = new AnimationDict();
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
        public void PlayAnimation(PlayerState PlayState, eAnimationDetail detail = eAnimationDetail.NO_DETAIL)
        {
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

        [Button("Play Idle")]
        public void PlayIdle()
        {
            PlayAnimation(PlayerState.IDLE);
        }
        [Button("Play Move")]
        public void PlayMove()
        {
            PlayAnimation(PlayerState.MOVE);
        }
        public void PlayAttack(float attackSpeed, eUnitAttackType attackType = default)
        {
            this.Anim.SetFloat("AttackSpeed", attackSpeed);
            switch (attackType)
            {
                case eUnitAttackType.NORMAL:
                    switch (Data.AttackKind)
                    {
                        case eUnitAttackKind.MELEE:
                            PlayNormalMeleeAttack(attackSpeed);
                            break;
                        case eUnitAttackKind.RANGE:
                            PlayNormalRangeAttack(attackSpeed);
                            break;
                        case eUnitAttackKind.SKILL:
                            PlayNormalSkillAttack(attackSpeed);
                            break;
                    }
                    break;
                case eUnitAttackType.SPECIAL:
                    switch (Data.AttackKind)
                    {
                        case eUnitAttackKind.MELEE:
                            PlaySpecialMeleeAttack(attackSpeed);
                            break;
                        case eUnitAttackKind.RANGE:
                            PlaySpecialRangeAttack(attackSpeed);
                            break;
                        case eUnitAttackKind.SKILL:
                            PlaySpecialSkillAttack(attackSpeed);
                            break;
                    }
                    break;
            }
        }
        [Button("Play Normal Melee Attack")]
        public void PlayNormalMeleeAttack(float attackSpeed)
        {
            this.Anim.SetFloat("AttackSpeed", attackSpeed);
            PlayAnimation(PlayerState.ATTACK, eAnimationDetail.NORMAL_ATTACK_MELEE);
        }
        [Button("Play Normal Range Attack")]
        public void PlayNormalRangeAttack(float attackSpeed)
        {
            this.Anim.SetFloat("AttackSpeed", attackSpeed);
            PlayAnimation(PlayerState.ATTACK, eAnimationDetail.NORMAL_ATTACK_RANGE);
        }
        [Button("Play Normal Skill Attack")]
        public void PlayNormalSkillAttack(float attackSpeed)
        {
            this.Anim.SetFloat("AttackSpeed", attackSpeed);
            PlayAnimation(PlayerState.ATTACK, eAnimationDetail.NORMAL_ATTACK_SKILL);
        }
        [Button("Play Special Melee Attack")]
        public void PlaySpecialMeleeAttack(float attackSpeed)
        {
            this.Anim.SetFloat("AttackSpeed", attackSpeed);
            PlayAnimation(PlayerState.ATTACK, eAnimationDetail.SPECIAL_ATTACK_MELEE);
        }
        [Button("Play Special Range Attack")]
        public void PlaySpecialRangeAttack(float attackSpeed)
        {
            this.Anim.SetFloat("AttackSpeed", attackSpeed);
            PlayAnimation(PlayerState.ATTACK, eAnimationDetail.SPECIAL_ATTACK_RANGE);
        }
        [Button("Play Special Skill Attack")]
        public void PlaySpecialSkillAttack(float attackSpeed)
        {
            this.Anim.SetFloat("AttackSpeed", attackSpeed);
            PlayAnimation(PlayerState.ATTACK, eAnimationDetail.SPECIAL_ATTACK_SKILL);
        }
        [Button("Play Concentrate")]
        public void PlayConcentrate()
        {
            PlayAnimation(PlayerState.ATTACK, eAnimationDetail.CONCENTRATE);
        }
        [Button("Play Buff")]
        public void PlayBuff()
        {
            PlayAnimation(PlayerState.ATTACK, eAnimationDetail.BUFF);
        }
        [Button("Play Sit")]
        public void PlaySit()
        {
            PlayAnimation(PlayerState.ATTACK, eAnimationDetail.SIT);
        }
        public void PlayDeath()
        {
            PlayAnimation(PlayerState.DEATH);
        }
    }
}
