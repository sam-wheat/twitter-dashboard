using System.Reactive.Subjects;

namespace Dashboard.Domain;

public interface ITwitterClient
{
    Subject<string> Messages { get; }

    Task Start(CancellationToken token);
}