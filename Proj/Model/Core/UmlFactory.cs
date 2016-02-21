/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Model.Uml;
using Mkamo.Container.Core;

namespace Mkamo.Model.Core {
    public static class UmlFactory {
        // ========================================
        // static field
        // ========================================
        public static void Remove(object entity) {
            GetContainer().Remove(entity);
        }


        public static UmlAssociation CreateAssociation() {
            return GetContainer().Create<UmlAssociation>();
        }

        public static UmlClass CreateClass() {
            return GetContainer().Create<UmlClass>();
        }

        public static UmlDependency CreateDependency() {
            return GetContainer().Create<UmlDependency>();
        }

        public static UmlGeneralization CreateGeneralization() {
            return GetContainer().Create<UmlGeneralization>();
        }

        public static UmlInterface CreateInterface() {
            return GetContainer().Create<UmlInterface>();
        }

        public static UmlInterfaceRealization CreateInterfaceRealization() {
            return GetContainer().Create<UmlInterfaceRealization>();
        }

        public static UmlOperation CreateOperation() {
            return GetContainer().Create<UmlOperation>();
        }

        public static UmlPackage CreatePackage() {
            return GetContainer().Create<UmlPackage>();
        }

        //public static UmlParameter CreateParameter() {
        //    return GetContainer().Create<UmlParameter>();
        //}

        public static UmlProperty CreateProperty() {
            return GetContainer().Create<UmlProperty>();
        }

        public static UmlUsage CreateUsage() {
            return GetContainer().Create<UmlUsage>();
        }

        // ------------------------------
        // private
        // ------------------------------
        private static IEntityContainer GetContainer() {
            return ContainerFactory.GetContainer();
        }
    }
}
