namespace EBookLib;
/// <summary>
/// Интерфейс печати.
/// </summary>
public interface IPrinting
{
    /// <summary>
    /// Событие печати.
    /// </summary>
    public event EventHandler OnPrint;

    /// <summary>
    /// Метод для вызова события печати.
    /// </summary>
    public void Print();
}