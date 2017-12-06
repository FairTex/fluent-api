using System;
using System.Collections.Generic;
using System.Globalization;

namespace ObjectPrinting
{
    public class Config
    {
        public AddOnlyCollection<Type> ExcludedTypes { get; set; } = new AddOnlyCollection<Type>();
        public AddOnlyCollection<string> ExcludedProperties { get; set; } = new AddOnlyCollection<string>();

        public Dictionary<object, TypeConfig> SerializeTypes { get; set; } = new Dictionary<object, TypeConfig>();
        public Dictionary<string, Func<object, string>> SerializeProperties { get; set; } = new Dictionary<string, Func<object, string>>();

        // public CultureInfo CultureInfoForInt { get; set; }

        public void InitializeFor<T>(Dictionary<Type, TypeConfig> dict)
        {
            var type = typeof(T);
            if (!dict.ContainsKey(type))
            {
                dict.Add(type, new TypeConfig());
            }
        }

    }

    public class TypeConfig
    {
        public Func<object, string> Serialize { get; set; }
        public CultureInfo CultureInfo { get; set; }
    }
}