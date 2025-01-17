﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;

#pragma warning disable IDE1006 // Naming Styles

namespace Gehtsoft.Webview2.Uitest
{
    /// <summary>
    /// <para>A form with web browser. </para>
    /// <para>Don't create the form directly, use <see cref="WebBrowserDriver"/> class instead.</para>
    /// </summary>
    public partial class WebBrowserForm : Form
    {
        /// <summary>
        /// A web view object
        /// </summary>
        public WebView2 WebView => webViewControl;

        /// <summary>
        /// The flag indicating that the form initialization is completed.
        /// </summary>
        public bool LoadingCompled { get; private set; } = false;

        /// <summary>
        /// The flag indicating that the last initiated navigation operation is completed.
        /// </summary>
        public bool NavigationCompleted { get; private set; } = false;

        /// <summary>
        /// Constructor
        /// </summary>
        public WebBrowserForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initialize the control
        /// </summary>
        /// <param name="cacheFolder"></param>
        /// <returns></returns>
        public async Task InitializeWebControlAsync(string cacheFolder)
        {
            if (!string.IsNullOrEmpty(cacheFolder))
            {
                var env = await CoreWebView2Environment.CreateAsync(null, cacheFolder);
                await webViewControl.EnsureCoreWebView2Async(env);
            }
            else
                await webViewControl.EnsureCoreWebView2Async();
        }

        /// <summary>
        /// Starts navigation to the URL specified
        /// </summary>
        /// <param name="url"></param>
        public void NavigateTo(string url)
        {
            NavigationCompleted = false;
            webViewControl.CoreWebView2.Navigate(url);
        }

        /// <summary>
        /// Sets the browser content to HTML specified
        /// </summary>
        /// <param name="html"></param>
        public void SetContent(string html)
        {
            NavigationCompleted = false;
            webViewControl.CoreWebView2.NavigateToString(html);
        }

        private void webViewControl_CoreWebView2InitializationCompleted(object sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            LoadingCompled = true;
        }

        private void webViewControl_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            NavigationCompleted = true;
        }

        private void webViewControl_NavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs e)
        {
            NavigationCompleted = false;
        }

        /// <summary>
        /// Returns the list of the cookies.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public async Task<IReadOnlyList<Cookie>> GetCookies(string uri)
        {
            var cookies = await WebView.CoreWebView2.CookieManager.GetCookiesAsync(uri).ConfigureAwait(false);
            List<Cookie> result = new();
            foreach (var cookie in cookies)
            {
                result.Add(new Cookie()
                {
                    Path = cookie.Path,
                    Name = cookie.Name,
                    IsSecure = cookie.IsSecure,
                    IsSession = cookie.IsSession,
                    Value = cookie.Value,
                    Expires = cookie.Expires,
                });
            }
            return result;
        }
    }
}
