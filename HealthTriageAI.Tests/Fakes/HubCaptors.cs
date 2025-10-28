using Microsoft.AspNetCore.SignalR;

namespace HealthTriageAI.Tests.Fakes;

public class CapturingHubContext<THub> : IHubContext<THub> where THub : Hub
{
    public CapturingHubContext()
    {
        Clients = new CapturingHubClients();
        Groups = new NoopGroupManager();
    }

    public IHubClients Clients { get; }
    public IGroupManager Groups { get; }

    public class CapturingHubClients : IHubClients
    {
        public CapturingClientProxy AllProxy { get; } = new CapturingClientProxy();
        public IClientProxy All => AllProxy;
        public IClientProxy AllExcept(IReadOnlyList<string> _) => AllProxy;
        public IClientProxy Client(string _) => AllProxy;
        public IClientProxy Clients(IReadOnlyList<string> _) => AllProxy;
        public IClientProxy Group(string _) => AllProxy;
        public IClientProxy GroupExcept(string _, IReadOnlyList<string> __) => AllProxy;
        public IClientProxy Groups(IReadOnlyList<string> _) => AllProxy;
        public IClientProxy User(string _) => AllProxy;
        public IClientProxy Users(IReadOnlyList<string> _) => AllProxy;
    }

    public record CapturedCall(int Seq, string Method, object?[] Args, DateTimeOffset At);

    public class CapturingClientProxy : IClientProxy
    {
        private readonly object _gate = new();
        private int _seq;
        private readonly List<CapturedCall> _calls = new();

        public IReadOnlyList<CapturedCall> Calls
        {
            get { lock (_gate) return _calls.ToList(); }
        }

        public Task SendCoreAsync(string method, object?[] args, CancellationToken cancellationToken = default)
        {
            var seq = Interlocked.Increment(ref _seq);
            var call = new CapturedCall(seq, method, args, DateTimeOffset.UtcNow);
            lock (_gate) _calls.Add(call);
            return Task.CompletedTask;
        }
    }

    public class NoopGroupManager : IGroupManager
    {
        public Task AddToGroupAsync(string _, string __, CancellationToken ___ = default) => Task.CompletedTask;
        public Task RemoveFromGroupAsync(string _, string __, CancellationToken ___ = default) => Task.CompletedTask;
    }
}
