using Godot;
using System;

/// <summary>
/// It's a simple button in the Godot scene that, when pressed, starts the simulation.
/// </summary>
public class StartButton : Button
{
    private static readonly String mainPath = "/root/Main";

    /// <summary>
    /// Called when the button is pressed. Starts the simulation.
    /// </summary>
    public void OnButtonPressed() 
    {
        var mainNode = (Main) GetNode(mainPath);
        Controller.Instance.Start(mainNode.carsNumber);
        this.Visible = false;
    }
}
