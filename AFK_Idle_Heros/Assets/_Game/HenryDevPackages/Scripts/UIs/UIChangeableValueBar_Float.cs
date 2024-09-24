using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HenryDev
{
    public enum eFillType
    {
        Static = 0,
        Dynamic = 1,
        DynamicDiff = 2
    }
    public class UIChangeableValueBar_Float : MonoBehaviour
    {
        [SerializeField] Image fillImage;
        [SerializeField] Color fillColor;

        [SerializeField] eFillType fillType;

        [BoxGroup("Diff setting"), ShowIf(nameof(fillType), eFillType.DynamicDiff), SerializeField] Color reduceDiffColor;
        [BoxGroup("Diff setting"), ShowIf(nameof(fillType), eFillType.DynamicDiff), SerializeField] Color increaseDiffColor;
        [BoxGroup("Diff setting"), ShowIf(nameof(fillType), eFillType.DynamicDiff), SerializeField] float dynamicCatchupSpeed = 5;
        [BoxGroup("Diff setting"), ShowIf(nameof(fillType), eFillType.DynamicDiff), SerializeField] float dynamicDiffSpeed = 2;
        [BoxGroup("Diff setting"), ShowIf(nameof(fillType), eFillType.DynamicDiff), SerializeField, Required] Image fillImageDiffDecrease;
        [BoxGroup("Diff setting"), ShowIf(nameof(fillType), eFillType.DynamicDiff), SerializeField, Required] Image fillImageDiffIncrease;
        [BoxGroup("Diff setting"), ShowIf(nameof(fillType), eFillType.DynamicDiff), SerializeField] float waitingTime = 0.5f;



        [BoxGroup("Dynamic setting"), ShowIf(nameof(fillType), eFillType.Dynamic), SerializeField] float dynamicSpeed = 3;


        [BoxGroup("Text Value"), SerializeField] bool isShowText;
        [BoxGroup("Text Value"), ShowIf(nameof(isShowText)), SerializeField] TMP_Text tmpText;
        
        private float currentValue;
        protected IChangeableValue<float> changeableValue;
        private float waitingTimeCounter;
        private bool isReducing = false;
        public void Init(IChangeableValue<float> changeableValue)
        {
            this.changeableValue = changeableValue;
            if (this.changeableValue == null)
                return;



            // * Values
            this.waitingTimeCounter = waitingTime;
            this.currentValue = changeableValue.GetValueNormalized();


            // * Colors
            this.fillImageDiffDecrease.color = fillColor;
            this.fillImageDiffIncrease.color = increaseDiffColor;
            this.fillImage.color = reduceDiffColor;


            // * Text
            this.tmpText.gameObject.SetActive(this.isShowText);
            UpdateText();


            // * Events
            this.changeableValue.OnValueChanged += HandleValueChanged;
            this.changeableValue.OnEmpty += HandleValueEmpty;
        }
        void Update()
        {
            switch (this.fillType)
            {
                case eFillType.Static:
                    UpdateFillStatic();
                    break;
                case eFillType.Dynamic:
                    UpdateFillDynamic();
                    break;
                case eFillType.DynamicDiff:
                    UpdateFillDiffDynamic();
                    break;
            }
        }

        void OnDestroy()
        {
            if (this.changeableValue == null)
                return;
            this.changeableValue.OnValueChanged -= HandleValueChanged;
            this.changeableValue.OnEmpty -= HandleValueEmpty;
        }
        void UpdateValue()
        {
            this.waitingTimeCounter = this.waitingTime;
            var lastValue = this.currentValue;
            this.currentValue = this.changeableValue.GetValueNormalized();
            this.isReducing = lastValue > this.currentValue;


            if (this.isShowText)
                UpdateText();
        }
        void UpdateFillDynamic()
        {
            this.fillImage.fillAmount = Mathf.MoveTowards(this.fillImage.fillAmount, this.currentValue, Time.deltaTime * this.dynamicSpeed);
            this.fillImageDiffIncrease.fillAmount = this.fillImage.fillAmount;
            this.fillImageDiffDecrease.fillAmount = this.fillImage.fillAmount;
        }
        void UpdateFillDiffDynamic()
        {
            Image fillImageDiff = this.isReducing ? this.fillImageDiffDecrease : this.fillImageDiffIncrease;
            fillImageDiff.fillAmount = Mathf.MoveTowards(fillImageDiff.fillAmount, this.currentValue, Time.deltaTime * this.dynamicDiffSpeed);
            if (this.waitingTimeCounter > 0)
            {
                this.waitingTimeCounter -= Time.deltaTime;
                return;
            }
            this.fillImage.fillAmount = Mathf.MoveTowards(this.fillImage.fillAmount, this.currentValue, Time.deltaTime * this.dynamicCatchupSpeed);

            Image oppositeFillImageDiff = this.isReducing ? this.fillImageDiffIncrease : this.fillImageDiffDecrease;
            oppositeFillImageDiff.fillAmount = Mathf.MoveTowards(oppositeFillImageDiff.fillAmount, this.currentValue, Time.deltaTime * this.dynamicDiffSpeed);
        }

        void UpdateFillStatic()
        {
            this.fillImage.fillAmount = this.currentValue;
            this.fillImageDiffDecrease.fillAmount = this.currentValue;
            this.fillImageDiffIncrease.fillAmount = this.currentValue;
        }
        void UpdateText()
        {
            this.tmpText.text = string.Format("{0}/{1}", this.changeableValue.GetValue(), this.changeableValue.GetMaxValue());
        }
        private void HandleValueChanged(float value)
        {
            UpdateValue();
        }
        private void HandleValueEmpty()
        {
            UpdateValue();
            this.isReducing = true;
        }
    }
}
