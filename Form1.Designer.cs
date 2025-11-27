using System.Reflection;
using System.Drawing;
using System.Windows.Forms;

namespace OrganizadorDeArquivos;

partial class Form1
{
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
        // Form básico
        this.Text = "Organizador de Arquivos";
        this.ClientSize = new Size(800, 420);
        this.FormBorderStyle = FormBorderStyle.FixedSingle;
        this.MaximizeBox = false;
        this.StartPosition = FormStartPosition.CenterScreen;
        this.BackColor = Color.FromArgb(245, 247, 250);
        Font fonte = new Font("Segoe UI", 9);

        // Funções auxiliares
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

        ListView CriarListView(View view, Size size, Color back, Color fore, Font f) =>
            new() { View = view, Size = size, BackColor = back, ForeColor = fore, Font = f, BorderStyle = BorderStyle.FixedSingle };

        Panel CriarPanel(Point loc, Size size, Color back, bool vis) =>
            new() { Location = loc, Size = size, BackColor = back, BorderStyle = BorderStyle.FixedSingle, Visible = vis };

        // ===== Controles principais =====
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

        btnEscolher = CriarBotao("📂 Escolher Pasta", new Point(320, 10), Color.FromArgb(0, 191, 255), 130);
        this.Controls.Add(btnEscolher);

        cmbCriterio = new ComboBox() { Location = new Point(10, 50), Width = 130, Font = fonte };
        cmbCriterio.Items.AddRange(new string[] { "Tipo", "Tamanho", "Data" });
        this.Controls.Add(cmbCriterio);

        btnOrganizar = CriarBotao("✅ Organizar", new Point(150, 50), Color.FromArgb(102, 205, 170), 90);
        btnPreVisualizar = CriarBotao("🔍 Pré-visualizar", new Point(250, 50), Color.FromArgb(255, 215, 0), 130);
        btnDesfazer = CriarBotao("↩️ Desfazer", new Point(390, 50), Color.FromArgb(255, 99, 71), 90);
        this.Controls.AddRange(new Control[] { btnOrganizar, btnPreVisualizar, btnDesfazer });

        lblStatus = new Label() { Text = "Aguardando ação...", Location = new Point(10, 90), Width = 420, Font = fonte };
        this.Controls.Add(lblStatus);

        pnlPreview = CriarPanel(new Point(490, 10), new Size(300, 370), Color.WhiteSmoke, false);
        lblPreviewTitulo = new Label() { Text = "Pré-visualização:", Location = new Point(10, 10), Font = new Font("Segoe UI", 12, FontStyle.Bold), AutoSize = true, ForeColor = Color.FromArgb(30, 30, 30) };
        pnlPreview.Controls.Add(lblPreviewTitulo);
        pnlExplorer = CriarPanel(new Point(25, 75), new Size(250, 200), Color.White, false);
        pnlPreview.Controls.Add(pnlExplorer);
        this.Controls.Add(pnlPreview);

        lstPreview = CriarListView(View.LargeIcon, new Size(250, 232), Color.White, Color.FromArgb(30, 30, 30), fonte);
        lstPreview.Location = new Point(25, 45);
        pnlPreview.Controls.Add(lstPreview);

        btnVoltarExplorer = CriarBotao("⬅ Voltar", new Point(10, 35), Color.LightGray, 80);
        btnVoltarExplorer.Visible = false;
        pnlPreview.Controls.Add(btnVoltarExplorer);

        lstExplorer = CriarListView(View.Details, new Size(250, 200), Color.White, Color.FromArgb(30, 30, 30), fonte);
        lstExplorer.Columns.Add("Nome do arquivo", 240, HorizontalAlignment.Left);
        lstExplorer.Dock = DockStyle.Fill;
        pnlExplorer.Controls.Add(lstExplorer);

        progressBar = new ProgressBar() { Location = new Point(10, 295), Size = new Size(280, 20), Style = ProgressBarStyle.Continuous };
        pnlPreview.Controls.Add(progressBar);

        btnConfirmar = CriarBotao("Confirmar", new Point(10, 325), Color.FromArgb(52, 152, 219), 130);
        btnCancelar = CriarBotao("Cancelar", new Point(160, 325), Color.FromArgb(231, 76, 60), 130);
        pnlPreview.Controls.AddRange(new Control[] { btnConfirmar, btnCancelar });

