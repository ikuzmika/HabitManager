using HabitManager.App.ViewModels;

namespace HabitManager.App.Views;

public partial class HabitListPage : ContentPage
{
    private bool _isLoading;
    public HabitListPage()
    {
        InitializeComponent();

        BindingContext = new HabitListViewModel(App.HabitService);
    }

    private async void OnHabitSelected(
        object? sender,
        SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.Count == 0)
        {
            return;
        }

        if (sender is not CollectionView collectionView)
        {
            return;
        }

        if (e.CurrentSelection[0] is not HabitListItemViewModel item)
        {
            return;
        }

        collectionView.SelectedItem = null;

        await Navigation.PushAsync(
            new HabitDetailsPage(item.Habit));
    }
    
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_isLoading)
        {
            return;
        }

        if (BindingContext is not HabitListViewModel viewModel)
        {
            return;
        }

        _isLoading = true;
        Content.IsEnabled = false;

        try
        {
            await viewModel.LoadAsync();

            Content.IsEnabled = true;
        }
        catch (Exception exception)
        {
            await DisplayAlertAsync(
                "Unable to load the data",
                exception.Message,
                "OK");
        }
        finally
        {
            _isLoading = false;
        }
    }
    
    private async void OnAddHabitClicked(object? sender, EventArgs e)
    {
        if (BindingContext is HabitListViewModel viewModel)
        {
            await Navigation.PushAsync(
                new HabitCreatePage(viewModel));
        }
    }
    
    private async void OnArchivedHabitsClicked(
        object? sender,
        EventArgs e)
    {
        await Navigation.PushAsync(new ArchivedHabitsPage());
    }
}