using Godot;
using System;

public partial class RemoveHandButton : Button
{
    private MoveHelper _moveHelper;

    // Disable and hide the button initially
    public override void _Ready()
    {
        this.SetDisabled(true);
        this.SetVisible(false);
    }

    public void SetMoveHelper(MoveHelper helper)
    {
        _moveHelper = helper;
    }

    // Disable and hide the button when it is pressed and the player has chosen a hand to remove in stage 2
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

    // The button should be enabled and shown at the start of stage 2
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
