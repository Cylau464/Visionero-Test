using System;

public interface ISelectable
{
    void Select();
    void Unselect();
    bool IsSelected { get; }
}