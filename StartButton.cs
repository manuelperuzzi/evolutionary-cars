using Godot;
using System;

public class StartButton : Button
{
    private static readonly String mainPath = "/root/Main";

    public void OnButtonPressed() 
    {
        var mainNode = (Main) GetNode(mainPath);
        Controller.Instance.Start(mainNode.carsNumber);
        this.Visible = false;
    }
}
