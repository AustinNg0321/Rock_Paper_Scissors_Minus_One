using Godot;
using System;

/*
 * Center: the main gameplay scene
 * -> Start/End a tournament/round/stage
 * -> Communicate with BotMove (the bot logic class) and other classes to control the behaviour of 
 *    buttons, text labels, etc. in the scene
 * 
 * Each tournament is divided into round(s), and each round is divided into 2 stages:
 * -> Stage 1: Each player or bot picks rock/paper/scissors on each hand
 * -> Stage 2: Each player or bot chooses one hand to remove to decide on a final move
 * 
 * The winner is determined according to classic rock paper scissors results of the final move
 * of each player: paper beats rock, scissors beats paper, rock beats scissors.
 * A tournament ends when a round ends in Classic mode or when there is a winner in other game modes
 *
 * Additional details:
 * -> The game mode, difficulty, round number, and the current number of wins/draws/losses are
 *    on the top of the scene
 * -> In each stage, the player chooses their moves by pressing buttons on the left and finalize
 *    their moves by pressing the button in the middle
 * -> Each round ends with the w/d/l values updated and starts after a 2 second pause
 * -> At the end of each game, the result is announced and players can choose to play again by
 *    pressing the play again button on the bottom right
 *
 * In this code, rock is enumerated as 0, paper as 1, and scissors as 2
 */
public partial class Center : Node2D
{
    /*
     * Custom signals: 
     * -> SetDifficulty: stores the transferred difficulty data in _difficulty
     * -> SendPlayerMoves: sends the 2 initial player moves to the BotMove, allowing the bot to generate
     *    actions based on player moves
     * -> NextRound: If the tournament has not ended yet, start a new round
     */
    [Signal] public delegate void SetDifficultyEventHandler(string difficulty);
    [Signal] public delegate void SendPlayerMovesEventHandler(int[] arr);
    [Signal] public delegate void NextRoundEventHandler();

    public enum Move { ROCK = 0, PAPER = 1, SCISSORS = 2};

    // Loading image
    private static Texture2D _rockImage = GD.Load<Texture2D>("res://assets/textures/rock.png");
    private static Texture2D _paperImage = GD.Load<Texture2D>("res://assets/textures/paper.png");       
    private static Texture2D _scissorsImage = GD.Load<Texture2D>("res://assets/textures/scissors.svg");
    private static Texture2D _greenArrowImage = GD.Load<Texture2D>("res://assets/textures/green_arrow.png");
    private static Texture2D _redArrowImage = GD.Load<Texture2D>("res://assets/textures/red_arrow.png");
    
    // Creating image textures from images
    private static ImageTexture _rockTexture = ImageTexture.CreateFromImage(_rockImage.GetImage()); 
    private static ImageTexture _paperTexture = ImageTexture.CreateFromImage(_paperImage.GetImage()); 
    private static ImageTexture _scissorsTexture = ImageTexture.CreateFromImage(_scissorsImage.GetImage());
    private static ImageTexture _greenArrowTexture = ImageTexture.CreateFromImage(_greenArrowImage.GetImage());
    private static ImageTexture _redArrowTexture = ImageTexture.CreateFromImage(_redArrowImage.GetImage());
    
    private static ImageTexture blank = null;

    /*
     * Private static variables:
     *
     * _tournamentType and _difficulty are initialized and overwritten by tournament and difficulty arguments 
     * sent when the StartGame signal is emitted. _firstToWin is determined directly from the tournament type
     *
     * _botMove stores the final bot move after the bot chose a hand to remove
     * _playerStage1Moves stores the 2 player moves after the first stage
     * _playerFinalMove stores the player's final move after removing a hand
     *
     * _playerScore, _numDraws, _botScore represents the number of games won, drawn, and lost respectively
     */
    private static string _tournamentType = "Classic";
    private static int _firstToWin = 1;
    private static string _difficulty = "Easy";
    private static int _botMove;
    private static int[] _playerStage1Moves = new int[2];
    private static int _playerFinalMove;
    private static int _playerScore = 0;
    private static int _botScore = 0;
    private static int _numDraws = 0;

    private static AudioStreamWav _winMusic = GD.Load<AudioStreamWav>("res://assets/sounds/mixkit-small-group-cheer-and-applause-518.wav");
    private static AudioStreamWav _loseMusic = GD.Load<AudioStreamWav>("res://assets/sounds/mixkit-arcade-retro-game-over-213.wav");
    private static bool _loopSoundEffect = false;

