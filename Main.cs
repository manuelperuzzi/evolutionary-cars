using Godot;
using System;

public class Main : Node2D
{
    [Export]
    public int carsNumber = 20;

    public override void _Ready()
    {
        Controller.Instance.Start(carsNumber);
    }
}
