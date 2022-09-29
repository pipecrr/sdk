
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace Siesa.SDK.Frontend.Services
{
    public class NavigationService : IDisposable
    {
        private const int MinHistorySize = 256;
        private const int AdditionalHistorySize = 64;
        private readonly NavigationManager _navigationManager;
        private readonly List<string> _history;

        private List<string> ReplaceQueque { get; set; } = new List<string>();

        private bool navigationManagerEnabled = false;

        public NavigationService(NavigationManager navigationManager)
        {
            _navigationManager = navigationManager;
            try
            {
                _history = new List<string>(MinHistorySize + AdditionalHistorySize);
                _history.Add(_navigationManager.Uri);
                _navigationManager.LocationChanged += OnLocationChanged;
                navigationManagerEnabled = true;
            }
            catch (System.Exception)
            {
                navigationManagerEnabled = false;
            }
        }

        /// <summary>
        /// Navigates to the specified url.
        /// </summary>
        /// <param name="url">The destination url (relative or absolute).</param>
        public void NavigateTo(string url)
        {
            if(navigationManagerEnabled)
            {
                _navigationManager.NavigateTo(url);
            }
        }

        /// <summary>
        /// Returns true if it is possible to navigate to the previous url.
        /// </summary>
        public bool CanNavigateBack => _history.Count >= 2;

        /// <summary>
        /// Navigates to the previous url if possible or does nothing if it is not.
        /// </summary>
        public void NavigateBack()
        {
            if(!navigationManagerEnabled)
            {
                return;
            }
            if (!CanNavigateBack) return;
            var currentPageUrl = _navigationManager.Uri;
            var backPageUrl = "";
            for (int i = 2; i <= _history.Count; i++)
            {
                backPageUrl = _history[^i];
                if (backPageUrl != currentPageUrl)
                {
                    _history.RemoveRange(_history.Count - i, i);
                    _navigationManager.NavigateTo(backPageUrl);
                    break;
                }
            }
        }

        // .. All other navigation methods.

        private void OnLocationChanged(object sender, LocationChangedEventArgs e)
        {
            EnsureSize();
            string location = e.Location;
            if(ReplaceQueque.Contains(location))
            {
                try
                {
                    ReplaceQueque.Remove(location);
                    this.RemoveCurrentItem();
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine("Error", ex.Message);
                }
            }
            _history.Add(location);
        }

        private void EnsureSize()
        {
            if (_history.Count < MinHistorySize + AdditionalHistorySize) return;
            _history.RemoveRange(0, _history.Count - MinHistorySize);
        }

        public void Dispose()
        {
            if(!navigationManagerEnabled)
            {
                return;
            }
            _navigationManager.LocationChanged -= OnLocationChanged;
        }

        public void RemoveCurrentItem()
        {
            _history.RemoveAt(_history.Count - 1);
        }

        public void RemoveItem(string url)
        {
            _history.Remove(url);
        }

        public void NavigateTo(string uri, bool forceLoad = false, bool replace = false)
        {
            if(!navigationManagerEnabled)
            {
                return;
            }
            if (replace)
            {
                var queueUrl = uri;
                //add base url if not exist
                if(!uri.StartsWith(_navigationManager.BaseUri))
                {
                    queueUrl = _navigationManager.BaseUri.Substring(0, _navigationManager.BaseUri.Length - 1) + uri;
                }
                ReplaceQueque.Add(queueUrl);
            }
            _navigationManager.NavigateTo(uri, forceLoad, replace);
        }

    }
}