        // ===== ImageList e carregamento de recursos =====
        ImageList imageList = new() { ImageSize = new Size(32, 32), ColorDepth = ColorDepth.Depth32Bit };
        var assembly = Assembly.GetExecutingAssembly();
        Image LoadImg(string res) => Image.FromStream(assembly.GetManifestResourceStream($"OrganizadorDeArquivos.resources.icons.{res}")!);
        try
        {
            imageList.Images.Add("folder", LoadImg("pasta.png")); imageList.Images.Add("video", LoadImg("videoteste.png"));
            imageList.Images.Add("image", LoadImg("imagem.png")); imageList.Images.Add("doc", LoadImg("texto.png"));
            imageList.Images.Add("exe", LoadImg("programa.png"));
        }
        catch { }
        lstPreview.LargeImageList = imageList;

        // ===== Eventos =====
        btnEscolher.Click += (s, e) => { using var d = new FolderBrowserDialog(); if (d.ShowDialog() == DialogResult.OK) txtPasta.Text = d.SelectedPath; };
        btnOrganizar.Click += (s, e) => { if (!ValidarEntrada()) return; OrganizarArquivos(txtPasta.Text, cmbCriterio.SelectedItem.ToString()); lblStatus.Text = "Arquivos organizados!"; };
        btnPreVisualizar.Click += (s, e) => { if (!ValidarEntrada()) return; GerarPreview(); };
        lstPreview.DoubleClick += (s, e) => AbrirExplorerSimulado();
        lstExplorer.DoubleClick += (s, e) => MessageBox.Show("Pré-visualização apenas. Nenhum arquivo real será aberto.");
        btnVoltarExplorer.Click += (s, e) => { pnlExplorer.Visible = false; lstPreview.Visible = true; btnVoltarExplorer.Visible = false; };
        btnDesfazer.Click += (s, e) => { if (historicoMovimentos.Count == 0) { MessageBox.Show("Nenhuma organização para desfazer."); lblStatus.Text = "Nenhuma ação para desfazer."; return; } DesfazerOrganizacao(); lblStatus.Text = "Organização desfeita!"; };
        btnConfirmar.Click += async (s, e) => await ConfirmarOrganizacaoAsync();
        btnCancelar.Click += (s, e) => { pnlPreview.Visible = false; lblStatus.Text = "Ação cancelada."; };
    }

    // ===== Funções auxiliares do designer =====
    private bool ValidarEntrada() => !string.IsNullOrWhiteSpace(txtPasta.Text) && cmbCriterio.SelectedItem != null;
    private void GerarPreview()
    {
        pnlPreview.Visible = true;
        lstPreview.Items.Clear();
        foreach (var grupo in GerarPreVisualizacaoAgrupada(txtPasta.Text, cmbCriterio.SelectedItem.ToString()))
        {
            string icone = grupo.Key switch { "Documentos" => "doc", "Imagens" => "image", "Programas" => "exe", "Vídeo" or "Video" => "video", _ => "folder" };
            lstPreview.Items.Add(new ListViewItem($"{grupo.Key} ({grupo.Value.Count})", icone) { Tag = grupo.Key });
        }
        lblStatus.Text = "Pré-visualização gerada.";
    }
    private void AbrirExplorerSimulado()
    {
        if (lstPreview.SelectedItems.Count == 0) return;
        string grupo = lstPreview.SelectedItems[0].Tag as string ?? "";
        if (string.IsNullOrWhiteSpace(grupo)) { MessageBox.Show("Erro ao determinar o grupo selecionado."); return; }
        pnlExplorer.Visible = true; lstPreview.Visible = false; btnVoltarExplorer.Visible = true;
        CarregarExplorerSimulado(grupo, lstExplorer, btnVoltarExplorer);
    }
    private async System.Threading.Tasks.Task ConfirmarOrganizacaoAsync()
    {
        if (!ValidarEntrada()) return;
        var arquivos = Directory.GetFiles(txtPasta.Text);
        if (arquivos.Length == 0) { MessageBox.Show("Nenhum arquivo encontrado!"); return; }
        progressBar.Maximum = arquivos.Length; progressBar.Value = 0;
        foreach (var a in arquivos) { await System.Threading.Tasks.Task.Delay(100); progressBar.Value++; }
        OrganizarArquivos(txtPasta.Text, cmbCriterio.SelectedItem.ToString());
        progressBar.Value = progressBar.Maximum; lblStatus.Text = "Arquivos organizados!"; pnlPreview.Visible = false;
    }
    #endregion
} 
