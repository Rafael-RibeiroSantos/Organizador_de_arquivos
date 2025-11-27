// Importa namespaces necessários para refletir recursos, manipular imagens e usar controles WinForms
using System.Reflection;
using System.Drawing;
using System.Windows.Forms;

namespace OrganizadorDeArquivos;

// Arquivo gerado pelo Designer (UI)
partial class Form1
{
    // Declaração dos controles da interface
    private TextBox txtPasta;
    private ComboBox cmbCriterio;
    private Label lblStatus;
    private Panel pnlPreview, pnlExplorer;
    private Label lblPreviewTitulo;
    private ListView lstPreview, lstExplorer;
    private Button btnVoltarExplorer, btnConfirmar, btnCancelar, btnDesfazer, btnEscolher, btnOrganizar, btnPreVisualizar;
    private ProgressBar progressBar;

    #region Windows Form Designer generated code
    private void InitializeComponent()
    {
        // ===== CONFIGURAÇÕES DO FORM =====
        // Define título, tamanho e aparência da janela
        this.Text = "Organizador de Arquivos";
        this.ClientSize = new Size(800, 420);
        this.FormBorderStyle = FormBorderStyle.FixedSingle;
        this.MaximizeBox = false;
        this.StartPosition = FormStartPosition.CenterScreen;
        this.BackColor = Color.FromArgb(245, 247, 250);
        Font fonte = new Font("Segoe UI", 9);

        // ===== FUNÇÕES AUXILIARES PARA CRIAR CONTROLES =====
        // Cria botão com estilo padrão
        Button CriarBotao(string texto, Point loc, Color back, int largura) =>
            new()
            {
                Text = texto,
                Location = loc,
                Font = fonte,
                BackColor = back,
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Width = largura,
                FlatAppearance = { BorderSize = 0 }
            };

        // Cria ListView estilizado
        ListView CriarListView(View view, Size size, Color back, Color fore, Font f) =>
            new() { View = view, Size = size, BackColor = back, ForeColor = fore, Font = f, BorderStyle = BorderStyle.FixedSingle };

        // Cria painel com estilo
        Panel CriarPanel(Point loc, Size size, Color back, bool vis) =>
            new() { Location = loc, Size = size, BackColor = back, BorderStyle = BorderStyle.FixedSingle, Visible = vis };

        // ===== CAMPO PARA SELECIONAR PASTA =====
        txtPasta = new TextBox()
        {
            Location = new Point(10, 10),
            Width = 300,
            Font = fonte,
            BorderStyle = BorderStyle.FixedSingle,
            BackColor = Color.White,
            ForeColor = Color.FromArgb(40, 40, 40)
        };
        this.Controls.Add(txtPasta);

        // Botão para escolher pasta
        btnEscolher = CriarBotao("📂 Escolher Pasta", new Point(320, 10), Color.FromArgb(0, 191, 255), 130);
        this.Controls.Add(btnEscolher);

        // ===== CRITÉRIO DE ORGANIZAÇÃO =====
        cmbCriterio = new ComboBox() { Location = new Point(10, 50), Width = 130, Font = fonte };
        cmbCriterio.Items.AddRange(new string[] { "Tipo", "Tamanho", "Data" });
        this.Controls.Add(cmbCriterio);

        // Botões principais: organizar, pré-visualizar e desfazer
        btnOrganizar = CriarBotao("✅ Organizar", new Point(150, 50), Color.FromArgb(102, 205, 170), 90);
        btnPreVisualizar = CriarBotao("🔍 Pré-visualizar", new Point(250, 50), Color.FromArgb(255, 215, 0), 130);
        btnDesfazer = CriarBotao("↩️ Desfazer", new Point(390, 50), Color.FromArgb(255, 99, 71), 90);
        this.Controls.AddRange(new Control[] { btnOrganizar, btnPreVisualizar, btnDesfazer });

        // Status inferior
        lblStatus = new Label() { Text = "Aguardando ação...", Location = new Point(10, 90), Width = 420, Font = fonte };
        this.Controls.Add(lblStatus);

        // ===== ÁREA DE PRÉ-VISUALIZAÇÃO =====
        pnlPreview = CriarPanel(new Point(490, 10), new Size(300, 370), Color.WhiteSmoke, false);

        // Título da pré-visualização
        lblPreviewTitulo = new Label() { Text = "Pré-visualização:", Location = new Point(10, 10), Font = new Font("Segoe UI", 12, FontStyle.Bold), AutoSize = true, ForeColor = Color.FromArgb(30, 30, 30) };
        pnlPreview.Controls.Add(lblPreviewTitulo);

        // Painel explorador interno
        pnlExplorer = CriarPanel(new Point(25, 75), new Size(250, 200), Color.White, false);
        pnlPreview.Controls.Add(pnlExplorer);
        this.Controls.Add(pnlPreview);

        // Lista de grupos (ex: Imagens, Vídeo...)
        lstPreview = CriarListView(View.LargeIcon, new Size(250, 232), Color.White, Color.FromArgb(30, 30, 30), fonte);
        lstPreview.Location = new Point(25, 45);
        pnlPreview.Controls.Add(lstPreview);

        // Botão voltar dentro da pré-visualização
        btnVoltarExplorer = CriarBotao("⬅ Voltar", new Point(10, 35), Color.LightGray, 80);
        btnVoltarExplorer.Visible = false;
        pnlPreview.Controls.Add(btnVoltarExplorer);

        // Lista interna (arquivos dentro do grupo)
        lstExplorer = CriarListView(View.Details, new Size(250, 200), Color.White, Color.FromArgb(30, 30, 30), fonte);
        lstExplorer.Columns.Add("Nome do arquivo", 240, HorizontalAlignment.Left);
        lstExplorer.Dock = DockStyle.Fill;
        pnlExplorer.Controls.Add(lstExplorer);

        // Barra de progresso para organização real
        progressBar = new ProgressBar() { Location = new Point(10, 295), Size = new Size(280, 20), Style = ProgressBarStyle.Continuous };
        pnlPreview.Controls.Add(progressBar);

        // Botões de confirmação e cancelamento
        btnConfirmar = CriarBotao("Confirmar", new Point(10, 325), Color.FromArgb(52, 152, 219), 130);
        btnCancelar = CriarBotao("Cancelar", new Point(160, 325), Color.FromArgb(231, 76, 60), 130);
        pnlPreview.Controls.AddRange(new Control[] { btnConfirmar, btnCancelar });

        // ===== ICONES DO LISTVIEW (IMAGENS EMBUTIDAS NO PROJETO) =====
        ImageList imageList = new() { ImageSize = new Size(32, 32), ColorDepth = ColorDepth.Depth32Bit };
        var assembly = Assembly.GetExecutingAssembly();

        // Carrega imagens do recurso interno
        Image LoadImg(string res) => Image.FromStream(assembly.GetManifestResourceStream($"OrganizadorDeArquivos.resources.icons.{res}")!);

        try
        {
            imageList.Images.Add("folder", LoadImg("pasta.png"));
            imageList.Images.Add("video", LoadImg("videoteste.png"));
            imageList.Images.Add("image", LoadImg("imagem.png"));
            imageList.Images.Add("doc", LoadImg("texto.png"));
            imageList.Images.Add("exe", LoadImg("programa.png"));
        }
        catch { }

        lstPreview.LargeImageList = imageList;

        // ===== EVENTOS DOS CONTROLES =====

        // Seleção de pasta
        btnEscolher.Click += (s, e) =>
        {
            using var d = new FolderBrowserDialog();
            if (d.ShowDialog() == DialogResult.OK)
                txtPasta.Text = d.SelectedPath;
        };

        // Organizar
        btnOrganizar.Click += (s, e) =>
        {
            if (!ValidarEntrada()) return;
            OrganizarArquivos(txtPasta.Text, cmbCriterio.SelectedItem.ToString());
            lblStatus.Text = "Arquivos organizados!";
        };

        // Pré-visualizar agrupamento
        btnPreVisualizar.Click += (s, e) =>
        {
            if (!ValidarEntrada()) return;
            GerarPreview();
        };

        // Abrir pasta dentro da pré-visualização
        lstPreview.DoubleClick += (s, e) => AbrirExplorerSimulado();

        // Impede abrir arquivos reais
        lstExplorer.DoubleClick += (s, e) =>
            MessageBox.Show("Pré-visualização apenas. Nenhum arquivo real será aberto.");

        // Botão voltar do Explorer
        btnVoltarExplorer.Click += (s, e) =>
        {
            pnlExplorer.Visible = false;
            lstPreview.Visible = true;
            btnVoltarExplorer.Visible = false;
        };

        // Desfazer organização
        btnDesfazer.Click += (s, e) =>
        {
            if (historicoMovimentos.Count == 0)
            {
                MessageBox.Show("Nenhuma organização para desfazer.");
                lblStatus.Text = "Nenhuma ação para desfazer.";
                return;
            }
            DesfazerOrganizacao();
            lblStatus.Text = "Organização desfeita!";
        };

        // Confirma organização real com barra de progresso
        btnConfirmar.Click += async (s, e) => await ConfirmarOrganizacaoAsync();

        // Cancela a tela de pré-visualização
        btnCancelar.Click += (s, e) =>
        {
            pnlPreview.Visible = false;
            lblStatus.Text = "Ação cancelada.";
        };
    }

