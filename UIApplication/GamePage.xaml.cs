using Microsoft.Maui.Controls;

namespace Sem2_Test;

public partial class GamePage : ContentPage
{
    public GamePage()
    {
        InitializeComponent();
    }

    private void OnUpClicked(object sender, EventArgs e)
    {
        img_arg.TranslationY -= 10;
    }

    private void OnDownClicked(object sender, EventArgs e)
    {
        img_arg.TranslationY += 10;
    }

    private void OnLeftClicked(object sender, EventArgs e)
    {
        img_arg.TranslationX -= 10;
    }

    private void OnRightClicked(object sender, EventArgs e)
    {
        img_arg.TranslationX += 10;
    }
}