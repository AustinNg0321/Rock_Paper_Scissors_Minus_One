using Godot;
using System;

// Contains rules, description of game modes, and credits of the game
public partial class Info : Node2D
{
    public override void _Ready()
    {

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
