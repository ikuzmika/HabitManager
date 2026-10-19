using HabitManager.App.ViewModels;
using Microsoft.Maui.Controls.Shapes;

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
    
    private void OnPageLayoutSizeChanged(object? sender, EventArgs e)
    {
        if (HeaderGrid == null ||
            HeaderTitle == null ||
            HeaderActions == null ||
            HabitsGridLayout == null)
        {
            return;
        }

        double availableWidth =
            PageLayout.Width -
            PageLayout.Padding.Left -
            PageLayout.Padding.Right;

        if (availableWidth <= 0)
        {
            return;
        }
        
        int columns = availableWidth >= 640 ? 2 : 1;

        if (HabitsGridLayout.Span != columns)
        {
            HabitsGridLayout.Span = columns;
        }
        
        if (availableWidth < 600)
        {
            Grid.SetColumnSpan(HeaderTitle, 2);

            Grid.SetRow(HeaderActions, 1);
            Grid.SetColumn(HeaderActions, 0);
            Grid.SetColumnSpan(HeaderActions, 2);
        }
        else
        {
            Grid.SetColumnSpan(HeaderTitle, 1);

            Grid.SetRow(HeaderActions, 0);
            Grid.SetColumn(HeaderActions, 1);
            Grid.SetColumnSpan(HeaderActions, 1);
        }
    }
    
    private void OnHabitPointerEntered(object? sender, PointerEventArgs e)
    {
        if (sender is Border card)
        {
            card.SetAppThemeColor(
                VisualElement.BackgroundColorProperty,
                Color.FromArgb("#FFFFFF"),
                Color.FromArgb("#35303F"));
        }
    }

    private void OnHabitPointerExited(object? sender, PointerEventArgs e)
    {
        if (sender is Border card)
        {
            card.SetAppThemeColor(
                VisualElement.BackgroundColorProperty,
                Color.FromArgb("#F8F6FC"),
                Color.FromArgb("#25222D"));
        }
    }
    
}