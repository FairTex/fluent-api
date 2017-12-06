using System;
using System.Globalization;
using NUnit.Framework;

namespace ObjectPrinting.Tests
{
    [TestFixture]
    public class ObjectPrinterAcceptanceTests
    {
        [Test]
        public void Demo()
        {
            var person = new Person {Name = "Alex", Age = 19};

            var printer = ObjectPrinter.For<Person>()
                //1. Исключить из сериализации свойства определенного типа
                .ExcludeType<Guid>()
                //2. Указать альтернативный способ сериализации для определенного типа
                .Printing<Guid>()
                .Using(prop => "Guid")
                //3. Для числовых типов указать культуру
                .Printing<int>()
                .UseCulture(CultureInfo.CurrentCulture)
                //4. Настроить сериализацию конкретного свойства
                .Printing(p => p.Name, id => "NAME ZERO")
                //5. Настроить обрезание строковых свойств (метод должен быть виден только для строковых свойств)
                .Printing<string>()
                .Cut(100)
                //6. Исключить из сериализации конкретного свойства
                .ExcludeProperty(p => p.Age);

            string s1 = printer.PrintToString(person);

            var a = 1;

            //7. Синтаксический сахар в виде метода расширения, сериализующего по-умолчанию		
            //8. ...с конфигурированием
        }
    }
}