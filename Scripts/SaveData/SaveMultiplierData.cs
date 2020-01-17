﻿using System;

namespace DopeEmpire
{
    [Serializable]
    public class SaveMultiplierData
    {
        public ProductMultiplierData productMultiplierData;
        public ProductQualityData productQualityData;
        public ResearchMultiplierData researchMultiplierData;

        public SaveMultiplierData()
        {
        }
    }

    // All multipliers that affect the income generated by Products.
    [Serializable]
    public class ProductMultiplierData
    {
        public float dirtyMoneyIncrease;
        public float cleanMoneyIncrease;
        public float reputationIncrease;
        public float influenceIncrease;
        public float timerReduction;

        public ProductMultiplierData(float dirtyMoneyIncrease, float cleanMoneyIncrease, float reputationIncrease, float influenceIncrease, float timerReduction)
        {
            this.dirtyMoneyIncrease = dirtyMoneyIncrease;
            this.cleanMoneyIncrease = cleanMoneyIncrease;
            this.reputationIncrease = reputationIncrease;
            this.influenceIncrease = influenceIncrease;
            this.timerReduction = timerReduction;
        }
    }

    [Serializable]
    public class ProductQualityData
    {
        public Grade currentGrade;
        public Product currentProduct;
        public Grade nextGrade;
        public Product nextProduct;

        public ProductQualityData(Grade currentGrade, Product currentProduct, Grade nextGrade, Product nextProduct)
        {
            this.currentGrade = currentGrade;
            this.currentProduct = currentProduct;
            this.nextGrade = nextGrade;
            this.nextProduct = nextProduct;
        }
    }

    // All multipliers that are purchased through the Research panel.
    [Serializable]
    public class ResearchMultiplierData
    {
        public float launderRateIncrease;
        public float experienceRateIncrease;
        public float secondsBetweenWeeksReduction;
        public float moneyButtonClickIncrease;
        public float researchCostReduction;
        public float adBonusLengthIncrease;
        public float adBonusRateIncrease;

        public ResearchMultiplierData(float launderRateIncrease, float experienceRateIncrease, float secondsBetweenWeeksReduction, float moneyButtonClickIncrease,
            float researchCostReduction, float adBonusLengthIncrease, float adBonusRateIncrease)
        {
            this.launderRateIncrease = launderRateIncrease;
            this.experienceRateIncrease = experienceRateIncrease;
            this.secondsBetweenWeeksReduction = secondsBetweenWeeksReduction;
            this.moneyButtonClickIncrease = moneyButtonClickIncrease;
            this.researchCostReduction = researchCostReduction;
            this.adBonusLengthIncrease = adBonusLengthIncrease;
            this.adBonusRateIncrease = adBonusRateIncrease;
        }
    }
}