using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HabitManager.App.ViewModels;
using HabitManager.Domain;

namespace HabitManager.App.Views;

public partial class HabitListPage : ContentPage
{
    public HabitListPage()
    {
        InitializeComponent();

        BindingContext = new HabitListViewModel();
    }

    private async void OnHabitSelected(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.Count == 0)
        {
            return;
        }

        if (sender is not CollectionView collectionView)
        {
            return;
        }

        Habit selectedHabit = (Habit)e.CurrentSelection[0];
        
        collectionView.SelectedItem = null;

        await Navigation.PushAsync(new HabitDetailsPage(selectedHabit));
    }
    
    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is HabitListViewModel viewModel)
        {
            viewModel.Refresh();
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
}