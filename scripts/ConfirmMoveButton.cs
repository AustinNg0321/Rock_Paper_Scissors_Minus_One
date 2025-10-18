using Godot;
using System;

// This class controls the behaviour of the ConfirmMoveButton
public partial class ConfirmMoveButton : Button
{   
    private MoveHelper _moveHelper;

    /* 
     * Initialization function
     * -> Enable and show the button initially
     */
    public override void _Ready()
    {
        this.SetDisabled(false);
        this.SetVisible(true);
    }

    public void SetMoveHelper(MoveHelper helper)
    {
        _moveHelper = helper;
    }

    /*
     * The ConfirmMoveButton is disabled and hidden when pressed provided that
     * the player has selected a move for each hand
     */
    private void OnPressed()
    {
        if (!IsInsideTree())
        {
            return;
        }

        if (_moveHelper.HasPlayerMadeAllMoves()
            )
        {
            this.SetDisabled(true);
            this.SetVisible(false);
        }
    }

    // The ConfirmMoveButton is reset (re-enabled and shown) each round
    private void OnNextRound()
    {
        this.SetDisabled(false);
        this.SetVisible(true);
    }
}