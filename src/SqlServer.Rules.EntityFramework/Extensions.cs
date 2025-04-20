static class Extensions
{
    public static Dictionary<string, object> ToDictionary(this DbParameterCollection collection)
    {
        var dictionary = new Dictionary<string, object>();
        foreach (DbParameter parameter in collection)
        {
            var direction = parameter.Direction;
            if (direction is ParameterDirection.Output or ParameterDirection.ReturnValue)
            {
                continue;
            }

            var nullChar = "";
            if (parameter.IsNullable)
            {
                nullChar = "?";
            }

            var key = $"{parameter.ParameterName} ({parameter.DbType}{nullChar})";
            dictionary[key] = parameter.Value!;
        }

        return dictionary;
    }
}