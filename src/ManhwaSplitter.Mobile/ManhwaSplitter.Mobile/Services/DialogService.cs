using System.Threading.Tasks;
using Avalonia.Data;
using AvaloniaDialogs.Views;
using ManhwaSplitter.Core.Dialogs.ViewModels;
using ManhwaSplitter.Core.Enums;
using ManhwaSplitter.Core.Models;
using ManhwaSplitter.Core.Services;
using ManhwaSplitter.Mobile.Dialogs.Views;

namespace ManhwaSplitter.Mobile.Services;

public class DialogService : IDialogService
{
    public async Task Show(string title, string content, DialogType type)
    {
        SingleActionDialog dialog = new()
        {
            Message = content,
            ButtonText = "OK"
        };

        await dialog.ShowAsync();
    }

    public async Task ShowFailedOperations(FailedFolder[] failedOperations)
    {
        FailedOperationsView view = new()
        {
            DataContext = new FailedOperationsViewModel(failedOperations)
        };

        await view.ShowAsync();
    }

    public async Task<bool> ShowYesNoDialog(string title, string content, DialogType type)
    {
        TwofoldDialog dialog = new()
        {
            Message = content,
            PositiveText = "Yes",
            NegativeText = "No"
        };

        Optional<bool> result = await dialog.ShowAsync();

        return result is { HasValue: true, Value: true };
    }
}