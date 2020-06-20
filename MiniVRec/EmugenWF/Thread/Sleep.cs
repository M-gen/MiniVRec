using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emugen.Thread
{
    public class Sleep
    {
        static public void Do(int time)
        {
            Task taskA = Task.Factory.StartNew(() => _Sleep_Task(time));
            taskA.Wait();
        }

        static private void _Sleep_Task(int time)
        {
            System.Threading.Thread.Sleep(time);
        }
    }
}
