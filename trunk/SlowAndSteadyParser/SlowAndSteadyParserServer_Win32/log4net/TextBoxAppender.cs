using System;
using System.Collections;
using System.Windows.Forms;
using log4net;
using log4net.Core;
using log4net.Appender;
using log4net.Repository.Hierarchy;

namespace log4net.Appender
{
    /// <summary>
    /// Implements a log4net appender to send output to a TextBox control.
    /// </summary>
    public class TextBoxAppender : AppenderSkeleton
    {

        private System.Windows.Forms.TextBox _textBox;
        private const int _maxTextLength = 100000;
        public TextBoxAppender()
        {
        }

        //protected override void Append(log4net.Core.LoggingEvent LoggingEvent)
        //{
        //    System.IO.StringWriter stringWriter = new System.IO.StringWriter(System.Globalization.CultureInfo.InvariantCulture);
        //    Layout.Format(stringWriter, LoggingEvent);
        //    _textBox.AppendText(stringWriter.ToString());
        //}

        protected override void Append(LoggingEvent LoggingEvent)
        {
            if (_textBox != null)
            {
                if (_textBox.InvokeRequired)
                {
                    _textBox.BeginInvoke(new UpdateControlDelegate(UpdateControl), new object[] { LoggingEvent });
                }
                else
                {
                    UpdateControl(LoggingEvent);
                }
            }
        }

        /// <summary>
        /// Delegate used to invoke UpdateControl
        /// </summary>
        /// <param name="loggingEvent">The event to log</param>
        /// <remarks>This delegate is used when UpdateControl must be
        /// called from a thread other than the thread that created the
        /// TextBox control.</remarks>
        private delegate void UpdateControlDelegate(LoggingEvent loggingEvent);

        /// <summary>
        /// Add logging event to configured control
        /// </summary>
        /// <param name="loggingEvent">The event to log</param>
        private void UpdateControl(LoggingEvent loggingEvent)
        {
            // There may be performance issues if the buffer gets too long
            // So periodically clear the buffer
            if (_textBox.TextLength > _maxTextLength)
            {
                _textBox.Clear();
                _textBox.AppendText(string.Format("(earlier messages cleared because log length exceeded maximum of {0})\n\n", _maxTextLength));
            }

            _textBox.AppendText(RenderLoggingEvent(loggingEvent));
        }

        public TextBox TextBox
        {
            set
            {
                _textBox = value;
            }
            get
            {
                return _textBox;
            }
        }

        public static void SetTextBox(TextBox tb)
        {
            foreach (log4net.Appender.IAppender appender in GetAppenders())
            {
                if (appender is TextBoxAppender)
                {
                    ((TextBoxAppender)appender).TextBox  = tb;
                }
            }
        }

        private static IAppender[] GetAppenders()
        {
            ArrayList appenders = new ArrayList();


            appenders.AddRange(((Hierarchy)LogManager.GetRepository()).Root.Appenders);

            foreach (ILog log in LogManager.GetCurrentLoggers())
            {

                appenders.AddRange(((Logger)log.Logger).Appenders);
            }

            return (IAppender[])appenders.ToArray(typeof(IAppender));
        }
    }
}