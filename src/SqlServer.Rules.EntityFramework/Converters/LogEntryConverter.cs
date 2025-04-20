class LogEntryConverter : WriteOnlyJsonConverter<LogEntry>
{
    public override void Write(VerifyJsonWriter writer, LogEntry logEntry)
    {
        writer.WriteStartObject();

        writer.WriteMember(logEntry, logEntry.Type, "Type");
        writer.WriteMember(logEntry, logEntry.HasTransaction, "HasTransaction");
        writer.WriteMember(logEntry, logEntry.Exception, "Exception");
        writer.WriteMember(logEntry, logEntry.Parameters, "Parameters");
        writer.WriteMember(logEntry, logEntry.Text, "Text");

        writer.WriteEndObject();
    }
}