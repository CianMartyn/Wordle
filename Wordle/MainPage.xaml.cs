using Microsoft.Maui.Controls.Compatibility;

namespace Wordle;

public partial class MainPage : ContentPage
{
    private List<string> words;
    private string currentWord;
    private int maxAttempts = 6;
    private int currentAttempt = 0;

    public MainPage()
    {
        InitializeComponent();
        LoadWords();
        PickWord();
    }

    private void LoadWords()
    {
        var file = "C:\\Users\\35387\\Downloads\\OneDrive - Atlantic TU\\Year 3\\Wordle\\words.txt"; // Your words file
        var fileContents = File.ReadAllLines(file);
        words = new List<string>(fileContents);
    }

    private void PickWord()
    {
        var random = new Random();
        currentWord = words[random.Next(words.Count)];
    }

    public void CheckGuessAndUpdateUI(string userGuess)
    {
        // Assuming 'currentWord' is the word to be guessed
        for (int i = 0; i < userGuess.Length; i++)
        {
            Color color = Colors.Transparent;

            if (i < currentWord.Length && userGuess[i] == currentWord[i])
            {
                // Letter is correct and in the correct position
                color = Colors.Green;
            }
            else if (currentWord.Contains(userGuess[i]))
            {
                // Letter is correct but in the wrong position
                color = Colors.Yellow;
            }

            // Update the UI for this letter
            var letterLabel = new Label
            {
                Text = userGuess[i].ToString(),
                BackgroundColor = color,
                // Other styling properties
            };

            Microsoft.Maui.Controls.Grid.SetRow(letterLabel, 0); // Set the row for the letter
            Microsoft.Maui.Controls.Grid.SetColumn(letterLabel, i); // Set the column based on the letter's position
            lettersGrid.Children.Add(letterLabel); // Add the label to the grid
        }
    }

    private void OnGuess(object sender, EventArgs e)
    {
        var userGuess = userInput.Text.ToUpper(); // Assuming userInput is an Entry
        if (userGuess.Length != 5) return; // Or other length check based on your word list



        // Compare guess with currentWord
        // Update UI based on correct letters and positions
        // Handle attempts

        currentAttempt++;
        if (currentAttempt >= maxAttempts)
        {
            // Handle game over
        }

        userInput.Text = ""; // Reset input field
    }
}
