using Godot;
using System;

// This class controls the behaviour of the RemoveHandButton
public partial class RemoveHandButton : Button
{
    private MoveHelper _moveHelper;

    /* 
     * Initialization function
     * -> Disable and hide the button initially
     */
    public override void _Ready()
    {
        this.SetDisabled(true);
        this.SetVisible(false);
    }

    public void SetMoveHelper(MoveHelper helper)
    {
        _moveHelper = helper;
    }

    /*
     * The ConfirmMoveButton is disabled and hidden when pressed provided that
     * the player has chosen a hand to remove
     */
    private void OnPressed()
    {
        if (!IsInsideTree())
        {
            return;
        }

        if (_moveHelper.HasPlayerChosenRemove())
        {
            this.SetDisabled(true);
            this.SetVisible(false);
        }
    }

    // The button should be enabled and shown in stage 2 (after player confirms initial moves)
    private void OnConfirmMoveButtonPressed()
    {
        if (!IsInsideTree())
        {
            return;
        }

        if (_moveHelper.HasPlayerMadeAllMoves())
        {
            this.SetDisabled(false);
            this.SetVisible(true);
        }
    }

    // The RemoveHandButton is reset (disabled and hidden) each round
    private void OnNextRound()
    {
        this.SetDisabled(true);
        this.SetVisible(false);
    }
}
