
namespace BBS.LayoutEditor
{
    partial class Form1
    {
        /// <summary>
        /// Variável de designer necessária.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpar os recursos que estão sendo usados.
        /// </summary>
        /// <param name="disposing">true se for necessário descartar os recursos gerenciados; caso contrário, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código gerado pelo Windows Form Designer

        /// <summary>
        /// Método necessário para suporte ao Designer - não modifique 
        /// o conteúdo deste método com o editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.arquivoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.abrirL2DToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fecharToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fecharTodosToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.salvarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.salvarTodosToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sairToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sobreToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selecionadoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportarTexturaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importarTexturaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importViewImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sP2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportarXMLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importarXMLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportarRecorteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importarRecorteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lY2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editarFontRenderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.arquivoToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.abrirToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.opçõesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.modoColoridoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showSequenceDrawToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showPartDrawToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sobreToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Location = new System.Drawing.Point(0, 27);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(716, 457);
            this.tabControl1.TabIndex = 11;
            this.tabControl1.Visible = false;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // arquivoToolStripMenuItem
            // 
            this.arquivoToolStripMenuItem.Name = "arquivoToolStripMenuItem";
            this.arquivoToolStripMenuItem.Size = new System.Drawing.Size(61, 19);
            this.arquivoToolStripMenuItem.Text = "Arquivo";
            // 
            // abrirL2DToolStripMenuItem
            // 
            this.abrirL2DToolStripMenuItem.Name = "abrirL2DToolStripMenuItem";
            this.abrirL2DToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.abrirL2DToolStripMenuItem.Text = "Abrir L2D";
            this.abrirL2DToolStripMenuItem.Click += new System.EventHandler(this.abrirL2DToolStripMenuItem_Click);
            // 
            // fecharToolStripMenuItem
            // 
            this.fecharToolStripMenuItem.Enabled = false;
            this.fecharToolStripMenuItem.Name = "fecharToolStripMenuItem";
            this.fecharToolStripMenuItem.Size = new System.Drawing.Size(120, 22);
            this.fecharToolStripMenuItem.Text = "Close";
            this.fecharToolStripMenuItem.Click += new System.EventHandler(this.fecharToolStripMenuItem_Click);
            // 
            // fecharTodosToolStripMenuItem
            // 
            this.fecharTodosToolStripMenuItem.Enabled = false;
            this.fecharTodosToolStripMenuItem.Name = "fecharTodosToolStripMenuItem";
            this.fecharTodosToolStripMenuItem.Size = new System.Drawing.Size(120, 22);
            this.fecharTodosToolStripMenuItem.Text = "Close All";
            this.fecharTodosToolStripMenuItem.Click += new System.EventHandler(this.fecharTodosToolStripMenuItem_Click);
            // 
            // salvarToolStripMenuItem
            // 
            this.salvarToolStripMenuItem.Enabled = false;
            this.salvarToolStripMenuItem.Name = "salvarToolStripMenuItem";
            this.salvarToolStripMenuItem.Size = new System.Drawing.Size(120, 22);
            this.salvarToolStripMenuItem.Text = "Save";
            this.salvarToolStripMenuItem.Click += new System.EventHandler(this.salvarToolStripMenuItem_Click);
            // 
            // salvarTodosToolStripMenuItem
            // 
            this.salvarTodosToolStripMenuItem.Enabled = false;
            this.salvarTodosToolStripMenuItem.Name = "salvarTodosToolStripMenuItem";
            this.salvarTodosToolStripMenuItem.Size = new System.Drawing.Size(120, 22);
            this.salvarTodosToolStripMenuItem.Text = "Save All";
            this.salvarTodosToolStripMenuItem.Click += new System.EventHandler(this.salvarTodosToolStripMenuItem_Click);
            // 
            // sairToolStripMenuItem
            // 
            this.sairToolStripMenuItem.Name = "sairToolStripMenuItem";
            this.sairToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.sairToolStripMenuItem.Text = "Sair";
            // 
            // sobreToolStripMenuItem
            // 
            this.sobreToolStripMenuItem.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.sobreToolStripMenuItem.Name = "sobreToolStripMenuItem";
            this.sobreToolStripMenuItem.Size = new System.Drawing.Size(49, 19);
            this.sobreToolStripMenuItem.Text = "Sobre";
            this.sobreToolStripMenuItem.Click += new System.EventHandler(this.sobreToolStripMenuItem_Click);
            // 
            // selecionadoToolStripMenuItem
            // 
            this.selecionadoToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportarTexturaToolStripMenuItem,
            this.importarTexturaToolStripMenuItem,
            this.importViewImageToolStripMenuItem});
            this.selecionadoToolStripMenuItem.Name = "selecionadoToolStripMenuItem";
            this.selecionadoToolStripMenuItem.Size = new System.Drawing.Size(47, 20);
            this.selecionadoToolStripMenuItem.Text = "SQ2P";
            this.selecionadoToolStripMenuItem.Visible = false;
            // 
            // exportarTexturaToolStripMenuItem
            // 
            this.exportarTexturaToolStripMenuItem.Name = "exportarTexturaToolStripMenuItem";
            this.exportarTexturaToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
            this.exportarTexturaToolStripMenuItem.Text = "Export TIM2";
            this.exportarTexturaToolStripMenuItem.Click += new System.EventHandler(this.exportarTexturaToolStripMenuItem_Click);
            // 
            // importarTexturaToolStripMenuItem
            // 
            this.importarTexturaToolStripMenuItem.Name = "importarTexturaToolStripMenuItem";
            this.importarTexturaToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
            this.importarTexturaToolStripMenuItem.Text = "Import TIM2";
            this.importarTexturaToolStripMenuItem.Click += new System.EventHandler(this.importarTexturaToolStripMenuItem_Click);
            // 
            // importViewImageToolStripMenuItem
            // 
            this.importViewImageToolStripMenuItem.Name = "importViewImageToolStripMenuItem";
            this.importViewImageToolStripMenuItem.Size = new System.Drawing.Size(174, 22);
            this.importViewImageToolStripMenuItem.Text = "Import View Image";
            this.importViewImageToolStripMenuItem.Visible = false;
            this.importViewImageToolStripMenuItem.Click += new System.EventHandler(this.importViewImageToolStripMenuItem_Click);
            // 
            // sP2ToolStripMenuItem
            // 
            this.sP2ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportarXMLToolStripMenuItem,
            this.importarXMLToolStripMenuItem,
            this.exportarRecorteToolStripMenuItem,
            this.importarRecorteToolStripMenuItem});
            this.sP2ToolStripMenuItem.Name = "sP2ToolStripMenuItem";
            this.sP2ToolStripMenuItem.Size = new System.Drawing.Size(38, 20);
            this.sP2ToolStripMenuItem.Text = "SP2";
            this.sP2ToolStripMenuItem.Visible = false;
            // 
            // exportarXMLToolStripMenuItem
            // 
            this.exportarXMLToolStripMenuItem.Enabled = false;
            this.exportarXMLToolStripMenuItem.Name = "exportarXMLToolStripMenuItem";
            this.exportarXMLToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.exportarXMLToolStripMenuItem.Text = "Export XML";
            this.exportarXMLToolStripMenuItem.Visible = false;
            // 
            // importarXMLToolStripMenuItem
            // 
            this.importarXMLToolStripMenuItem.Enabled = false;
            this.importarXMLToolStripMenuItem.Name = "importarXMLToolStripMenuItem";
            this.importarXMLToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.importarXMLToolStripMenuItem.Text = "Import XML";
            this.importarXMLToolStripMenuItem.Visible = false;
            // 
            // exportarRecorteToolStripMenuItem
            // 
            this.exportarRecorteToolStripMenuItem.Enabled = false;
            this.exportarRecorteToolStripMenuItem.Name = "exportarRecorteToolStripMenuItem";
            this.exportarRecorteToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.exportarRecorteToolStripMenuItem.Text = "Export UV Part";
            this.exportarRecorteToolStripMenuItem.Click += new System.EventHandler(this.exportarRecorteToolStripMenuItem_Click);
            // 
            // importarRecorteToolStripMenuItem
            // 
            this.importarRecorteToolStripMenuItem.Enabled = false;
            this.importarRecorteToolStripMenuItem.Name = "importarRecorteToolStripMenuItem";
            this.importarRecorteToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.importarRecorteToolStripMenuItem.Text = "Import UV Part";
            // 
            // lY2ToolStripMenuItem
            // 
            this.lY2ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editarFontRenderToolStripMenuItem});
            this.lY2ToolStripMenuItem.Name = "lY2ToolStripMenuItem";
            this.lY2ToolStripMenuItem.Size = new System.Drawing.Size(38, 20);
            this.lY2ToolStripMenuItem.Text = "LY2";
            this.lY2ToolStripMenuItem.Visible = false;
            // 
            // editarFontRenderToolStripMenuItem
            // 
            this.editarFontRenderToolStripMenuItem.Enabled = false;
            this.editarFontRenderToolStripMenuItem.Name = "editarFontRenderToolStripMenuItem";
            this.editarFontRenderToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.editarFontRenderToolStripMenuItem.Text = "Edit Font Render";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.arquivoToolStripMenuItem1,
            this.opçõesToolStripMenuItem,
            this.sP2ToolStripMenuItem,
            this.lY2ToolStripMenuItem,
            this.selecionadoToolStripMenuItem,
            this.sobreToolStripMenuItem1});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(716, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // arquivoToolStripMenuItem1
            // 
            this.arquivoToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.abrirToolStripMenuItem,
            this.salvarToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.salvarTodosToolStripMenuItem,
            this.fecharToolStripMenuItem,
            this.fecharTodosToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.arquivoToolStripMenuItem1.Name = "arquivoToolStripMenuItem1";
            this.arquivoToolStripMenuItem1.Size = new System.Drawing.Size(59, 20);
            this.arquivoToolStripMenuItem1.Text = "Archive";
            // 
            // abrirToolStripMenuItem
            // 
            this.abrirToolStripMenuItem.Name = "abrirToolStripMenuItem";
            this.abrirToolStripMenuItem.Size = new System.Drawing.Size(120, 22);
            this.abrirToolStripMenuItem.Text = "Open";
            this.abrirToolStripMenuItem.Click += new System.EventHandler(this.abrirL2DToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Enabled = false;
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(120, 22);
            this.saveAsToolStripMenuItem.Text = "Save As";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(120, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // opçõesToolStripMenuItem
            // 
            this.opçõesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.modoColoridoToolStripMenuItem,
            this.showSequenceDrawToolStripMenuItem,
            this.showPartDrawToolStripMenuItem});
            this.opçõesToolStripMenuItem.Name = "opçõesToolStripMenuItem";
            this.opçõesToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.opçõesToolStripMenuItem.Text = "Options";
            // 
            // modoColoridoToolStripMenuItem
            // 
            this.modoColoridoToolStripMenuItem.Checked = true;
            this.modoColoridoToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.modoColoridoToolStripMenuItem.Name = "modoColoridoToolStripMenuItem";
            this.modoColoridoToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.modoColoridoToolStripMenuItem.Text = "Part Color filling";
            this.modoColoridoToolStripMenuItem.Click += new System.EventHandler(this.modoColoridoToolStripMenuItem_Click);
            // 
            // showSequenceDrawToolStripMenuItem
            // 
            this.showSequenceDrawToolStripMenuItem.Name = "showSequenceDrawToolStripMenuItem";
            this.showSequenceDrawToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.showSequenceDrawToolStripMenuItem.Text = "Show Sequence Draw";
            this.showSequenceDrawToolStripMenuItem.Click += new System.EventHandler(this.showSequenceDrawToolStripMenuItem_Click);
            // 
            // showPartDrawToolStripMenuItem
            // 
            this.showPartDrawToolStripMenuItem.Checked = true;
            this.showPartDrawToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showPartDrawToolStripMenuItem.Name = "showPartDrawToolStripMenuItem";
            this.showPartDrawToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.showPartDrawToolStripMenuItem.Text = "Show Group Draw";
            this.showPartDrawToolStripMenuItem.Click += new System.EventHandler(this.showPartDrawToolStripMenuItem_Click);
            // 
            // sobreToolStripMenuItem1
            // 
            this.sobreToolStripMenuItem1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.sobreToolStripMenuItem1.Name = "sobreToolStripMenuItem1";
            this.sobreToolStripMenuItem1.Size = new System.Drawing.Size(52, 20);
            this.sobreToolStripMenuItem1.Text = "About";
            this.sobreToolStripMenuItem1.Click += new System.EventHandler(this.sobreToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::BBS.LayoutEditor.Properties.Resources.L2DeditorBACK;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(716, 484);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.tabControl1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "BirthBySleep LayoutEditor (L2D Powered)";
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Form1_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.Form1_DragEnter);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolStripMenuItem arquivoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem abrirL2DToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fecharToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fecharTodosToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem salvarToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem salvarTodosToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sairToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sobreToolStripMenuItem;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem arquivoToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem sobreToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem abrirToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem selecionadoToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem sP2ToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem lY2ToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem exportarTexturaToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem importarTexturaToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem exportarXMLToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem importarXMLToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem editarFontRenderToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem exportarRecorteToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem importarRecorteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem opçõesToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem modoColoridoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        public System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.ToolStripMenuItem showSequenceDrawToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showPartDrawToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem importViewImageToolStripMenuItem;
    }
}

