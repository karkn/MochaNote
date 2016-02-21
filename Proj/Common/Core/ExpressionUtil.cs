/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Linq.Expressions;

namespace Mkamo.Common.Core {
    public delegate T ObjectActivator<T>(params object[] args);

    public static class ExpressionUtil {
        // ========================================
        // static method
        // ========================================
        public static ObjectActivator<T> GetActivator<T>(ConstructorInfo ctor) {
            Type type = ctor.DeclaringType;
            ParameterInfo[] paramsInfo = ctor.GetParameters();

            //create a single param of type object[]
            ParameterExpression param = Expression.Parameter(typeof(object[]), "args");

            Expression[] argsExp = new Expression[paramsInfo.Length];

            //pick each arg from the params array 
            //and create a typed expression of them
            for (int i = 0; i < paramsInfo.Length; i++) {
                Expression index = Expression.Constant(i);
                Type paramType = paramsInfo[i].ParameterType;

                Expression paramAccessorExp = Expression.ArrayIndex(param, index);

                Expression paramCastExp = Expression.Convert(paramAccessorExp, paramType);

                argsExp[i] = paramCastExp;
            }

            //make a NewExpression that calls the
            //ctor with the args we just created
            NewExpression newExp = Expression.New(ctor, argsExp);

            //create a lambda with the New
            //Expression as body and our param object[] as arg
            LambdaExpression lambda = Expression.Lambda(typeof(ObjectActivator<T>), newExp, param);

            //compile it
            ObjectActivator<T> compiled = (ObjectActivator<T>) lambda.Compile();
            return compiled;
        }


        # region Property
        public static Func<object, T> CallPropertyGetter<T>(this PropertyInfo propertyInfo) {
            if (!propertyInfo.CanRead || propertyInfo.GetGetMethod() == null) return null;

            ParameterExpression instance = Expression.Parameter(typeof(object), "value");

            LambdaExpression expression = Expression.Lambda(Expression.Convert(
                                                               Expression.Property(
                                                                   Expression.ConvertChecked(instance, propertyInfo.DeclaringType),
                                                               propertyInfo),
                                                            typeof(T)), instance);

            return (Func<object, T>)expression.Compile();

        }

        public static Action<object, T> CallPropertySetter<T>(this PropertyInfo propertyInfo) {
            if (!propertyInfo.CanWrite || propertyInfo.GetSetMethod() == null) return null;

            ParameterExpression returnParameter = Expression.Parameter(typeof(object), "return");
            ParameterExpression valueArgument = Expression.Parameter(typeof(T), "value");

            MethodCallExpression setterCall = Expression.Call(
                                          Expression.ConvertChecked(returnParameter, propertyInfo.DeclaringType),
                                          propertyInfo.GetSetMethod(),
                                          Expression.Convert(valueArgument, propertyInfo.PropertyType));
            return Expression.Lambda<Action<object, T>>(setterCall, returnParameter, valueArgument).Compile();
        }
        # endregion

        # region Field
        public static Func<object, T> CallFieldGetter<T>(this FieldInfo fieldInfo) {
            if (!fieldInfo.IsPublic) return null;

            ParameterExpression instance = Expression.Parameter(typeof(object), "value");

            LambdaExpression expression = Expression.Lambda(Expression.Convert(
                                                               Expression.Field(
                                                                   Expression.ConvertChecked(instance, fieldInfo.DeclaringType),
                                                               fieldInfo),
                                                            typeof(T)), instance);

            return (Func<object, T>)expression.Compile();
        }
        
        //public static Action<object, T> CallFieldSetter<T>(this FieldInfo fieldInfo) {
        //    if (!fieldInfo.IsPublic || fieldInfo.IsInitOnly) return null;
        //    ParameterExpression returnParameter = Expression.Parameter(typeof(object), "value");
        //    ParameterExpression valueArgument = Expression.Parameter(typeof(T), "argument");

        //    var setter = Expression.Assign(
        //                    Expression.Field(Expression.ConvertChecked(returnParameter, fieldInfo.DeclaringType), fieldInfo),
        //                        Expression.Convert(valueArgument, fieldInfo.FieldType));
        //    return Expression.Lambda<Action<object, T>>(setter, returnParameter, valueArgument).Compile();
        //}
        # endregion

        # region Method
        public static Action<object, T> CallMethod<T>(this MethodInfo methodInfo)
        {
            if (!methodInfo.IsPublic) return null;

            ParameterExpression returnParameter = Expression.Parameter(typeof(object), "method");
            ParameterExpression valueArgument = Expression.Parameter(typeof(T), "argument");

            MethodCallExpression setterCall = Expression.Call(
                                          Expression.ConvertChecked(returnParameter, methodInfo.DeclaringType),
                                          methodInfo,
                                          Expression.Convert(valueArgument, typeof(T)));
            return Expression.Lambda<Action<object, T>>(setterCall, returnParameter, valueArgument).Compile();
        }
        # endregion

    }
}
