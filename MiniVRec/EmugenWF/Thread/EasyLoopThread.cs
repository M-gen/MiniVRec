using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Emugen.Thread
{
    public class EasyLoopThread
    {
        public delegate void EasyLoopThreadFunc(object[] args);
        volatile EasyLoopThreadFunc action;

        volatile public bool isEnd = false;
        volatile System.Threading.Thread thread;
        volatile object[] args;

        volatile public bool isBreak = false;

        int sleepTime;

        public EasyLoopThread(EasyLoopThreadFunc action, object[] args, int sleepTime = 300)
        {
            this.action = action;
            this.args = args;
            this.sleepTime = sleepTime;

            thread = new System.Threading.Thread(new System.Threading.ThreadStart(Core));
            thread.SetApartmentState(ApartmentState.STA); // mciSendString など一部、この指定がないと動作しない
            thread.Start();
        }

        private void Core()
        {
            while (!isBreak)
            {
                action(args);
                Sleep.Do(sleepTime);
            }
            isEnd = true;
        }

        public void Dispose()
        {
            if (!isEnd)
            {
                isBreak = true;

                

                //try
                //{
                //    thread.Abort();
                //}
                //catch
                //{
                //}
            }
        }
    }
}
