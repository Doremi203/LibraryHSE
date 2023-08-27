using System.Collections;
using System.Runtime.Serialization;
using System.Text;

namespace EBookLib;
/// <summary>
/// Класс библиотеки.
/// </summary>
/// <typeparam name="T">Тип объектов хранящихся в библиотеке.</typeparam>
[DataContract]
public class MyLibrary<T> : IEnumerable<T> where T : PrintEdition
{
    /// <summary>
    /// Список содержащий первые буквы книг из библиотеки.
    /// </summary>
    [DataMember]
    public List<char> FirstLettersOfTheBooks { get; private set; }
    
    [DataMember]
    private List<T>? Library { get; set; }
    
    /// <summary>
    /// Событие изъятия книг.
    /// </summary>
    public event EventHandler<char>? OnTake;

    /// <summary>
    /// Среднее арифметическое количества страниц книг.
    /// </summary>
    public double AverageBookPages
    {
        get
        {
            if (!this.Any(edition => edition is Book))
                return 0;
            return this.Where(edition => edition is Book).Average(edition => edition.Pages);
        }
    }

    /// <summary>
    /// Среднее арифметическое количества страниц журналов.
    /// </summary>
    public double AverageMagazinePages
    {
        get
        {
            if (!this.Any(edition => edition is Magazine))
                return 0;
            return this.Where(edition => edition is Magazine).Average(edition => edition.Pages);
        }
    }

    public MyLibrary(int n)
    {
        Library = new List<T>(n);
        FirstLettersOfTheBooks = new List<char>(n / 2);
    }
    
    /// <summary>
    /// Метод вызова события для изъятия книг.
    /// </summary>
    /// <param name="start">Буква с которой начинаются книги для изъятия.</param>
    public void TakeBooks(char start)
    {
        Library!.RemoveAll(edition => edition is Book && edition.Name!.StartsWith(start));
        FirstLettersOfTheBooks.RemoveAll(c => c == start);
        OnTake?.Invoke(this, start);
    }

    /// <summary>
    /// Метод добавления печатного издания в библиотеку.
    /// </summary>
    /// <param name="printed">Печатное издание, добавляемое в библиотеку.</param>
    public void Add(T printed)
    {
        Library!.Add(printed);
        if (printed is Book)
            FirstLettersOfTheBooks.Add(printed.Name![0]);
    }

    public IEnumerator<T> GetEnumerator() => new MyLibraryEnumerator<T>(Library);

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append($"Overall pages = {this.Sum(edition => edition.Pages)}");
        foreach (var edition in this)
        {
            sb.Append("\n" + edition);
        }

        return sb.ToString();
    }
}