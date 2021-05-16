using NLog.Config;
using NLog.Layouts;

namespace RAJ.GrayLog
{
    [NLogConfigurationItem]
    public class ParameterInfo
    {
        public ParameterInfo()
           : this(null, null)
        {
        }
        public ParameterInfo(string parameterName, Layout parameterLayout)
        {
            this.Name = parameterName;
            this.Layout = parameterLayout;
        }
        [RequiredParameter]
        public string Name { get; set; }
        [RequiredParameter]
        public Layout Layout { get; set; }

    }
}