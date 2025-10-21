using Godot;
using System;

/*
 * This scene allows the player to choose r/p/s moves with r/p/s buttons around a large lavender circle
 * The chosen move is shown in the lavender circle
 */
public partial class ChooseRPS : Node2D
{
    // Loading images
    private static Texture2D _rockImage = GD.Load<Texture2D>("res://assets/textures/rock.png");
    private static Texture2D _paperImage = GD.Load<Texture2D>("res://assets/textures/paper.png");       
    private static Texture2D _scissorsImage = GD.Load<Texture2D>("res://assets/textures/scissors.svg");
    
    // Creating image textures from images
    private static ImageTexture _rockTexture = ImageTexture.CreateFromImage(_rockImage.GetImage()); 
    private static ImageTexture _paperTexture = ImageTexture.CreateFromImage(_paperImage.GetImage()); 
    private static ImageTexture _scissorsTexture = ImageTexture.CreateFromImage(_scissorsImage.GetImage());

    // Exported NodePaths
    [Export] private NodePath _displayPath;

    // Private instance variables
    private Sprite2D _display;

    public override void _Ready()
    {
        _display = GetNode<Sprite2D>(_displayPath);
    }

    // The below 3 methods changes the displayed move in the Sprite2D object based on the button clicked.
    private void OnRockButtonPressed() 
    {
        _display.SetTexture(_rockTexture);
    }

    private void OnPaperButtonPressed() 
    {
        _display.SetTexture(_paperTexture);
    }

    private void OnScissorsButtonPressed() 
    {
        _display.SetTexture(_scissorsTexture);
    }
}
