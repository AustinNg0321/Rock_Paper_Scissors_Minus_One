using Godot;
using System;

public partial class Settings : Node2D
{
    //private static variables
    private static Texture2D _audioOffImage = GD.Load<Texture2D>("res://assets/textures/Kenney_gameIcons/PNG/White/1x/audioOff.png");
    private static Texture2D _audioOnImage = GD.Load<Texture2D>("res://assets/textures/Kenney_gameIcons/PNG/White/1x/audioOn.png");

    private static ImageTexture _audioOffTexture = ImageTexture.CreateFromImage(_audioOffImage.GetImage());
    private static ImageTexture _audioOnTexture = ImageTexture.CreateFromImage(_audioOnImage.GetImage());

    //0.0 to 1.0
    private static double _volume = (double)Music.GetVolume();

    //custom signals
    [Export] private NodePath _musicIconPath;
    [Export] private NodePath _musicSliderPath;

    private Sprite2D _musicIcon;
    private HSlider _musicSlider;

    //Initialization function
    public override void _Ready()
    {
        _musicIcon = GetNode<Sprite2D>(_musicIconPath);
        _musicSlider = GetNode<HSlider>(_musicSliderPath);

        _musicSlider.SetValue(_volume);
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

    private void OnHSliderDragEnded(bool valueChanged)
    {
        if (valueChanged)
        {
            _volume = _musicSlider.Value;
            Music.SetVolume((float)_volume);
            Music.UpdateVolume();

            if (_volume == 0)
            {
                _musicIcon.SetTexture(_audioOffTexture);
            }
            else 
            {
                _musicIcon.SetTexture(_audioOnTexture);
            }
        }
    }
}
