using System.Text;
using System.Web;
using EPiServer.Find.Tracing;
using log4net;

namespace Site.Business.Log
{
    /// <summary>
    /// Sample used from: https://github.com/episerver/find-tracing-log4net
    /// </summary>
    public class TraceListener : ITraceListener
    {
        private ILog log = LogManager.GetLogger("EPiServer.Find");

        public void Add(ITraceEvent traceEvent)
        {
            var message = new StringBuilder();
            if (HttpContext.Current != null)
            {
                message.Append("During HTTP request to: ");
                message.Append(HttpContext.Current.Request.Url);
                message.AppendLine();
            }

            message.Append("Instance ");
            message.Append(traceEvent.Source.TraceId);
            message.Append(" of type ");
            message.Append(traceEvent.Source.GetType());
            message.Append(" reported: ");
            message.Append(traceEvent.Message);
            message.AppendLine();

            if (traceEvent.IsError)
            {
                log.Error(message.ToString());
            }
            else
            {
                log.Debug(message.ToString());
            }
        }
    }
}