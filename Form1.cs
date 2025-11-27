namespace OrganizadorDeArquivos;

public partial class Form1 : Form
{
    private Dictionary<string, List<string>> estruturaPreviewSimulada = new();
    private string? explorerGrupoAtual = null;
    private string explorerCaminhoAtual = string.Empty;
    private List<MoveAction> historicoMovimentos = new();

    public class MoveAction { public string? Source { get; set; } public string? Destination { get; set; } }

    public Form1()
    {
         this.AutoScaleMode = AutoScaleMode.Dpi;
        InitializeComponent();
        try
        {
            string iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resources", "icons", "organizando.ico");
            if (File.Exists(iconPath)) Icon = new Icon(iconPath);
        }
        catch { }
    }

    // ===== DESFAZER =====
    private void DesfazerOrganizacao()
    {
        if (!historicoMovimentos.Any()) { MessageBox.Show("Não há ações para desfazer."); return; }
        int erros = 0;

        foreach (var acao in historicoMovimentos.AsEnumerable().Reverse())
        {
            if (acao.Source == null || acao.Destination == null || !File.Exists(acao.Destination)) { erros++; continue; }
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(acao.Source)!);
                bool movido = false;
                for (int t = 0; t < 10; t++)
                {
                    try { File.Move(acao.Destination, acao.Source, true); movido = true; break; }
                    catch (IOException) { Thread.Sleep(150); }
                }
                if (!movido) { erros++; MessageBox.Show($"Não foi possível desfazer o arquivo:\n{acao.Destination}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            }
            catch (Exception ex) { erros++; MessageBox.Show($"Erro ao desfazer arquivo:\n{acao.Destination}\nMotivo: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        historicoMovimentos.Select(m => Path.GetDirectoryName(m.Destination)).Distinct().Where(p => p != null)
            .ToList().ForEach(p => { DeletarPastasVazias(p!); RemoverPastasPaiVazias(txtPasta.Text, p!); });

        MessageBox.Show(erros > 0 ? $"Desfazer concluído com alguns erros ({erros} arquivos)." : "Organização desfeita com sucesso!");
        historicoMovimentos.Clear();
    }

    private void DeletarPastasVazias(string caminho)
    {
        if (!Directory.Exists(caminho)) return;
        foreach (var sub in Directory.GetDirectories(caminho)) DeletarPastasVazias(sub);
        if (!Directory.EnumerateFileSystemEntries(caminho).Any()) try { Directory.Delete(caminho); } catch { }
    }

    private void RemoverPastasPaiVazias(string raiz, string caminho)
    {
        string? pasta = Path.GetDirectoryName(caminho);
        while (!string.IsNullOrEmpty(pasta) && pasta.StartsWith(raiz))
        {
            if (Directory.Exists(pasta) && !Directory.EnumerateFileSystemEntries(pasta).Any()) try { Directory.Delete(pasta); } catch { break; }
            pasta = Path.GetDirectoryName(pasta);
        }
    }

    // ===== ORGANIZAR =====
    private void OrganizarArquivos(string pasta, string criterio)
    {
        if (!Directory.Exists(pasta)) { MessageBox.Show("Pasta inválida!"); return; }
        foreach (var arquivo in Directory.GetFiles(pasta))
        {
            var info = new FileInfo(arquivo);
            string destino = criterio.ToLower() switch
            {
                "tipo" => info.Extension.ToLower() switch
                {
                    ".jpg" or ".jpeg" or ".png" or ".gif" or ".bmp" => Path.Combine(pasta, "Imagens"),
                    ".pdf" or ".docx" or ".txt" => Path.Combine(pasta, "Documentos"),
                    ".mp4" or ".mkv" => Path.Combine(pasta, "Vídeo"),
                    ".exe" => Path.Combine(pasta, "Programas"),
                    _ => Path.Combine(pasta, info.Extension.TrimStart('.').ToUpper())
                },
                "tamanho" => Path.Combine(pasta, info.Length switch
                {
                    <= 104_857_600 => "Pequenos",
                    <= 1_073_741_824 => "Médios",
                    _ => "Grandes"
                }),
                "data" => Path.Combine(pasta, info.LastWriteTime.ToString("yyyy\\MM\\dd")),
                _ => pasta
            };
            Directory.CreateDirectory(destino);
            string destFile = Path.Combine(destino, info.Name);
            File.Move(arquivo, destFile);
            historicoMovimentos.Add(new MoveAction { Source = arquivo, Destination = destFile });
        }
        MessageBox.Show("Arquivos organizados com sucesso!");
    }

    // ===== PRÉ-VISUALIZAÇÃO SIMULADA =====
    private Dictionary<string, List<string>> GerarPreVisualizacaoAgrupada(string pastaOrigem, string criterio)
    {
        var grupos = new Dictionary<string, List<string>>();
        foreach (var arquivo in Directory.GetFiles(pastaOrigem))
        {
            var info = new FileInfo(arquivo);
            string nomePasta = criterio.ToLower() switch
            {
                "tipo" => info.Extension.ToLower() switch
                {
                    ".jpg" or ".jpeg" or ".png" or ".gif" or ".bmp" => "Imagens",
                    ".pdf" or ".docx" or ".txt" => "Documentos",
                    ".mp4" or ".mkv" => "Video",
                    ".exe" => "Programas",
                    _ => "Outros"
                },
                "tamanho" => info.Length switch
                {
                    <= 104_857_600 => "Pequenos (até 100MB)",
                    <= 1_073_741_824 => "Médios (100MB-1GB)",
                    _ => "Grandes (>1GB)"
                },
                "data" => info.LastWriteTime.ToString("yyyy\\MM\\dd"),
                _ => "Outros"
            };
            if (!grupos.ContainsKey(nomePasta)) grupos[nomePasta] = new List<string>();
            grupos[nomePasta].Add(info.Name);
        }
        estruturaPreviewSimulada = grupos;
        return grupos;
    }

    private void CarregarExplorerSimulado(string grupo, ListView lstExplorer, Button btnVoltar)
    {
        lstExplorer.Items.Clear();
        explorerGrupoAtual = grupo;
        btnVoltar.Visible = true;
        if (!estruturaPreviewSimulada.ContainsKey(grupo)) return;
        estruturaPreviewSimulada[grupo].ForEach(f => lstExplorer.Items.Add(new ListViewItem(f) { Tag = f }));
    }
}
 