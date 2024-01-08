using Microsoft.Maui.Controls.Compatibility;
using System.Collections.Generic;
using System.IO;
using Microsoft.Maui.Graphics;
using System.Diagnostics;

namespace Wordle;

public partial class MainPage : ContentPage
{
    private List<string> words;
    private string currentWord;
    private int maxAttempts = 6;
    private const int wordLength = 5; // Assuming the word length is 5
    private int currentAttempt = 0;

    public MainPage()
    {
        InitializeComponent();
        LoadWords();
        PickWord();
        CreateGuessGrid();
    }

    private void LoadWords()
    {
        try
        {
            var file = "C:\\Users\\35387\\Downloads\\OneDrive - Atlantic TU\\Year 3\\Wordle\\words.txt";
            var fileContents = File.ReadAllLines(file);
            words = new List<string>(fileContents);
        }
        catch (Exception ex)
        {
            // Handle the error
            Debug.WriteLine($"Error reading file: {ex.Message}");
        }
    }

    private void PickWord()
    {
        var random = new Random();
        currentWord = words[random.Next(words.Count)];

        // Debugging: Output the picked word to the console or debug window
        Debug.WriteLine($"Picked word: {currentWord}");
    }

    public void CheckGuessAndUpdateUI(string userGuess)
    {
        userGuess = userGuess.ToUpper(); // Ensure user guess is in the same case as the word

        for (int i = 0; i < userGuess.Length; i++)
        {
            var backgroundColor = Colors.Transparent; // Default background color

            if (i < currentWord.Length)
            {
                if (userGuess[i] == currentWord[i])
                {
                    backgroundColor = Colors.Green; // Correct letter and in the correct position
                }
                else if (currentWord.Contains(userGuess[i]))
                {
                    backgroundColor = Colors.Yellow; // Letter is in the word but in the wrong position
                }
            }

            int labelIndex = currentAttempt * wordLength + i;
            if (labelIndex < guessGrid.Children.Count)
            {
                var frame = guessGrid.Children[labelIndex] as Frame;
                if (frame?.Content is Label label)
                {
                    label.Text = userGuess[i].ToString();
                    frame.BackgroundColor = backgroundColor; // Update the background color of the frame
                }
            }
        }

        if (currentWord.Equals(userGuess, StringComparison.OrdinalIgnoreCase))
        {
            // Handle correct guess
            ShowResultScreen(true); // true indicates a win
        }
        else if (currentAttempt >= maxAttempts - 1)
        {
            // Handle game over
            ShowResultScreen(false); // false indicates a loss
        }

        currentAttempt++;
        if (currentAttempt < maxAttempts)
        {
            PrepareGridForNewAttempt();
        }
    }
    private void ShowResultScreen(bool isWin)
    {
        string message = isWin
            ? "Congratulations! You've guessed the word!"
            : $"You've run out of attempts! The word was {currentWord}.";  // Corrected string interpolation

        string title = isWin ? "Congratulations" : "Game Over";

        // Using a simple display alert to show the result
        Device.BeginInvokeOnMainThread(async () =>
        {
            await DisplayAlert(title, message, "OK");
            ResetGame(); // Reset the game for a new round
        });
    }

    private void ResetButton_Clicked(object sender, EventArgs e)
    {
        ResetGame();
    }

    private void CreateGuessGrid()
    {
        for (int row = 0; row < maxAttempts; row++)
        {
            for (int col = 0; col < wordLength; col++)
            {
                var letterFrame = new Frame
                {
                    BorderColor = Colors.Gray,
                    CornerRadius = 5,
                    Padding = 10,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                };
                // Ensure the Frame does not exceed the cell size
                letterFrame.WidthRequest = 50;
                letterFrame.HeightRequest = 50;


                var letterLabel = new Label
                {
                    Text = "",
                    FontSize = 24,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                };

                letterFrame.Content = letterLabel;

                Microsoft.Maui.Controls.Grid.SetRow(letterFrame, row);
                Microsoft.Maui.Controls.Grid.SetColumn(letterFrame, col);

                guessGrid.Children.Add(letterFrame);
            }
        }
    }


    private void PrepareGridForNewAttempt()
    {
        if (currentAttempt < maxAttempts)
        {
            // Clear the next row for the new attempt
            for (int i = 0; i < wordLength; i++)
            {
                int labelIndex = currentAttempt * wordLength + i;
                if (labelIndex < guessGrid.Children.Count)
                {
                    var frame = guessGrid.Children[labelIndex] as Frame;
                    if (frame?.Content is Label label)
                    {
                        label.Text = "";
                    }
                }
            }
        }
    }

    private void ResetGame()
    {
        // Show an alert with the chosen word
        Device.BeginInvokeOnMainThread(async () =>
        {
            await DisplayAlert("Game Reset", $"The word was: {currentWord}", "OK");

            // Reset each frame in the grid
            foreach (var child in guessGrid.Children)
            {
                if (child is Frame frame && frame.Content is Label label)
                {
                    label.Text = ""; // Clear the text
                    frame.BackgroundColor = Colors.Transparent; // Reset background color
                }
            }

            currentAttempt = 0;
            PickWord();

            // Clear the user input text
            userInput.Text = string.Empty;
        });
    }

    private void OnGuess(object sender, EventArgs e)
    {
        var userGuess = userInput.Text.ToUpper().Trim(); // Convert to uppercase and trim
        if (userGuess.Length != wordLength) return;

        //if (!words.Contains(userGuess))
        //{
        //    Device.BeginInvokeOnMainThread(async () =>
        //    {
        //        await DisplayAlert("Invalid Word", "The guessed word is not in the list.", "OK");
        //        userInput.Text = "";
        //    });
        //    return;
        //}

        Device.BeginInvokeOnMainThread(() =>
        {
            CheckGuessAndUpdateUI(userGuess);
            userInput.Text = ""; // Clear the input field
        });
    }
}

