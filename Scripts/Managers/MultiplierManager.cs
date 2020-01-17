using System;

namespace DopeEmpire
{
    public enum MultiplierTypes
    {
        DirtyMoneyIncrease, CleanMoneyIncrease, ReputationIncrease, InfluenceIncrease, TimerReduction, LaunderRateIncrease, NumberOfProductsIncrease, ProductQuality, AllProductIncomeIncrease,
        AdBonusIncrease, AdLengthIncrease, ExperienceIncrease, SecondsBetweenWeeksReduction, MoneyButtonClickIncrease, ResearchCostReduction
    }

    public class MultiplierManager : MonoBehavRefScript
    {
        public static MultiplierManager Instance { get; private set; }

        //All the multiplers in the game. This should make the calculation process, while simplifying the code.
        public float DirtyMoneyIncreaseMultiplier { get; private set; }

        public float CleanMoneyIncreaseMultiplier { get; private set; }
        public float ReputationIncreaseMultiplier { get; private set; }
        public float InfluenceIncreaseMultiplier { get; private set; }
        public float TimerReductionMultiplier { get; private set; }

        public Product CurrentResearchedProduct { get; private set; }
        public Grade CurrentResearchedGrade { get; private set; }
        public Product NextResearchedProduct { get; private set; }
        public Grade NextResearchedGrade { get; private set; }

        public float LaunderRateIncreaseMultiplier { get; private set; }
        public float ExperienceIncreaseMultiplier { get; private set; }
        public float SecondsBetweenWeeksReductionMultiplier { get; private set; }
        public float MoneyButtonClickIncreaseMultiplier { get; private set; }
        public float ResearchCostReductionMultiplier { get; private set; }
        public float AdBonusRateIncreaseMultiplier { get; private set; }
        public float AdBonusLengthIncreaseMultiplier { get; private set; }

        public event Action OnPurchaseMultiplier;

        #region Initialization

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
        }

        public void SetAllMultiplierValuesViaJson(SaveMultiplierData json)
        {
            DirtyMoneyIncreaseMultiplier = json.productMultiplierData.dirtyMoneyIncrease;
            CleanMoneyIncreaseMultiplier = json.productMultiplierData.cleanMoneyIncrease;
            ReputationIncreaseMultiplier = json.productMultiplierData.reputationIncrease;
            InfluenceIncreaseMultiplier = json.productMultiplierData.influenceIncrease;
            TimerReductionMultiplier = json.productMultiplierData.timerReduction;

            CurrentResearchedGrade = json.productQualityData.currentGrade;
            CurrentResearchedProduct = json.productQualityData.currentProduct;
            NextResearchedGrade = json.productQualityData.nextGrade;
            NextResearchedProduct = json.productQualityData.nextProduct;

            LaunderRateIncreaseMultiplier = json.researchMultiplierData.launderRateIncrease;
            ExperienceIncreaseMultiplier = json.researchMultiplierData.experienceRateIncrease;
            SecondsBetweenWeeksReductionMultiplier = json.researchMultiplierData.secondsBetweenWeeksReduction;
            MoneyButtonClickIncreaseMultiplier = json.researchMultiplierData.moneyButtonClickIncrease;
            ResearchCostReductionMultiplier = json.researchMultiplierData.researchCostReduction;
            AdBonusRateIncreaseMultiplier = json.researchMultiplierData.adBonusRateIncrease;
            AdBonusLengthIncreaseMultiplier = json.researchMultiplierData.adBonusLengthIncrease;
        }

        #endregion Initialization

        #region Custom Methods

        public void UpdateAndApplyMultiplier(MultiplierTypes type, float bonus)
        {
            //This section here are for the shared Multipliers (bonuses where more than one different type of skills can increase it.)
            //i.e. Daily Life, Research, and Character Skill upgrades all increase these Multipliers.
            if (type == MultiplierTypes.DirtyMoneyIncrease || type == MultiplierTypes.AllProductIncomeIncrease || type == MultiplierTypes.ProductQuality)
            {
                DirtyMoneyIncreaseMultiplier += bonus;
                print(DirtyMoneyIncreaseMultiplier);
            }

            if (type == MultiplierTypes.CleanMoneyIncrease || type == MultiplierTypes.AllProductIncomeIncrease || type == MultiplierTypes.ProductQuality)
            {
                CleanMoneyIncreaseMultiplier += bonus;
            }

            if (type == MultiplierTypes.ReputationIncrease || type == MultiplierTypes.AllProductIncomeIncrease || type == MultiplierTypes.ProductQuality)
            {
                ReputationIncreaseMultiplier += bonus;
            }

            if (type == MultiplierTypes.InfluenceIncrease)
            {
                InfluenceIncreaseMultiplier += bonus;
            }

            if (type == MultiplierTypes.TimerReduction)
            {
                TimerReductionMultiplier += bonus;
            }

            if (type == MultiplierTypes.LaunderRateIncrease)
            {
                LaunderRateIncreaseMultiplier += bonus;
            }

            //This section here are for Research-Unique Multipliers.
            if (type == MultiplierTypes.ProductQuality)
            {
                CurrentResearchedGrade++;
                NextResearchedGrade++;
                if ((int)CurrentResearchedGrade > 3)
                {
                    CurrentResearchedGrade = 0;
                    CurrentResearchedProduct++;
                }

                if ((int)CurrentResearchedProduct > 5)
                {
                    CurrentResearchedProduct = (Product)5;
                    CurrentResearchedGrade = (Grade)3;
                }

                if ((int)NextResearchedGrade > 3)
                {
                    NextResearchedGrade = 0;
                    NextResearchedProduct++;
                }

                if ((int)NextResearchedProduct > 5)
                {
                    NextResearchedProduct = (Product)5;
                    NextResearchedGrade = (Grade)3;
                }
            }

            if (type == MultiplierTypes.AdBonusIncrease)
            {
                AdBonusRateIncreaseMultiplier += bonus;
            }

            if (type == MultiplierTypes.AdLengthIncrease)
            {
                AdBonusLengthIncreaseMultiplier += bonus;
            }

            if (type == MultiplierTypes.ExperienceIncrease)
            {
                ExperienceIncreaseMultiplier += bonus;
            }

            if (type == MultiplierTypes.SecondsBetweenWeeksReduction)
            {
                SecondsBetweenWeeksReductionMultiplier += bonus;
                GameManager.Instance.ApplySecondsBetweenWeeksReduction();
            }

            if (type == MultiplierTypes.MoneyButtonClickIncrease)
            {
                MoneyButtonClickIncreaseMultiplier += bonus;
                CurrencyManager.Instance.ApplyMoneyButtonMultiplier();
            }

            //This section here are for Other-Unique Multipliers.
            if (type == MultiplierTypes.NumberOfProductsIncrease)
            {
                ProductManager.Instance.IncrementTotalNumberOfProductsCanSell();
            }

            if (type == MultiplierTypes.ResearchCostReduction)
            {
                ResearchCostReductionMultiplier += bonus;
            }

            OnPurchaseMultiplier?.Invoke();
        }

        #endregion Custom Methods
    }
}