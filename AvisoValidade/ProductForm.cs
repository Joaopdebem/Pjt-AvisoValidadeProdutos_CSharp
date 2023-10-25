public class ProductForm : Form
{
	private DataGridView dataGridView1;
	private List<Produto> produtos;
	private Dictionary<string, bool> recolhidoState;

	public ProductForm(List<Produto> produtos)
	{
		this.produtos = produtos;
		this.recolhidoState = LoadRecolhidoState();

		dataGridView1 = new DataGridView();

		dataGridView1.Location = new Point(40, 20);
		dataGridView1.Size = new Size(800, 400);
		dataGridView1.Font = new Font("Verdana", 12);

		dataGridView1.Columns.Add("Produto", "Produto");
		dataGridView1.Columns.Add("DiasRestantes", "Dias Restantes");
		dataGridView1.Columns.Add("Aviso", "Aviso");
		dataGridView1.Columns.Add(new DataGridViewCheckBoxColumn { Name = "Recolhido", HeaderText = "Recolhido" });

		dataGridView1.Columns["Produto"].Width = 417;
		dataGridView1.Columns["DiasRestantes"].Width = 140;
		dataGridView1.Columns["Aviso"].Width = 100;
		dataGridView1.Columns["Recolhido"].Width = 100;

		foreach (var produto in produtos)
		{
			int index = dataGridView1.Rows.Add();
			dataGridView1.Rows[index].Cells["Produto"].Value = produto.Descricao;
			dataGridView1.Rows[index].Cells["DiasRestantes"].Value = produto.DiasRestantes.ToString();
			dataGridView1.Rows[index].Cells["Aviso"].Value = produto.Aviso;
			dataGridView1.Rows[index].Cells["Recolhido"].Value = recolhidoState.ContainsKey(produto.Descricao) ? recolhidoState[produto.Descricao] : false;
		}

		foreach (DataGridViewColumn column in dataGridView1.Columns)
		{
			if (column.Name != "Recolhido")
			{
				column.ReadOnly = true;
			}
		}

		this.StartPosition = FormStartPosition.CenterScreen;
		this.Controls.Add(dataGridView1);
		this.Text = "Aviso validade";
		this.Size = new Size(895, 480);
		this.FormClosing += (s, e) => SaveRecolhidoState();
	}

	private Dictionary<string, bool> LoadRecolhidoState()
	{
		var state = new Dictionary<string, bool>();

		if (File.Exists("recolhidoState.txt"))
		{
			var lines = File.ReadAllLines("recolhidoState.txt");
			foreach (var line in lines)
			{
				var parts = line.Split(',');
				if (parts.Length == 2 && bool.TryParse(parts[1], out var recolhido))
				{
					state[parts[0]] = recolhido;
				}
			}
		}

		return state;
	}

	private void SaveRecolhidoState()
	{
		var lines = new List<string>();

		foreach (DataGridViewRow row in dataGridView1.Rows)
		{
			if (row.Cells["Produto"].Value != null && row.Cells["Recolhido"].Value != null)
			{
				var descricao = row.Cells["Produto"].Value.ToString();
				var recolhido = (bool)row.Cells["Recolhido"].Value;

				lines.Add($"{descricao},{recolhido}");
			}
		}

		File.WriteAllLines("recolhidoState.txt", lines);
	}

}
