using IntegraCamp.ViewModels;

namespace IntegraCamp.Views;

public partial class AssignPage : ContentPage
{
    public AssignPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        var vm = BindingContext as AssignViewModel;
        if (vm != null)
            await vm.LoadFreeCounselors();
    }
}