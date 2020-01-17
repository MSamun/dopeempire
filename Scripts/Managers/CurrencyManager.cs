using System;
using UnityEngine;
using UnityEngine.UI;

namespace DopeEmpire
{
    public enum Income { DirtyMoney, CleanMoney, Reputation, Influence, Crowns, Timer }

    public class CurrencyManager : MonoBehavRefScript
    {
        //Singleton.
        public static CurrencyManager Instance { get; private set; }

        public float CleanMoneyWeekly { get; private set; }
        public float CleanMoneyTotal { get; private set; }
        public float CleanMoneyLifeTime { get; private set; }
        public float DirtyMoneyWeekly { get; private set; }
        public float DirtyMoneyTotal { get; private set; }
        public float DirtyMoneyLifeTime { get; private set; }
        public float ReputationWeekly { get; private set; }
        public float ReputationTotal { get; private set; }
        public float ReputationLifeTime { get; private set; }
        public float InfluenceTotal { get; private set; }
        public float InfluenceLifetime { get; private set; }
        public float LaunderIncomeCleanMoneyLifetime { get; private set; }
        public float LaunderIncomeReputationLifetime { get; private set; }

        #region Variables

        private float localMoneyButtonIncreaseMultiplier;

        public event Action OnMoneyValuesChanged;

        #region Game Components

        [Header("---------- INCOME VALUE COMPONENTS ----------", order = 0)]
        [Header("Dirty Money", order = 1)]
        [SerializeField] private Text dirtyMoneyTotalText = null;

        [SerializeField] private Text dirtyMoneyWeeklyText = null;

        [Header("Clean Money")]
        [SerializeField] private Text cleanMoneyTotalText = null;

        [SerializeField] private Text cleanMoneyWeeklyText = null;

        [Header("Reputation")]
        [SerializeField] private Text reputationTotalText = null;

        [SerializeField] private Text reputationWeeklyText = null;

        #endregion Game Components

        #endregion Variables

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

        private void Start()
        {
            localMoneyButtonIncreaseMultiplier = 1;
            GameManager.Instance.OnWeekPassed += AddWeeklyIncomeToTotalIncomeValues;
        }

