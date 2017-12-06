using System;
using System.Globalization;

namespace ObjectPrinting
{
    public static class PropertyPrintingConfigExtension
    {
        public static PrintingConfig<TOwner> UseCulture<TOwner>(this PropertyPrintingConfig<TOwner, int> p,
            CultureInfo cultureInfo)
        {
            if (!p.Config.SerializeTypes.ContainsKey(typeof(int)))
                p.Config.SerializeTypes[typeof(int)] = new TypeConfig();
            p.Config.SerializeTypes[typeof(int)].CultureInfo = cultureInfo;

            return p.PrintingConfig;
        }
        
        public static PrintingConfig<TOwner> Cut<TOwner>(this PropertyPrintingConfig<TOwner, string> p, 
            int count)
        {
            var type = typeof(string);
            if (!p.Config.SerializeTypes.ContainsKey(type))
                p.Config.SerializeTypes[type] = new TypeConfig();

            p.Config.SerializeTypes[type].Serialize =
                x => ((string)x).Substring(0, Math.Min(count, ((string)x).Length));

            return p.PrintingConfig;
        }
    }
}