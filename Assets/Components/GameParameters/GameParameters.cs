using UnityEditor;
using UnityEngine;

namespace Components.GameParameters
{
    public static class GameParameters
    {
        public static GameMode CurrentGameMode;
        public static bool PlayerCheated;
        public static int CurrentBestScore => PlayerPrefs.GetInt(BEST_SCORE_KEY, 0);
        
        private const string BEST_SCORE_KEY = "BEST_SCORE";
        
        public enum GameMode
        {
            SANDBOX,
            STANDARD
        }
        
        // Reset the static values when the game starts.
        [InitializeOnLoadMethod]
        private static void Reset()
        {
            CurrentGameMode = GameMode.SANDBOX;
            PlayerCheated = false;
        }

        public static void SetBestScore(int score)
        {
            if (CurrentGameMode != GameMode.STANDARD || PlayerCheated)
            {
                return;
            }

            if (CurrentBestScore > score)
            {
                return;
            }

            Debug.Log($"New best score saved: {score}, previous best: {CurrentBestScore}");
            PlayerPrefs.SetInt(BEST_SCORE_KEY, score);
        }
    }
}