    // Exported NodePaths
    [Export] private NodePath _timerPath;
    [Export] private NodePath _playAgainButtonPath;
    [Export] private NodePath _remove1Path;
    [Export] private NodePath _remove2Path;
    [Export] private NodePath _chooseRPS1Path;
    [Export] private NodePath _chooseRPS2Path;
    [Export] private NodePath _botDisplay1Path;
    [Export] private NodePath _botDisplay2Path;
    [Export] private NodePath _botLabelPath;
    [Export] private NodePath _tournamentTypeLabelPath;
    [Export] private NodePath _wdlLabelPath;
    [Export] private NodePath _warningLabelPath;
    [Export] private NodePath _roundLabelPath;
    [Export] private NodePath _playerScoreLabelPath;
    [Export] private NodePath _botScoreLabelPath;
    [Export] private NodePath _drawLabelPath;
    [Export] private NodePath _moveHelperPath;
    [Export] private NodePath _confirmMoveButtonPath;
    [Export] private NodePath _removeHandButtonPath;
    [Export] private NodePath _botPath;
    [Export] private NodePath _soundEffectPlayerPath;

    // Private static variables
    private Timer _timer;
    private TextureButton _playAgainButton;
    private TextureButton _remove1;
    private TextureButton _remove2;
    private Node2D _chooseRPS1;
    private Node2D _chooseRPS2;
    private Sprite2D _botDisplay1;
    private Sprite2D _botDisplay2;
    private Label _botLabel;
    private Label _tournamentTypeLabel;
    private Label _wdlLabel;
    private Label _warningLabel;
    private Label _roundLabel;
    private Label _playerScoreLabel;
    private Label _botScoreLabel;
    private Label _drawLabel;
    private MoveHelper _moveHelper;
    private ConfirmMoveButton _confirmMoveButton;
    private RemoveHandButton _removeHandButton;
    private BotMove _bot;
    private AudioStreamPlayer _soundEffectPlayer;
    
    public override void _Ready()
    {
        _timer = GetNode<Timer>(_timerPath);
        _playAgainButton = GetNode<TextureButton>(_playAgainButtonPath);
        _remove1 = GetNode<TextureButton>(_remove1Path);
        _remove2 = GetNode<TextureButton>(_remove2Path);
        _chooseRPS1 = GetNode<Node2D>(_chooseRPS1Path);
        _chooseRPS2 = GetNode<Node2D>(_chooseRPS2Path);
        _botDisplay1 = GetNode<Sprite2D>(_botDisplay1Path);
        _botDisplay2 = GetNode<Sprite2D>(_botDisplay2Path);
        _botLabel = GetNode<Label>(_botLabelPath);
        _tournamentTypeLabel = GetNode<Label>(_tournamentTypeLabelPath);
        _wdlLabel = GetNode<Label>(_wdlLabelPath);
        _warningLabel = GetNode<Label>(_warningLabelPath);
        _roundLabel = GetNode<Label>(_roundLabelPath);
        _playerScoreLabel = GetNode<Label>(_playerScoreLabelPath);
        _botScoreLabel = GetNode<Label>(_botScoreLabelPath);
        _drawLabel = GetNode<Label>(_drawLabelPath);

        _botLabel.SetText("Bot (" + _difficulty + ")");
        _tournamentTypeLabel.SetText(_tournamentType);
        _timer.SetWaitTime(2.0);
        _timer.SetOneShot(true);

        _moveHelper = GetNode<MoveHelper>(_moveHelperPath);
        _confirmMoveButton = (ConfirmMoveButton)(GetNode<Button>(_confirmMoveButtonPath));
        _removeHandButton = (RemoveHandButton)(GetNode<Button>(_removeHandButtonPath));
        _bot = (BotMove)(GetNode<Node2D>(_botPath));
        _confirmMoveButton.SetMoveHelper(_moveHelper);
        _removeHandButton.SetMoveHelper(_moveHelper);
        _bot.SetMoveHelper(_moveHelper);

        _soundEffectPlayer = GetNode<AudioStreamPlayer>(_soundEffectPlayerPath);
    }

    private void ToggleTextureButton(TextureButton tb, bool visible, bool disabled)
    {
        tb.SetVisible(visible);
        tb.SetDisabled(disabled);
    }

