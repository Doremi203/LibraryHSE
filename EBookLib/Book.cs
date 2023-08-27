using System.Runtime.Serialization;
using System.Text;

namespace EBookLib;
/// <summary>
/// Класс книги.
/// </summary>
[DataContract]
public class Book : PrintEdition
{
    [DataMember]
    private string? Author { get; set; }
    public Book(string? name, int pages, string? author) : base(name, pages)
    {
        Author = author;
    }

    public override string ToString()
    {
        return $"name={Name}; pages={Pages}; author={Author}";
    }
}