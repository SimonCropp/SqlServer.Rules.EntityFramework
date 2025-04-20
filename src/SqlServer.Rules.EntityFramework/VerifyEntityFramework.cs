namespace VerifyTests;

public static class VerifyEntityFramework
{
    public static void Initialize(DbContext context) =>
        Initialize(context.Model);

    public static bool Initialized { get; private set; }

    public static void Initialize(IModel? model = null)
    {
        if (Initialized)
        {
            throw new("Already Initialized");
        }

        Initialized = true;

        InnerVerifier.ThrowIfVerifyHasBeenRun();

        VerifierSettings.IgnoreMembersWithType(typeof(IDbContextFactory<>));
        VerifierSettings.IgnoreMembersWithType<DbContext>();
        var converters = DefaultContractResolver.Converters;
        converters.Add(new LogEntryConverter());
    }

    public static DbContextOptionsBuilder<TContext> EnableRecording<TContext>(this DbContextOptionsBuilder<TContext> builder)
        where TContext : DbContext
        => builder.EnableRecording(null);

    public static DbContextOptionsBuilder<TContext> EnableRecording<TContext>(this DbContextOptionsBuilder<TContext> builder, string? identifier)
        where TContext : DbContext =>
        builder.AddInterceptors(new LogCommandInterceptor(identifier));

    static ConcurrentBag<Guid> recordingDisabledContextIds = [];

    public static void DisableRecording<TContext>(this TContext context)
        where TContext : DbContext =>
        recordingDisabledContextIds.Add(context.ContextId.InstanceId);

    internal static bool IsRecordingDisabled<TContext>(this TContext context)
        where TContext : DbContext =>
        recordingDisabledContextIds.Contains(context.ContextId.InstanceId);
}