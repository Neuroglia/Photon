﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Photon
{

    /// <summary>
    /// Encapsulates a Photon application
    /// </summary>
    public class Application
        : DependencyObject, IDependencyElement
    {

        /// <summary>
        /// The default frequency, in milliseconds, at which the application will check for events
        /// </summary>
        private const int DEFAULT_UPDATE_FREQUENCY = 500;

        /// <summary>
        /// This event is fired on <see cref="Application"/> start up
        /// </summary>
        public event EventHandler Startup;
        /// <summary>
        /// This event is fired when the <see cref="Application"/> shuts down
        /// </summary>
        public event EventHandler Exit;

        /// <summary>
        /// The default constructor for the <see cref="Application"/> class
        /// </summary>
        public Application()
        {
            Application.Current = this;
            this._Windows = new ObservableHashSet<Window>();
            this._Windows.CollectionChanged += this.OnWindowCollectionChanged;
        }

        /// <summary>
        /// Gets an array of string representing the application's statup arguments
        /// </summary>
        public string[] StartupArguments { get; private set; }

        /// <summary>
        /// Gets/sets the <see cref="Uri"/> of application's startup <see cref="IUIElement"/>
        /// </summary>
        [XmlIgnore]
        public Uri StartupUri { get; set; }

        /// <summary>
        /// Gets/sets the string from which the <see cref="Application.StartupUri"/> is created
        /// </summary>
        [XmlElement("StartupUri")]
        public string StartupUriString
        {
            get
            {
                return this.StartupUri.ToString();
            }
            set
            {
                this.StartupUri = new Uri(value);
            }
        }

        /// <summary>
        /// Gets/sets the application's <see cref="Photon.ShutdownMode"/>
        /// </summary>
        public ShutdownMode ShutdownMode { get; set; }

        /// <summary>
        /// Gets the application's <see cref="ApplicationState"/>
        /// </summary>
        public ApplicationState State { get; private set; }

        /// <summary>
        /// Gets/sets the application's main <see cref="Window"/>
        /// </summary>
        public Window MainWindow { get; set; }

        private ObservableHashSet<Window> _Windows;
        /// <summary>
        /// Gets a <see cref="IReadOnlyList{T}"/> containing all the application's active <see cref="Window"/>s
        /// </summary>
        public IReadOnlyList<Window> Windows
        {
            get
            {
                return (IReadOnlyList<Window>)this.Windows;
            }
        }

        /// <summary>
        /// Registers the specified <see cref="Window"/>
        /// </summary>
        /// <param name="window">The <see cref="Window"/> to register</param>
        internal void RegisterWindow(Window window)
        {
            lock (this._Windows)
            {
                this._Windows.Add(window);
            }
        }

        /// <summary>
        /// Starts the application
        /// </summary>
        public void Run()
        {
            Stream xamlStream;
            if(this.Startup != null)
            {
                this.Startup(this, new EventArgs());
            }
            this.OnStartup();
            try
            {
                xamlStream = Application.GetResourceStream(this.StartupUri);
            }
            catch (Exception ex)
            {
                throw new Exception("An exception occured while attempting to retrieve the xaml file of the window specified by the Application's 'StartupUri' property. Location searched: '" + this.StartupUri.ToString() + "'. See inner exception for more details", ex);
            }
            try
            {
                this.MainWindow = Markup.XamlParser.LoadDependencyElementFrom<Window>(xamlStream);
                this.MainWindow.Show();
            }
            catch (Exception ex)
            {
                throw new Exception("An exception occured while loading the window's xaml file. See inner exception for more details", ex);
            }
            while (this.State == ApplicationState.Running)
            {
                Thread.Sleep(Application.DEFAULT_UPDATE_FREQUENCY);
            }
            this.State = ApplicationState.NotRunning;
        }

        /// <summary>
        /// Shuts the application down
        /// </summary>
        public void Shutdown()
        {
            if (this.Exit != null)
            {
                this.Exit(this, new EventArgs());
            }
            this.OnExit();
            this.State = ApplicationState.ShuttingDown;
        }

        /// <summary>
        /// When overriden in a class, allows the execution of code when the application is starting up
        /// </summary>
        protected virtual void OnStartup()
        {

        }

        /// <summary>
        /// Handles the <see cref="Application.Windows"/>'s <see cref="ObservableHashSet{TElement}.CollectionChanged"/> event
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The <see cref="EventArgs"/> associated with the event</param>
        private void OnWindowCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Window window;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    window = (Window)e.NewItems[0];
                    window.Closed += this.OnWindowClosed;
                    break;
                case NotifyCollectionChangedAction.Remove:
                    break;
            }
        }

        /// <summary>
        /// Handles the <see cref="Window.Closed"/> event of the application's registered, active <see cref="Window"/>s
        /// </summary>
        /// <param name="sender">The event's sender</param>
        /// <param name="e">The event's arguments</param>
        private void OnWindowClosed(object sender, EventArgs e)
        {
            Window window;
            window = (Window)sender;
            window.Closed -= this.OnWindowClosed;
            lock (this._Windows)
            {
                this._Windows.Remove(window);
            }
            switch (this.ShutdownMode)
            {
                case ShutdownMode.OnMainWindowClosed:
                    if(window == this.MainWindow)
                    {
                        this.Shutdown();
                    }
                    break;
                case ShutdownMode.OnLastWindowClosed:
                    if(this._Windows.Count < 1)
                    {
                        this.Shutdown();
                    }
                    break;
            }
        }

        /// <summary>
        /// When overriden in a class, allows the execution of code when the application is exiting
        /// </summary>
        protected virtual void OnExit()
        {

        }

        /// <summary>
        /// Gets the current <see cref="Application"/>
        /// </summary>
        public static Application Current { get; private set; }

        /// <summary>
        /// Gets the <see cref="Stream"/> of the specified resource
        /// </summary>
        /// <param name="resourceUri">The resource's <see cref="Uri"/></param>
        /// <returns>The specified resource's <see cref="Stream"/></returns>
        public static Stream GetResourceStream(Uri resourceUri)
        {
            return ResourceManager.GetResourceStream(resourceUri);
        }

        /// <summary>
        /// Starts a new application fo the specified type
        /// </summary>
        /// <typeparam name="TApplication">The type of the application to create</typeparam>
        /// <param name="startupArguments">An array of string representing the application's startup arguments</param>
        public static void Start<TApplication>(string[] startupArguments)
            where TApplication : Application
        {
            string xamlFilePath;
            Stream xamlStream;
            Application application;
            xamlFilePath = "/" + typeof(TApplication).Assembly.GetName().Name + ";component/" + typeof(TApplication).Name + ".xaml";
            try
            {
                xamlStream = Application.GetResourceStream(new Uri(xamlFilePath, UriKind.Relative));
            }
            catch(Exception ex)
            {
                throw new Exception("An exception occured while attempting to retrieve the application's xaml file. This may occur if the xaml file name differs from the application type name. Location searched: '" + xamlFilePath + "'. See inner exception for more details", ex);
            }
            application = null;
            try
            {
                application = Markup.XamlParser.LoadDependencyElementFrom<TApplication>(xamlStream);
                application.StartupArguments = startupArguments;
            }
            catch(Exception ex)
            {
                throw new Exception("An exception occured while loading the application's xaml file. See inner exception for more details", ex);
            }
            try
            {
                application.Run();
            }
            catch(Exception ex)
            {
                throw new Exception("An unhandled exception has been thrown during the application's runtime. See inner exception for more details.", ex);
            }
        }

    }

}
