using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class ListEventReporter<T> : List<T>
{
    Action EventReport;

    public ListEventReporter(Action EventReport = null)
    {
        this.EventReport = EventReport;
    }

    public new void Add(T item)
    {
        base.Add(item);
        EventReport?.Invoke();
    }

    public new void AddRange(IEnumerable<T> enumie)
    {
        base.AddRange(enumie);
        EventReport?.Invoke();
    }

    public new void Insert(int index, T item)
    {
        base.Insert(index, item);
        EventReport?.Invoke();
    }

    public new void Clear()
    {
        base.Clear();
        EventReport?.Invoke();
    }

    public new bool Remove(T item)
    {
        bool success = base.Remove(item);
        if(success) EventReport?.Invoke();
        return success;
    } 

    public new void RemoveAt(int index)
    {
        base.RemoveAt(index);
        EventReport?.Invoke();
    }

}

