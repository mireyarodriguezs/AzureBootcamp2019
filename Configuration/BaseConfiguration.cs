using Microsoft.Azure.Documents.Client;
using System;

namespace ServerlessDataPipeline.Configuration
{
    public abstract class BaseConfiguration<T>
    {
        public abstract T InitializeFromEnvironment();

        public static string GetEnvironmentVariable(string name)
        {
            return Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
        }
    }
}
