using System;
using UnityEngine;
[CreateAssetMenu(fileName = "Int", menuName = "VarSCOB/Int")]
public class IntSCOB : ScriptableObject
{
    [SerializeField] private int _v;
    private void OnEnable() => _v = 0;   // solo se ejecuta al cargar el asset
    private void OnDisable() 
    {
        _v = 0;
        A_OnValueChange = null;
    }  // limpia al parar el editor
    public event Action<int> A_OnValueChange;
    public int Value => _v;
    public void SetValue(int newValue)
    {
        if (_v == newValue) return;
        _v = newValue;
        A_OnValueChange?.Invoke(_v);
    }
    public void Add(int amount) =>SetValue(_v + amount);
    public void Reset() => SetValue(0);
    private void OnValidate() => A_OnValueChange?.Invoke(_v);
    public void SetValueSilent(int newValue) => _v = newValue;
}
