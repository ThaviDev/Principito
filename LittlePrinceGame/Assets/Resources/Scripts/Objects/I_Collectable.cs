public interface I_Collectable
{
    int Value { get; }
    void Collect(I_Collector who);
}
