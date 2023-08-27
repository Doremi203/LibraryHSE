using System.Runtime.Serialization.Json;
using System.Text;
using EBookLib;
using Utils;

namespace LibraryHSE;

internal static class Program
{
    private static bool _isCanceled;
    private static MyLibrary<PrintEdition>? _library;
    private static readonly Random Rand = new ();
    private static readonly MenuItem[] Menu = {
        new ("0", "выйти из программы", () => _isCanceled = true),
        new ("1", "выполнить весь алгоритм задания", RunProgram),
    };

    private static void Main()
    {
        while (!_isCanceled)
        {
            ShowMenu();
            ProcessMenu();
        }
    }

    /// <summary>
    /// Обработка выбранного пользователем элемента меню приложения.
    /// </summary>
    /// <exception cref="ArgumentException">Исключение выбрасывается при попытке неверного ввода пункта меню.</exception>
    private static void ProcessMenu()
    {
        try
        {
            var input = Console.ReadLine();
            var menuItem = Menu.FirstOrDefault(i => i.Input == input);
            if (menuItem is null)
                throw new ArgumentException("Несуществующая команда, повторите ввод");
            menuItem.Action();
        }
        catch (IOException)
        {
            Console.WriteLine("Закройте файл, над которым проводится операция");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    /// <summary>
    /// Вывод меню приложения.
    /// </summary>
    private static void ShowMenu()
    {
        foreach (var item in Menu)
        {
            Console.WriteLine($"Введите \"{item.Input}\", чтобы {item.Description}");
        }
    }
    
    /// <summary>
    /// Выполнение алгоритма программа.
    /// </summary>
    private static void RunProgram()
    {
        CreateLibrary();
        
        _library!.OnTake += TakeHandler;

        foreach (var book in _library.Where(edition => edition is Book))
        {
            book.Print();
        }

        Console.WriteLine("\n" + _library);
        
        if (_library.FirstLettersOfTheBooks.Count != 0)
            _library.TakeBooks(_library.FirstLettersOfTheBooks[Rand.Next(_library.FirstLettersOfTheBooks.Count)]);
        else
            Console.WriteLine("\nКниг для удаления не было");
        
        Console.WriteLine("\n" + _library);
        
        SerializeLibrary();
        DeserializeLibrary();
        
        Console.WriteLine("\n" + _library);
        
        Console.WriteLine($"\nAverageBookPages = {_library.AverageBookPages:f2}");
        Console.WriteLine($"\nAverageMagazinePages = {_library.AverageMagazinePages:f2}");
    }

    /// <summary>
    /// Создание библиотеки.
    /// </summary>
    private static void CreateLibrary()
    {
        Console.WriteLine("Введите N");
        var n = ReadNumberOfPrintedEditions();
        _library = new MyLibrary<PrintEdition>(n);

        for (int i = 0; i < n; i++)
        {
            try
            {
                var pages = Rand.Next(-10, 101);
                var rand1 = Rand.Next(2);
                var name = CreateRandomNameOrAuthor();

                if (rand1 == 0)
                {
                    var book = new Book(name, pages, CreateRandomNameOrAuthor());
                    _library.Add(book);
                    book.OnPrint += PrintHandler;
                }
                else
                    _library.Add(new Magazine(name, pages, Rand.Next(-10, 101)));
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Пересоздание объекта.....");
                i--;
            }
        }
    }

    /// <summary>
    /// Считывание числа N.
    /// </summary>
    /// <returns>Число N.</returns>
    /// <exception cref="FormatException">Исключение выбрасывается при неверном формате введенного числа.</exception>
    /// <exception cref="ArgumentException">Исключение выбрасывается, если введенное число равно 0.</exception>
    private static int ReadNumberOfPrintedEditions()
    {
        var n = 0;
        var parsed = false;
        while (!parsed)
        {
            try
            {
                if (!int.TryParse(Console.ReadLine()!, out n))
                    throw new FormatException("Неверный формат, повторите ввод");
                if (n <= 0)
                    throw new ArgumentException("Нельзя создать 0 или меньше объектов, повторите ввод");
                parsed = true;
            }
            catch (FormatException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        return n;
    }

    /// <summary>
    /// Сериализация библиотеки.
    /// </summary>
    private static void SerializeLibrary()
    {
        var jsonFormatter = new DataContractJsonSerializer(typeof(MyLibrary<PrintEdition>));
        using var fs = new FileStream("myLibrary.json", FileMode.Create);
        using var writer = JsonReaderWriterFactory.CreateJsonWriter(fs, Encoding.Unicode, false, true);
        jsonFormatter.WriteObject(writer, _library);
        Console.WriteLine("\nБиблиотека успешно сериализована...");
    } 
    
    /// <summary>
    /// Десериализация библиотеки.
    /// </summary>
    private static void DeserializeLibrary()
    {
        var jsonFormatter = new DataContractJsonSerializer(typeof(MyLibrary<PrintEdition>));
        using var fs = new FileStream("myLibrary.json", FileMode.Open);
        _library = jsonFormatter.ReadObject(fs) as MyLibrary<PrintEdition> ?? throw new InvalidOperationException("Нечего десериализовывать, сначала сохраните информацию в файл");
        Console.WriteLine("\nБиблиотека успешно десериализована...");
    }

    /// <summary>
    /// Метод генерирования рандомного названия или имени автора.
    /// </summary>
    private static string CreateRandomNameOrAuthor()
    {
        var rand = new Random();
        var str = new StringBuilder();
        var length = rand.Next(1,11);
        str.Append((char)rand.Next('A', 'Z' + 1));
        for (int j = 1; j < length; j++)
        {
            str.Append((char)rand.Next('a', 'z' + 1));
        }

        return str.ToString();
    }

    /// <summary>
    /// Метод-обработчик события печати.
    /// </summary>
    /// <param name="sender">Отправитель события.</param>
    /// <param name="eventArgs">Параметры события.</param>
    private static void PrintHandler(object? sender, EventArgs eventArgs)
    {
        Console.WriteLine("\nPRINTED!");
        Console.WriteLine(sender);
    }

    /// <summary>
    /// Метод-обработчик события изъятия книг.
    /// </summary>
    /// <param name="sender">Отправитель события.</param>
    /// <param name="start">Буква с которой начинаются книги для изъятия.</param>
    private static void TakeHandler(object? sender, char start)
    {
        Console.WriteLine($"\nATTENTION! Books starts with {start} were taken!");
    }
}