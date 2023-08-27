using System.Runtime.Serialization;

namespace EBookLib;

/// <summary>
/// Класс журнала.
/// </summary>
[DataContract]
public class Magazine : PrintEdition
{
    private readonly int _period;

    [DataMember]
    private int Period
    {
        get => _period;
        init
        {
            if (value < 1)
            {
                throw new ArgumentException("Период выпуска журнала не может быть отрицательным или равен 0");
            }
            _period = value;
        }
    }
    
    public Magazine(string? name, int pages, int period) : base(name, pages)
    {
        Period = period;
    }

    public override string ToString()
    {
        return $"name={Name}; pages={Pages}; period={Period}";
    }
}