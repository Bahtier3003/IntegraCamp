using IntegraCamp.ViewModels;

namespace IntegraCamp.Views;

public partial class ProfilePage : ContentPage
{
    public ProfilePage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        var vm = BindingContext as ProfileViewModel;
        if (vm != null)
            await vm.LoadProfile();
    }
}