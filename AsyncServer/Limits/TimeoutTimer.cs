using Timers = System.Timers;
namespace Asterion.Limits {
    class TimeoutTimer : Timers.Timer {
        private object tag;
        
        public object Tag {
            get { return tag; }
        }
    
        public TimeoutTimer(object tag) : base() {
            this.tag = tag;
        }
        
        public void Restart() {
            Stop();
            Start();
        }
    }
}
