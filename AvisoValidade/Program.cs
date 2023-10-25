using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;

namespace AvisoValidade;

public class Program
{
	[STAThread]
	static void Main()
	{

		Application.EnableVisualStyles();
		Application.SetCompatibleTextRenderingDefault(false);

		var encapsulados = new XLWorkbook(@"C:\"); // Local com a planilha de produtos encapsulados
		var produtosVencimentos = new XLWorkbook(@"C:\"); // Local com a planilha baixa de produtos com validade

		var planilhaEncapsulados = encapsulados.Worksheet(1);
		var planilhaProdutosVencimentos = produtosVencimentos.Worksheet(1);

		var codigosEncapsulados = planilhaEncapsulados.Column(1).CellsUsed().Select(c => c.Value.ToString()).ToList();
		var codigosProdutosVencimentos = planilhaProdutosVencimentos.Column(2).CellsUsed().Select(c => c.Value.ToString()).ToList();

		var codigosComuns = codigosEncapsulados.Intersect(codigosProdutosVencimentos).ToList();

		var produtos = new List<Produto>();

		foreach (var codigo in codigosComuns)
		{
			var linhaEncapsulados = planilhaEncapsulados.RowsUsed().First(r => r.Cell(1).Value.ToString() == codigo);
			var linhaVencimentos = planilhaProdutosVencimentos.RowsUsed().First(r => r.Cell(2).Value.ToString() == codigo);

			var coluna2 = linhaVencimentos.Cell(2).Value.ToString();
			var coluna3 = linhaVencimentos.Cell(3).Value.ToString();

			DateTime coluna7;
			DateTime.TryParse(linhaVencimentos.Cell(7).Value.ToString(), out coluna7);

			var dataAtual = DateTime.Now;
			var diasRestantes = (int)(coluna7 - dataAtual).TotalDays;

			var diasAviso = int.Parse(linhaEncapsulados.Cell(3).Value.ToString());

			if (diasRestantes <= diasAviso)
			{
				string tipoAviso = "";
				if (diasAviso == 60)
				{
					tipoAviso = "60 dias!";
				}
				else if (diasAviso == 120)
				{
					tipoAviso = "120 dias!";
				}
				else if (diasAviso == 240)
				{
					tipoAviso = "240 dias!";
				}

				produtos.Add(new Produto
				{
					Codigo = codigo,
					Descricao = $"{coluna2} - {coluna3}",
					DiasRestantes = diasRestantes,
					Aviso = tipoAviso
				});
			}
		}

		Application.Run(new ProductForm(produtos));
	}
}