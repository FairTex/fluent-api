using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using NUnit.Framework;

namespace ObjectPrinting
{
    public class Config
    {
        public HashSet<Type> ExcludedTypes { get; set; } = new HashSet<Type>();
        public HashSet<string> ExcludedProperties { get; set; } = new HashSet<string>();

        public Dictionary<object, Func<object, string>> SerializeTypes { get; set; } = new Dictionary<object, Func<object, string>>();
        public Dictionary<string, Func<object, string>> SerializeProperties { get; set; } = new Dictionary<string, Func<object, string>>();

        public CultureInfo CultureInfoForInt { get; set; }
        public string MemberName { get; set; }
    }

    public class PrintingConfig<TOwner>
    {
        public Config Config = new Config();

        public string PrintToString(TOwner obj)
        {
            return PrintToString(obj, 0);
        }

        private string PrintToString(object obj, int nestingLevel)
        {
            // Не знаю как определить имя obj, чтобы воспользоваться словарем SerializeProperties
            if (obj == null)
                return "null" + Environment.NewLine;

            var objType = obj.GetType();

            if (Config.SerializeTypes.ContainsKey(objType))
            {
                return Config.SerializeTypes[objType](obj) + Environment.NewLine;
            }

            if (objType == typeof(int) && Config.CultureInfoForInt != null)
            {
                return ((int)obj).ToString("c", Config.CultureInfoForInt) + Environment.NewLine;
            }

            if (Config.ExcludedTypes.Contains(objType))
            {
                return "" + Environment.NewLine;
            }
            

            var finalTypes = new[]
            {
                typeof(int), typeof(double), typeof(float), typeof(string),
                typeof(DateTime), typeof(TimeSpan)
            };
            if (finalTypes.Contains(objType))
                return obj + Environment.NewLine;

            var identation = new string('\t', nestingLevel + 1);
            var sb = new StringBuilder();
            var type = obj.GetType();
            sb.AppendLine(type.Name);
            foreach (var propertyInfo in type.GetProperties())
            {
                sb.Append(identation + propertyInfo.Name + " = " +
                          PrintToString(propertyInfo.GetValue(obj),
                              nestingLevel + 1));
            }
            return sb.ToString();
        }

        public PrintingConfig<TOwner> ExcludeType<TPropType>()
        {
            Config.ExcludedTypes.Add(typeof(TPropType));
            return this;
        }

        public PrintingConfig<TOwner> ExcludeProperty<TPropType>(Expression<Func<TOwner, TPropType>> func)
        {
            var memberExpression = (MemberExpression) func.Body;
            var memberName = memberExpression.Member.Name;
            Config.ExcludedProperties.Add(memberName);
            return this;
        }

        // Возвращаем контекст для свойства типа TPropType
        public PropertyPrintingConfig<TOwner, TPropType> Printing<TPropType>()
        {
            return new PropertyPrintingConfig<TOwner, TPropType>(this, Config);
        }

        
        public PropertyPrintingConfig<TOwner, TPropType> Printing<TPropType>(Expression<Func<TOwner, TPropType>> func)
        {
            var memberExpression = (MemberExpression) func.Body;
            Config.MemberName = memberExpression.Member.Name;

            return new PropertyPrintingConfig<TOwner, TPropType>(this, Config);
        }

    }   

    public class PropertyPrintingConfig<TOwner, TPropType>
    {
        public PrintingConfig<TOwner> PrintingConfig;
        public Config Config;
        public PropertyPrintingConfig(PrintingConfig<TOwner> printingConfig, Config config)
        {
            PrintingConfig = printingConfig;
            Config = config;
        }

        // Указываем конкретный способ сериализации для свойства типа TPropType
        public PrintingConfig<TOwner> Using(Func<TPropType, string> func)
        {
            if (Config.MemberName != null)
            {
                if (!Config.SerializeProperties.ContainsKey(Config.MemberName))
                    Config.SerializeProperties.Add(Config.MemberName, x => func((TPropType) x));
            }
            else
            {
                if (!Config.SerializeTypes.ContainsKey(typeof(TPropType)))
                    Config.SerializeTypes.Add(typeof(TPropType), x => func((TPropType)x));
            }

            return PrintingConfig;
        }
    }

    public static class PropertyPrintingConfigExtension
    {
        // Метод расширения для типа int
        public static PrintingConfig<TOwner> UseCulture<TOwner>(this PropertyPrintingConfig<TOwner, int> p,
            CultureInfo cultureInfo)
        {
            p.Config.CultureInfoForInt = cultureInfo;
            return p.PrintingConfig;
        }

        // Метод расширения для типа string
        public static PrintingConfig<TOwner> Cut<TOwner>(this PropertyPrintingConfig<TOwner, string> p, 
            int count)
        {
            p.Config.SerializeTypes.Add(typeof(string), x => ((string)x).Substring(0, Math.Min(count, ((string)x).Length)));
            return p.PrintingConfig;
        }
    }
}