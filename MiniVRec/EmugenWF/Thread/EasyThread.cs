using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emugen.Thread
{
    public class EasyThread
    {
        public delegate void EasyThreadFunc(object[] args);
        volatile EasyThreadFunc action;

        volatile public bool isEnd = false;
        volatile System.Threading.Thread thread;
        volatile object[] args;



        public EasyThread(EasyThreadFunc action, object[] args)
        {
            this.action = action;
            this.args = args;

            thread = new System.Threading.Thread(new System.Threading.ThreadStart(Core));
            thread.Start();
        }

        private void Core()
        {
            action(args);
            isEnd = true;
        }

        public void Dispose()
        {
            if (!isEnd)
            {
                try
                {
                    thread.Abort();
                }
                catch
                {
                }
            }
        }
    }
}
