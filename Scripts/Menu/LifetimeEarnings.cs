using UnityEngine;
using UnityEngine.UI;

namespace DopeEmpire
{
    public class LifetimeEarnings : MonoBehavRefScript
    {
        #region Game Components

        [Header("---------- LIFETIME TEXT COMPONENTS ----------", order = 0)]
        [Header("Dirty Money", order = 1)]
        [SerializeField] private Text dirtyMoneyLifetimeText = null;

        [Header("Clean Money")]
        [SerializeField] private Text cleanMoneyLifetimeText = null;

        [Header("Reputation")]
        [SerializeField] private Text reputationLifetimeText = null;

        [Header("Influence")]
        [SerializeField] private Text influenceLifetimeText = null;

        [Header("Experience")]
        [SerializeField] private Text experienceLifetimeText = null;

        [Header("Launder Income")]
        [SerializeField] private Text launderCleanMoneyLifetimeText = null;

        [SerializeField] private Text launderReputationLifetimeText = null;

        #endregion Game Components

        #region Initialization

        private void Start()
        {
            UpdateLifetimeTextValues();
            CurrencyManager.Instance.OnMoneyValuesChanged += UpdateLifetimeTextValues;
        }

        #endregion Initialization

        #region Custom Methods

        private void UpdateLifetimeTextValues()
        {
            dirtyMoneyLifetimeText.text = string.Format(CurrencyManager.Instance.FormatValues(CurrencyManager.Instance.DirtyMoneyLifeTime));
            cleanMoneyLifetimeText.text = string.Format(CurrencyManager.Instance.FormatValues(CurrencyManager.Instance.CleanMoneyLifeTime));
            reputationLifetimeText.text = string.Format(CurrencyManager.Instance.FormatValues(CurrencyManager.Instance.ReputationLifeTime));
            influenceLifetimeText.text = string.Format(CurrencyManager.Instance.FormatValues(CurrencyManager.Instance.InfluenceLifetime));
            experienceLifetimeText.text = string.Format(CurrencyManager.Instance.FormatValues(PlayerLevelManager.Instance.LifetimeExperienceEarned));
            launderCleanMoneyLifetimeText.text = string.Format(CurrencyManager.Instance.FormatValues(CurrencyManager.Instance.LaunderIncomeCleanMoneyLifetime));
            launderReputationLifetimeText.text = string.Format(CurrencyManager.Instance.FormatValues(CurrencyManager.Instance.LaunderIncomeReputationLifetime));
        }

        #endregion Custom Methods
    }
}