    // ===== FUNÇÕES AUXILIARES DO DESIGNER =====

    // Valida se pasta e critério foram preenchidos
    private bool ValidarEntrada() =>
        !string.IsNullOrWhiteSpace(txtPasta.Text) && cmbCriterio.SelectedItem != null;

    // Gera lista de grupos para pré-visualização
    private void GerarPreview()
    {
        pnlPreview.Visible = true;
        lstPreview.Items.Clear();

        // Adiciona cada grupo ao ListView com ícone correspondente
        foreach (var grupo in GerarPreVisualizacaoAgrupada(txtPasta.Text, cmbCriterio.SelectedItem.ToString()))
        {
            string icone = grupo.Key switch
            {
                "Documentos" => "doc",
                "Imagens" => "image",
                "Programas" => "exe",
                "Vídeo" or "Video" => "video",
                _ => "folder"
            };
            lstPreview.Items.Add(new ListViewItem($"{grupo.Key} ({grupo.Value.Count})", icone) { Tag = grupo.Key });
        }

        lblStatus.Text = "Pré-visualização gerada.";
    }

    // Abre os itens dentro do grupo clicado
    private void AbrirExplorerSimulado()
    {
        if (lstPreview.SelectedItems.Count == 0) return;

        string grupo = lstPreview.SelectedItems[0].Tag as string ?? "";
        if (string.IsNullOrWhiteSpace(grupo))
        {
            MessageBox.Show("Erro ao determinar o grupo selecionado.");
            return;
        }

        pnlExplorer.Visible = true;
        lstPreview.Visible = false;
        btnVoltarExplorer.Visible = true;

        // Carrega itens dentro do grupo
        CarregarExplorerSimulado(grupo, lstExplorer, btnVoltarExplorer);
    }

    // Simula progresso da organização real
    private async System.Threading.Tasks.Task ConfirmarOrganizacaoAsync()
    {
        if (!ValidarEntrada()) return;

        var arquivos = Directory.GetFiles(txtPasta.Text);
        if (arquivos.Length == 0)
        {
            MessageBox.Show("Nenhum arquivo encontrado!");
            return;
        }

        progressBar.Maximum = arquivos.Length;
        progressBar.Value = 0;

        // Barra progride artificialmente para UX
        foreach (var a in arquivos)
        {
            await System.Threading.Tasks.Task.Delay(100);
            progressBar.Value++;
        }

        // Executa organização real
        OrganizarArquivos(txtPasta.Text, cmbCriterio.SelectedItem.ToString());

        progressBar.Value = progressBar.Maximum;
        lblStatus.Text = "Arquivos organizados!";
        pnlPreview.Visible = false;
    }
    #endregion
}
