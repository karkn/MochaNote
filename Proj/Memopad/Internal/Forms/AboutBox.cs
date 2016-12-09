/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Text;
using Mkamo.Common.Forms.Themes;
using Mkamo.Memopad.Internal.Core;

namespace Mkamo.Memopad.Internal.Forms {
    internal partial class AboutBox: Form {
        // ========================================
        // field
        // ========================================
        private MemopadApplication _facade;

        // ========================================
        // constructor
        // ========================================
        public AboutBox() {
            _facade = MemopadApplication.Instance;

            InitializeComponent();

            //var ver = _facade.Settings.Version;
            var ver = "1.0.0";

            Text = String.Format("MochaNoteのバージョン情報");
            _productNameLabel.Text = "MochaNote Version " + ver;
            _copyrightLabel.Text = "Copyright (c) 2010-2016 mocha All rights reserved.";
            _descriptionTextBox.Text = GetDescription();
        }

        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================
        private string GetDescription() {
            var ret = new StringBuilder();

            ret.AppendLine("MochaNoteでは以下のフリーソフトウェアを利用しています。");
            ret.AppendLine();

            ret.AppendLine("ライブラリ");
            ret.AppendLine();

            /// Apache Software Foundation License 2.0
            /// http://www.castleproject.org/
            ret.AppendLine("  Castle.Core.dll");
            ret.AppendLine("    Castle Project");
            ret.AppendLine("    http://www.castleproject.org/");
            ret.AppendLine();

            /// 独自ライセンス
            /// C:\Program Files\Component Factory\Krypton Toolkit 4.1.6\Bin\EULA Krypton Toolkit Free.rtf
            ret.AppendLine("  ComponentFactory.Krypton.Toolkit.dll");
            ret.AppendLine("    Component Factory");
            ret.AppendLine("    http://www.componentfactory.com/");
            ret.AppendLine();

            /// Apache License, Version 2.0
            /// http://logging.apache.org/log4net/license.html
            ret.AppendLine("  log4net.dll");
            ret.AppendLine("    Apache Software Foundation");
            ret.AppendLine("    http://logging.apache.org/log4net/");
            ret.AppendLine();

            /// License:  Microsoft Public License (Ms-PL)
            /// http://dotnetzip.codeplex.com/license
            //ret.AppendLine("  Ionic.Zip.Reduced.dll");
            //ret.AppendLine("    Cheeso, Jaans");
            //ret.AppendLine("    http://dotnetzip.codeplex.com/");
            //ret.AppendLine();

            /// License:  Microsoft Public License (Ms-PL)
            /// 
            ret.AppendLine("  HtmlAgilityPack.dll");
            ret.AppendLine("    simonm, DarthObiwan, Jessynoo");
            ret.AppendLine("    http://htmlagilitypack.codeplex.com/");
            ret.AppendLine();

            /// License:  MIT
            /// http://www.codeproject.com/KB/database/CsvReader.aspx
            ret.AppendLine("  CSV Reader");
            ret.AppendLine("    Sebastien Lorion");
            ret.AppendLine("    http://www.codeproject.com/KB/database/CsvReader.aspx");
            ret.AppendLine();

            ret.AppendLine("アイコン");
            ret.AppendLine();

            ret.AppendLine("  Diagona Icons");
            ret.AppendLine("    Yusuke Kamiyamane");
            ret.AppendLine("    http://p.yusukekamiyamane.com/");
            ret.AppendLine();

            ret.AppendLine("  Silk icon set");
            ret.AppendLine("    Mark James");
            ret.AppendLine("    http://www.famfamfam.com/lab/icons/silk/");
            ret.AppendLine();

            ret.AppendLine("  FatCow Free Web Icons");
            ret.AppendLine("    FatCow Web Hosting");
            ret.AppendLine("    http://www.fatcow.com/free-icons/");
            ret.AppendLine();

            ret.AppendLine("  Fugue Icons");
            ret.AppendLine("    Yusuke Kamiyamane");
            ret.AppendLine("    http://p.yusukekamiyamane.com/");
            ret.AppendLine();

            /// filter
            ret.AppendLine("  Eclipse");
            ret.AppendLine("    Eclipse Project");
            ret.AppendLine("    http://www.eclipse.org/");
            ret.AppendLine();

            /// checkbox
            ret.AppendLine("  UIDesign Icons");
            ret.AppendLine("    Alexey Egorov");
            ret.AppendLine("    http://burlesck.livejournal.com/");
            
            return ret.ToString();
        }



        #region アセンブリ属性アクセサ

        public string AssemblyTitle {
            get {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0) {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute) attributes[0];
                    if (titleAttribute.Title != "") {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public string AssemblyVersion {
            get {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public string AssemblyDescription {
            get {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length == 0) {
                    return "";
                }
                return ((AssemblyDescriptionAttribute) attributes[0]).Description;
            }
        }

        public string AssemblyProduct {
            get {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0) {
                    return "";
                }
                return ((AssemblyProductAttribute) attributes[0]).Product;
            }
        }

        public string AssemblyCopyright {
            get {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0) {
                    return "";
                }
                return ((AssemblyCopyrightAttribute) attributes[0]).Copyright;
            }
        }

        public string AssemblyCompany {
            get {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (attributes.Length == 0) {
                    return "";
                }
                return ((AssemblyCompanyAttribute) attributes[0]).Company;
            }
        }
        #endregion

    }
}
