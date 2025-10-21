using Godot;
using System;

/*
 * Bot (BotMove class):
 * 
 * This class communicates with Center (the main gameplay scene) and contains the algorithms required to
 * generate bot actions based on selected difficulty.
 * 
 * More about strategies:
 * -> Random Strategy: performs each possible option with equal probabilities in each stage
 * -> Round NE Strategy:
 *    -> Choose RP/PS/SR with equal probabilities (1/3 each) in the first stage
 *    -> If players and bots play different strategies within RP/PS/SR, secure a draw 2/3 of the time
 *       and "defect" 1/3 of the time in the second stage
 * 
 * Note: Round NE is NOT the overall NE with respect to the entire tournament!
 *
 * The game has a higher drawing rate (under round NE) at 17/27 (63.0%) compared to traditional
 * rock paper scissors at 1/3 (33.3%)
 */
public partial class BotMove : Node2D
{
    /*
     * Custom signals:
     * -> EndRound: ends the round (and the tournament if there is a winner)
     * -> SendBotMoves: send the final bot move back to Center
     */
    [Signal] public delegate void EndRoundEventHandler();
    [Signal] public delegate void SendBotMovesEventHandler(int botMove);

    public enum Move { ROCK = 0, PAPER = 1, SCISSORS = 2};

    // Loading images
    private static Texture2D _rockImage = GD.Load<Texture2D>("res://assets/textures/rock.png");
    private static Texture2D _paperImage = GD.Load<Texture2D>("res://assets/textures/paper.png");       
    private static Texture2D _scissorsImage = GD.Load<Texture2D>("res://assets/textures/scissors.svg");
    
    // Creating image textures from images
    private static ImageTexture _rockTexture = ImageTexture.CreateFromImage(_rockImage.GetImage()); 
    private static ImageTexture _paperTexture = ImageTexture.CreateFromImage(_paperImage.GetImage()); 
    private static ImageTexture _scissorsTexture = ImageTexture.CreateFromImage(_scissorsImage.GetImage());

    // Private static variables
    private static int[] _playerStage1Moves = new int[2];
    private static int[] _botRound1Moves;
    private static string _difficulty = "Easy";
    private static int EASY_HARD_SPLIT_CHANCE = 20;

    // Exported NodePaths
    [Export] private NodePath _display1Path;
    [Export] private NodePath _display2Path;

    // Private instance variables
    private Sprite2D _display1;
    private Sprite2D _display2;
    private MoveHelper _moveHelper;

    public override void _Ready()
    {
        _display1 = GetNode<Sprite2D>(_display1Path);
        _display2 = GetNode<Sprite2D>(_display2Path);
    }

    public void SetMoveHelper(MoveHelper helper)
    {
        _moveHelper = helper;
    }

    private bool UseRandomStrategy()
    {
        Random rng = new Random();
        return _difficulty.Equals("Easy") || (_difficulty.Equals("Medium") && rng.Next(0, 100) < EASY_HARD_SPLIT_CHANCE);
    }

    private int[] GenerateBotStage1Moves()
    {
        Random rng = new Random();
        if (UseRandomStrategy())
        {
            // Generates stage 1 action randomly
            return new int[] { rng.Next(3), rng.Next(3) };
        }

        // Generates stage 1 action according to round NE
        int baseMove = rng.Next(3);
        return new int[] { baseMove, (baseMove + 1) % 3};
    }

    private int GenerateBotStage2Moves()
    {
        Random rng = new Random();
        if (UseRandomStrategy())
        {
            // Generates stage 2 action randomly
            return rng.Next(0, 2);
        }

        else
        {
            // Generates stage 2 action according to round NE
            if (_botRound1Moves[0] == _botRound1Moves[1])
            {
                return 0;
            }

            int temp = _botRound1Moves[0];
            int mixedStrategy = rng.Next(0, 3);
            if ((_playerStage1Moves[0] == temp && _playerStage1Moves[1] == temp) ||
                (_playerStage1Moves[0] == (temp + 1) % 3 && _playerStage1Moves[1] == (temp + 1) % 3) ||
                (_playerStage1Moves[0] == temp && _playerStage1Moves[1] == (temp + 1) % 3) ||
                (_playerStage1Moves[0] == (temp + 1) % 3 && _playerStage1Moves[1] == temp))
            {
                return 0;
            }
            else if (_playerStage1Moves[0] == (temp + 2) % 3 && _playerStage1Moves[1] == (temp + 2) % 3)
            {
                return 1;
            }
            else if (_playerStage1Moves[0] == temp && _playerStage1Moves[1] == (temp + 2) % 3 ||
                     _playerStage1Moves[0] == (temp + 2) % 3 && _playerStage1Moves[1] == temp)
            {
                if (mixedStrategy == 0)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
            else
            {
                if (mixedStrategy == 0)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }
    }

    private void UpdateDisplayTexture(int move, Sprite2D display)
    {
        if (move == (int)Move.ROCK) 
        {
            display.SetTexture(_rockTexture);
        }
        else if (move == (int)Move.PAPER) 
        {
            display.SetTexture(_paperTexture);
        }
        else
        {
            display.SetTexture(_scissorsTexture);
        }
    }

    /*
     * Connected to a signal in the Center class
     *
     * Transfer the difficulty data from the Center class and store it in 
     * this class (in _difficulty)
     */ 
    private void OnSetDifficulty(string difficulty)
    {
        _difficulty = difficulty;
    }

    /*
     * Finalizes player's stage 1 moves and generates bot stage 1 moves accordingly
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
            _botRound1Moves = GenerateBotStage1Moves();
            
            UpdateDisplayTexture(_botRound1Moves[0], _display1);
            UpdateDisplayTexture(_botRound1Moves[1], _display2);
        }
    }

    /*
     * Finalize the final player move and generate a final bot move independent of the stage 2
     * player action
     * Shows a warning if the player has not chosen a hand to remove
     */
    private void OnRemoveHandButtonPressed()
    {
        if (!IsInsideTree())
        {
            return;
        }

        if (_moveHelper.HasPlayerChosenRemove())
        {
            int removeChoice = GenerateBotStage2Moves();

            if (removeChoice == 0) 
            {
                // First hand removed
                _display1.SetVisible(false);
                _display2.SetVisible(true);
                EmitSignal(SignalName.SendBotMoves, _botRound1Moves[1]);
            }
            else
            {
                // Second hand removed
                _display1.SetVisible(true);
                _display2.SetVisible(false);
                EmitSignal(SignalName.SendBotMoves, _botRound1Moves[0]);
            }

            EmitSignal(SignalName.EndRound);
        }
    }

    /*
     * Connected to a signal in the Center class
     *
     * Transfer the data for the 2 initial player moves (that the bot use to decide which hand to 
     * remove in stage 2) from Center to this class (in _playerStage1Moves)
     */
    private void OnSendPlayerMoves(int[] arr)
    {
        _playerStage1Moves = arr;
    }
}
