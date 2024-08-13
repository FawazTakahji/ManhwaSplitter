using ManhwaSplitter.Core.Enums;
using ManhwaSplitter.Core.Models;

namespace ManhwaSplitter.Core.Services;

public interface IDialogService
{
    public Task Show(string title, string content, DialogType type);
    public Task ShowFailedOperations(FailedFolder[] failedOperations);
    public Task<bool> ShowYesNoDialog(string title, string content, DialogType type);
}