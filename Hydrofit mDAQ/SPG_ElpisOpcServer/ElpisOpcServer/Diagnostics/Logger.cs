using log4net;


namespace ElpisOpcServer.Diagnostics
{
    internal class Logger : ILogger
    {

        private static readonly ILog log;
        static Logger()
        {
            log = LogManager.GetLogger(typeof(Logger));
            log4net.Config.XmlConfigurator.Configure();
            
        }
        public void LogError(string message)
        {
            log.Error(message);
        }

        public void LogInfo(string message)
        {
            log.Info(message);
        }

        public void LogWarning(string message)
        {
            log.Warn(message);
        }
    }
}
