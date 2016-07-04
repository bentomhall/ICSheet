using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Runtime.Serialization;
using log4net;
using System.Windows.Threading;

namespace ICSheet5e
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App() : base()
        {
            Dispatcher.UnhandledException += OnDispatcherUnhandledException;
        }

        private static readonly ILog logger = LogManager.GetLogger("MainLogger");

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            logger.Fatal($"{e.Exception.Source} caused a fatal error: {e.Exception.Message}");
        }
    }
}
