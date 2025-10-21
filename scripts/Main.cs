using Godot;
using System;

/*
 * This is the main scene of the game.
 * 
 * All classes:
 * 
 * Music
 * Main
 * -> SelectionMenu
 *    -> Center (the main gameplay scene)
 *       -> BotMove: handles bot logic and action
 *       -> MoveHelper x2: scene for making and displaying both r/p/s moves of the player or the bot
 *          -> ChooseRPS x2: scene for making and displaying a single r/p/s move
 *       -> ConfirmMoveButton
 *       -> RemoveHandButton
 * -> Info
 * -> Settings
 * 
 * More information of the gameplay can be found in Center.cs
 */
public partial class Main : Node2D
{
    // Private static variables
    private static AudioStreamWav _backgroundMusic = GD.Load<AudioStreamWav>("res://assets/sounds/mixkit-light-rain-loop-2393.wav");

    // Exported NodePaths
    [Export] private NodePath _backgroundMusicPlayerPath;
    [Export] private NodePath _exitConfirmationPath;

    // Private static variables
    private AudioStreamPlayer _backgroundMusicPlayer;
    private Popup _exitConfirmation;
    private static bool _loopBackgroundMusic = true;

    // Initialization function
    public override void _Ready()
    {
        _backgroundMusicPlayer = GetNode<AudioStreamPlayer>(_backgroundMusicPlayerPath);
        _exitConfirmation = GetNode<Popup>(_exitConfirmationPath);

        _exitConfirmation.Hide();
        Music.PlayMusic(_backgroundMusicPlayer, _backgroundMusic);
    }

    private void OnBackgroundMusicFinished()
    {
        if (_loopBackgroundMusic)
        {
            _backgroundMusicPlayer.Play();
        }
    }

    private void OnPlayButtonPressed()
    {
        if (!IsInsideTree())
        {
            return;
        }

        GetTree().ChangeSceneToFile("res://scenes/SelectionMenu.tscn");
    }

    private void OnSettingsButtonPressed()
    {
        if (!IsInsideTree())
        {
            return;
        }

        GetTree().ChangeSceneToFile("res://scenes/Settings.tscn");
    }

    private void OnInfoButtonPressed()
    {
        if (!IsInsideTree())
        {
            return;
        }
        GetTree().ChangeSceneToFile("res://scenes/Info.tscn");
    }
    private void OnQuitButtonPressed()
    {
        _exitConfirmation.Show();
    }

    private void OnConfirmExit()
    {
        if (!IsInsideTree())
        {
            return;
        }
        
        GetTree().Quit();
    }

    private void OnStay()
    {
        _exitConfirmation.Hide();
    }
}
