using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace DopeEmpire
{
    public class GameManager : MonoBehavRefScript
    {
        //Singleton.
        public static GameManager Instance { get; private set; }

        public float SecondsBetweenWeeks
        {
            get
            {
                return secondsBetweenWeeks;
            }
        }

        #region Variables

        private float localSecondsBetweenWeeksReduction;

        //Event Actions.
        public event Action OnWeekPassed;

        public event Action OnMonthPassed;

        public event Action OnYearPassed;

        /*Total gameplay time.
        private float gameplayTime = 0f;*/

        //Used to determine how many seconds have passed.
        private float currentSeconds = 0f;

        //Used to loop the co-routines.
        private bool weeklyTimer;

        //Current number of weeks, months, and years that have passed in-game.
        private int currentWeek;

        private int currentMonthIndex;
        private int currentYear;

        private readonly string[] arrayOfMonths = new string[] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };

        //Timer until button gives player money.
        private bool hasClickedButton = false;

        private float currentTimer = 0f;
        private float totalTimeToSell;

        #region Game Components

        [Header("---------- VARIABLE DECLARATIONS ----------", order = 0)]
        [SerializeField] private float secondsBetweenWeeks = 3f;

        [Header("---------- GAME TIME COMPONENTS ----------", order = 0)]
        [Header("Week ##, Month, ####", order = 1)]
        [SerializeField] private Text timePassedText = null;

        //[Header("Total Gameplay Time")]
        //[SerializeField]
        //private Text gameplayTimeText;
        [Header("Time Slider")]
        [SerializeField] private Slider timeSlider = null;

        [Header("---------- MONEY GEN. BUTTON COMPONENTS ----------", order = 0)]
        [Header("Timer Text", order = 1)]
        [SerializeField] private Text totalTimeText = null;

        [Header("Progress Bar Fill")]
        [SerializeField] private Image progressBarFill = null;

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
            currentWeek = 1;
            currentMonthIndex = 0;
            currentYear = 1996;
            localSecondsBetweenWeeksReduction = 0;

            weeklyTimer = true;
            timePassedText.text = string.Format("Week 0{0}, {1} {2}", currentWeek, arrayOfMonths[currentMonthIndex], currentYear);
            progressBarFill.fillAmount = 1f;

            StartCoroutine(ManageWeeklyTimer());
            SetTotalTimeToSell();
        }

        #endregion Initialization

        #region Custom Methods

        private IEnumerator ManageWeeklyTimer()
        {
            while (weeklyTimer)
            {
                while (currentSeconds < secondsBetweenWeeks)
                {
                    timeSlider.maxValue = secondsBetweenWeeks;
                    currentSeconds += Time.fixedDeltaTime;

                    float percentage = currentSeconds / timeSlider.maxValue;
                    timeSlider.value = Mathf.Lerp(0, secondsBetweenWeeks, percentage);
                    yield return new WaitForSeconds(1f / (float)Math.Pow(10, 10));
                }

                if (currentSeconds >= secondsBetweenWeeks)
                {
                    currentWeek++;
                    OnWeekPassed?.Invoke();

                    if (currentWeek > 4)
                    {
                        currentMonthIndex++;
                        currentWeek = 1;

                        OnMonthPassed?.Invoke();
                    }

                    if (currentMonthIndex > 11)
                    {
                        currentYear++;
                        currentMonthIndex = 0;
                        currentWeek = 1;

                        OnYearPassed?.Invoke();
                    }

                    timePassedText.text = string.Format("Week 0{0}, {1} {2}", currentWeek, arrayOfMonths[currentMonthIndex], currentYear);
                    currentSeconds = 0f;
                    yield return new WaitForSeconds(1f / (float)Math.Pow(10, 10));
                }
            }
        }

        public void MoneyGenButtonOnClick()
        {
            if (!hasClickedButton)
            {
                SetTotalTimeToSell();
                currentTimer = totalTimeToSell;
                StartCoroutine(ManageMoneyGenClick());
                hasClickedButton = true;
            }
            return;
        }

        public void SetTotalTimeToSell()
        {
            totalTimeToSell = ProductManager.Instance.TotalTimeToSell;
            totalTimeText.text = string.Format("{0:#0.00}s", Mathf.Round(totalTimeToSell * 100f) / 100f);
        }

        private IEnumerator ManageMoneyGenClick()
        {
            while (currentTimer > 0)
            {
                currentTimer -= Time.fixedDeltaTime;
                totalTimeText.text = string.Format("{0:#0.00}s", Mathf.Round(currentTimer * 100f) / 100f);

                float percentage = currentTimer / totalTimeToSell;

                progressBarFill.fillAmount = Mathf.Lerp(0f, 1f, percentage);
                yield return new WaitForSeconds(1f / (float)Math.Pow(10, 10));
            }

            if (currentTimer <= 0)
            {
                currentTimer = 0;
                totalTimeText.text = string.Format("{0:#0.00}s", Mathf.Round(currentTimer * 100f) / 100f);

                progressBarFill.fillAmount = 1f;
                hasClickedButton = false;
                SetTotalTimeToSell();

                CurrencyManager.Instance.AddWeeklyIncomeAndExperienceViaMoneyButton();
            }
        }

        public void ApplySecondsBetweenWeeksReduction()
        {
            if (localSecondsBetweenWeeksReduction != MultiplierManager.Instance.SecondsBetweenWeeksReductionMultiplier)
            {
                secondsBetweenWeeks += localSecondsBetweenWeeksReduction;
                secondsBetweenWeeks -= MultiplierManager.Instance.SecondsBetweenWeeksReductionMultiplier;
                localSecondsBetweenWeeksReduction = MultiplierManager.Instance.SecondsBetweenWeeksReductionMultiplier;
            }
        }

        #endregion Custom Methods
    }
}