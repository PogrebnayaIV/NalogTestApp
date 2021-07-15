using System;
using System.Globalization;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Serilog;

namespace NalogTestApp
{
    /// <summary>
    /// Класс тестирования
    /// </summary>
    public class TestSite
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        public TestSite()
        {
            Driver = new ChromeDriver();
            Driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(20);
            Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(20);
            Driver.Manage().Timeouts().AsynchronousJavaScript = TimeSpan.FromSeconds(20);
        }

        /// <summary>
        /// Драйвер для доступа к браузеру
        /// </summary>
        public IWebDriver Driver { get; set; }

        private IWebElement _webElementName;

        /// <summary>
        /// Открытие сайта: https://www.nalog.ru
        /// </summary>
        public TestResult OpenSite()
        {
            const string titlePage = "Федеральная налоговая служба"; 
            try
            {
                Driver.Navigate().GoToUrl("https://www.nalog.ru");
                if (Driver.Title == titlePage) 
                    Log.Logger.Information("Открыта страница " + titlePage);
                else 
                    Log.Logger.Error("Открыта страница " + Driver.Title + " Ожидаемый результат: " + titlePage);
            }
            catch(Exception e)
            {
                return new TestResult {Valid = false,  ErrorText = "Ошибка открытия сайта: " + e.Message, ErrorStack = e.StackTrace};
            }

            return new TestResult {Valid = true, ValidText = "Сайт успешно открыт"};
        }

        /// <summary>
        /// На странице выполнить поиск элемента «Личный кабинет» для физических лиц, выполнить операцию Click для перехода в личный кабинет
        /// </summary>
        public TestResult PersonlCabinet()
        {
            const string titlePage = "Личный кабинет налогоплательщика — физического лица"; 
            try
            {
                var element = Driver.FindElement(By.ClassName("main-menu__top_individual")).FindElement(By.ClassName("main-menu__enter"));
                element.Click();
                if (Driver.Title == titlePage) 
                    Log.Logger.Information("Открыта страница " + titlePage);
                else 
                    Log.Logger.Error("Открыта страница " + Driver.Title  + " Ожидаемый результат: " + titlePage);
            }
            catch (Exception e)
            {
                return new TestResult { Valid = false, ErrorText = "Ошибка открытия личного кабинета: " + e.Message, ErrorStack = e.StackTrace };
            }

            return new TestResult { Valid = true, ValidText = "Личный кабинет открыт успешно" };
        }

        /// <summary>
        /// На странице входа в личный кабинет произвести нажатие на элемент «ДЕМО-ВЕРСИЯ»
        /// </summary>
        public TestResult DemoVersion()
        {
            const string nameDemo= "Иванов Иван Иванович";
            try
            {
                var element = Driver.FindElement(By.LinkText("ДЕМО-ВЕРСИЯ"));
                element.Click();

                _webElementName = Driver.FindElement(By.ClassName("src-modules-App-components-UserInfo-UserInfo-module__text"));
                if (!_webElementName.Text.Contains(nameDemo)) 
                    Log.Logger.Warning("Имя пользователя Демо-версии: " + _webElementName.Text + " Ожидаемый результат: " + nameDemo);
            }
            catch (Exception e)
            {
                return new TestResult { Valid = false, ErrorText = "Ошибка при переходе в Демо-версию: " + e.Message, ErrorStack = e.StackTrace };
            }
            return new TestResult { Valid = true, ValidText = "Выполнен переход в Демо-версию и пользователь " + _webElementName.Text };
        }

