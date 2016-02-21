/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using Mkamo.Common.Win32.User32;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using Mkamo.Memopad.Remote;

namespace Mkamo.Memopad.Remote {
    public static class MemopadClient {
        private const string TargetExeFilename = "MochaNote.exe";
        private const string TargetWindowTitle = "MochaNote";

        private const string Create = "create";
        private const string Load = "load";
        private const string Remove = "remove";
        private const string Uris = "uris";
        private const string Exist = "exist";
        private const string Title = "title";

        private static string GetMemoId(string memoUri) {
            if (string.IsNullOrEmpty(memoUri)) {
                return null;
            } else {
                if (memoUri.StartsWith("memo:///")) {
                    return memoUri.Substring("memo:///".Length);
                } else {
                    return null;
                }
            }
        }

        public static void Start(string[] args, bool console) {
            if (args == null || args.Length < 1) {
                if (console) {
                    ShowUsage();
                }
                return;
            }

            /// check args
            var validMethods = new[] { Create, Load, Remove, Uris, Exist, Title, };
            var method = args[0];
            if (!validMethods.Contains(method)) {
                ShowUsage();
                return;
            }

            var rest = default(string[]);
            var memoId = "";
            var title = "";
            switch (method) {
                case Uris: {
                    rest = new string[args.Length - 1];
                    Array.Copy(args, 1, rest, 0, args.Length - 1);
                    break;
                }
                case Create: {
                    if (args.Length < 2) {
                        ShowUsage();
                        return;
                    }
                    rest = new string[args.Length - 2];
                    Array.Copy(args, 2, rest, 0, args.Length - 2);
                    title = args[1];
                    if (string.IsNullOrEmpty(title)) {
                        ShowUsage();
                        return;
                    }
                    break;
                }
                case Load:
                case Remove:
                case Exist:
                case Title: {
                    if (args.Length < 2) {
                        ShowUsage();
                        return;
                    }
                    rest = new string[args.Length - 2];
                    Array.Copy(args, 2, rest, 0, args.Length - 2);
                    memoId = GetMemoId(args[1]);
                    if (string.IsNullOrEmpty(memoId)) {
                        ShowUsage();
                        return;
                    }
                    break;
                }
            }

            var needActivate = false;
            var needStart = false;
            var secondsForWait = 20;
            foreach (var arg in rest) {
                if (arg.Equals("-a")) {
                    needActivate = true;
                } else if (arg.StartsWith("-s")) {
                    if (arg.Length > 3) {
                        var s = arg.Substring(3);
                        try {
                            var seconds = int.Parse(s);
                            secondsForWait = seconds;
                        } catch (Exception) {
                            continue;
                        }
                        needStart = true;
                    } else {
                        needStart = true;
                    }
                }
            }


            /// check server
            var facade = GetFacade();
            if (facade == null) {
                WriteLine("Failed to initialize", console);
                return;
            }

            if (!IsAlive(facade)) {
                if (!needStart) {
                    WriteLine("Failed to connect", console);
                    return;
                }

                Process.Start(TargetExeFilename);

                var seconds = 0;
                while (!IsAlive(facade)) {
                    if (seconds >= secondsForWait) {
                        break;
                    }
                    Thread.Sleep(1000);
                    ++seconds;
                }

                if (!IsAlive(facade)) {
                    WriteLine("Failed to start MochaNote", console);
                    return;
                }
            }

            /// request
            switch (method) {
                case Create: {
                    var ret = facade.CreateMemo(title);
                    WriteLine(ret == null ? "false" : ("memoUri=memo:///" + ret), console);
                    if (needActivate) {
                        ActivateMochaNote();
                    }
                    break;
                }
                case Load: {
                    var ret = facade.LoadMemo(memoId);
                    WriteLine(ret ? "true" : "false", console);
                    if (needActivate) {
                        ActivateMochaNote();
                    }
                    break;
                }
                case Remove: {
                    var ret = facade.RemoveMemo(memoId);
                    WriteLine(ret ? "true" : "false", console);
                    if (needActivate) {
                        ActivateMochaNote();
                    }
                    break;
                }
                case Uris: {
                    var ret = facade.GetMemoIds();
                    if (ret == null) {
                        WriteLine("false", console);
                    } else {
                        foreach (var id in ret) {
                            WriteLine("memoUri=memo:///" + id, console);
                        }
                    }
                    if (needActivate) {
                        ActivateMochaNote();
                    }
                    break;
                }
                case Exist: {
                    var ret = facade.ExistsMemo(memoId);
                    WriteLine(ret ? "true" : "false", console);
                    if (needActivate) {
                        ActivateMochaNote();
                    }
                    break;
                }
                case Title: {
                    var ret = facade.GetMemoTitle(memoId);
                    WriteLine(ret == null ? "false" : ("title=" + ret), console);
                    if (needActivate) {
                        ActivateMochaNote();
                    }
                    break;
                }
            }
        }

        private static void ActivateMochaNote() {
            User32Util.ActivateWindow(null, TargetWindowTitle, TargetExeFilename);
        }

        private static bool IsAlive(IMemopadRemoteFacade facade) {
            try {
                facade.Ping();
                return true;
            } catch (Exception) {
                return false;
            }
        }

        private static IMemopadRemoteFacade GetFacade() {
            try {
                var client = new IpcClientChannel();
                ChannelServices.RegisterChannel(client, true);
                var ret = (IMemopadRemoteFacade) Activator.GetObject(
                    typeof(MemopadRemoteFacadeProxy),
                    "ipc://mochanote/facade"
                );
                return ret;
            } catch (Exception) {
                return null;
            }
        }

        private static void ShowUsage() {
            Console.WriteLine("Usage:");
            Console.WriteLine("  MochaNoteClient.exe <method and args> [options]");
            Console.WriteLine();

            Console.WriteLine("  <method and args> are");
            Console.WriteLine("    create <title>: create a new memo");
            Console.WriteLine("    load <memoUri>: load a memo");
            Console.WriteLine("    remove <memoUri>: remove a memo");
            Console.WriteLine("    uris: query all memo URIs");
            Console.WriteLine("    exist <memoUri>: query existence of a memo");
            Console.WriteLine("    title <memoUri>: query a title of a memo");
            Console.WriteLine();

            Console.WriteLine("    <title> is a string that is a title of a creating memo");
            Console.WriteLine("    <memoUri> is a string that was returned on creating or querying memo URIs");
            Console.WriteLine();

            Console.WriteLine("  [options] are");
            Console.WriteLine("    -a: activate MochaNote window");
            Console.WriteLine("    -s[=seconds]: start MochaNote if not started and wait for seconds.");
            Console.WriteLine("                  If seconds omitted, wait for 20 seconds");
            Console.WriteLine();

            Console.WriteLine("  Example:");
            Console.WriteLine("    MochaNoteClient.exe create \"My memo\"");
            Console.WriteLine("      create a memo titled \"My memo\"");
            Console.WriteLine("    MochaNoteClient.exe load MEMO_URI");
            Console.WriteLine("      load a memo specified by MEMO_URI");
            Console.WriteLine("    MochaNoteClient.exe create \"My Memo\" -s=30 -a");
            Console.WriteLine("      start MochaNote if not started, and create a memo, and activate");

            Console.WriteLine();
            Console.WriteLine("Copyright (c) 2010-2016 mocha All rights reserved.");
        }

        private static void WriteLine(string s, bool console) {
            if (console) {
                Console.WriteLine(s);
            }
        }
    }
}
