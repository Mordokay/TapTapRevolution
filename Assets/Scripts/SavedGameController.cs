using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SavedGameController : MonoBehaviour {

    public Text coinText;
    public Text feedbackCoinText;
    public int coins;
    bool hasToIncrement;
    int incrementAmount;
    float timeSinceLastIncrementTry;

    private void Start()
    {
        hasToIncrement = false;
        incrementAmount = 0;
        UpdateCurrentCoins();
    }

    private void Update()
    {
        timeSinceLastIncrementTry += Time.deltaTime;
        if (hasToIncrement && timeSinceLastIncrementTry > 1.0f)
        {
            WriteIncrementedCoins(incrementAmount);
            timeSinceLastIncrementTry = 0.0f;
        }
        coinText.text = "Coins: " + coins;
    }

    public void ReadSavedGame(string filename,
                         Action<SavedGameRequestStatus, ISavedGameMetadata> callback)
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
        savedGameClient.OpenWithAutomaticConflictResolution(
            filename,
            DataSource.ReadNetworkOnly,
            ConflictResolutionStrategy.UseLongestPlaytime,
            callback);
    }

    public void WriteSavedGame(ISavedGameMetadata game, byte[] savedData,
                               Action<SavedGameRequestStatus, ISavedGameMetadata> callback)
    {

        SavedGameMetadataUpdate.Builder builder = new SavedGameMetadataUpdate.Builder();
       //     .WithUpdatedPlayedTime(TimeSpan.FromMinutes(game.TotalTimePlayed.Minutes + 1))
       //     .WithUpdatedDescription("Saved at: " + System.DateTime.Now);

        // You can add an image to saved game data (such as as screenshot)
        // byte[] pngData = <PNG AS BYTES>;
        // builder = builder.WithUpdatedPngCoverImage(pngData);

        SavedGameMetadataUpdate updatedMetadata = builder.Build();

        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
        savedGameClient.CommitUpdate(game, updatedMetadata, savedData, callback);
    }

    public void UpdateCurrentCoins()
    {
        if (PlayGamesPlatform.Instance.localUser.authenticated)
        {
            feedbackCoinText.text = "";
            //Local variable
            ISavedGameMetadata currentGame = null;

            // CALLBACK: Handle the result of a binary read
            Action<SavedGameRequestStatus, byte[]> readBinaryCallback =
            (SavedGameRequestStatus status, byte[] data) =>
            {
                if (status == SavedGameRequestStatus.Success)
                {
                // Read coins from the Saved Game
                try
                    {
                        string coinsString = System.Text.Encoding.UTF8.GetString(data);
                        coins = Convert.ToInt32(coinsString);
                        feedbackCoinText.text += "Coins In read are: " + coins + System.Environment.NewLine;
                    }
                    catch (Exception e)
                    {
                        feedbackCoinText.text += "Saved Game Write: convert Byte to String exception" + System.Environment.NewLine;
                    }
                }
            };

            Action<SavedGameRequestStatus, ISavedGameMetadata> readCallback =
                (SavedGameRequestStatus status, ISavedGameMetadata game) =>
                {
                    if (status == SavedGameRequestStatus.Success)
                    {
                        feedbackCoinText.text += "File \"file_coins\" found and Loaded" + System.Environment.NewLine;
                        currentGame = game;
                        PlayGamesPlatform.Instance.SavedGame.ReadBinaryData(currentGame, readBinaryCallback);
                    }
                };
            ReadSavedGame("file_coins", readCallback);
        }
    }

    public void IncrementCoins(int increment)
    {
        hasToIncrement = true;
        incrementAmount = increment;
    }
    void WriteIncrementedCoins(int increment)
    {
        if (PlayGamesPlatform.Instance.localUser.authenticated)
        {
            //feedbackCoinText.text = "Going to increment coins ..." + Time.timeSinceLevelLoad + System.Environment.NewLine;
            //Local variable
            ISavedGameMetadata currentGame = null;

            Action<SavedGameRequestStatus, ISavedGameMetadata> writeCallback =
            (SavedGameRequestStatus status, ISavedGameMetadata game) =>
            {
                feedbackCoinText.text += "Saved Game Write: " + status.ToString() + System.Environment.NewLine;
            };

            Action<SavedGameRequestStatus, byte[]> readBinaryCallback =
            (SavedGameRequestStatus status, byte[] data) =>
            {
                feedbackCoinText.text += Time.timeSinceLevelLoad + System.Environment.NewLine;
                feedbackCoinText.text += "Saved Game Binary Read: " + status.ToString() + System.Environment.NewLine;
                if (status == SavedGameRequestStatus.Success)
                {
                        // Read coins from the Saved Game
                        try
                    {
                        string coinsString = System.Text.Encoding.UTF8.GetString(data);
                        coins = Convert.ToInt32(coinsString);
                        feedbackCoinText.text += "Coins In read are: " + coins + System.Environment.NewLine;
                    }
                    catch (Exception e)
                    {
                        feedbackCoinText.text += "Saved Game Write: convert exception: " + e.Message + System.Environment.NewLine;
                    }

                        // Increment coins, convert to byte[]
                        coins += increment;
                    string newCoinsString = Convert.ToString(coins);
                    feedbackCoinText.text += "New Coin Count: " + newCoinsString + System.Environment.NewLine;
                    byte[] newData = System.Text.Encoding.UTF8.GetBytes(newCoinsString);

                        // Write new data
                        WriteSavedGame(currentGame, newData, writeCallback);
                }
            };

            Action<SavedGameRequestStatus, ISavedGameMetadata> readCallback =
            (SavedGameRequestStatus status, ISavedGameMetadata game) =>
            {
                
                if (status == SavedGameRequestStatus.Success && hasToIncrement)
                {
                    feedbackCoinText.text = "File \"file_coins\" found and Loaded" + System.Environment.NewLine;
                    currentGame = game;
                    PlayGamesPlatform.Instance.SavedGame.ReadBinaryData(currentGame, readBinaryCallback);
                    hasToIncrement = false;
                }
                else
                {
                    feedbackCoinText.text += "ReadCallback failed";
                    //feedbackCoinText.text += "Could not load file ... " + status.ToString();
                }
            };

            ReadSavedGame("file_coins", readCallback);
        }
    }
}