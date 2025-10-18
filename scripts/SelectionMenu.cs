using Godot;
using System;

/*
 * Rock Paper Scissors Minus One
 * 
 * Rules:
 * -> Each player or bot picks rock/paper/scissors on each hand
 * -> Each player chooses one hand to remove
 * -> The winner is determined according to classic rock paper scissors results of the remaining
 *    hands of each player: paper beats rock, scissors beats paper, rock beats scissors 
 *
 * Optimal strategy (according to Nash Equilibrium)
 * -> Choose RP/PS/SR with equal probabilities (1/3 each)
 * -> If players and bots play different strategies within RP/PS/SR, secure a draw 2/3 of the time
 *    and "defect" 1/3 of the time
 * 
 * The game has a higher drawing rate (under optimal play) at 17/27 (63.0%) compared to traditional
 * rock paper scissors at 1/3 (33.3%)
 *
 * Game Mechanics:
 * -> Player chooses the tournament (Classic (1 round), First To Win 3 (first one to win 3 games 
 *    win the tournament), and First To Win 5 (first one to win 5 games win the tournament)) and
 *    difficulty in the game selection menu (SelectionMenu class)
 * -> If the player has selected anything for tournament and difficulty, the scene switches to
 *    "Center" (the gameplay) upon pressing the start button
 * -> The game mode, difficulty, round number, and the current number of wins/draws/losses are
 *    shown in respective labels
 * -> Players choose their moves by pressing buttons on the left and finalize their moves by
 *    pressing the button in the middle
 * -> Chosen moves are shown in the lavender circles around the middle button
 * -> The bot make moves differently depending on the difficulty
 *    -> Easy: random moves (makes each decision with equal probabilities) 
 *    -> Hard: Optimal play according to Nash Equilibrium
 *    -> Medium: A mixed strategy of easy mode (20% of the time) and hard mode (80% of the time) 
 *       in each round
 * -> Each round ends with the w/d/l values updated and starts after a 2 second pause
 * -> At the end of each game, the result is announced and players can choose to play again by
 *    pressing the play again button on the bottom right
 *
 * Implementation:
 * In this code, rock is often represented as 0, paper as 1, and scissors as 2
 *
 * Currently, the core game mechanics are finished, but ......
 */

/*
 * SelectionMenu class:
 *  
 * The selection menu allows the user to choose the difficulty (Easy/Medium/Hard) and 
 * tournament mode (Classic, First To Win 3, First To Win 5) of the next tournament 
 * before moving on to the gameplay
 */
public partial class SelectionMenu : Node2D
{
    // A custom signal to start a game with a specified tournament mode and bot difficulty
    [Signal] public delegate void StartGameEventHandler(string tournament, string difficulty);
    
    //...
    [Export] private NodePath _tournamentItemListPath;
    [Export] private NodePath _difficultyItemListPath;
    [Export] private NodePath _warningPath;

    private ItemList _tournamentItemList;
    private ItemList _difficultyItemList;
    private RichTextLabel _warning;

    // Initialization function
    public override void _Ready()
    {
        _tournamentItemList = GetNode<ItemList>(_tournamentItemListPath);
        _difficultyItemList = GetNode<ItemList>(_difficultyItemListPath);
        _warning = GetNode<RichTextLabel>(_warningPath);
    }

    /*
     * Signalling functions for the start button
     * 
     * Checks if the user selected a tournament and a difficulty
     * -> Yes: Emits the StartGame signal and transfers the strings representing the 
     *    tournament and difficulty to Center.cs (the gameplay scene)
     * -> No: Show a warning on the bottom of the page
     */
    private void OnStartButtonPressed()
    {
        if (_tournamentItemList.IsAnythingSelected() &&
            _difficultyItemList.IsAnythingSelected())
        {
            string tournament = _tournamentItemList.GetItemText(_tournamentItemList.GetSelectedItems()[0]);
            string difficulty = _difficultyItemList.GetItemText(_difficultyItemList.GetSelectedItems()[0]);
    
            EmitSignal(SignalName.StartGame, tournament, difficulty);
        }

        else
        {
            _warning.Text = "";
            if (!_tournamentItemList.IsAnythingSelected()) 
            {
                _warning.Text += "Please select a tournament.\n";
            }
            if (!_difficultyItemList.IsAnythingSelected()) 
            {
                _warning.Text += "Please select a difficulty.\n";
            }
        }
    }

    private void OnHomeButtonPressed()
    {
        if (!IsInsideTree())
        {
            return;
        }

        GetTree().ChangeSceneToFile("res://scenes/Main.tscn");
    }
}
