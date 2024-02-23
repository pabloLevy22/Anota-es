using System;
using System.Data.SqlClient;

public class Program
{
    static public bool Encerrar = false;
    static public int numeroDoRegistro;
    static public int NumeroDoRegistro {get{return numeroDoRegistro;}set{numeroDoRegistro = value; OnValorAtribuido();}}
    static public object[][] registros = new object[0][];

    static public event EventHandler ValorAtribuido;

    static public void OnValorAtribuido()
    {
        ValorAtribuido?.Invoke(null, EventArgs.Empty);
    }

    public static void Main()
    {
        Console.Clear();
        Console.WriteLine("Anotações\nAperte ↑↓ para seleciona a anotação\nAperte C para cria anotação\nAperte Enter para visualizar a anotação selecionada\nAperte esq para sair");
        Console.WriteLine("Numero do registro: esperar");

        var linha = Console.CursorTop - 1;

        if(Sql.VerificaExistenciaBanco())
        {
            Sql.CreateDatabase();
            Sql.CreateTable();
        }

        if (registros.Length > 0)
        {
            foreach (var registro in registros)
            {
                Console.WriteLine($"{NumeroDoRegistro}: {registro[(int)TabelaAnotações.Coluna.Nome]}");
            }
        }
        else if (Sql.VerificaQuantosRegistroTem(TabelaAnotações.VerificaQuantosRegistroTem))
        {
            foreach (var registro in Sql.Consultar(TabelaAnotações.consultar))
            {
                Array.Resize(ref registros, NumeroDoRegistro + 1);

                registros[NumeroDoRegistro] = registro;

                Console.WriteLine($"{NumeroDoRegistro}: {registros[NumeroDoRegistro][(int)TabelaAnotações.Coluna.Nome]}");
            }
        }

        ValorAtribuido += (sender, e) =>
        {
            Console.SetCursorPosition(0, linha);
            Console.WriteLine(("Numero de registros: " + NumeroDoRegistro).PadRight(100));
        };

        NumeroDoRegistro = 0;

        while (true)
        {
            ConsoleKeyInfo tecla = Console.ReadKey(true);

            switch (tecla.Key)
            {
                case ConsoleKey.UpArrow:
                    if(registros.Length > numeroDoRegistro) numeroDoRegistro++;
                    continue;

                case ConsoleKey.DownArrow:
                    if(0 < numeroDoRegistro) numeroDoRegistro--;
                    continue;

                case ConsoleKey.C:
                    while (true)
                    {
                        Criar();

                        if (FluxoDeExecução("Deseja continua criado anotações? Tecla Enter para Sim e Esc para não")) break;
                    }
                    break;

                case ConsoleKey.Enter:
                    Visualizar();
                    break;

                case ConsoleKey.Escape:
                    return;

                default:
                    continue;
            }

            if (FluxoDeExecução()) Main();

            return;
        }
    }

    public static void Criar()
    {
        string? nome = EntradaTratada("Insira o nome da anotação");
        string? texto = EntradaTratada("Insira a anotação");

        DateTime dataAtual = DateTime.Now;

        string dataFormatada = dataAtual.ToString("yyyy-MM-dd");
        string inserir = $"INSERT INTO Anotação(Nome, Texto, Data) VALUES('{nome}', '{texto}', '{dataFormatada}')";

        Sql.Atualização(inserir);

        Array.Resize(ref registros, registros.Length + 1);

        foreach (var registro in Sql.Consultar(TabelaAnotações.consultar))
        {
            registros[registros.Length - 1] = registro;
        }
    }


    public static void Visualizar()
    {
        int index = NumeroDoRegistro - 1;

        Console.Clear();
        Console.WriteLine($"{registros[index][(int)TabelaAnotações.Coluna.Nome]} {registros[index][(int)TabelaAnotações.Coluna.Data]}");
        Console.WriteLine($"{registros[index][(int)TabelaAnotações.Coluna.Texto]}");
        Console.WriteLine();
        Console.WriteLine("Tecla 'E' para edita\tTecla 'R' para remove\tTecla 'Esc' para voltar");

        while (true)
        {
            ConsoleKeyInfo tecla = Console.ReadKey(true);

            switch (tecla.Key)
            {
                case ConsoleKey.E:
                    if (FluxoDeExecução("Deseja redefine qual? Enter para nome e Esc para texto"))
                    {
                        registros[index][(int)TabelaAnotações.Coluna.Nome] = EntradaTratada("Ensíra o novo nome");

                        Sql.Atualização($"UPDATE Anotação SET Nome = '{registros[index][(int)TabelaAnotações.Coluna.Nome]}' WHERE ID = {registros[index][(int)TabelaAnotações.Coluna.ID]}");
                    }
                    else
                    {
                        registros[index][(int)TabelaAnotações.Coluna.Texto] = EntradaTratada("Ensíra o novo Texto");

                        Sql.Atualização($"UPDATE Anotação SET Nome = '{registros[index][(int)TabelaAnotações.Coluna.Texto]}' WHERE ID = {registros[index][(int)TabelaAnotações.Coluna.ID]}");
                    }

                    Visualizar();
                    return;

                case ConsoleKey.R:
                    Sql.Atualização($"DELETE FROM Anotação WHERE ID = {registros[index][(int)TabelaAnotações.Coluna.ID]}");

                    registros[index] = null;

                    Array.Sort(registros);
                    Array.Reverse(registros);
                    Array.Resize(ref registros, registros.Length - 1);
                    return;

                case ConsoleKey.Escape:
                    return;

                default:
                    continue;
            }
        }
    }

    public static string EntradaTratada(string Pergunta)
    {
        Console.Clear();
        Console.WriteLine(Pergunta);

        var linha = Console.CursorTop;
        var entrada = "";

        do
        {
            Console.SetCursorPosition(0, linha);

            entrada = Console.ReadLine();

            if (entrada == null) continue;

            Console.WriteLine("Tem certeza dessa entrada?\n Tecla Enter para confirma e Esc para volta");

            ConsoleKeyInfo tecla = Console.ReadKey(true);

            switch (tecla.Key)
            {
                case ConsoleKey.Enter:
                    break;

                case ConsoleKey.Escape:
                    entrada = EntradaTratada(Pergunta);
                    break;

                default:
                    continue;
            }

            return entrada;

        } while (true);
    }

    static bool FluxoDeExecução(string mesagem = "Deseja Continua? Tecla Enter para Sim e Esc para não")
    {
        Console.Clear();
        Console.WriteLine(mesagem);

        while (true)
        {
            ConsoleKeyInfo tecla = Console.ReadKey(true);

            switch (tecla.Key)
            {
                case ConsoleKey.Enter:
                    return true;

                case ConsoleKey.Escape:
                    return false;

                default:
                    continue;
            }
        }

    }
}