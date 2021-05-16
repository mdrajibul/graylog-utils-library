using Newtonsoft.Json;
using NLog.Config;
using System.Collections.Generic;
using System;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using RAJ.GrayLog.Interfaces;
using NLog.Targets;
using RAJ.GrayLog.Transport;
using RAJ.GrayLog.Client;
using RAJ.GrayLog.Utils;
using NLog;

namespace RAJ.GrayLog
{
    [Target("Gelf")]
    public class GelfTarget : TargetWithLayout
    {
        private Lazy<ITransport> _lazyITransport;
        private string _facility;
        private Uri _endpoint;

        [Required]
        public string EndPoint
        {
            get { return _endpoint.ToString(); }
            set { _endpoint = !string.IsNullOrEmpty(value) ? new Uri(Environment.ExpandEnvironmentVariables(value)) : null; }
        }

        [ArrayParameter(typeof(ParameterInfo), "parameter")]
        public IList<ParameterInfo> Parameters { get; private set; }

        public string Facility
        {
            get { return _facility; }
            set { _facility = value != null ? Environment.ExpandEnvironmentVariables(value) : null; }
        }

        public bool SendLastFormatParameter { get; set; }

        public IGelfConverter Converter { get; private set; }
        public IEnumerable<ITransport> Transports { get; private set; }

        public GelfTarget() : this(
            new ITransport[] {
                new HttpTransport(new HttpTransportClient()),
                new UdpTransport(new UdpTransportClient())
            },
            new GelfConverter())
        {
        }

        public GelfTarget(IEnumerable<ITransport> transports, IGelfConverter converter)
        {
            Transports = transports;
            Converter = converter;
            Parameters = new List<ParameterInfo>();
            _lazyITransport = new Lazy<ITransport>(() =>
            {
                return Transports.Single(x => x.Scheme.ToUpper() == _endpoint?.Scheme?.ToUpper());
            });
        }

        public void WriteLogEventInfo(LogEventInfo logEvent)
        {
            Write(logEvent);
        }

        protected override void Write(LogEventInfo logEvent)
        {
            foreach (var par in this.Parameters)
            {
                if (!logEvent.Properties.ContainsKey(par.Name))
                {
                    string stringValue = par.Layout.Render(logEvent);

                    logEvent.Properties.Add(par.Name, stringValue);
                }
            }

            if (SendLastFormatParameter && logEvent.Parameters != null && logEvent.Parameters.Any())
            {
                if (!logEvent.Properties.ContainsKey(ConverterConstants.PromoteObjectPropertiesMarker))
                {
                    logEvent.Properties.Add(ConverterConstants.PromoteObjectPropertiesMarker, logEvent.Parameters.Last());
                }
            }

            var jsonObject = Converter.Convert(logEvent, Facility);
            if (jsonObject == null)
                return;
            if (_endpoint != null)
                _lazyITransport.Value.Send(_endpoint, jsonObject.ToString(Formatting.None, null));
        }
    }
}