        /// <summary>
        /// Проверить, что в личном кабинете, в разделе «Footer» загрузилась фотография профиля пользователя, размер которой равен 31*31.
        /// </summary>
        public TestResult CheckPhoto()
        {
            bool flag;
            const int sizePhoto = 31;
            try
            {
                _webElementName.Click();

                IWebElement webElementPersonal = Driver.FindElement(By.CssSelector("#react-tabs-2"));
                webElementPersonal.Click();

                var webElementProfile = Driver.FindElement(By.Id("react-tabs-3")).FindElement(By.TagName("img"));
                var photoProfile = webElementProfile.GetProperty("src");

                Driver.Navigate().Back();
                Driver.Navigate().Back();

                var headerElementsA = Driver.FindElement(By.ClassName("header__main")).FindElements(By.TagName("a"));
                var headerElementsImg = Driver.FindElement(By.ClassName("header__main")).FindElements(By.TagName("img"));

                flag = false;
                var widthPhoto = 0;
                var heightPhoto = 0;

                var count = headerElementsA.Count;
                var i = 0;
                while ((i < count) && (!flag) && (widthPhoto != sizePhoto) && (heightPhoto != sizePhoto))
                {
                    try
                    {
                        var style = headerElementsA[i].GetAttribute("style");
                        if (!(string.IsNullOrEmpty(style)) && style.Contains(photoProfile))
                        {
                            flag = true;
                            widthPhoto = headerElementsA[i].Size.Width;
                            heightPhoto = headerElementsA[i].Size.Height;
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Logger.Warning("Поиск Фото на Header: " + e.Message);
                    }

                    i++;
                }

                if ((!flag) || (widthPhoto != sizePhoto) || (heightPhoto != sizePhoto))
                {
                    count = headerElementsImg.Count;
                    i = 0;
                    while ((i < count) && (!flag) && (widthPhoto != sizePhoto) && (heightPhoto != sizePhoto))
                    {
                        try
                        {
                            var src = headerElementsImg[i].GetAttribute("src");
                            if (!(string.IsNullOrEmpty(src)) && src.Contains(photoProfile))
                            {
                                flag = true;
                                widthPhoto = headerElementsImg[i].Size.Width;
                                heightPhoto = headerElementsImg[i].Size.Height;
                            }
                        }
                        catch (Exception e)
                        {
                            Log.Logger.Warning("Поиск Фото на Header: " + e.Message);
                        }

                        i++;
                    }

                }

                if (flag) 
                    Log.Logger.Information("Фото на Header и размер: Ширина = " + widthPhoto + " Высота = " + heightPhoto);
                else 
                    Log.Logger.Error("Фото на Header нет");

                flag = false;
                widthPhoto = 0;
                heightPhoto = 0;

                var footerElementsA = Driver.FindElement(By.ClassName("footer__main")).FindElements(By.TagName("a"));
                var footerElementsImg = Driver.FindElement(By.ClassName("footer__main")).FindElements(By.TagName("img"));

                count = footerElementsA.Count;
                i = 0;
                while ((i < count) && (!flag) && (widthPhoto != sizePhoto) && (heightPhoto != sizePhoto))
                {
                    try
                    {
                        var style = footerElementsA[i].GetAttribute("style");
                        if (!(string.IsNullOrEmpty(style)) && style.Contains(photoProfile))
                        {
                            flag = true;
                            widthPhoto = footerElementsA[i].Size.Width;
                            heightPhoto = footerElementsA[i].Size.Height;
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Logger.Warning("Поиск Фото на Footer: " + e.Message);
                    }

                    i++;
                }

                if ((!flag) || (widthPhoto != sizePhoto) || (heightPhoto != sizePhoto))
                {
                    count = footerElementsImg.Count;
                    i = 0;
                    while ((i < count) && (!flag) && (widthPhoto != sizePhoto) && (heightPhoto != sizePhoto))
                    {
                        try
                        {
                            var src = footerElementsImg[i].GetAttribute("src");
                            if (!(string.IsNullOrEmpty(src)) && src.Contains(photoProfile))
                            {
                                flag = true;
                                widthPhoto = footerElementsImg[i].Size.Width;
                                heightPhoto = footerElementsImg[i].Size.Height;
                            }
                        }
                        catch (Exception e)
                        {
                            Log.Logger.Warning("Поиск Фото на Footer: " + e.Message);
                        }

                        i++;
                    }

                }

                if (flag) 
                    Log.Logger.Information("Фото на Footer и размер: Ширина = " + widthPhoto + " Высота = " + heightPhoto);
                else 
                    Log.Logger.Error("Фото на Footer нет");

            }
            catch (Exception e)
            {
                return new TestResult { Valid = false, ErrorText = "Ошибка при проверке Фото " + e.Message, ErrorStack = e.StackTrace };
            }
            return new TestResult { Valid = true, ValidText = "Фото " + (flag ? "найдено" : "не найдено")};
        }

        /// <summary>
        /// Проверить, что в личном кабинете, в основной части страницы, указана сумма к оплате не больше 200 000 р
        /// </summary>
        public TestResult CheckNalogSum()
        {
            const decimal maxNalogSum = 200000;
            decimal sum;
            try
            {
                IWebElement webElementSum = Driver.FindElement(By.ClassName("main-page_title_sum"));
                if (string.IsNullOrEmpty(webElementSum.Text)) 
                    return new TestResult { Valid = false, ErrorText = "Сумма налога не указана"};
                else 
                {
                    string sumStr = webElementSum.Text.Replace(" ", string.Empty);
                    sum = Convert.ToDecimal(sumStr, CultureInfo.InvariantCulture);
                }
            }
            catch (Exception e)
            {
                return new TestResult { Valid = false, ErrorText = "Ошибка при проверке суммы налога: " + e.Message, ErrorStack = e.StackTrace };
            }
            return new TestResult { Valid = true, 
                ValidText = "Сумма налога " + (sum < maxNalogSum ? "меньше " : "больше или равна ") + maxNalogSum + " и равна " + sum };
        }

        /// <summary>
        /// Закрытие сайта
        /// </summary>
        public TestResult CloseSite()
        {
            try
            {
                Driver.Quit();
                try
                {
                    var title = Driver.Title;
                    return new TestResult {Valid = false, ErrorText = "Процесс не завершен", ErrorStack = string.Empty};
                }
                catch
                {
                    // ignored
                }
            }
            catch (Exception e)
            {
                return new TestResult { Valid = false, ErrorText = "Ошибка закрытия сайта: " + e.Message, ErrorStack = e.StackTrace };
            }

            return new TestResult { Valid = true, ValidText = "Сайт успешно закрыт"};
        }
    }
}