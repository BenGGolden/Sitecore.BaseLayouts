// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILog.cs" company="Aware Web Solutions">
//   Copyright 2008 - 2014 Aware Web Solutions.  All Rights Reserved.
// </copyright>
// <summary>
//   The Log interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace Sitecore.BaseLayouts.Abstractions
{
    /// <summary>
    /// The Log interface.
    /// </summary>
    public interface ILog
    {
        #region Public Methods and Operators

        /// <summary>
        /// The audit.
        /// </summary>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        void Audit(string format, params object[] parameters);

        /// <summary>
        /// The debug.
        /// </summary>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        void Debug(string format, params object[] parameters);

        /// <summary>
        /// The error.
        /// </summary>
        /// <param name="ex">
        /// The ex.
        /// </param>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        void Error(Exception ex, string format, params object[] parameters);

        /// <summary>
        /// The fatal.
        /// </summary>
        /// <param name="ex">
        /// The ex.
        /// </param>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        void Fatal(Exception ex, string format, params object[] parameters);

        /// <summary>
        /// The info.
        /// </summary>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        void Info(string format, params object[] parameters);

        /// <summary>
        /// The warn.
        /// </summary>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        void Warn(string format, params object[] parameters);

        /// <summary>
        /// The warn.
        /// </summary>
        /// <param name="ex">
        /// The ex.
        /// </param>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        void Warn(Exception ex, string format, params object[] parameters);
        
        #endregion
    }
}