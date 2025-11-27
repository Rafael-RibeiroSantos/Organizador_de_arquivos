namespace OrganizadorDeArquivos;

//define a classe principal do app (não pode ser instanciada).
static class Program
{
    //obrigatório em apps de interface Windows; define o modelo de threading para UI.
    [STAThread]

    //é o método principal, o que o Windows executa ao abrir o .exe.
    static void Main()
    {
        //aplica configurações padrão (tema, DPI, etc.).
        ApplicationConfiguration.Initialize();
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.SetHighDpiMode(HighDpiMode.SystemAware);

        //abre a janela principal do aplicativo (Form1).
        try
        {
            Application.Run(new Form1());
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString(), "Erro ao iniciar");
        }

    }
}