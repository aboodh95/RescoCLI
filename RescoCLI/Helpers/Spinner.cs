using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RescoCLI
{
    public class Spinner : IDisposable
    {
        private const string Sequence = @"/-\|";
        private int counter = 0;

        private readonly int delay;
        private bool active;
        private readonly Thread thread;

        public Spinner(int delay = 100)
        {

            this.delay = delay;
            thread = new Thread(Spin);
        }

        public void Start()
        {
            active = true;
            if (!thread.IsAlive)
                thread.Start();
        }

        public void Stop()
        {
            active = false;
            Draw(' ');
        }

        private void Spin()
        {
            while (active)
            {
                Turn();
                Thread.Sleep(delay);
            }
        }

        private void Draw(char c)
        {
            try
            {
                Console.Write(c);
                Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
            }
            catch
            {
            }

        }

        private void Turn()
        {
            Draw(Sequence[++counter % Sequence.Length]);
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
