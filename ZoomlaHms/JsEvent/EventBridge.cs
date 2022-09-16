using System;
using System.Collections.Generic;
using System.Configuration.Provider;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using ZoomlaHms.Common;

namespace ZoomlaHms.JsEvent
{
    public class EventBridge
    {
        private static readonly Type _implementBaseType = typeof(IJsEvent);
        private static readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        private List<EventImplement> implements = new List<EventImplement>();

        public static EventBridge Insatance { get; }

        static EventBridge()
        {
            Insatance = new EventBridge();
        }

        private EventBridge()
        {
            var list = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(f => f.GetTypes())
                .Where(w => _implementBaseType.IsAssignableFrom(w) && !w.IsAbstract && !w.IsInterface && w.IsPublic);
            foreach (var item in list)
            {
                implements.Add(new EventImplement(item));
            }
        }

        public void SingleModuleView(string url, string title)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                new SingleModule(url) { Title = title };
            });
        }

        public void Prompt(string message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (App.LastActivatedWindow == null)
                {
                    PromptBox.Show(Application.Current.MainWindow, message, cancel: false);
                    return;
                }

                PromptBox.Show(App.LastActivatedWindow, message, cancel: false);
            });
        }

        public object SayHello()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                PromptBox.Show(Application.Current.MainWindow, "hello");
            });
            return "hello";
        }

        public void OpenFile(string path)
        {
            if (string.IsNullOrEmpty(path))
            { return; }

            try
            {
                Logging.Info($"Open file '{path}'.");

                string arg = string.Empty;
                if (System.IO.Directory.Exists(path))
                { arg = path; }
                if (System.IO.File.Exists(path))
                { arg = $"/select,{path}"; }
                if (string.IsNullOrEmpty(arg))
                { return; }

                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("explorer")
                {
                    Arguments = arg,
                });
            }
            catch (Exception ex)
            {
                Logging.Error($"Cannot open file '{path}'.", ex);
            }
        }

        public void OpenUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
            { return; }

            try
            {
                Logging.Info($"Open url '{url}'.");
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(url)
                {
                    UseShellExecute = true,
                });
            }
            catch (Exception ex)
            {
                Logging.Error($"Cannot open url '{url}'.", ex);
            }
        }

        public object Call(string identity, params object[] parameters)
        {
            try
            {
                string[] names = (identity ?? string.Empty).Split(".", StringSplitOptions.RemoveEmptyEntries);
                if (names.Length != 2)
                {
                    string msg = $"Invalid identity: {identity}.";
                    Logging.Error(msg);
                    return "ERROR:" + msg;
                }

                var qProvider = implements.Where(w => w.Name.ToLower() == names[0].ToLower());
                if (!qProvider.Any())
                {
                    string msg = $"Provider miss match. (provider '{names[0]}')";
                    return "ERROR:" + msg;
                }

                var foat = PrepareParameters(parameters);
                var res = qProvider.First().Emit(names[1], foat.Item1, foat.Item2);
                if (res == null)
                { return res; }

                var resType = res.GetType();
                if (resType.IsValueType || typeof(string).IsAssignableFrom(resType))
                { return res; }

                return JsonSerializer.Serialize(res, _jsonSerializerOptions);
            }
            catch (Exception ex)
            {
                Logging.Error($"Unable to handle '{identity}' event.", ex);
                return $"ERROR:{ex.Message}";
            }
        }

        private Tuple<ParameterType[], object[]> PrepareParameters(object[] parameters)
        {
            ParameterType[] types = new ParameterType[parameters.Length];
            object[] values = new object[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                string paramStr = parameters[i]?.ToString();
                if (paramStr == null)
                {
                    types[i] = ParameterType.String;
                    values[i] = string.Empty;
                    continue;
                }

                if (paramStr.EndsWith("::string"))
                {
                    types[i] = ParameterType.String;
                    values[i] = paramStr.Substring(0, paramStr.Length - 8);
                    continue;
                }
                else if (paramStr.EndsWith("::double"))
                {
                    types[i] = ParameterType.Float;
                    values[i] = double.TryParse(paramStr.Substring(0, paramStr.Length - 8), out double v) ? v : 0;
                    continue;
                }

                if (DateTime.TryParse(paramStr.Trim(), out DateTime dateTime))
                {
                    types[i] = ParameterType.DateTime;
                    values[i] = dateTime;
                }
                else if (paramStr.Contains(".") && double.TryParse(paramStr.Trim(), out double doubleNum))
                {
                    types[i] = ParameterType.Float;
                    values[i] = doubleNum;
                }
                else if (int.TryParse(paramStr.Trim(), out int intNum))
                {
                    types[i] = ParameterType.Integer;
                    values[i] = intNum;
                }
                else
                {
                    types[i] = ParameterType.String;
                    values[i] = paramStr;
                }
            }

            return new Tuple<ParameterType[], object[]>(types, values);
        }
    }
}
