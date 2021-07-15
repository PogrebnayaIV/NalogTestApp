using Serilog;

namespace NalogTestApp
{
    class Program
    {
        /// <summary>
        /// Проверка на валидность результата теста и вывод в Log
        /// </summary>
        static bool Validate(TestResult value)
        {
            if (value.Valid)
            {
                Log.Logger.Information(value.ValidText);
                return true;
            }

            Log.Logger.Error(value.ErrorText);
            return false;
        }

        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("log/NalogTestApp.txt", rollingInterval: RollingInterval.Month)
                .CreateLogger();

            var test = new TestSite();
            TestResult result;
            try
            {
                Log.Logger.Information("====== Начало тестирования ======");

                result = test.OpenSite();
                if (!Validate(result))
                {
                    return;
                }

                result = test.PersonlCabinet();
                if (!Validate(result))
                {
                    return;
                }

                result = test.DemoVersion();
                if (!Validate(result))
                {
                    return;
                }

                result = test.CheckPhoto();
                if (!Validate(result))
                {
                    return;
                }

                result = test.CheckNalogSum();
                if (!Validate(result))
                {
                    return;
                }
            }
            finally
            {
                result = test.CloseSite();
                Validate(result);
            }
           
        }
    }
}
