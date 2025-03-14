using System;

namespace Core.Base.Utils.Env
{
    public static class EnvUtils
    {
        public static string GetEnv(string variable, string? defaultValue = null)
        {
            var value = Environment.GetEnvironmentVariable(variable);
            if (string.IsNullOrEmpty(value) && defaultValue == null)
            {
                throw new Exception($"A variavel de ambiente '{variable}' não foi definida.");
            }
            return value ?? defaultValue!;
        }
        public static bool GetEnvBool(string variable, bool defaultValue = false)
        {
            var value = Environment.GetEnvironmentVariable(variable);
            return !string.IsNullOrEmpty(value) ? value.ToLower() == "true" : defaultValue;
        }
    }
}

