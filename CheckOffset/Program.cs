namespace CheckOffset
{
    internal static class Program
    {
        public static For_Main? s_For_Main;

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            tnGlobal.Initialize();

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            s_For_Main = new For_Main();
            Application.Run(s_For_Main);
        }
    }
}