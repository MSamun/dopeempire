using System;

namespace DopeEmpire
{
    [Serializable]
    public class SavePlayerData
    {
        public PlayerLevelData playerLevelData;
        public CharacterData characterData;
        public CharacterSkillsData[] characterSkillsData;

        public SavePlayerData()
        {
        }
    }

    #region Character/Player Level

    //NOTE: JSON, for some reason, does not take in properties or private variables. It only takes public variables.
    [Serializable]
    public class PlayerLevelData
    {
        public int currentLevel;
        public float experienceEarned;
        public int requiredExperienceToNextLevel;

        public PlayerLevelData(int currentLevel, float experienceEarned, int requiredExperienceToNextLevel)
        {
            this.currentLevel = currentLevel;
            this.experienceEarned = experienceEarned;
            this.requiredExperienceToNextLevel = requiredExperienceToNextLevel;
        }
    }

    [Serializable]
    public class CharacterData
    {
        public int ownedSkillPoints;
        public int charIconID;

        public CharacterData(int ownedSkillPoints, int charIconID)
        {
            this.ownedSkillPoints = ownedSkillPoints;
            this.charIconID = charIconID;
        }
    }

    [Serializable]
    public class CharacterSkillsData
    {
        public int skillID;
        public int currentSkillLevel;

        public CharacterSkillsData(int skillID, int currentSkillLevel)
        {
            this.skillID = skillID;
            this.currentSkillLevel = currentSkillLevel;
        }
    }

    #endregion Character/Player Level
}