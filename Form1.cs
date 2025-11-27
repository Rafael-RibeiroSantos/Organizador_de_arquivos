namespace OrganizadorDeArquivos;

public partial class Form1 : Form
{
    // Dicionário usado para armazenar a pré-visualização simulada
    private Dictionary<string, List<string>> estruturaPreviewSimulada = new();

    // Guarda qual "grupo/pasta" o usuário está navegando no Explorer da pré-visualização
    private string? explorerGrupoAtual = null;

    // Histórico das movimentações reais feitas ao organizar (usado para desfazer)
    private List<MoveAction> historicoMovimentos = new();

    // Representa um registro de movimentação real: origem → destino
    public class MoveAction { public string? Source { get; set; } public string? Destination { get; set; } }

    // Construtor do formulário
    public Form1()
    {
        // Configuração inicial do formulário
        this.AutoScaleMode = AutoScaleMode.Dpi;
        InitializeComponent();

        // Carrega o ícone do programa caso exista
        try
        {
            string iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resources", "icons", "organizando.ico");
            if (File.Exists(iconPath)) Icon = new Icon(iconPath);
        }
        catch { }
    }

    // ============================================================
    // ===============        DESFAZER AÇÕES        ===============
    // ============================================================

    private void DesfazerOrganizacao()
    {
        // Verifica se há ações a desfazer
        if (!historicoMovimentos.Any()) { MessageBox.Show("Não há ações para desfazer."); return; }

        int erros = 0;

        // Reverte ações na ordem inversa (último movimento → primeiro)
        foreach (var acao in historicoMovimentos.AsEnumerable().Reverse())
        {
            // Validações básicas
            if (acao.Source == null || acao.Destination == null || !File.Exists(acao.Destination)) { erros++; continue; }

            try
            {
                // Garante que a pasta de origem exista
                Directory.CreateDirectory(Path.GetDirectoryName(acao.Source)!);

                bool movido = false;

                // Tenta mover com múltiplas tentativas (para evitar erro de arquivo em uso)
                for (int t = 0; t < 10; t++)
                {
                    try { File.Move(acao.Destination, acao.Source, true); movido = true; break; }
                    catch (IOException) { Thread.Sleep(150); }
                }

                if (!movido)
                {
                    erros++;
                    MessageBox.Show($"Não foi possível desfazer o arquivo:\n{acao.Destination}", "Erro",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                erros++;
                MessageBox.Show($"Erro ao desfazer arquivo:\n{acao.Destination}\nMotivo: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Remove pastas vazias após desfazer
        historicoMovimentos.Select(m => Path.GetDirectoryName(m.Destination))
            .Distinct()
            .Where(p => p != null)
            .ToList()
            .ForEach(p =>
            {
                DeletarPastasVazias(p!);
                RemoverPastasPaiVazias(txtPasta.Text, p!);
            });

        // Exibe resultado final
        MessageBox.Show(erros > 0 ?
            $"Desfazer concluído com alguns erros ({erros} arquivos)." :
            "Organização desfeita com sucesso!");

        historicoMovimentos.Clear();
    }

    // Apaga recursivamente pastas vazias dentro de um caminho
    private void DeletarPastasVazias(string caminho)
    {
        if (!Directory.Exists(caminho)) return;

        foreach (var sub in Directory.GetDirectories(caminho))
            DeletarPastasVazias(sub);

        if (!Directory.EnumerateFileSystemEntries(caminho).Any())
            try { Directory.Delete(caminho); } catch { }
    }

    // Sobe pastas acima do caminho e remove as que estiverem vazias
    private void RemoverPastasPaiVazias(string raiz, string caminho)
    {
        string? pasta = Path.GetDirectoryName(caminho);

        // Vai subindo nível por nível até sair da raiz
        while (!string.IsNullOrEmpty(pasta) && pasta.StartsWith(raiz))
        {
            if (Directory.Exists(pasta) && !Directory.EnumerateFileSystemEntries(pasta).Any())
                try { Directory.Delete(pasta); } catch { break; }

            pasta = Path.GetDirectoryName(pasta);
        }
    }

    // ============================================================
    // ===============        ORGANIZAR ARQUIVOS     ===============
    // ============================================================

    private void OrganizarArquivos(string pasta, string criterio)
    {
        // Valida pasta
        if (!Directory.Exists(pasta)) { MessageBox.Show("Pasta inválida!"); return; }

        // Varre arquivos do diretório
        foreach (var arquivo in Directory.GetFiles(pasta))
        {
            var info = new FileInfo(arquivo);

            // Decide em qual pasta o arquivo deve ser colocado
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

            // Garante que a pasta existe
            Directory.CreateDirectory(destino);

            // Move o arquivo
            string destFile = Path.Combine(destino, info.Name);
            File.Move(arquivo, destFile);

            // Registra no histórico para poder desfazer
            historicoMovimentos.Add(new MoveAction { Source = arquivo, Destination = destFile });
        }

        MessageBox.Show("Arquivos organizados com sucesso!");
    }

    // ============================================================
    // ===============      PRÉ-VISUALIZAÇÃO        ===============
    // ============================================================

    private Dictionary<string, List<string>> GerarPreVisualizacaoAgrupada(string pastaOrigem, string criterio)
    {
        var grupos = new Dictionary<string, List<string>>();

        // Lê todos os arquivos da pasta
        foreach (var arquivo in Directory.GetFiles(pastaOrigem))
        {
            var info = new FileInfo(arquivo);

            // Define o nome do grupo de acordo com o critério escolhido
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

            // Adiciona o item no grupo correspondente
            if (!grupos.ContainsKey(nomePasta))
                grupos[nomePasta] = new List<string>();

            grupos[nomePasta].Add(info.Name);
        }

        // Armazena na estrutura interna para navegação
        estruturaPreviewSimulada = grupos;

        return grupos;
    }

    // Carrega no ListView os itens de um grupo selecionado
    private void CarregarExplorerSimulado(string grupo, ListView lstExplorer, Button btnVoltar)
    {
        lstExplorer.Items.Clear();
        explorerGrupoAtual = grupo;

        // Exibe botão de voltar
        btnVoltar.Visible = true;

        // Se não existe o grupo, não faz nada
        if (!estruturaPreviewSimulada.ContainsKey(grupo)) return;

        // Adiciona arquivos/subpastas simulados ao ListView
        estruturaPreviewSimulada[grupo].ForEach(f =>
            lstExplorer.Items.Add(new ListViewItem(f) { Tag = f })
        );
    }
}
