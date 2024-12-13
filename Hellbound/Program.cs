namespace Casull
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (Main g = new()) {
                g.Run();
            }
        }
    }
}
