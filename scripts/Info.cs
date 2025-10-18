using Godot;
using System;

public partial class Info : Node2D
{
    //private static variables

    //custom signals

    //Initialization function
    public override void _Ready()
    {

    }

    //signalling functions
    private void OnHomeButtonPressed()
    {
        if (!IsInsideTree())
        {
            return;
        }

        GetTree().ChangeSceneToFile("res://scenes/Main.tscn");
    }

}
