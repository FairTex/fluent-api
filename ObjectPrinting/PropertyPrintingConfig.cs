using System;

namespace ObjectPrinting
{
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
            if (!Config.SerializeTypes.ContainsKey(typeof(TPropType)))
                Config.SerializeTypes.Add(typeof(TPropType), new TypeConfig());

            Config.SerializeTypes[typeof(TPropType)].Serialize = x => func((TPropType) x);

            return PrintingConfig;
        }
    }
}