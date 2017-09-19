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
    int coins;

    public void ReadSavedGame(string filename,
                         Action<SavedGameRequestStatus, ISavedGameMetadata> callback)
    {

        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
        savedGameClient.OpenWithAutomaticConflictResolution(
            filename,
            DataSource.ReadCacheOrNetwork,
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
        ISavedGameMetadata currentGame = null;

        // CALLBACK: Handle the result of a binary read
        Action<SavedGameRequestStatus, byte[]> readBinaryCallback =
        (SavedGameRequestStatus status, byte[] data) => {
            if (status == SavedGameRequestStatus.Success)
            {
                // Read coins from the Saved Game
                try
                {
                    string coinsString = System.Text.Encoding.UTF8.GetString(data);
                    coins = Convert.ToInt32(coinsString);
                    feedbackCoinText.text += "Coins In read are: " + System.Environment.NewLine;
                }
                catch (Exception e)
                {
                    feedbackCoinText.text += "Saved Game Write: convert exception" + System.Environment.NewLine;
                }
            }
        };

        // CALLBACK: Handle the result of a read, which should return metadata
        Action<SavedGameRequestStatus, ISavedGameMetadata> readCallback =
        (SavedGameRequestStatus status, ISavedGameMetadata game) => {
            feedbackCoinText.text += "Saved Game Read: " + status.ToString() + System.Environment.NewLine;
            //Debug.Log("Saved Game Read: " + status.ToString());
            if (status == SavedGameRequestStatus.Success)
            {
                // Read the binary game data
                currentGame = game;
                PlayGamesPlatform.Instance.SavedGame.ReadBinaryData(game,
                                                    readBinaryCallback);
            }
        };
        ReadSavedGame("file_coins", readCallback);
    }

    public void WriteIncrementedCoins(int increment)
    {
        if (PlayGamesPlatform.Instance.localUser.authenticated)
        {
            // Local variable
            ISavedGameMetadata currentGame = null;

            // CALLBACK: Handle the result of a write
            Action<SavedGameRequestStatus, ISavedGameMetadata> writeCallback =
            (SavedGameRequestStatus status, ISavedGameMetadata game) =>
            {
                feedbackCoinText.text += "Saved Game Write: " + status.ToString() + System.Environment.NewLine;
                //Debug.Log("Saved Game Write: " + status.ToString());
            };

            // CALLBACK: Handle the result of a binary read
            Action<SavedGameRequestStatus, byte[]> readBinaryCallback =
            (SavedGameRequestStatus status, byte[] data) =>
            {
                feedbackCoinText.text += "Saved Game Binary Read: " + status.ToString() + System.Environment.NewLine;
                //Debug.Log("Saved Game Binary Read: " + status.ToString());
                if (status == SavedGameRequestStatus.Success)
                {
                // Read coins from the Saved Game
                int coins = 0;
                    try
                    {
                        string coinsString = System.Text.Encoding.UTF8.GetString(data);
                        coins = Convert.ToInt32(coinsString);
                    }
                    catch (Exception e)
                    {
                        feedbackCoinText.text += "Saved Game Write: convert exception" + System.Environment.NewLine;
                        //Debug.Log("Saved Game Write: convert exception");
                    }

                // Increment coins, convert to byte[]
                int newCoins = coins + increment; // + mHits;
                string newCoinsString = Convert.ToString(newCoins);
                    byte[] newData = System.Text.Encoding.UTF8.GetBytes(newCoinsString);

                // Write new data
                WriteSavedGame(currentGame, newData, writeCallback);
                }
            };

            // CALLBACK: Handle the result of a read, which should return metadata
            Action<SavedGameRequestStatus, ISavedGameMetadata> readCallback =
            (SavedGameRequestStatus status, ISavedGameMetadata game) =>
            {
                feedbackCoinText.text += "Saved Game Read: " + status.ToString() + System.Environment.NewLine;
                //Debug.Log("Saved Game Read: " + status.ToString());
                if (status == SavedGameRequestStatus.Success)
                {
                // Read the binary game data
                currentGame = game;
                    PlayGamesPlatform.Instance.SavedGame.ReadBinaryData(game,
                                                        readBinaryCallback);
                }
            };

            // Read the current data and kick off the callback chain
            feedbackCoinText.text += "Saved Game: Reading" + System.Environment.NewLine;
            ReadSavedGame("file_coins", readCallback);
        }
    }

    void UpdateCoinText()
    {
        feedbackCoinText.text = "";
        if (PlayGamesPlatform.Instance.localUser.authenticated)
        {
            UpdateCurrentCoins();
        }
    }

    private void Update()
    {
        coinText.text = "Coins: " + coins;
    }

    private void Start()
    {
        InvokeRepeating("UpdateCoinText", 0.0f, 2.0f);
    }
}
