using Godot;
using System;

public partial class ConfirmMoveButton : Button
{   
    private MoveHelper _moveHelper;

    // Enable and show the button initially
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
     * Disable and hide the button when it is pressed and the player has selected a move for both hands
     * in stage 1
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

    // The button is reset (re-enabled and shown) each round
    private void OnNextRound()
    {
        this.SetDisabled(false);
        this.SetVisible(true);
    }
}