// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   The log.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using Sitecore.Diagnostics;

namespace Sitecore.BaseLayouts.Abstractions
{
    /// <summary>
    ///     The log.
    /// </summary>
    public class LogWrapper : ILog
    {
        #region Methods

        /// <summary>
        ///     The get message.
        /// </summary>
        /// <param name="format">
        ///     The format.
        /// </param>
        /// <param name="parameters">
        ///     The parameters.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        private string GetMessage(string format, params object[] parameters)
        {
            Assert.ArgumentNotNullOrEmpty(format, "format");
            return parameters == null || parameters.Length == 0 ? format : string.Format(format, parameters);
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The audit.
        /// </summary>
        /// <param name="format">
        ///     The format.
        /// </param>
        /// <param name="parameters">
        ///     The parameters.
        /// </param>
        public void Audit(string format, params object[] parameters)
        {
            Log.Audit(GetMessage(format, parameters), typeof (LogWrapper));
        }

        /// <summary>
        ///     The debug.
        /// </summary>
        /// <param name="format">
        ///     The format.
        /// </param>
        /// <param name="parameters">
        ///     The parameters.
        /// </param>
        public void Debug(string format, params object[] parameters)
        {
            Log.Debug(GetMessage(format, parameters), this);
        }

        /// <summary>
        ///     The error.
        /// </summary>
        /// <param name="ex">
        ///     The ex.
        /// </param>
        /// <param name="format">
        ///     The format.
        /// </param>
        /// <param name="parameters">
        ///     The parameters.
        /// </param>
        public void Error(Exception ex, string format, params object[] parameters)
        {
            Log.Error(GetMessage(format, parameters), ex, typeof (LogWrapper));
        }

        /// <summary>
        ///     The fatal.
        /// </summary>
        /// <param name="ex">
        ///     The ex.
        /// </param>
        /// <param name="format">
        ///     The format.
        /// </param>
        /// <param name="parameters">
        ///     The parameters.
        /// </param>
        public void Fatal(Exception ex, string format, params object[] parameters)
        {
            Log.Fatal(GetMessage(format, parameters), ex, typeof (LogWrapper));
        }

        /// <summary>
        ///     The info.
        /// </summary>
        /// <param name="format">
        ///     The format.
        /// </param>
        /// <param name="parameters">
        ///     The parameters.
        /// </param>
        public void Info(string format, params object[] parameters)
        {
            Log.Info(GetMessage(format, parameters), this);
        }

        /// <summary>
        ///     The warn.
        /// </summary>
        /// <param name="format">
        ///     The format.
        /// </param>
        /// <param name="parameters">
        ///     The parameters.
        /// </param>
        public void Warn(string format, params object[] parameters)
        {
            Log.Warn(GetMessage(format, parameters), this);
        }

        /// <summary>
        ///     The warn.
        /// </summary>
        /// <param name="ex">
        ///     The ex.
        /// </param>
        /// <param name="format">
        ///     The format.
        /// </param>
        /// <param name="parameters">
        ///     The parameters.
        /// </param>
        public void Warn(Exception ex, string format, params object[] parameters)
        {
            Log.Warn(GetMessage(format, parameters), ex, this);
        }

        #endregion
    }
}