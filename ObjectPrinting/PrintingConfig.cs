using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace ObjectPrinting
{
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
                if (Config.SerializeTypes[objType].CultureInfo != null)
                {
                    if (objType == typeof(int))
                        return ((int)obj).ToString("c", Config.SerializeTypes[objType].CultureInfo) + Environment.NewLine;

                    if (objType == typeof(double))
                        return ((double)obj).ToString("c", Config.SerializeTypes[objType].CultureInfo) + Environment.NewLine;

                    if (objType == typeof(long))
                        return ((long)obj).ToString("c", Config.SerializeTypes[objType].CultureInfo) + Environment.NewLine;

                    if (objType == typeof(float))
                        return ((float)obj).ToString("c", Config.SerializeTypes[objType].CultureInfo) + Environment.NewLine;

                }
                if (Config.SerializeTypes[objType].Serialize != null)
                {
                    return Config.SerializeTypes[objType].Serialize(obj) + Environment.NewLine;
                }
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
                var name = propertyInfo.Name;

                if (Config.ExcludedProperties.Contains(name))
                    continue;

                if (Config.SerializeProperties.ContainsKey(name))
                    sb.Append(identation + name + " = " +
                        Config.SerializeProperties[name](propertyInfo.GetValue(obj)));
                else
                    sb.Append(identation + name + " = " +
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
            var memberName = GetMemberName(func);
            Config.ExcludedProperties.Add(memberName);
            return this;
        }


        public string GetMemberName<TPropType>(Expression<Func<TOwner, TPropType>> func)
        {
            var memberExpression = (MemberExpression)func.Body;
            return memberExpression.Member.Name;
        }

       
        public PropertyPrintingConfig<TOwner, TPropType> Printing<TPropType>()
        {
            return new PropertyPrintingConfig<TOwner, TPropType>(this, Config);
        }

        
        public PrintingConfig<TOwner> Printing<TPropType>(Expression<Func<TOwner, TPropType>> func, Func<TPropType, string> printing)
        {
            var memberName = GetMemberName(func);
            
            Config.SerializeProperties[memberName] = x => printing((TPropType) x);

            return this;
        }

    }
}