        private void FixedUpdate()
        {
            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.E))
            {
                CleanMoneyTotal += 2000;
                DirtyMoneyTotal += 2000;
                ReputationTotal += 2000;
                InfluenceTotal += 2000;
                UpdateTotalIncomeTextValues();
            }
        }

        #endregion Initialization

        #region Custom Methods

        public void SetIncomeValuesViaJson(TotalIncomeData jsonData)
        {
            //Sets the values to the ones read from the data.json file.
            DirtyMoneyTotal = jsonData.totalDirtyMoney;
            CleanMoneyTotal = jsonData.totalCleanMoney;
            ReputationTotal = jsonData.totalReputation;
            UpdateTotalIncomeTextValues();

            float dataDrivenDM = 0;
            float dataDrivenCM = 0;
            float dataDrivenREP = 0;

            //This is strictly used as a precaution. It first grabs the weekly Dirty Money, Clean Money, and Repuation values from the data.json file.
            //Then, it checks to see if the values in the data.json file match the Weekly Dirty Money, Clean Money, and Repuation calculated from all the currently owned Products.
            //If it does, it'll use the data from the .json file, otherwise it'll use the calculated value.
            //For example, let's say you own 2 Products that earn you 50 DirtyMoney each, totalling WeeklyDirtyMoney to 100. If someone changes the data.json file's WeeklyDirtyMoney to 300,
            //it won't work cause the 2 Products you own only calculates the WeeklyDirtyMoney to 100. It'll then proceed to use the caluclated value of WeeklyDirtyMoney (100).

            dataDrivenDM = jsonData.weeklyDirtyMoney;
            dataDrivenCM = jsonData.weeklyCleanMoney;
            dataDrivenREP = jsonData.weeklyReputation;

            DirtyMoneyWeekly = ProductManager.Instance.TotalProductDirtyMoneyRevenue == dataDrivenDM ? dataDrivenDM : ProductManager.Instance.TotalProductDirtyMoneyRevenue;
            CleanMoneyWeekly = ProductManager.Instance.TotalProductCleanMoneyRevenue == dataDrivenCM ? dataDrivenCM : ProductManager.Instance.TotalProductCleanMoneyRevenue;
            ReputationWeekly = ProductManager.Instance.TotalProductReputation == dataDrivenREP ? dataDrivenREP : ProductManager.Instance.TotalProductReputation;
        }

        public void SetWeeklyIncomeValues()
        {
            DirtyMoneyWeekly = ProductManager.Instance.TotalProductDirtyMoneyRevenue;
            CleanMoneyWeekly = ProductManager.Instance.TotalProductCleanMoneyRevenue;
            ReputationWeekly = ProductManager.Instance.TotalProductReputation;

            dirtyMoneyWeeklyText.text = string.Format("+ {0}", FormatValues(DirtyMoneyWeekly));
            reputationWeeklyText.text = string.Format("+ {0}", FormatValues(ReputationWeekly));
            cleanMoneyWeeklyText.text = string.Format("+ {0}", FormatValues(CleanMoneyWeekly));
        }

        public void AddWeeklyIncomeToTotalIncomeValues()
        {
            if (DirtyMoneyWeekly > 0)
            {
                DirtyMoneyLifeTime += DirtyMoneyWeekly;
                DirtyMoneyTotal += DirtyMoneyWeekly;
            }

            if (CleanMoneyWeekly > 0)
            {
                CleanMoneyLifeTime += CleanMoneyWeekly;
                CleanMoneyTotal += CleanMoneyWeekly;
            }

            if (ReputationWeekly > 0)
            {
                ReputationLifeTime += ReputationWeekly;
                ReputationTotal += ReputationWeekly;
            }

            float experienceEarned = DirtyMoneyWeekly + CleanMoneyWeekly + ReputationWeekly;
            PlayerLevelManager.Instance.AddExperience(experienceEarned);
            UpdateTotalIncomeTextValues();
        }

        public void AddWeeklyIncomeAndExperienceViaMoneyButton()
        {
            if (DirtyMoneyWeekly > 0)
            {
                DirtyMoneyLifeTime += DirtyMoneyWeekly * localMoneyButtonIncreaseMultiplier;
                DirtyMoneyTotal += DirtyMoneyWeekly * localMoneyButtonIncreaseMultiplier;
            }

            if (CleanMoneyWeekly > 0)
            {
                CleanMoneyLifeTime += CleanMoneyWeekly * localMoneyButtonIncreaseMultiplier;
                CleanMoneyTotal += CleanMoneyWeekly * localMoneyButtonIncreaseMultiplier;
            }

            if (ReputationWeekly > 0)
            {
                ReputationLifeTime += ReputationWeekly * localMoneyButtonIncreaseMultiplier;
                ReputationTotal += ReputationWeekly * localMoneyButtonIncreaseMultiplier;
            }

            float experienceEarned = (DirtyMoneyWeekly * localMoneyButtonIncreaseMultiplier) + (CleanMoneyWeekly * localMoneyButtonIncreaseMultiplier) + (ReputationWeekly * localMoneyButtonIncreaseMultiplier);
            PlayerLevelManager.Instance.AddExperience(experienceEarned);
            UpdateTotalIncomeTextValues();
        }

        private void UpdateTotalIncomeTextValues()
        {
            dirtyMoneyTotalText.text = string.Format(FormatValues(DirtyMoneyTotal));
            reputationTotalText.text = string.Format(FormatValues(ReputationTotal));
            cleanMoneyTotalText.text = string.Format(FormatValues(CleanMoneyTotal));

            OnMoneyValuesChanged?.Invoke();
        }

        public void TotalIncomeGate(bool isDeduct, Income type, float value)
        {
            if (type == Income.DirtyMoney)
            {
                if (isDeduct)
                {
                    DirtyMoneyTotal -= value;
                }

                if (!isDeduct)
                {
                    DirtyMoneyTotal += value;
                    DirtyMoneyLifeTime += value;
                }
            }

            if (type == Income.CleanMoney)
            {
                if (isDeduct)
                {
                    CleanMoneyTotal -= value;
                }

                if (!isDeduct)
                {
                    CleanMoneyTotal += value;
                    CleanMoneyLifeTime += value;
                }
            }

            if (type == Income.Reputation)
            {
                if (isDeduct)
                {
                    ReputationTotal -= value;
                }

                if (!isDeduct)
                {
                    ReputationTotal += value;
                    ReputationLifeTime += value;
                }
            }

            if (type == Income.Influence)
            {
                if (isDeduct)
                {
                    InfluenceTotal -= value;
                }

                if (!isDeduct)
                {
                    InfluenceTotal += value;
                    InfluenceLifetime += value;
                }
            }

            UpdateTotalIncomeTextValues();
        }

        //Used to keep track of Launder Income Lifetime values. Too lazy to implement it through the TotalIncomeGate.
        public void LaunderIncomeGate(Income type, float value)
        {
            if (type == Income.CleanMoney)
            {
                CleanMoneyTotal += value;
                CleanMoneyLifeTime += value;
                LaunderIncomeCleanMoneyLifetime += value;
            }

            if (type == Income.Reputation)
            {
                ReputationTotal += value;
                ReputationLifeTime += value;
                LaunderIncomeReputationLifetime += value;
            }
        }

        #region Format Values

        public string FormatValues(float value)
        {
            string[] moneySuffixes = new string[] { "M", "B", "T" };
            double suffixIndex = 0;
            float totalDigitsDivider = 0;

            string correctSuffix = string.Empty;
            string newValueText = string.Empty;
            float newValue = 0;

            suffixIndex = Math.Floor(Math.Log10(value));        //Determines how many integers are in the number (before the first number). ie: 8,234,567 = 6 integers.

            if (suffixIndex >= 6 && suffixIndex <= 8)  //Between 1M - 999M.
            {
                //Use the "M" suffix.
                suffixIndex = 6;
                correctSuffix = moneySuffixes[0];
            }
            else if (suffixIndex >= 9 && suffixIndex <= 11) //Between 1B - 999B.
            {
                //Use the "B" suffix.
                suffixIndex = 9;
                correctSuffix = moneySuffixes[1];
            }
            else if (suffixIndex >= 12)                     //Over 1T.
            {
                //Use the "T" suffix.
                suffixIndex = 12;
                correctSuffix = moneySuffixes[2];
            }

            //Divide the value by the proper amount (1K for values between 1K - 999K, 1M for values between 1M - 999M, etc).
            totalDigitsDivider = (float)Math.Pow(10, suffixIndex);

            //Above 1M
            if (suffixIndex >= 6)
            {
                //Round to two decimal places.
                newValue = Mathf.Round((value / totalDigitsDivider) * 100f) / 100f;
                newValueText = string.Format("{0:#0.00}{1}", newValue.ToString(), correctSuffix);
            }
            else
            {
                newValue = Mathf.Round(value * 100f) / 100f;
                newValueText = string.Format("{0:#0.00}{1}", newValue.ToString("n0"), correctSuffix);
            }

            return newValueText;
        }

        public void ApplyMoneyButtonMultiplier()
        {
            if (localMoneyButtonIncreaseMultiplier != MultiplierManager.Instance.MoneyButtonClickIncreaseMultiplier)
            {
                localMoneyButtonIncreaseMultiplier = 1 + MultiplierManager.Instance.MoneyButtonClickIncreaseMultiplier;
            }
            return;
        }

        #endregion Format Values

        #endregion Custom Methods
    }
}