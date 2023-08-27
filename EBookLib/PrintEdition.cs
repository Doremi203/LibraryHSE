using System.Runtime.Serialization;

namespace EBookLib;

/// <summary>
/// Абстрактный класс печатного издания.
/// </summary>
[DataContract, KnownType(typeof(Book)), KnownType(typeof(Magazine))]
public abstract class PrintEdition : IPrinting
{
    private readonly int _pages;
    
    /// <summary>
    /// Событие печати.
    /// </summary>
    public event EventHandler? OnPrint;

    protected PrintEdition(string? name, int pages)
    {
        Name = name;
        Pages = pages;
    }
    
    /// <summary>
    /// Название печатного издания.
    /// </summary>
    [DataMember]
    public string? Name { get; set; }

    /// <summary>
    /// Количество страниц печатного издания.
    /// </summary>
    /// <exception cref="ArgumentException">Исключение выбрасывается, если количество страниц меньше или равно 0.</exception>
    [DataMember]
    public int Pages
    {
        get => _pages;
        init
        {
            if (value < 1)
            {
                throw new ArgumentException("Количество страниц не может быть отрицательным или равно 0");
            }
            _pages = value;
        }
    }

    /// <summary>
    /// Метод вызова события печати.
    /// </summary>
    public void Print()
    {
        OnPrint?.Invoke(this, EventArgs.Empty);
    }

    public override string ToString()
    {
        return $"name={Name}; pages={Pages}";
    }
}