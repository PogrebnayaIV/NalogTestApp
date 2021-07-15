namespace NalogTestApp
{
    /// <summary>
    /// Результат теста
    /// </summary>
    public struct TestResult
    {
        /// <summary>
        /// True - тест прошел без ошибки
        /// </summary>
        public bool Valid { get; set; }

        /// <summary>
        /// Сообщение о корректном прохождении теста
        /// </summary>
        public string ValidText { get; set; }

        /// <summary>
        /// Сообщение об ошибке
        /// </summary>
        public string ErrorText { get; set; }

        /// <summary>
        /// Стек вызовов при ошибке
        /// </summary>
        public string ErrorStack { get; set; }
    }
}
