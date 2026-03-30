namespace FacturationTnAppWinForms
{
    internal static class Program
    {
        [System.STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
        }
    }
}
