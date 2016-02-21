/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Diagnostics;
using Mkamo.Memopad.Core;

namespace Mkamo.Memopad.Internal.Core {
    internal class Updater {
        // ========================================
        // static field
        // ========================================
        private static readonly log4net.ILog Logger =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // ========================================
        // field
        // ========================================
        private string _latestUrl;

        // ========================================
        // constructor
        // ========================================

        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================
        public bool IsLatest(string current) {
            var latestVerStr = "";

            try {
                using (var client = new WebClient())
                using (var stream = client.OpenRead(MemopadConsts.LatestUrl))
                using (var reader = new StreamReader(stream, Encoding.UTF8)) {
                    latestVerStr = reader.ReadLine();
                    if (latestVerStr != null) {
                        _latestUrl = reader.ReadLine();
                    }
                    stream.Close();
                }
            } catch (WebException e) {
                Logger.Warn("latest.txt download failed.", e);
                throw;
            }

            var latestVer = default(Version);
            if (!string.IsNullOrEmpty(latestVerStr)) {
                latestVer = new Version(latestVerStr);
            }

            var currentVer = new Version(current);
            return !(currentVer < latestVer);
        }

        public void IsLatestAsync(string current, Action updateAction) {
            var latestVerStr = "";

            try {
                var client = new WebClient();
                var uri = new Uri(MemopadConsts.LatestUrl);
                client.OpenReadCompleted += (sender, e) => {
                    if (e.Error == null && !e.Cancelled) {
                        using (var reader = new StreamReader(e.Result, Encoding.UTF8)) {
                            latestVerStr = reader.ReadLine();
                            if (latestVerStr != null) {
                                _latestUrl = reader.ReadLine();
                            }
                            reader.Close();
                        }

                        var latestVer = default(Version);
                        if (!string.IsNullOrEmpty(latestVerStr)) {
                            latestVer = new Version(latestVerStr);
                        }

                        var currentVer = new Version(current);
                        if (currentVer < latestVer){
                            if (updateAction != null) {
                                updateAction();
                            }
                        }

                    }

                    var clientToDispose = (WebClient) e.UserState;
                    clientToDispose.Dispose();
                };

                client.OpenReadAsync(uri, client);

            } catch (WebException e) {
                Logger.Warn("latest.txt download failed.", e);
                throw;
            }
        }

        public void DownloadLatest() {
            if (!string.IsNullOrEmpty(_latestUrl)) {
                Process.Start(_latestUrl);
            }
        }
    }
}
