using Serilog.Core;
using System.Linq;
using Serilog.Events;
using Serilog.Enrichers.CustomRequestHeader.Accessors;
using Serilog.Enrichers.CustomRequestHeader.Extentions;

namespace Serilog.Enrichers
{
    public class CustomRequestHeaderEnricher : ILogEventEnricher
    {
        private readonly string _headerKey;
        private readonly string _propertyName;
        private readonly IHttpContextAccessor _contextAccessor;

        public CustomRequestHeaderEnricher(string headerKey, string propertyName) : this(headerKey, propertyName, new HttpContextAccessor())
        {
        }

        internal CustomRequestHeaderEnricher(string headerKey, string propertyName, IHttpContextAccessor contextAccessor)
        {
            _headerKey = headerKey;
            _propertyName = propertyName;
            _contextAccessor = contextAccessor;
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            if (_contextAccessor.HttpContext == null)
                return;

            var customHeaderValue = GetCustomHeaderValue();
            if (!string.IsNullOrEmpty(customHeaderValue))
            {
                var logEventProperty = new LogEventProperty(_propertyName, new ScalarValue(customHeaderValue));
                logEvent.AddOrUpdateProperty(logEventProperty);
            }
        }

        private string GetCustomHeaderValue()
        {
            string header = null;
            if (_contextAccessor.HttpContext.Request.Headers.TryGetValue(_headerKey, out var values))
                header = values.FirstOrDefault();
            else if (_contextAccessor.HttpContext.Response.Headers.TryGetValue(_headerKey, out values))
                header = values.FirstOrDefault();


            if (!_contextAccessor.HttpContext.Response.HeadersWritten &&
                !_contextAccessor.HttpContext.Response.Headers.AllKeys.Contains(_headerKey))
            {
                _contextAccessor.HttpContext.Response.Headers.Add(_headerKey, header);
            }

            return header;
        }
    }
}
