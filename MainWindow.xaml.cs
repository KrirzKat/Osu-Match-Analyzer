using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.IO;
using System.Windows.Media.TextFormatting;
using Accessibility;
using System.Net.Http;
using System.Threading;
using HtmlAgilityPack;
using System.Printing;
using System.Runtime.CompilerServices;
using System.Xml.Schema;

namespace QualifierAnalyzer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            //Initialize components
            InitializeComponent();

            //Prepare the media player for a surprise (if i finish it lol)
            //MediaPlayer player = new MediaPlayer();
            //player.Open(new Uri("file://Megalovania.mp3"));
        }

        private void AnalyzeButton_Click(object sender, RoutedEventArgs e)
        {
            //Download the html file
            if (!downloadFile())
                //Yell at the user if they do it wrong
                WinnerBlock.Text = "Please enter a valid url!";
            else
            {
                //Get the contents of the file, and trim it down into a string to parse the match data
                string file = TrimFile(ReadFile().ToArray());
                //Write the text to the file
                File.WriteAllText("tmp.txt", file);
                //Format the file for parsing
                string[] contents = FormatFile();
                //Parse the file
                ParseHeadBattle(contents);
            }
        }

        private void TeamButton_Click(object sender, RoutedEventArgs e)
        {
            //Download the html file
            if (!downloadFile())
                //Yell at the user if they do it wrong
                WinnerBlock.Text = "Please enter a valid url!";
            else
            {
                //Get the contents of the file, and trim it down into a string to parse the match data
                string file = TrimFile(ReadFile().ToArray());
                //Write the text to the file
                File.WriteAllText("tmp.txt", file);
                //Format the file for parsing
                string[] contents = FormatFile();
                //Parse the file
                ParseTeamBattle(contents);
            }
        }

        //Download the html file from the inputted url
        private bool downloadFile()
        {
            //Start using a new WebClient to download
            using (WebClient client = new WebClient())
            {
                //Make the the uri can be made
                try
                {
                    Uri uri = new Uri(urlBox.Text);
                }
                catch (Exception c)
                {
                    //Return false for further handling
                    return false;
                }

                //Check if the user inputs a valid url
                if (urlBox.Text.Contains("https://osu.ppy.sh/community/matches/"))
                    //If he does, download the file.
                    client.DownloadFile(urlBox.Text, "tmp.txt");
                else
                    //If not, return false
                    return false;

                //Return true if all is well
                return true;
            }
        }

        //Read and return the html file
        private IEnumerable<string> ReadFile()
        {
            //Using a StreamReader...
            using (StreamReader reader = File.OpenText("tmp.txt"))
            {
                //Create a new variable
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    //Yield on returning the line, but keep adding to the value as the loop goes on.
                    yield return line;
                }
            }
        }

        //Parse JSON data in a team battle setting
        private void ParseTeamBattle(string[] contents)
        {
            //Initialize the score for each team
            int redTeamScore = 0;
            int blueTeamScore = 0;

            //Initialize a temporary score for detection
            int tmpScore = 0;

            //Go through the contents of the file
            for (int i = 0; i < contents.Length; i++)
            {
                //Check if the current line contains the word "score"
                if (contents[i].Contains("\"score\""))
                {
                    //Get the the score from the file
                    tmpScore = GetScore(contents, i);
                }
                //Check if the player is on blue team
                if (contents[i].Contains("\"team\"") && contents[i].Contains("\"blue\""))
                {
                    //Add their score to the blue teams score
                    blueTeamScore += tmpScore;
                    tmpScore = 0;
                }
                //Do the same if the player is on red
                else if (contents[i].Contains("\"team\"") && contents[i].Contains("\"red\""))
                {
                    redTeamScore += tmpScore;
                    tmpScore = 0;
                }
            }

            //Win detection block, checking for wins, ties, and errors.
            if (redTeamScore > blueTeamScore)
                WinnerBlock.Text = String.Format("Red team wins with a score of {0:#,##0}!", redTeamScore);
            else if (blueTeamScore > redTeamScore)
                WinnerBlock.Text = String.Format("Blue team wins with a score of {0:#,##0}!", blueTeamScore);
            else if (blueTeamScore == redTeamScore)
                WinnerBlock.Text = String.Format("Red and Blue have tied with scores of {0:#,##0}!", redTeamScore);
            else
                WinnerBlock.Text = "There was an error processing the score.";

            //Delete the temp file used throughout the program.
            File.Delete("tmp.txt");
        }


        //Parse the JSON data in a head to head battle setting
        private void ParseHeadBattle(string[] contents)
        {
            //Declare lists of player IDs and their scores
            List<int> playerIDs = new List<int>();
            List<int> scores = new List<int>();
            //Declare a temporary variable for the current id being used
            int currentID = 0;

            //Go through each line, looking for matches, player ids, and scores.
            bool foundBattle = false;
            for (int i = 0; i < contents.Length; i++)
            {
                //Check the contents for scores, if it has the program found a battle
                if (contents[i].Contains("scores"))
                {
                    foundBattle = true;
                }

                //If the battle has been found...
                if (foundBattle)
                {
                    //Check if the contents marks the end of the block
                    if (contents[i] == "]")
                        foundBattle = false;
                    //If the current line is the user id line...
                    if(contents[i].Contains("user_id"))
                    {
                        //Set the currentID to be the one returned from GetID
                        currentID = GetID(contents, i, playerIDs, false);
                        //If the id was not equal to 0...
                        if (currentID != 0)
                        {
                            //Add the ID to the IDs, and add a new score to the scores list.
                            playerIDs.Add(currentID);
                            scores.Add(0);
                        }
                        //Else just preserve the contents in GetID and dont add it to anything.
                        else
                        {
                            currentID = GetID(contents, i, playerIDs, true);
                        }
                    }
                }

                //Get the score and set it to the right ID.

                //If the current line contains the score
                if (contents[i].Contains("\"score\""))
                {
                    //Get the score
                    int score = GetScore(contents, i);

                    //Loop through each playerID...
                    for (int j = 0; j < playerIDs.Count; j++)
                    {
                        //And if the 2 IDs match, add the score to that players score.
                        if (playerIDs[j] == currentID)
                            scores[j] += score;
                    }
                }
            }

            //Create new variables to detect the win
            int highestScore = 0;
            int id = 0;
            //Loop through each score
            //3,440,433

            //Set the winner block of text to be the winner, and score, formatted.
            //WinnerBlock.Text = String.Format("{0} wins with a score of {1:#,##0}!", id, highestScore);
            //Delete the temporary file used throughout this program.
            File.Delete("tmp.txt");

            int[] tmpScores = scores.ToArray();
            int[] sortedIDs = new int[scores.Count];

            //Sort the scores
            scores.Sort();
            scores.Reverse();

            //Empty the text
            WinnerBlock.Text = "";
            //Loop through all the scores
            for (int i = 0; i < scores.Count; i++)
            {
                //Loop through the scores again, for sorting
                for (int j = 0; j < scores.Count; j++)
                {
                    //If the tmp score equals the sorted scores...
                    if (tmpScores[i] == scores[j])
                    {
                        //Set the sorted ID to the ID at index i
                        sortedIDs[j] = playerIDs[i];
                    }
                }
            }

            //Loop through again to display the scores
            for(int i = 0; i < scores.Count; i++)
            {
                //This checks each condition to display. It makes sure that even if it goes over 10, it still works.
                if (i == 0)
                    WinnerBlock.Text = String.Format("{0} wins with a score of {1:#,##0}!", GetName(sortedIDs[i]), scores[i]);
                else if (i.ToString().EndsWith('0') && i != 11)
                    WinnerBlock.Text += String.Format("\n{0} is in {1}st with a score of {2:#,##0}!", GetName(sortedIDs[i]), i + 1, scores[i]);
                else if (i.ToString().EndsWith('1') && i != 12)
                    WinnerBlock.Text += String.Format("\n{0} is in {1}nd with a score of {2:#,##0}!", GetName(sortedIDs[i]), i + 1, scores[i]);
                else if (i.ToString().EndsWith('2') && i != 13)
                    WinnerBlock.Text += String.Format("\n{0} is in {1}rd with a score of {2:#,##0}!", GetName(sortedIDs[i]), i + 1, scores[i]);
                else
                    WinnerBlock.Text += String.Format("\n{0} is in {1}th with a score of {2:#,##0}!", GetName(sortedIDs[i]), i + 1, scores[i]);
            }
        }

        //Gets the name of the user who won.
        private string GetName(int id)
        {
            //Sets a new string to null.
            string name = null;

            //Using the WebClient, write to a new file the contents of the page from the user ID passed in
            using(WebClient client = new WebClient())
            {
                File.WriteAllText("tmp.txt", client.DownloadString("https://osu.ppy.sh/users/" + id));
            }

            //Read all those lines, and store them in an array of strings
            //This is unfortunately slow, and I need to think of a way better way of doing things.
            string[] contents = File.ReadAllLines("tmp.txt");

            //Loop through the contents of the string
            for(int i = 0; i < contents.Length; i++)
            {
                //If the line contains the title
                if (contents[i].Contains("<title>"))
                {
                    
                    int j = 14;
                    //Starting at the 14th character, if the char does not contain a space...
                    while(contents[i][j] != ' ')
                    {
                        //Increment j, and add the contents to name
                        j++;
                        name += contents[i][j];
                    }
                }
            }

            //Delete the file a little too late
            File.Delete("tmp.txt");
            //Replace all instances of &nbsp with a space. 
            //This could just denote certain special characters, which will be fixed if reported
            name = name.Replace("&nbsp;", " ");
            //Return the name
            return name;
        }
        
        //Get a player ID at a line
        private int GetID(string[] contents, int line, List<int> playerIDs, bool isPreserved)
        {
            bool foundNums = false;
            bool found = false;
            string tmp = "";
            for (int j = 0; j < contents[line].Length; j++)
            {
                if (Char.IsDigit(contents[line][j]))
                {
                    if (!foundNums && !found)
                        foundNums = true;
                    if (foundNums)
                        tmp += contents[line][j];

                    found = true;
                }
                else
                {
                    if (foundNums)
                        foundNums = false;
                }
            }
            if (playerIDs.Count == 0)
            {
                int id;
                int.TryParse(tmp, out id);
                return id;
            }
            else
            {
                int id;
                int.TryParse(tmp, out id);
                if (!playerIDs.Contains(id))
                    return id;
                else
                    return 0;
            }
        }

        //Get the score of a player at the line
        private int GetScore(string[] contents, int line)
        {
            //Set variables to a set staye
            bool foundNums = false, found = false;
            string tmp = "";
            int score;
            
            //Loop through contents of the string array
            for (int j = 0; j < contents[line].Length; j++)
            {
                //6893425
                //If the character we're looking at is a digit
                if (Char.IsDigit(contents[line][j]))
                {
                    if (!foundNums && !found)
                        foundNums = true;
                    if (foundNums)
                        tmp += contents[line][j];

                    found = true;
                }
                else
                {
                    if (foundNums)
                        foundNums = false;
                }
            }

            int.TryParse(tmp, out score);
            return score;

        }

        //Format the file for parsing
        private string[] FormatFile()
        {
            //Create a new string, and read all the text from the temp file created earlier
            string json = File.ReadAllText("tmp.txt");
            //Create this to return later
            string[] returnValue;

            //Counter variables, i stores iterations and length stores total length.
            int i = 0;
            int length = json.Length;
            //While i is less than length
            while (i < length)
            {
                //Check if the json file has any characters that should have a new line placed after them
                if (json.Substring(i, 1) == "{" || json.Substring(i, 1) == "}" || json.Substring(i, 1) == "[" || json.Substring(i, 1) == "]")
                {
                    //Insert a new line into the string
                    json = json.Insert(i + 1, "\n");
                    //Iterate both counters to signal that the length has changed
                    i++;
                    length++;
                }
                //Iterate i
                i++;

                //Create the new return value, inefficiently
                returnValue = new string[length];
            }

            //Write that text to the file
            File.WriteAllText("tmp.txt", json);
            //Return the file
            return File.ReadAllLines("tmp.txt");
        }

        //Trim the file so only the right data is stored
        private string TrimFile(string[] contents)
        {
            //Cycle through the length of the file
            for(int i = 0; i < contents.Length; i++)
            {
                //If it contains the script for the events...
                if(contents[i].Contains("<script id=\"json-events\" type=\"application/json\">"))
                {
                    //Return the line below it
                    return contents[i + 1];
                }
            }
            //Otherwise return null
            return null;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

    }
}