    private void ToggleTextureButton(TextureButton tb, bool visible, bool disabled, bool pressed)
    {
        tb.SetVisible(visible);
        tb.SetDisabled(disabled);
        tb.SetPressed(pressed);
    }

    private void ToggleMultipleTextureButtons(Node2D bg, bool visible, bool disabled)
    {
        ToggleTextureButton(bg.GetNode<TextureButton>("RockButton"), visible, disabled);
        ToggleTextureButton(bg.GetNode<TextureButton>("PaperButton"), visible, disabled);
        ToggleTextureButton(bg.GetNode<TextureButton>("ScissorsButton"), visible, disabled);
    }

    private void ToggleMultipleTextureButtons(Node2D bg, bool visible, bool disabled, bool pressed)
    {
        ToggleTextureButton(bg.GetNode<TextureButton>("RockButton"), visible, disabled, pressed);
        ToggleTextureButton(bg.GetNode<TextureButton>("PaperButton"), visible, disabled, pressed);
        ToggleTextureButton(bg.GetNode<TextureButton>("ScissorsButton"), visible, disabled, pressed);
    }

    private void SetButtonTexture(TextureButton tb, ImageTexture texture)
    {
        tb.SetTextureNormal(texture);
        tb.SetTexturePressed(texture);
        tb.SetTextureHover(texture);
        tb.SetTextureFocused(texture);
        tb.SetTextureDisabled(texture);
    }

    private int GetMoveFromButtonGroup(Node2D bg)
    {
        if (bg.GetNode<TextureButton>("RockButton").IsPressed())
        {
            return (int)Move.ROCK;
        }
        else if (bg.GetNode<TextureButton>("PaperButton").IsPressed())
        {
            return (int)Move.PAPER;
        }
        else 
        {
            return (int)Move.SCISSORS;
        }
    }

    private void DetermineResult()
    {
        if (_playerScore >= _firstToWin)
        {
            _wdlLabel.SetText("You win!");
            ToggleTextureButton(_playAgainButton, true, false);
            Music.PlayMusic(_soundEffectPlayer, _winMusic);
        }
        else if (_botScore >= _firstToWin)
        {
            _wdlLabel.SetText("You lose.");
            ToggleTextureButton(_playAgainButton, true, false);
            Music.PlayMusic(_soundEffectPlayer, _loseMusic);
        }
        else
        {   
            // Starts another round if it is not classic mode
            if (_firstToWin != 1)
            {
                _timer.Start();
            }
            // Declares a draw otherwise
            else
            {
                _wdlLabel.SetText("It's a draw.");
                ToggleTextureButton(_playAgainButton, true, false);
                Music.PlayMusic(_soundEffectPlayer, _winMusic);
            }
        }
    }

    // Starts a tournament from SelectionMenu
    private void OnStartGame(string tournament, string difficulty)
    {
        if (!IsInsideTree())
        {
            return;
        }
        GetTree().ChangeSceneToFile("res://scenes/Center.tscn");
        
        _tournamentType = tournament;
        _difficulty = difficulty;
        if (_tournamentType.Equals("Classic"))
        {
            _firstToWin = 1;
        }
        else if (_tournamentType.Equals("First To Win 3"))
        {
            _firstToWin = 3;
        }
        else
        {
            _firstToWin = 5;
        }

        // For transferring difficulty data
        EmitSignal(SignalName.SetDifficulty, _difficulty);
    }

    /*
     * Finalizes player stage 1 moves and transitions to the next stage of the round
     * Shows a warning if the player has not chosen move for both hands
     */
    private void OnConfirmMoveButtonPressed()
    {
        if (!IsInsideTree())
        {
            return;
        }

        if (_moveHelper.HasPlayerMadeAllMoves())
        {
            _playerStage1Moves[0] = GetMoveFromButtonGroup(_chooseRPS1);
            _playerStage1Moves[1] = GetMoveFromButtonGroup(_chooseRPS2);

            ToggleMultipleTextureButtons(_chooseRPS1, false, true);
            ToggleMultipleTextureButtons(_chooseRPS2, false, true);
            ToggleTextureButton(_remove1, true, false);
            ToggleTextureButton(_remove2, true, false);

            _warningLabel.SetText("");

            EmitSignal(SignalName.SendPlayerMoves, _playerStage1Moves);
        }
        else
        {
            _warningLabel.SetText("Please choose r/p/s for both hands!");
        }
    }

