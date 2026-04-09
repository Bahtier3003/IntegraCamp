using IntegraCamp.ViewModels;

namespace IntegraCamp.Views;

public partial class CounselorsPage : ContentPage
{
    public CounselorsPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        var vm = BindingContext as CounselorsViewModel;
        if (vm != null)
            await vm.LoadCounselors();
    }
}