using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ZoomlaHms.JsEvent
{
    public class EventImplement
    {
        private static readonly Type _providerBasedType = typeof(IJsEvent);
        private static readonly string[] MethodIgnore = new string[] { "Equals", "ReferenceEquals", "GetHashCode", "GetType", "ToString", "MemberwiseClone" };

        private List<ProviderMethod> providerMethods = new List<ProviderMethod>();
        private IJsEvent provider = null;

        public string Name { get; }
        public Type ProviderType { get; }

        public EventImplement(Type providerType)
        {
            if (!_providerBasedType.IsAssignableFrom(providerType) || providerType.IsAbstract || providerType.IsInterface || !providerType.IsPublic)
            {
                throw new ArgumentException($"Provider must inherit from {_providerBasedType.Name}.");
            }

            Name = providerType.Name.Replace(_providerBasedType.Name.Substring(1), string.Empty);
            ProviderType = providerType;

            foreach (var method in providerType.GetMethods())
            {
                if (!method.IsPublic || method.IsConstructor || method.IsStatic || method.IsAbstract)
                { continue; }
                if (MethodIgnore.Contains(method.Name))
                { continue; }

                providerMethods.Add(new ProviderMethod(method));
            }
        }

        public object Emit(string name, ParameterType[] parameterTypes, object[] parameters)
        {
            if (provider == null)
            { provider = Activator.CreateInstance(ProviderType) as IJsEvent; }

            var matches = providerMethods.Where(w => w.Name.ToLower() == name.ToLower() && w.ParameterTypes.Length == parameterTypes.Length);
            if (!matches.Any())
            {
                throw new ArgumentException($"Cannot found method '{name}'.");
            }

            ProviderMethod method = null;
            foreach (var item in matches)
            {
                bool pass = true;
                for (int i = 0; i < item.ParameterTypes.Length; i++)
                {
                    if (!pass)
                    { break; }

                    pass = parameterTypes[i] == item.ParameterTypes[i];
                }

                if (pass)
                {
                    method = item;
                    break;
                }
            }

            if (method == null)
            {
                throw new ArgumentException($"Parameters miss match. (method '{name}')");
            }

            return method.Source.Invoke(provider, parameters);
        }


        private class ProviderMethod
        {
            public string Name { get; }
            public ParameterType[] ParameterTypes { get; }
            public MethodInfo Source { get; }

            private static readonly Type IntType = typeof(int);
            private static readonly Type DoubleType = typeof(double);
            private static readonly Type StringType = typeof(string);
            private static readonly Type DateTimeType = typeof(DateTime);

            public ProviderMethod(MethodInfo methodInfo)
            {
                Source = methodInfo;
                Name = methodInfo.Name;
                
                var parameterInfos = methodInfo.GetParameters();
                ParameterTypes = new ParameterType[parameterInfos.Length];
                for (int i = 0; i < parameterInfos.Length; i++)
                {
                    var parameter = parameterInfos[i];
                    if (IntType.FullName.Equals(parameter.ParameterType.FullName))
                    { ParameterTypes[i] = ParameterType.Integer; }
                    else if (DoubleType.FullName.Equals(parameter.ParameterType.FullName))
                    { ParameterTypes[i] = ParameterType.Float; }
                    else if (StringType.FullName.Equals(parameter.ParameterType.FullName))
                    { ParameterTypes[i] = ParameterType.String; }
                    else if (DateTimeType.FullName.Equals(parameter.ParameterType.FullName))
                    { ParameterTypes[i] = ParameterType.DateTime; }
                    else
                    {
                        throw new NotSupportedException($"The parameter type '{parameter.ParameterType.FullName}' is not in the support list. (Method: {methodInfo.DeclaringType.FullName}.{methodInfo.Name})");
                    }
                }
            }
        }
    }
}
