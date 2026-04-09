using IntegraCamp.Models;
using IntegraCamp.Services;

namespace IntegraCamp.Views;

public partial class CounselorFormPage : ContentPage
{
    private DatabaseService _db;
    private Counselor _counselor;
    private bool _isEditing;

    public CounselorFormPage(Counselor counselor = null)
    {
        InitializeComponent();
        _db = new DatabaseService();
        _counselor = counselor ?? new Counselor();
        _isEditing = counselor != null;

        LoadData();
    }

    private void LoadData()
    {
        if (_isEditing)
        {
            Title = "Редактирование вожатого";
            FullNameEntry.Text = _counselor.FullName;
            PhoneEntry.Text = ""; // Телефон не показываем из хэша
            BuildingPicker.SelectedItem = _counselor.Building;
            SquadEntry.Text = _counselor.SquadNumber?.ToString();
            ChildrenCountEntry.Text = _counselor.ChildrenCount.ToString();
            RoomEntry.Text = ""; // Комнату не показываем из хэша
            StatusSwitch.IsToggled = _counselor.Status == "Закреплен";
        }
        else
        {
            Title = "Добавление вожатого";
            StatusSwitch.IsToggled = false;
        }
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        try
        {
            // Валидация
            if (string.IsNullOrWhiteSpace(FullNameEntry.Text))
            {
                await DisplayAlert("Ошибка", "Введите ФИО", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(PhoneEntry.Text))
            {
                await DisplayAlert("Ошибка", "Введите телефон", "OK");
                return;
            }

            if (BuildingPicker.SelectedItem == null)
            {
                await DisplayAlert("Ошибка", "Выберите корпус", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(RoomEntry.Text) || !SecurityService.ValidateRoom(RoomEntry.Text))
            {
                await DisplayAlert("Ошибка", "Введите корректный номер комнаты (101-108, 201-212, 301-312)", "OK");
                return;
            }

            if (!int.TryParse(ChildrenCountEntry.Text, out int childrenCount))
            {
                await DisplayAlert("Ошибка", "Введите корректное количество детей", "OK");
                return;
            }

            // Заполнение данных
            _counselor.FullName = FullNameEntry.Text;
            _counselor.PhoneHash = SecurityService.HashString(PhoneEntry.Text);
            _counselor.Building = BuildingPicker.SelectedItem.ToString();
            _counselor.ChildrenCount = childrenCount;
            _counselor.RoomHash = SecurityService.HashString(RoomEntry.Text);
            _counselor.Status = StatusSwitch.IsToggled ? "Закреплен" : "Свободен";

            if (!string.IsNullOrWhiteSpace(SquadEntry.Text) && int.TryParse(SquadEntry.Text, out int squad))
            {
                _counselor.SquadNumber = squad;
                _counselor.Status = "Закреплен";
            }
            else
            {
                _counselor.SquadNumber = null;
                if (!StatusSwitch.IsToggled)
                    _counselor.Status = "Свободен";
            }

            await _db.Init();

            if (_isEditing)
                await _db.UpdateCounselor(_counselor);
            else
                await _db.SaveCounselor(_counselor);

            await DisplayAlert("Успех", "Данные сохранены", "OK");
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка", ex.Message, "OK");
        }
    }
}