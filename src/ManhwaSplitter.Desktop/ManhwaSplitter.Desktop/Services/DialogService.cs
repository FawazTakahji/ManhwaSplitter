using System.Threading.Tasks;
using Avalonia.Controls;
using ManhwaSplitter.Core.Models;
using ManhwaSplitter.Core.Services;
using ManhwaSplitter.Core.Dialogs.ViewModels;
using ManhwaSplitter.Core.Enums;
using ManhwaSplitter.Desktop.Dialogs.Views;
using Ursa.Controls;

namespace ManhwaSplitter.Desktop.Services;

public class DialogService : IDialogService
{
    public async Task Show(string title, string content, DialogType type)
    {
        TextBlock textBlock = new()
        {
            Text = content
        };

        DialogMode mode = type switch
        {
            DialogType.Info => DialogMode.Info,
            DialogType.Warning => DialogMode.Warning,
            DialogType.Error => DialogMode.Error,
            DialogType.Question => DialogMode.Question,
            DialogType.Success => DialogMode.Success,
            DialogType.None => DialogMode.None,

            _ => DialogMode.None
        };

        await OverlayDialog.ShowModal(textBlock, null, options: new OverlayDialogOptions
        {
            Title = title,
            Mode = mode,
            Buttons = DialogButton.OK,
            CanLightDismiss = false
        });
    }

    public async Task ShowFailedOperations(FailedFolder[] failedOperations)
    {
        await OverlayDialog.ShowModal<FailedOperationsView, FailedOperationsViewModel>(
            new FailedOperationsViewModel(failedOperations),
            options: new OverlayDialogOptions
            {
                Title = "Failed to split some folders/images.",
                Mode = DialogMode.Warning,
                Buttons = DialogButton.OK,
                CanLightDismiss = false
            });
    }

    public async Task<bool> ShowYesNoDialog(string title, string content, DialogType type)
    {
        TextBlock textBlock = new()
        {
            Text = content
        };

        DialogMode mode = type switch
        {
            DialogType.Info => DialogMode.Info,
            DialogType.Warning => DialogMode.Warning,
            DialogType.Error => DialogMode.Error,
            DialogType.Question => DialogMode.Question,
            DialogType.Success => DialogMode.Success,
            DialogType.None => DialogMode.None,

            _ => DialogMode.None
        };

        DialogResult result = await OverlayDialog.ShowModal(textBlock, null,options: new OverlayDialogOptions
        {
            Title = title,
            Mode = mode,
            Buttons = DialogButton.YesNo,
            CanLightDismiss = false
        });

        return result == DialogResult.Yes;
    }
}