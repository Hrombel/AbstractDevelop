﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AbstractDevelop.Properties {
    using System;
    
    
    /// <summary>
    ///   Класс ресурса со строгой типизацией для поиска локализованных строк и т.д.
    /// </summary>
    // Этот класс создан автоматически классом StronglyTypedResourceBuilder
    // с помощью такого средства, как ResGen или Visual Studio.
    // Чтобы добавить или удалить член, измените файл .ResX и снова запустите ResGen
    // с параметром /str или перестройте свой проект VS.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Возвращает кэшированный экземпляр ResourceManager, использованный этим классом.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("AbstractDevelop.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Перезаписывает свойство CurrentUICulture текущего потока для всех
        ///   обращений к ресурсу с помощью этого класса ресурса со строгой типизацией.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на &lt;head&gt;
        ///&lt;/head&gt;
        ///&lt;body&gt;
        ///    &lt;div id=&quot;title&quot;&gt;
        ///        &lt;center&gt;&lt;h1&gt;Точки останова и пошаговое выполнение&lt;/h1&gt;&lt;/center&gt;
        ///    &lt;/div&gt;
        ///    &lt;hr /&gt;
        ///    &lt;div id=&quot;main&quot;&gt;
        ///        &lt;h2&gt;Пошаговое выполнение программы&lt;/h2&gt;
        ///        Все абстрактные вычислители поддерживают пошаговое выполнение программ. Для того чтобы войти в режим пошагового выполнения следует нажать кнопку
        ///        паузы во время выполнения программы. Кода абстрактный вычислитель входит в такой режим, то становятся доступными дополнительные функции. [остаток строки не уместился]&quot;;.
        /// </summary>
        internal static string BreakPointInfo {
            get {
                return ResourceManager.GetString("BreakPointInfo", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на &lt;head&gt;
        ///&lt;/head&gt;
        ///&lt;body&gt;
        ///    &lt;div id=&quot;title&quot;&gt;
        ///        &lt;center&gt;&lt;h1&gt;Окно отладки&lt;/h1&gt;&lt;/center&gt;
        ///    &lt;/div&gt;
        ///    &lt;hr /&gt;
        ///    &lt;div id=&quot;main&quot;&gt;
        ///        &lt;h2&gt;Отладочная информация&lt;/h2&gt;
        ///        В ходе своей работы абстрактные вычислители могут отображать информацию в консоли отладки. Это может быть как сообщение об ошибке трансляции исходного
        ///        текста, ошибка времени выполнения, так и выходные данные. Окно отладки устроено таким образом, чтобы максимально удобно отражать приходящую в него
        ///        информац [остаток строки не уместился]&quot;;.
        /// </summary>
        internal static string DebugInfo {
            get {
                return ResourceManager.GetString("DebugInfo", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Поиск локализованного ресурса типа System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap IconLogo {
            get {
                object obj = ResourceManager.GetObject("IconLogo", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Поиск локализованного ресурса типа System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap Logo {
            get {
                object obj = ResourceManager.GetObject("Logo", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Поиск локализованного ресурса типа System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap PauseBtn {
            get {
                object obj = ResourceManager.GetObject("PauseBtn", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Поиск локализованного ресурса типа System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap PlayBtn {
            get {
                object obj = ResourceManager.GetObject("PlayBtn", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на &lt;head&gt;
        ///&lt;/head&gt;
        ///&lt;body&gt;
        ///    &lt;div id=&quot;title&quot;&gt;
        ///        &lt;center&gt;&lt;h1&gt;Машина Поста&lt;/h1&gt;&lt;/center&gt;
        ///    &lt;/div&gt;
        ///    &lt;hr /&gt;
        ///    &lt;div id=&quot;main&quot;&gt;
        ///        &lt;h2&gt;Принцип работы&lt;/h2&gt;
        ///        Машина Поста состоит из каретки (или считывающей и записывающей головки) и разбитой на ячейки бесконечной в обе стороны ленты (см. пример ниже).
        ///        Каждая ячейка ленты может находиться в 2 состояниях — быть либо пустой, либо отмеченной.
        ///        За такт работы машины каретка может сдвинуться на одну позицию влево или вправо [остаток строки не уместился]&quot;;.
        /// </summary>
        internal static string PostInfo {
            get {
                return ResourceManager.GetString("PostInfo", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на &lt;head&gt;
        ///&lt;/head&gt;
        ///&lt;body&gt;
        ///    &lt;div id=&quot;title&quot;&gt;
        ///        &lt;center&gt;&lt;h1&gt;Создание проекта&lt;/h1&gt;&lt;/center&gt;
        ///    &lt;/div&gt;
        ///    &lt;hr /&gt;
        ///    &lt;div id=&quot;main&quot;&gt;
        ///        &lt;h2&gt;Выбор абстрактного вычислителя&lt;/h2&gt;
        ///        При запуске программы окно создания проекта открывается автоматически. Оно предоставляет выбор абстрактного вычислителя, под который создается проект.
        ///        При нажатии на соответствующие элементы меню справа будет отображаться вспомогательная информация о конкретном абстрактном вычислителе. Когда выбор сде [остаток строки не уместился]&quot;;.
        /// </summary>
        internal static string ProjectCreateInfo {
            get {
                return ResourceManager.GetString("ProjectCreateInfo", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на &lt;head&gt;
        ///&lt;/head&gt;
        ///&lt;body&gt;
        ///    &lt;div id=&quot;title&quot;&gt;
        ///        &lt;center&gt;&lt;h1&gt;Машина с бесконечными регистрами&lt;/h1&gt;&lt;/center&gt;
        ///    &lt;/div&gt;
        ///    &lt;hr /&gt;
        ///    &lt;div id=&quot;main&quot;&gt;
        ///        &lt;h2&gt;Принцип работы&lt;/h2&gt;
        ///        В данной программе реализованы модели классической машины с бесконечными регистрами(далее - МБР) и её модификации - параллельной машины с бескочеными регистрами(далее - ПМБР).
        ///        Обе вариации МБР имеют неограниченное количество пронумерованных от нуля до бесконечности регистров, способных хранить числа б [остаток строки не уместился]&quot;;.
        /// </summary>
        internal static string RegisterInfo {
            get {
                return ResourceManager.GetString("RegisterInfo", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Поиск локализованного ресурса типа System.Byte[].
        /// </summary>
        internal static byte[] ScintillaNET {
            get {
                object obj = ResourceManager.GetObject("ScintillaNET", resourceCulture);
                return ((byte[])(obj));
            }
        }
        
        /// <summary>
        ///   Поиск локализованного ресурса типа System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap StepBtn {
            get {
                object obj = ResourceManager.GetObject("StepBtn", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Поиск локализованного ресурса типа System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap StopBtn {
            get {
                object obj = ResourceManager.GetObject("StopBtn", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Поиск локализованного ресурса типа System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap TapeCell {
            get {
                object obj = ResourceManager.GetObject("TapeCell", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Поиск локализованного ресурса типа System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap TapeCellChecked {
            get {
                object obj = ResourceManager.GetObject("TapeCellChecked", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Поиск локализованного ресурса типа System.Drawing.Bitmap.
        /// </summary>
        internal static System.Drawing.Bitmap TapeCellFocused {
            get {
                object obj = ResourceManager.GetObject("TapeCellFocused", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на &lt;head&gt;
        ///&lt;/head&gt;
        ///&lt;body&gt;
        ///    &lt;div id=&quot;title&quot;&gt;
        ///        &lt;center&gt;&lt;h1&gt;Машина Тьюринга&lt;/h1&gt;&lt;/center&gt;
        ///    &lt;/div&gt;
        ///    &lt;hr /&gt;
        ///    &lt;div id=&quot;main&quot;&gt;
        ///        &lt;h2&gt;Принцип работы&lt;/h2&gt;
        ///        В данной программе реализована модель многоленточной машины Тьюринга с изменяемым количеством лент. То есть, данная реализация удовлетворяет требованиям
        ///        задач, предназначенных для решения как на классической машине Тьюринга, так и на многоленточной.
        ///        В состав машины Тьюринга входит неограниченная в обе стороны [остаток строки не уместился]&quot;;.
        /// </summary>
        internal static string TuringInfo {
            get {
                return ResourceManager.GetString("TuringInfo", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Ищет локализованную строку, похожую на &lt;head&gt;
        ///&lt;/head&gt;
        ///&lt;body&gt;
        ///    &lt;div id=&quot;title&quot;&gt;
        ///        &lt;center&gt;&lt;h1&gt;Рабочая среда&lt;/h1&gt;&lt;/center&gt;
        ///    &lt;/div&gt;
        ///    &lt;hr /&gt;
        ///    &lt;div id=&quot;main&quot;&gt;
        ///        &lt;h2&gt;Управление абстрактными вычислителями&lt;/h2&gt;
        ///        После создания проекта программа перейдет в рабочую среду и откроет созданный проект в новой вкладке. Всякий раз, когда создается или открывается
        ///        проект, он отображается в рабочей среде во вкладке. Это позволяет легко переключаться между ними. Чтобы закрыть вкладку, необходимо активировать ее,
        ///   [остаток строки не уместился]&quot;;.
        /// </summary>
        internal static string WorkEnvironmentInfo {
            get {
                return ResourceManager.GetString("WorkEnvironmentInfo", resourceCulture);
            }
        }
    }
}
