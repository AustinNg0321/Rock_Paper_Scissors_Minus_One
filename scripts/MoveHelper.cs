using Godot;
using System;

/* 
 * The MoveHelper class is a helper class that determines if the player has finished making their move(s)
 * in each stage before pressing the ConfirmMove or RemoveHand button in the middle
 */
public partial class MoveHelper : Node2D
{   
    // Exported NodePaths
    [Export] private NodePath _chooseRPS1Path;
    [Export] private NodePath _chooseRPS2Path;
    [Export] private NodePath _remove1Path;
    [Export] private NodePath _remove2Path;

    // Private instance variables
    private Node2D _chooseRPS1;
    private Node2D _chooseRPS2;
    private TextureButton _remove1;
    private TextureButton _remove2;
    private TextureButton _rock1, _paper1, _scissors1;
    private TextureButton _rock2, _paper2, _scissors2;

    public override void _Ready()
    {
        _chooseRPS1 = GetNode<Node2D>(_chooseRPS1Path);
        _chooseRPS2 = GetNode<Node2D>(_chooseRPS2Path);
        _remove1 = GetNode<TextureButton>(_remove1Path);
        _remove2 = GetNode<TextureButton>(_remove2Path);
        _rock1 = _chooseRPS1.GetNode<TextureButton>("RockButton");
        _paper1 = _chooseRPS1.GetNode<TextureButton>("PaperButton");
        _scissors1 = _chooseRPS1.GetNode<TextureButton>("ScissorsButton");
        _rock2 = _chooseRPS2.GetNode<TextureButton>("RockButton");
        _paper2 = _chooseRPS2.GetNode<TextureButton>("PaperButton");
        _scissors2 = _chooseRPS2.GetNode<TextureButton>("ScissorsButton");
    }

    public bool HasPlayerMadeAllMoves()
    {
        return (_rock1.IsPressed() || _paper1.IsPressed() || _scissors1.IsPressed()) &&
               (_rock2.IsPressed() || _paper2.IsPressed() || _scissors2.IsPressed());
    }

    public bool HasPlayerChosenRemove()
    {
        return _remove1.IsPressed() || _remove2.IsPressed();
    }
}
