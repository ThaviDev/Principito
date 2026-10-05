using System;
using UnityEngine;
[CreateAssetMenu(fileName = "Float", menuName = "VarSCOB/Float")]
public class FloatSCOB : ScriptableObject
{
    [SerializeField] private float _v;
    private void OnEnable() => _v = 0;   // solo se ejecuta al cargar el asset
    private void OnDisable() => _v = 0;  // limpia al parar el editor
    public event Action<float> A_OnValueChange;
    public float Value => _v;
    public void SetValue(float newValue)
    {
        if (_v == newValue) return;
        _v = newValue;
        A_OnValueChange?.Invoke(_v);
    }
    public void Add(float amount) => SetValue(_v + amount);
    public void Reset() => SetValue(0);
}
