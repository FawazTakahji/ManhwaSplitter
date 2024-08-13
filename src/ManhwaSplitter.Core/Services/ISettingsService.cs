using ErrorOr;
using ManhwaSplitter.Core.Models;

namespace ManhwaSplitter.Core.Services;

public interface ISettingsService
{
    public Settings CurrentSettings { get; }

    public ErrorOr<Settings> Load();

    public ErrorOr<Success> Save(Settings settings);

    public ErrorOr<Success> Apply(Settings settings);

    public void Reset();
}