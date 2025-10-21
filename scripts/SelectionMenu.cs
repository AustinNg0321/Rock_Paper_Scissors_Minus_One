using Godot;
using System;

/*
 * SelectionMenu class:
 *  
 * The selection menu allows the user to choose the difficulty (Easy/Medium/Hard) and 
 * tournament mode (Classic, First To Win 3, First To Win 5) of the next tournament 
 * before moving on to the gameplay
 *
 * The bot adapts different strategies depending on the difficulty:
 * -> Easy: random
 * -> Medium: mixed strategy (default: 20% random, 80% round NE)
 * -> Hard: round NE
 *
 * More information about bot move generation algorithms can be found in BotMove.cs
 */
public partial class SelectionMenu : Node2D
{
    // Custom signals
    [Signal] public delegate void StartGameEventHandler(string tournament, string difficulty);
    
    // Exported NodePaths
    [Export] private NodePath _tournamentTypeItemListPath;
    [Export] private NodePath _difficultyItemListPath;
    [Export] private NodePath _warningPath;

    // Private instance variables
    private ItemList _tournamentTypeItemList;
    private ItemList _difficultyItemList;
    private RichTextLabel _warning;

    // Initialization function
    public override void _Ready()
    {
        _tournamentTypeItemList = GetNode<ItemList>(_tournamentTypeItemListPath);
        _difficultyItemList = GetNode<ItemList>(_difficultyItemListPath);
        _warning = GetNode<RichTextLabel>(_warningPath);
    }

    // Signalling functions

    /* 
     * Checks if the user selected a tournament and a difficulty
     * -> Yes: Emits the StartGame signal and transfers the strings representing the 
     *    tournament and difficulty to Center.cs (the gameplay scene)
     * -> No: Show a warning on the bottom of the page
     */
    private void OnStartButtonPressed()
    {
        if (_tournamentTypeItemList.IsAnythingSelected() &&
            _difficultyItemList.IsAnythingSelected())
        {
            string tournament = _tournamentTypeItemList.GetItemText(_tournamentTypeItemList.GetSelectedItems()[0]);
            string difficulty = _difficultyItemList.GetItemText(_difficultyItemList.GetSelectedItems()[0]);
    
            EmitSignal(SignalName.StartGame, tournament, difficulty);
        }

        else
        {
            _warning.Text = "";
            if (!_tournamentTypeItemList.IsAnythingSelected()) 
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
