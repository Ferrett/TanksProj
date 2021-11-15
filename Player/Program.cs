using System;


namespace TankGame
{
    public static class Program
    {

        [STAThread]
        public static void Main()
        {
            Start.StartForm();

            if (Start.Login)
            {
                
                using (var game = new Game())
                    game.Run();
            }
        }
    }
}
