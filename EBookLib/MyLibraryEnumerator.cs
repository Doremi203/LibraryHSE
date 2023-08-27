using System.Collections;

namespace EBookLib;

/// <summary>
/// Класс перечислитель для библиотеки.
/// </summary>
/// <typeparam name="TU">Тип перечислителя.</typeparam>
public class MyLibraryEnumerator<TU> : IEnumerator<TU>
{
    private readonly List<TU> _library;
    private int _index;
    private TU? _current;

    public MyLibraryEnumerator(List<TU>? library)
    {
        _library = library!;
        _index = 0;
        _current = default;
    }

    public bool MoveNext()
    {
        if (_index >= _library.Count) return false;
        _current = _library[_index];
        _index++;
        return true;

    }

    public void Reset()
    {
        _index = 0;
        _current = default;
    }
    
    public TU Current => _current!;

    object? IEnumerator.Current => Current;

    public void Dispose() { }
}