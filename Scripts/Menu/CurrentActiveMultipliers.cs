using UnityEngine;
using UnityEngine.UI;

namespace DopeEmpire
{
    public class CurrentActiveMultipliers : MonoBehavRefScript
    {
        #region Game Components

        [Header("---------- MULTIPLIER TEXT COMPONENTS ----------", order = 0)]
        [Header("Dirty Money Increase", order = 1)]
        [SerializeField] private Text dirtyMoneyTotalMultiplierText = null;

        [Header("Clean Money Increase")]
        [SerializeField] private Text cleanMoneyTotalMultiplierText = null;

        [Header("Reputation Increase")]
        [SerializeField] private Text reputationTotalMultiplierText = null;

        [Header("Influence Increase")]
        [SerializeField] private Text influenceTotalMultiplierText = null;

        [Header("Timer Reduction")]
        [SerializeField] private Text timerTotalMultiplierText = null;

        [Header("Launder Income Increase")]
        [SerializeField] private Text launderIncomeTotalMultiplierText = null;

        [Header("Experience Increase")]
        [SerializeField] private Text experienceTotalMultiplierText = null;

        [Header("Money Button Increase")]
        [SerializeField] private Text moneyButtonTotalMultiplierText = null;

        [Header("Research Cost Reduction")]
        [SerializeField] private Text researchCostTotalMultiplierText = null;

        #endregion Game Components

        #region Initialization

        private void Start()
        {
            UpdateTextValues();
            MultiplierManager.Instance.OnPurchaseMultiplier += UpdateTextValues;
        }

        #endregion Initialization

        #region Custom Methods

        private void UpdateTextValues()
        {
            dirtyMoneyTotalMultiplierText.text = string.Format("INCREASED BY: {0}%", Mathf.Round(MultiplierManager.Instance.DirtyMoneyIncreaseMultiplier * 100f));
            cleanMoneyTotalMultiplierText.text = string.Format("INCREASED BY: {0}%", Mathf.Round(MultiplierManager.Instance.CleanMoneyIncreaseMultiplier * 100f));
            reputationTotalMultiplierText.text = string.Format("INCREASED BY: {0}%", Mathf.Round(MultiplierManager.Instance.ReputationIncreaseMultiplier * 100f));
            influenceTotalMultiplierText.text = string.Format("INCREASED BY: {0}%", Mathf.Round(MultiplierManager.Instance.InfluenceIncreaseMultiplier * 100f));
            timerTotalMultiplierText.text = string.Format("REDUCED BY: {0}%", Mathf.Round(MultiplierManager.Instance.TimerReductionMultiplier * 100f));
            launderIncomeTotalMultiplierText.text = string.Format("INCREASED BY: {0}%", Mathf.Round(MultiplierManager.Instance.LaunderRateIncreaseMultiplier * 100f));
            experienceTotalMultiplierText.text = string.Format("INCREASED BY: {0}%", Mathf.Round(MultiplierManager.Instance.ExperienceIncreaseMultiplier * 100f));
            moneyButtonTotalMultiplierText.text = string.Format("INCREASED BY: {0}%", Mathf.Round(MultiplierManager.Instance.MoneyButtonClickIncreaseMultiplier * 100f));
            researchCostTotalMultiplierText.text = string.Format("REDUCED BY: {0}%", Mathf.Round(MultiplierManager.Instance.ResearchCostReductionMultiplier * 100f));
        }

        #endregion Custom Methods
    }
}