    // Shows a warning if the player has not chosen which hand to remove
    private void OnRemoveHandButtonPressed()
    {
        if (!IsInsideTree())
        {
            return;
        }

        if (_moveHelper.HasPlayerChosenRemove())
        {
            if (_remove1.IsPressed())
            {
                _playerFinalMove = _playerStage1Moves[1];
            }
            else
            {
                _playerFinalMove = _playerStage1Moves[0];
            }
            
            ToggleTextureButton(_remove1, false, true);
            ToggleTextureButton(_remove2, false, true);
            
            _warningLabel.SetText("");
        }
        else
        {
            _warningLabel.SetText("Please choose which hand to remove!");
        }
    }

    /* 
     * OnRemove1Pressed and OnRemove2Pressed changes the colour/texture of each of the remove buttons
     * when one of them is pressed
     */
    private void OnRemove1Pressed()
    {
        SetButtonTexture(_remove1, _redArrowTexture);
        SetButtonTexture(_remove2, _greenArrowTexture);

        _chooseRPS1.GetNode<Sprite2D>("Display").SetVisible(false);
        _chooseRPS2.GetNode<Sprite2D>("Display").SetVisible(true);
    }

    private void OnRemove2Pressed()
    {
        SetButtonTexture(_remove1, _greenArrowTexture);
        SetButtonTexture(_remove2, _redArrowTexture);

        _chooseRPS1.GetNode<Sprite2D>("Display").SetVisible(true);
        _chooseRPS2.GetNode<Sprite2D>("Display").SetVisible(false);
    }

    /*
     * Connected to a signal in the BotMove class
     *
     * Transfer the data for the final bot move after the second stage from the BotMove class 
     * and store it in this class (in _botMove)
     */ 
    private void OnSendBotMoves(int botMove)
    {
        _botMove = botMove;
    }

    private void OnSoundEffectFinished()
    {
        if (_loopSoundEffect)
        {
            _soundEffectPlayer.Play();
        }
    }

    private void OnNextRound()
    {
        _roundLabel.SetText("Round " + (_playerScore + _numDraws + _botScore + 1).ToString());
        
        GetNode<Sprite2D>("BotMove/Display1").SetTexture(blank);
        GetNode<Sprite2D>("BotMove/Display1").SetVisible(true);
        GetNode<Sprite2D>("BotMove/Display2").SetTexture(blank);
        GetNode<Sprite2D>("BotMove/Display2").SetVisible(true);
        _chooseRPS1.GetNode<Sprite2D>("Display").SetTexture(blank);
        _chooseRPS1.GetNode<Sprite2D>("Display").SetVisible(true);
        _chooseRPS2.GetNode<Sprite2D>("Display").SetTexture(blank);
        _chooseRPS2.GetNode<Sprite2D>("Display").SetVisible(true);

        ToggleMultipleTextureButtons(_chooseRPS1, true, false, false);
        ToggleMultipleTextureButtons(_chooseRPS2, true, false, false);

        SetButtonTexture(_remove1, _greenArrowTexture);
        SetButtonTexture(_remove2, _greenArrowTexture);

        _remove1.SetPressed(false);
        _remove2.SetPressed(false);
    }

    private void OnTimerTimeout()
    {
        EmitSignal(SignalName.NextRound);
    }

    /*
     * The EndRound signal is triggered/emitted in the BotMove class after the bot decides 
     * on its final move
     */
    private void OnEndRound()
    {
        if ((_playerFinalMove - _botMove + 3) % 3 == 0)
        {
            _numDraws++;
        }
        else if ((_playerFinalMove - _botMove + 3) % 3 == 1)
        {
            _playerScore++;
        }
        else
        {
            _botScore++;
        }
        _playerScoreLabel.SetText(_playerScore.ToString());
        _botScoreLabel.SetText(_botScore.ToString());
        _drawLabel.SetText(_numDraws.ToString());
        
        DetermineResult();
    }

    // Resets the scores and goes back to the selection menu
    private void OnPlayAgainButtonPressed()
    {
        _playerScore = 0;
        _botScore = 0;
        _numDraws = 0;

        if (!IsInsideTree())
        {
            return;
        }
        GetTree().ChangeSceneToFile("res://scenes/SelectionMenu.tscn");
    }    
}
