using FootballLogicLib;
using UIApplication.Connection;
using UIApplication.ViewModels;

namespace UIApplication.Views;

public partial class MainPage : ContentPage
{
	public MainPage(MainViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }
}
