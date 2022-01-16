using Serilog.Configuration;
using Serilog.Enrichers;
using System;

namespace Serilog
{
    public static class CustomRequestHeaderLoggerConfigurationExtentions
    {
        public static LoggerConfiguration WithCustomRequestHeader(
            this LoggerEnrichmentConfiguration enrichmentConfiguration,
            string headerKey, string propertyName)
        {
            if (enrichmentConfiguration == null) throw new ArgumentNullException(nameof(enrichmentConfiguration));
            return enrichmentConfiguration.With(new CustomRequestHeaderEnricher(headerKey, propertyName));
        }
    }
}
